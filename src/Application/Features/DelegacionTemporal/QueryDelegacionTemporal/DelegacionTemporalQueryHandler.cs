namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryDelegacionTemporal
{
    using APICore.Enums;
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MaestrosAPI.SDK.Contracts.DTOs.MotivosDelegacion;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.DelegacionTemporal.DTOs;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public class DelegacionTemporalQueryHandler : IQueryPaginatedHandler<DelegacionTemporalQuery, DelegacionTemporalDTO>
    {
        public DelegacionTemporalQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuthenticatedUserService authenticatedUserService,
            IMotivosDelegacionServiceConsumer motivosDelegacionServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.AuthenticatedUserService = authenticatedUserService;
            this.MotivosDelegacionServiceConsumer = motivosDelegacionServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IAuthenticatedUserService AuthenticatedUserService { get; }

        private IMotivosDelegacionServiceConsumer MotivosDelegacionServiceConsumer { get; }

        public async Task<IPaginatedResponse<DelegacionTemporalDTO>> Handle(DelegacionTemporalQuery query, CancellationToken cancellationToken)
        {
            var esAdmin = this.AuthenticatedUserService.AuthenticatedUserEsRol(RolesEnum.Administrador);
            var userId = this.AuthenticatedUserService.GetAuthenticatedUserId();

            var filterDTO = new DelegacionTemporalFilterAndOrderDTO
            {
                NombreCompletoDelegado = query.dto.NombreCompletoDelegado,
                NombreCompletoDelegador = query.dto.NombreCompletoDelegador,
                DelegadoIdRed = query.dto.DelegadoIdRed,
                DelegadorIdRed = query.dto.DelegadorIdRed,
                FechaDesde = query.dto.FechaDesde,
                FechaHasta = query.dto.FechaHasta,
                MotivoId = query.dto.MotivoId,
                Estado = query.dto.Estado,
                Vigente = query.dto.Vigente,
                SortBy = query.dto.SortBy,
                OrderAsc = query.dto.OrderAsc,
            };

            if (!esAdmin)
            {
                filterDTO.DelegadorODelegadoId = userId;
            }

            // El nombre del motivo se resuelve contra MaestrosAPI: no es ordenable en la DB.
            var sortByMotivo = string.Equals(filterDTO.SortBy, "motivo", StringComparison.OrdinalIgnoreCase);
            var sortOrderAsc = filterDTO.OrderAsc;
            if (sortByMotivo)
            {
                filterDTO.SortBy = null;
            }

            var queryDelegaciones = this.UnitOfWork.DelegacionTemporalRepository
                .GetAllByFilters(filterDTO)
                .ProjectTo<DelegacionTemporalDTO>(this.AutoMapper.ConfigurationProvider);

            List<DelegacionTemporalDTO> entidadDTO;
            PaginationData pagination;

            if (sortByMotivo)
            {
                var allResults = await queryDelegaciones.ToListAsync(cancellationToken);
                var allMotivosIds = allResults.Select(x => x.MotivoId ?? 0).Where(x => x > 0).Distinct().ToList();

                var motivoNombres = new Dictionary<int, string>();
                if (allMotivosIds.Count > 0)
                {
                    var (motivosAll, errorMotivosAll) = await this.MotivosDelegacionServiceConsumer.GetMotivosDelegacionByIds(allMotivosIds);

                    if (errorMotivosAll is null && motivosAll is not null)
                    {
                        motivoNombres = motivosAll.ToDictionary(r => r.Id, r => r.NombreMotivo ?? string.Empty);
                    }
                }

                var sorted = sortOrderAsc == true
                    ? allResults.OrderBy(x => motivoNombres.TryGetValue(x.MotivoId ?? 0, out var n) ? n : "\uffff")
                    : allResults.OrderByDescending(x => motivoNombres.TryGetValue(x.MotivoId ?? 0, out var n) ? n : string.Empty);

                var totalCount = allResults.Count;
                entidadDTO = sorted
                    .Skip((query.dto.PageNumber - 1) * query.dto.PageSize)
                    .Take(query.dto.PageSize)
                    .ToList();

                pagination = new PaginationData
                {
                    TotalCount = totalCount,
                    PageNumber = query.dto.PageNumber,
                    PageSize = query.dto.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)query.dto.PageSize),
                };
            }
            else
            {
                (entidadDTO, pagination) = await queryDelegaciones
                    .ToPaginatedListAsync(query.dto.PageNumber, query.dto.PageSize);
            }

            var motivosIds = entidadDTO.Select(x => x.MotivoId ?? 0).Where(x => x > 0).Distinct().ToList();

            var (motivos, errorMotivos) = await this.MotivosDelegacionServiceConsumer.GetMotivosDelegacionByIds(motivosIds);

            var motivosMap = new Dictionary<int, MotivoDelegacionDTO>();

            if (errorMotivos is null && motivos is not null)
            {
                motivosMap = motivos.ToDictionary(r => r.Id);
            }

            foreach (var dto in entidadDTO)
            {
                if (dto.MotivoId != null && dto.MotivoId > 0 && motivosMap.TryGetValue(dto.MotivoId.Value, out var motivoDelegacion))
                {
                    dto.Motivo = motivoDelegacion.NombreMotivo;
                }
            }

            return PaginatedResponse<DelegacionTemporalDTO>.Success(entidadDTO, pagination);
        }
    }
}
