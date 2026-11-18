namespace UsuariosAPI.Domain.RolPermiso
{
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Permisos;
    using UsuariosAPI.Domain.Rol;

    public sealed class RolPermiso : Entity
    {
        public RolPermiso()
        {
        }

        public Rol? Rol { get; set; }

        required public int RolId { get; set; }

        public Permiso? Permiso { get; set; }

        required public int PermisoId { get; set; }
    }
}
