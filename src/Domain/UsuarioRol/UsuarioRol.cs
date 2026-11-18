namespace UsuariosAPI.Domain.UsuarioRol
{
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.Usuario;

    public sealed class UsuarioRol : Entity
    {
        public UsuarioRol()
        {
        }

        public UsuarioSRC? UsuarioSRC { get; set; }

        required public int UsuarioSRCId { get; set; }

        public Rol? Rol { get; set; }

        required public int RolId { get; set; }
    }
}
