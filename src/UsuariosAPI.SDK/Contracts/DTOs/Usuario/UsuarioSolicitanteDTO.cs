namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class UsuarioSolicitanteDTO
    {
        public UsuarioSolicitanteDTO()
        {
        }

        public int UsuarioCodigo { get; set; } = 0;

        public string UsuarioNombreCompleto { get; set; } = string.Empty;

        public string UoUsuario { get; set; } = string.Empty;

        public string RDIUo { get; set; } = string.Empty;
    }
}
