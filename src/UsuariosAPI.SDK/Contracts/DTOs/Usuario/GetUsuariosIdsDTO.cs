namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class GetUsuariosIdsDTO
    {
        public int? Codigo { get; set; }

        public string? NombreCompleto { get; set; }

        public string? IdRed { get; set; }

        public string? CorreoElectronico { get; set; }

        public int? RolId { get; set; }

        public int? SociedadId { get; set; }

        public int? ContratistaId { get; set; }

        public List<int>? ContratistasIds { get; set; }

        public int? EstudioAuditorId { get; set; }

        public int? SedeId { get; set; }

        public string? CodigoUO { get; set; }

        public string? PalabraClave { get; set; }
    }
}
