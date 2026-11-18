namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class DesactivarUsuarioExternoDTO
    {
        public DesactivarUsuarioExternoDTO()
        {
        }

        public int UsuarioId { get; set; }

        public string? CodigoIncidencia { get; set; }

        public string Comentario { get; set; } = string.Empty;
    }
}
