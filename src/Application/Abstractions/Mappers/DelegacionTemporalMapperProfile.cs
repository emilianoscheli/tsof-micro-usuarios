namespace UsuariosAPI.Application.Abstractions.Mappers
{
    using AutoMapper;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.DelegacionTemporal.DTOs;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public class DelegacionTemporalMapperProfile : Profile
    {
        public DelegacionTemporalMapperProfile()
        {
            this.CreateMap<DelegacionTemporal, DelegacionTemporalDTO>()
                .ForMember(dest => dest.Observaciones, opt => opt.MapFrom(src => src.Observacion))
                .ForMember(dest => dest.DelegadorIdRed, opt => opt.MapFrom(src => src.UsuarioDelegador!.IdRed))
                .ForMember(dest => dest.DelegadoIdRed, opt => opt.MapFrom(src => src.UsuarioDelegado!.IdRed))
                .ForMember(dest => dest.NombreCompletoDelegador, opt => opt.MapFrom(src => src.UsuarioDelegador!.NombreCompleto))
                .ForMember(dest => dest.NombreCompletoDelegado, opt => opt.MapFrom(src => src.UsuarioDelegado!.NombreCompleto))
                .ForMember(dest => dest.MotivoId, opt => opt.MapFrom(src => src.MotivoDelegacionFkId))
                .ForMember(dest => dest.Motivo, opt => opt.Ignore())
                .ReverseMap();

            this.CreateMap<DelegacionTemporal, ExcelDelegacionTemporalDTO>()
                .ForMember(dest => dest.Observaciones, opt => opt.MapFrom(src => src.Observacion))
                .ForMember(dest => dest.DelegadorIdRed, opt => opt.MapFrom(src => src.UsuarioDelegador!.IdRed))
                .ForMember(dest => dest.DelegadoIdRed, opt => opt.MapFrom(src => src.UsuarioDelegado!.IdRed))
                .ForMember(dest => dest.NombreCompletoDelegador, opt => opt.MapFrom(src => src.UsuarioDelegador!.NombreCompleto))
                .ForMember(dest => dest.NombreCompletoDelegado, opt => opt.MapFrom(src => src.UsuarioDelegado!.NombreCompleto))
                .ForMember(dest => dest.MotivoId, opt => opt.MapFrom(src => src.MotivoDelegacionFkId))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado ? "Activo" : "Inactivo"))
                .ForMember(dest => dest.Motivo, opt => opt.Ignore());

            this.CreateMap<CreateDelegacionTemporalDTO, DelegacionTemporal>()
                .ForMember(dest => dest.MotivoDelegacionFkId, opt => opt.MapFrom(src => src.MotivoId))
                .ForMember(dest => dest.Observacion, opt => opt.MapFrom(src => src.Observacion))
                .ForMember(dest => dest.TiempoLimite, opt => opt.MapFrom(src => src.TiempoLimite))
                .ForMember(dest => dest.UsuarioDelegadoId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioDelegadorId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioDelegado, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioDelegador, opt => opt.Ignore());

            // Mapeo para el reporte HU09
            this.CreateMap<DelegacionTemporal, DelegacionTemporalRowDTO>()
                .ForMember(dest => dest.IdUsuarioDelegador, opt => opt.MapFrom(src => src.UsuarioDelegador != null ? src.UsuarioDelegador.IdRed : string.Empty))
                .ForMember(dest => dest.ApellidoNombreUsuarioDelegador, opt => opt.MapFrom(src => src.UsuarioDelegador != null ? src.UsuarioDelegador.NombreCompleto : string.Empty))
                .ForMember(dest => dest.IdUsuarioDelegado, opt => opt.MapFrom(src => src.UsuarioDelegado != null ? src.UsuarioDelegado.IdRed : string.Empty))
                .ForMember(dest => dest.ApellidoNombreUsuarioDelegado, opt => opt.MapFrom(src => src.UsuarioDelegado != null ? src.UsuarioDelegado.NombreCompleto : string.Empty))
                .ForMember(dest => dest.Vigente, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.RolesDelegados, opt => opt.MapFrom(src => src.RolesTemporales != null ? src.RolesTemporales.Select(r => r.Rol != null ? r.Rol.Nombre : string.Empty).ToList() : new List<string>()))
                .ForMember(dest => dest.MotivoDelegacionFkId, opt => opt.MapFrom(src => src.MotivoDelegacionFkId))
                .ForMember(dest => dest.MotivoDelegacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaDesde, opt => opt.MapFrom(src => src.FechaDesde))
                .ForMember(dest => dest.FechaHasta, opt => opt.MapFrom(src => src.FechaHasta));

            this.CreateMap<DelegacionTemporalRowDTO, ReporteDelegacionTemporalDTO>()
                .ForMember(dest => dest.IdUsuarioDelegador, opt => opt.MapFrom(src => src.IdUsuarioDelegador))
                .ForMember(dest => dest.ApellidoNombreUsuarioDelegador, opt => opt.MapFrom(src => src.ApellidoNombreUsuarioDelegador))
                .ForMember(dest => dest.IdUsuarioDelegado, opt => opt.MapFrom(src => src.IdUsuarioDelegado))
                .ForMember(dest => dest.ApellidoNombreUsuarioDelegado, opt => opt.MapFrom(src => src.ApellidoNombreUsuarioDelegado))
                .ForMember(dest => dest.MotivoDelegacion, opt => opt.MapFrom(src => src.MotivoDelegacion))
                .ForMember(dest => dest.FechaDesde, opt => opt.MapFrom(src => src.FechaDesde.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.FechaHasta, opt => opt.MapFrom(src => src.FechaHasta.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Vigente, opt => opt.MapFrom(src => src.Vigente))
                .ForMember(dest => dest.RolesDelegados, opt => opt.MapFrom(src => string.Join(", ", src.RolesDelegados)));

            this.CreateMap<GetPaginatedReporteDelegacionTemporalDTO, GetByFiltersReporteDelegacionTemporalDTO>();

            this.CreateMap<DelegacionTemporal, ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>()
                .ForMember(dest => dest.CodigoDelegacion, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Delegado, opt => opt.MapFrom(src => src.UsuarioDelegado!.NombreCompleto))
                .ForMember(dest => dest.FechaDesde, opt => opt.MapFrom(src => src.FechaDesde.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.FechaHasta, opt => opt.MapFrom(src => src.FechaHasta.ToString("yyyy-MM-dd")));
        }
    }
}
