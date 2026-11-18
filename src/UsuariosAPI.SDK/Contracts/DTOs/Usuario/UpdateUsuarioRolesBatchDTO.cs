namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class UpdateUsuarioRolesBatchDTO
    {
        required public List<int> UsuariosIds { get; set; }

        required public int RolId { get; set; }
    }
}
