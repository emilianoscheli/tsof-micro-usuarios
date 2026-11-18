namespace UsuariosAPI.Domain.CompatibilidadRol
{
    using UsuariosAPI.Domain.Abstractions;

    public sealed class CompatibilidadRol : Entity
    {
        public CompatibilidadRol()
        {
        }

        required public int RolOrigenId { get; set; }

        required public int RolDestinoId { get; set; }

        public Rol.Rol? RolOrigen { get; set; }

        public Rol.Rol? RolDestino { get; set; }
    }
}
