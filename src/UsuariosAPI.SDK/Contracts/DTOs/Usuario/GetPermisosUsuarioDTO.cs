namespace UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol
{
    using APICore.Requests;

    public class GetPermisosUsuarioDTO
    {
        public GetPermisosUsuarioDTO()
        {
        }

        required public string IdRed { get; init; }
    }
}
