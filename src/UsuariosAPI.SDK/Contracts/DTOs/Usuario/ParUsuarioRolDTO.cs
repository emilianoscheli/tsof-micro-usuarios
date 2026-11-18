namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class ParUsuarioRolDTO
    {
        public ParUsuarioRolDTO()
        {
        }

        required public int UsuarioId { get; set; }

        required public int RolId { get; set; }
    }
}
