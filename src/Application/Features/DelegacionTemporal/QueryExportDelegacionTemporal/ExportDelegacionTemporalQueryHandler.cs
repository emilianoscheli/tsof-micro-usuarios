namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryExportDelegacionTemporal
{
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Interfaces.Services;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MaestrosAPI.SDK.Contracts.DTOs.MotivosDelegacion;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.DelegacionTemporal.DTOs;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public class ExportDelegacionTemporalQueryHandler : IQueryHandler<ExportDelegacionTemporalQuery, FileResponseDTO>
    {
        public ExportDelegacionTemporalQueryHandler(
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IFileAdministrator fileAdministrator,
          IAuthenticatedUserService authenticatedUserService,
          IMotivosDelegacionServiceConsumer motivosDelegacionServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.FileAdministrator = fileAdministrator;
            this.AuthenticatedUserService = authenticatedUserService;
            this.MotivosDelegacionServiceConsumer = motivosDelegacionServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IFileAdministrator FileAdministrator { get; }

        private IAuthenticatedUserService AuthenticatedUserService { get; }

        private IMotivosDelegacionServiceConsumer MotivosDelegacionServiceConsumer { get; }

        public async Task<IResponse<FileResponseDTO>> Handle(ExportDelegacionTemporalQuery query, CancellationToken cancellationToken)
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

            var entidadDTO = await this.UnitOfWork.DelegacionTemporalRepository
                .GetAllByFilters(filterDTO)
                .ProjectTo<ExcelDelegacionTemporalDTO>(this.AutoMapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

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

            var columnHeaders = new Dictionary<string, string>
            {
                { nameof(ExcelDelegacionTemporalDTO.NombreCompletoDelegador), "Nombre y apellido usuario delegador" },
                { nameof(ExcelDelegacionTemporalDTO.DelegadorIdRed), "Id de red usuario delegador" },
                { nameof(ExcelDelegacionTemporalDTO.NombreCompletoDelegado), "Nombre y apellido usuario delegado" },
                { nameof(ExcelDelegacionTemporalDTO.DelegadoIdRed), "Id de red usuario delegado" },
                { nameof(ExcelDelegacionTemporalDTO.FechaDesde), "Fecha desde" },
                { nameof(ExcelDelegacionTemporalDTO.FechaHasta), "Fecha hasta" },
                { nameof(ExcelDelegacionTemporalDTO.Motivo), "Motivo" },
                { nameof(ExcelDelegacionTemporalDTO.TiempoLimite), "Tiempo límite" },
                { nameof(ExcelDelegacionTemporalDTO.Observaciones), "Observaciones" },
            };

            var fileContent = this.FileAdministrator.ExportToExcel(entidadDTO, "Delegaciones Temporales", columnHeaders);

            if (fileContent is null || fileContent.Length == 0)
            {
                return Response.Failure<FileResponseDTO>(UsuariosError.ArchivoVacio);
            }

            var responseData = new FileResponseDTO
            {
                FileContent = fileContent,
                FileName = "Delegaciones temporales de usuarios",
            };

            return Response.Success(responseData);
        }
    }
}
