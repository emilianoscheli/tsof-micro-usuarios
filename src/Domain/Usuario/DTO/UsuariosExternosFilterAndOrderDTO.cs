namespace UsuariosAPI.Domain.Usuario.DTO
{
    public class UsuariosExternosFilterAndOrderDTO
    {
        public UsuariosExternosFilterAndOrderDTO()
        {
        }

        public string? Id { get; set; }

        public string? NombreCompleto { get; set; }

        public string? IdRed { get; set; }

        public string? CorreoElectronico { get; set; }

        public int? RolId { get; set; }

        public int? ContratistaId { get; set; }

        public List<int>? ContratistasIds { get; set; }

        public int? EstudioAuditorId { get; set; }

        public int? SedeId { get; set; }

        public string? CodigoIncidencia { get; set; }

        public bool? Activo { get; set; }

        public string? SortBy { get; set; }

        public bool? OrderAsc { get; set; }

        //public string? PalabraClave { get; set; }
    }
}
