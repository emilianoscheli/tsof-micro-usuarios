namespace UsuariosAPI.Domain.DelegacionTemporal
{
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Rol;

    public class RolTemporal : LegacyEntity
    {
        required public int DelegacionTemporalId { get; set; }

        public int RolId { get; set; }

        public Rol? Rol { get; set; }

        public DelegacionTemporal? DelegacionTemporal { get; set; }
    }
}
