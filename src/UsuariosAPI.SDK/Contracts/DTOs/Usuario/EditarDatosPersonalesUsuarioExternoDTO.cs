namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class EditarDatosPersonalesUsuarioExternoDTO
    {
        public EditarDatosPersonalesUsuarioExternoDTO()
        {
        }

        public int UsuarioId { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public int? Pais { get; set; }

        public string? EmailEmpresa { get; set; }

        public string? TelefonoParticular { get; set; }

        public int? ContratistaId { get; set; }

        // Solo para rol Estudio Auditor
        public int? EstudioAuditorId { get; set; }

        public int? SedeId { get; set; }
    }
}
