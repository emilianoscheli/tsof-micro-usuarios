namespace UsuariosAPI.SDK.Contracts.DTOs.UsuarioSociedad
{
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;

    public class UsuarioSociedadDTO
    {
        public SociedadUsuarioDTO? Sociedad { get; set; }

        required public int UsuarioSRCId { get; set; }
    }
}
