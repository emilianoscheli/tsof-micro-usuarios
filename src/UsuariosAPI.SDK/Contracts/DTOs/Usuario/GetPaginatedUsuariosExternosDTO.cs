namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    using APICore.Requests;

    public class GetPaginatedUsuariosExternosDTO : PaginatedOrderedRequest
    {
        public GetPaginatedUsuariosExternosDTO()
        {
        }

        public int? Codigo { get; set; }

        public string? NombreCompleto { get; set; }

        public string? IdRed { get; set; }

        public string? CorreoElectronico { get; set; }

        public int? RolId { get; set; }

        public int? ContratistaId { get; set; }

        public string? CodigoIncidencia { get; set; }

        public int? EstudioAuditorId { get; set; }

        public int? SedeId { get; set; }

        public bool? Activo { get; set; }
    }
}
