namespace UsuariosAPI.SDK.Contracts.DTOs.CompatibilidadRol
{
    public class ValidarCompatibilidadRolesDTO
    {
        public List<int> RolesOrigenIds { get; set; } = [];

        public List<int> RolesDestinoIds { get; set; } = [];
    }
}
