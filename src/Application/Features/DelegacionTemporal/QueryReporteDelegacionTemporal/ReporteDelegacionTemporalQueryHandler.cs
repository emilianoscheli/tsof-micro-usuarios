namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryReporteDelegacionTemporal
{
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using MaestrosAPI.SDK.Contracts.DTOs.MotivosDelegacion;
    using Microsoft.Extensions.Logging;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.DelegacionTemporal.DTOs;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed class ReporteDelegacionTemporalQueryHandler : IQueryPaginatedHandler<ReporteDelegacionTemporalQuery, ReporteDelegacionTemporalDTO>
    {
        public ReporteDelegacionTemporalQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMotivosDelegacionServiceConsumer motivosDelegacionServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.MotivosDelegacionServiceConsumer = motivosDelegacionServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IMotivosDelegacionServiceConsumer MotivosDelegacionServiceConsumer { get; }

        public async Task<IPaginatedResponse<ReporteDelegacionTemporalDTO>> Handle(ReporteDelegacionTemporalQuery request, CancellationToken cancellationToken)
        {
            var filtros = this.AutoMapper.Map<GetByFiltersReporteDelegacionTemporalDTO>(request.dto);

            bool tieneAlguno =
                filtros.UsuarioDelegadoId.HasValue ||
                filtros.UsuarioDelegadorId.HasValue ||
                (filtros.MotivosDelegacionIds?.Any() ?? false) ||
                filtros.Vigente.HasValue ||
                filtros.FechaDesde.HasValue ||
                filtros.FechaHasta.HasValue;

            if (!tieneAlguno)
            {
                return PaginatedResponse<ReporteDelegacionTemporalDTO>.Failure(UsuariosError.ReporteErrorValidacion("Debes seleccionar al menos un filtro para generar el reporte."));
            }

            var query = this.UnitOfWork.DelegacionTemporalRepository.GetDelegacionesTemporalesReporte(filtros);

            // Paginación
            var pageNumber = request.dto.PageNumber;
            var pageSize = request.dto.PageSize;
            var (rows, pagination) = await query.ToPaginatedListAsync(pageNumber, pageSize);

            // motivos de delegación
            var motivosIds = rows.Select(x => x.MotivoDelegacionFkId).Distinct().ToList();
            var motivosInfoMap = new Dictionary<int, MotivoDelegacionDTO>();

            if (motivosIds.Count > 0)
            {
                var (motivosInfo, motivosErr) = await this.MotivosDelegacionServiceConsumer.GetMotivosDelegacionByIds(motivosIds, request.gdiAuth);

                if (motivosErr is not null)
                {
                    return PaginatedResponse<ReporteDelegacionTemporalDTO>.Failure(motivosErr);
                }

                if (motivosInfo != null)
                {
                    motivosInfoMap = motivosInfo.ToDictionary(a => a.Id);
                }
            }

            foreach (var row in rows)
            {
                if (row.MotivoDelegacionFkId > 0 && motivosInfoMap.TryGetValue(row.MotivoDelegacionFkId, out var motivoDelegacion))
                {
                    row.MotivoDelegacion = motivoDelegacion.NombreMotivo;
                }
            }

            var dtos = this.AutoMapper.Map<List<ReporteDelegacionTemporalDTO>>(rows);
            return PaginatedResponse<ReporteDelegacionTemporalDTO>.Success(dtos!, pagination);
        }
    }
}