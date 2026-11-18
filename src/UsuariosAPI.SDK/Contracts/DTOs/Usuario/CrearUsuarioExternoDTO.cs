namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class CrearUsuarioExternoDTO
    {
        public CrearUsuarioExternoDTO()
        {
        }

        public int RolId { get; set; }

        public int TipoDocumentoId { get; set; }

        public string NumeroDocumento { get; set; } = string.Empty;

        public string CUIL { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public int PaisId { get; set; }

        public string EmailUsuario { get; set; } = string.Empty;

        public string? TelefonoParticular { get; set; }

        public string? CodigoIncidencia { get; set; }

        public int ContratistaId { get; set; }

        // Solo para rol Estudio Auditor
        public int? EstudioAuditorId { get; set; }

        public int? SedeId { get; set; }
    }
}
