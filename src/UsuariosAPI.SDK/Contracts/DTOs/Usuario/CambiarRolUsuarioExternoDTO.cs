namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class CambiarRolUsuarioExternoDTO
    {
        public CambiarRolUsuarioExternoDTO()
        {
        }

        public int UsuarioId { get; set; }

        public int NuevoRolId { get; set; }

        // Solo para cambio a rol Estudio Auditor
        public int? EstudioAuditorId { get; set; }

        public int? SedeId { get; set; }
    }
}
