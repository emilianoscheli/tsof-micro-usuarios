namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    using APICore.Requests;

    public class GetPaginatedReporteUsuariosRolesSRCDTO : PaginatedRequest
    {
        public GetPaginatedReporteUsuariosRolesSRCDTO()
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
