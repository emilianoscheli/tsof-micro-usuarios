namespace UsuariosAPI.Domain.Usuario.DTO
{
    public class GetByFiltersReporteUsuariosRolesSRCDTO
    {
        public GetByFiltersReporteUsuariosRolesSRCDTO()
        {
        }

        public List<int>? UsuariosIds { get; set; }

        public List<int>? RolesIds { get; set; }

        public List<int>? ContratistasIds { get; set; }

        public List<int>? EstudiosAuditoresIds { get; set; }

        public bool? Activo { get; set; }

        public int UsuarioId { get; set; }
    }
}
