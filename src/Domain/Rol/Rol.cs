namespace UsuariosAPI.Domain.Rol
{
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.RolPermiso;

    public sealed class Rol : Entity
    {
        public Rol()
        {
        }

        required public string Nombre { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public List<RolPermiso> Permisos { get; set; } = new List<RolPermiso>();

        required public bool AsignaSociedad { get; set; } = false;

        required public bool EsManual { get; set; } = false;
    }
}
