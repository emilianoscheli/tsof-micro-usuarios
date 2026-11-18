namespace UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol
{
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuariosRolesDTO
    {
        public RolDetailsDTO? Rol { get; set; }

        required public int UsuarioSRCId { get; set; }
    }
}
