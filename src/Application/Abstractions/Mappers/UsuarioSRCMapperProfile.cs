namespace UsuariosAPI.Application.Abstractions.Mappers
{
    using APICore.Responses.DTOs;
    using AutoMapper;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.ResponsableUO;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioSociedad;

    public class UsuarioSRCMapperProfile : Profile
    {
        public UsuarioSRCMapperProfile()
        {
            this.CreateMap<UsuarioSRC, UsuarioRolesPermisosDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdRed, opt => opt.MapFrom(src => src.IdRed))
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto))
                .ForMember(dest => dest.ContratistaId, opt => opt.MapFrom(src => src.ContratistaFkId))
                .ForMember(dest => dest.EstudioAuditorId, opt => opt.MapFrom(src => src.EstudioAuditorFkId))
                .ForMember(dest => dest.SedeId, opt => opt.MapFrom(src => src.SedeFkId))
                .ForMember(dest => dest.SociedadesId, opt => opt.MapFrom(src => src.Sociedades!.Select(x => x.SociedadFkId)))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles!
                    .Select(r => new RolDTO { Id = r.Rol!.Id, Nombre = r.Rol.Nombre, Descripcion = r.Rol.Descripcion })))
                .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src => src.Roles!.SelectMany(r => r.Rol!.Permisos
                    .Select(p => new PermisoDTO { Id = p.PermisoId, Codigo = p.Permiso!.Codigo, Descripcion = r.Rol.Descripcion }))));

            this.CreateMap<UsuarioRol, UsuariosRolesDTO>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol));

            this.CreateMap<UsuarioSociedad, UsuarioSociedadDTO>()
                .ForMember(dest => dest.UsuarioSRCId, opt => opt.MapFrom(src => src.UsuarioSRCId))
                .ForMember(dest => dest.Sociedad, opt => opt.Ignore());

            this.CreateMap<UsuarioSRC, UsuarioDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => string.Join(',', src.Roles!.Select(x => $"{x.Rol!.Nombre} {(x.Rol.EsManual ? "- M" : "- A")}"))))
                .ForMember(dest => dest.Sociedades, opt => opt.Ignore())
                .ForMember(dest => dest.Contratista, opt => opt.MapFrom(src => src.ContratistaFkId.HasValue ? new ContratistaUsuarioDTO { Id = src.ContratistaFkId.Value } : null))
                .ForMember(dest => dest.RolesTemporales, opt => opt.MapFrom(src => src.DelegacionTemporales))
                .ForMember(dest => dest.SociedadesList, opt => opt.MapFrom(src => src.Sociedades))
                .ForMember(dest => dest.RolesList, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.EstudioAuditor, opt => opt.MapFrom(src => src.EstudioAuditorFkId.HasValue ? new EstudioAuditorUsuarioDTO { Id = src.EstudioAuditorFkId.Value } : null))
                .ForMember(dest => dest.Sede, opt => opt.MapFrom(src => src.SedeFkId.HasValue ? new SedeUsuarioDTO { Id = src.SedeFkId.Value } : null));

            this.CreateMap<UsuarioSRC, UsuariosDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => string.Join(',', src.Roles!.Select(x => $"{x.Rol!.Nombre} {(x.Rol.EsManual ? "- M" : "- A")}"))))
                .ForMember(dest => dest.Sociedades, opt => opt.Ignore())
                .ForMember(dest => dest.Contratista, opt => opt.Ignore())
                .ForMember(dest => dest.SedeEstudioAuditor, opt => opt.Ignore());

            this.CreateMap<UsuarioSRC, UsuarioAcotadoDTO>()
                .ForMember(dest => dest.ContratistaId, opt => opt.MapFrom(src => src.ContratistaFkId))
                .ForMember(dest => dest.EstudioAuditorId, opt => opt.MapFrom(src => src.EstudioAuditorFkId))
                .ForMember(dest => dest.SedeId, opt => opt.MapFrom(src => src.SedeFkId))
                .ForMember(dest => dest.SociedadesIds, opt => opt.MapFrom(src => src.Sociedades != null ? src.Sociedades.Select(x => x.SociedadFkId) : new List<int>()))
                .ForMember(dest => dest.RolesIds, opt => opt.MapFrom(src => src.Roles != null ? src.Roles.Select(x => x.RolId) : new List<int>()))
                .ForMember(dest => dest.RolesTemporalesIds, opt => opt.MapFrom(src => src.DelegacionTemporales != null ? src.DelegacionTemporales.SelectMany(x => x.RolesTemporales!).Select(y => y.RolId) : new List<int>()));

            this.CreateMap<MaestrosAPI.SDK.Contracts.DTOs.Sociedades.SociedadDTO, SociedadUsuarioDTO>();
            this.CreateMap<MaestrosAPI.SDK.Contracts.DTOs.EstudiosAuditores.EstudioAuditorDTO, EstudioAuditorUsuarioDTO>();
            this.CreateMap<MaestrosAPI.SDK.Contracts.DTOs.EstudiosAuditores.SedeDTO, SedeUsuarioDTO>();
            this.CreateMap<ContratosAPI.SDK.Contracts.DTOs.Contratista.ContratistaDTO, ContratistaUsuarioDTO>();
            this.CreateMap<ContratosAPI.SDK.Contracts.DTOs.Contratista.ContratistaAcotadoDTO, ContratistaUsuarioDTO>();

            this.CreateMap<Rol, RolDetailsDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));
            this.CreateMap<RolDetailsDTO, Rol>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.Permisos, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.NombreUsuario, opt => opt.Ignore());

            this.CreateMap<Rol, RolDTO>();
            this.CreateMap<ParUsuarioRolDTO, UsuarioRol>()
                .ForMember(dest => dest.UsuarioSRCId, opt => opt.MapFrom(src => src.UsuarioId))
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.UsuarioSRC, opt => opt.Ignore())
                .ForMember(dest => dest.Rol, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.NombreUsuario, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore());

            this.CreateMap<GetPaginatedUsuariosDTO, UsuariosSRCFilterAndOrderDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Codigo.ToString()));

            this.CreateMap<GetUsuariosDTO, UsuariosSRCFilterAndOrderDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Codigo.ToString()))
                .ForMember(dest => dest.ContratistasIds, opt => opt.Ignore())
                .ForMember(dest => dest.PalabraClave, opt => opt.Ignore());

            // Mapeos para usuarios externos
            this.CreateMap<UsuarioSRC, UsuariosExternosDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Roles!.FirstOrDefault() != null ? src.Roles.FirstOrDefault()!.Rol!.Nombre : string.Empty))
                .ForMember(dest => dest.Contratista, opt => opt.Ignore())
                .ForMember(dest => dest.SedeEstudioAuditor, opt => opt.Ignore());

            this.CreateMap<UsuarioSRC, UsuarioExternoDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Roles!.FirstOrDefault() != null ? src.Roles.FirstOrDefault()!.Rol!.Nombre : string.Empty))
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.Roles!.FirstOrDefault() != null ? src.Roles.FirstOrDefault()!.RolId : 0))
                .ForMember(dest => dest.Contratista, opt => opt.Ignore())
                .ForMember(dest => dest.EstudioAuditor, opt => opt.Ignore())
                .ForMember(dest => dest.Sede, opt => opt.Ignore())
                .ForMember(dest => dest.TipoDocumento, opt => opt.Ignore())
                .ForMember(dest => dest.NumeroDocumento, opt => opt.Ignore())
                .ForMember(dest => dest.CUIL, opt => opt.Ignore())
                .ForMember(dest => dest.Pais, opt => opt.Ignore())
                .ForMember(dest => dest.EmailEmpresa, opt => opt.MapFrom(src => src.CorreoElectronico ?? string.Empty))
                .ForMember(dest => dest.TelefonoParticular, opt => opt.MapFrom(src => src.TelefonoParticular ?? string.Empty))
                .ForMember(dest => dest.CodigoIncidencia, opt => opt.MapFrom(src => src.CodigoIncidencia ?? string.Empty));

            this.CreateMap<GetPaginatedUsuariosExternosDTO, UsuariosExternosFilterAndOrderDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Codigo.ToString()))
                .ForMember(dest => dest.ContratistasIds, opt => opt.Ignore());

            this.CreateMap<GetUsuariosExternosDTO, UsuariosExternosFilterAndOrderDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Codigo.ToString()))
                .ForMember(dest => dest.ContratistasIds, opt => opt.Ignore());

            this.CreateMap<MaestrosAPI.SDK.Contracts.DTOs.ResponsablesUO.ResponsableUODetalleDTO, ResponsableUoMaestroDTO>()
                .ForMember(dest => dest.BdOrigenId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Rdi, opt => opt.Ignore()) // no puede mappear automaticamente objetos
                .ForMember(dest => dest.Bp1, opt => opt.Ignore())
                .ForMember(dest => dest.Bp2, opt => opt.Ignore())
                .ForMember(dest => dest.Gip, opt => opt.Ignore())
                .ReverseMap();

            this.CreateMap<MaestrosAPI.SDK.Contracts.DTOs.ResponsablesUO.UsuarioResponsableUODTO, UsuarioDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.BdOrigenId))
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.CorreoElectronico, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.RolesList, opt => opt.Ignore())
                .ForMember(dest => dest.Sociedades, opt => opt.Ignore())
                .ForMember(dest => dest.SociedadesList, opt => opt.Ignore())
                .ForMember(dest => dest.Contratista, opt => opt.Ignore())
                .ForMember(dest => dest.EstudioAuditor, opt => opt.Ignore())
                .ForMember(dest => dest.Sede, opt => opt.Ignore())
                .ForMember(dest => dest.RolesTemporales, opt => opt.Ignore())
                .ForMember(dest => dest.Legajo, opt => opt.Ignore());

            this.CreateMap<DelegacionTemporal, RolesTemporalesDTO>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.RolesTemporales!.Select(x => x.Rol)))
                .ForMember(dest => dest.DelegadorRedId, opt => opt.MapFrom(src => src.UsuarioDelegador != null ? src.UsuarioDelegador.IdRed : string.Empty))
                .ForMember(dest => dest.NombreCompletoDelegador, opt => opt.MapFrom(src => src.UsuarioDelegador != null ? src.UsuarioDelegador.NombreCompleto : string.Empty))
                .ForMember(dest => dest.FechaDesde, opt => opt.MapFrom(src => src.FechaDesde))
                .ForMember(dest => dest.FechaHasta, opt => opt.MapFrom(src => src.FechaHasta));

            this.CreateMap<GetPaginatedReporteUsuariosRolesSRCDTO, GetByFiltersReporteUsuariosRolesSRCDTO>();
        }
    }
}
