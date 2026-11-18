namespace UsuariosAPI.Domain.Permisos
{
    using UsuariosAPI.Domain.Abstractions;

    public sealed class Permiso : Entity
    {
        public Permiso()
        {
        }

        required public string Codigo { get; set; }

        public string Descripcion { get; set; } = string.Empty;
    }
}
