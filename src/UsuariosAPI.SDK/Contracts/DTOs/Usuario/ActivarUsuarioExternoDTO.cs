namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class ActivarUsuarioExternoDTO
    {
        public ActivarUsuarioExternoDTO()
        {
        }

        public int UsuarioId { get; set; }

        public string? CodigoIncidencia { get; set; }

        public int RolId { get; set; }
    }
}
