namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class UpdateUsuarioRolesSociedadesDTO
    {
        required public int UsuarioSRCId { get; set; }

        public List<int>? RolesIds { get; set; }

        public List<int>? SociedadesIds { get; set; }
    }
}
