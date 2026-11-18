namespace UsuariosAPI.Application.Features.Usuario.QueryReporteUsuariosRolesSRC
{
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Application.ServiceConsumers;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed class ReporteUsuariosRolesSRCQueryHandler : IQueryPaginatedHandler<ReporteUsuariosRolesSRCQuery, ReporteUsuariosRolesSRCDTO>
    {
        public ReporteUsuariosRolesSRCQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuthenticatedUserService authenticatedUserService,
            ISociedadServiceConsumer sociedadServiceConsumer,
            IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
            IContratistaServiceConsumer contratistaServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.Mapper = mapper;
            this.AuthenticatedUserService = authenticatedUserService;
            this.SociedadServiceConsumer = sociedadServiceConsumer;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper Mapper { get; }

        private IAuthenticatedUserService AuthenticatedUserService { get; }

        private ISociedadServiceConsumer SociedadServiceConsumer { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        public async Task<IPaginatedResponse<ReporteUsuariosRolesSRCDTO>> Handle(ReporteUsuariosRolesSRCQuery request, CancellationToken cancellationToken)
        {
            var f = request.dto;

            bool tieneAlguno =
                (f.UsuariosIds?.Any() ?? false) ||
                (f.RolesIds?.Any() ?? false) ||
                (f.ContratistasIds?.Any() ?? false) ||
                (f.EstudiosAuditoresIds?.Any() ?? false) ||
                f.Activo.HasValue;

            if (!tieneAlguno)
            {
                return PaginatedResponse<ReporteUsuariosRolesSRCDTO>.Failure(UsuariosError.ReporteErrorValidacion("Debes seleccionar al menos un filtro para generar el reporte."));
            }

            var filtros = this.Mapper.Map<GetByFiltersReporteUsuariosRolesSRCDTO>(request.dto);

            var (rows, pagination) = await this.UnitOfWork.UsuarioSRCRepository
                .GetUsuariosRolesSRC(filtros)
                .ToPaginatedListAsync(request.dto.PageNumber, request.dto.PageSize);

            // contratistas
            var contratistasIds = rows
                .Select(a => a.ContratistaFkId)
                .Where(i => i != null)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var contratistasInfoMap = new Dictionary<int, ContratistaUsuarioDTO>();

            if (contratistasIds.Count > 0)
            {
                var (contratistasInfo, contratistasErr) = await this.ContratistaServiceConsumer.GetContratistasByIds(contratistasIds, gdiAuth: request.gdiAuth);

                if (contratistasErr is not null)
                {
                    return PaginatedResponse<ReporteUsuariosRolesSRCDTO>.Failure(contratistasErr);
                }

                if (contratistasInfo != null)
                {
                    contratistasInfoMap = contratistasInfo.ToDictionary(a => a.Id);
                }
            }

            // sociedades
            var sociedadesIds = rows.SelectMany(r => r.SociedadesIds).Distinct().ToList();

            var sociedadesInfoMap = new Dictionary<int, SociedadUsuarioDTO>();

            if (sociedadesIds.Count > 0)
            {
                var (sociedadesInfo, sociedadesErr) = await this.SociedadServiceConsumer.GetSociedadesByIds(sociedadesIds, gdiAuth: request.gdiAuth);

                if (sociedadesErr is not null)
                {
                    return PaginatedResponse<ReporteUsuariosRolesSRCDTO>.Failure(sociedadesErr);
                }

                if (sociedadesInfo != null)
                {
                    sociedadesInfoMap = sociedadesInfo.ToDictionary(a => a.Id);
                }
            }

            // estudios auditores
            var estudiosAuditoresIds = rows
                .Select(a => a.EstudioAuditorFkId)
                .Where(i => i != null)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var estudiosAuditoresInfoMap = new Dictionary<int, EstudioAuditorUsuarioDTO>();

            if (estudiosAuditoresIds.Count > 0)
            {
                var (estudiosAuditoresInfo, estudiosAuditoresErr) = await this.EstudioSedeServiceConsumer.GetEstudiosByIds(estudiosAuditoresIds, gdiAuth: request.gdiAuth);

                if (estudiosAuditoresErr is not null)
                {
                    return PaginatedResponse<ReporteUsuariosRolesSRCDTO>.Failure(estudiosAuditoresErr);
                }

                if (estudiosAuditoresInfo != null)
                {
                    estudiosAuditoresInfoMap = estudiosAuditoresInfo.ToDictionary(a => a.Id);
                }
            }

            var data = rows.Select(r =>
            {
                var sociedadesNombres = r.SociedadesIds.Count == 0
                    ? new List<string>()
                    : r.SociedadesIds
                        .Select(id =>
                            sociedadesInfoMap.TryGetValue(id, out var dto) && !string.IsNullOrWhiteSpace(dto.Nombre)
                                ? dto.Nombre
                                : id.ToString())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();

                var sociedades = sociedadesNombres.Count == 0
                    ? string.Empty
                    : string.Join(", ", sociedadesNombres);

                string contratista = string.Empty;
                string contratistaCuit = string.Empty;

                if (r.ContratistaFkId.HasValue && contratistasInfoMap.TryGetValue(r.ContratistaFkId.Value, out var cDto))
                {
                    contratista = cDto.RazonSocial ?? string.Empty;
                    contratistaCuit = cDto.NumeroIdentificacion ?? string.Empty;
                }

                string estudio = string.Empty;

                if (r.EstudioAuditorFkId.HasValue)
                {
                    if (estudiosAuditoresInfoMap.TryGetValue(r.EstudioAuditorFkId.Value, out var eDto))
                    {
                        var nombre = !string.IsNullOrWhiteSpace(eDto.Nombre) ? eDto.Nombre : string.Empty;
                        estudio = string.IsNullOrWhiteSpace(nombre) ? r.EstudioAuditorFkId.Value.ToString() : nombre!;
                    }
                    else
                    {
                        estudio = r.EstudioAuditorFkId.Value.ToString();
                    }
                }

                return new ReporteUsuariosRolesSRCDTO
                {
                    IdUsuario = r.IdUsuario,
                    ApellidoNombreUsuario = r.ApellidoNombreUsuario ?? string.Empty,
                    Email = r.Email ?? string.Empty,
                    TipoAsignacionDelRol = r.TipoAsignacionDelRol ?? string.Empty,
                    RolEnSRC = r.RolEnSRC ?? string.Empty,
                    Sociedades = sociedades,
                    ContratistaId = r.ContratistaFkId,
                    Contratista = contratista,
                    ContratistaCuit = contratistaCuit,
                    EstudioAuditor = estudio,

                    // agregar activo cuando este en el modelo
                    Activo = true,
                };
            }).ToList();

            return PaginatedResponse<ReporteUsuariosRolesSRCDTO>.Success(data!, pagination);
        }
    }
}
