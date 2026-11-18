namespace UsuariosAPI.Domain.UsuarioSociedad
{
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario;

    public sealed class UsuarioSociedad : Entity
    {
        public UsuarioSociedad()
        {
        }

        public int UsuarioSRCId { get; set; }

        public UsuarioSRC? UsuarioSRC { get; set; }

        public int SociedadFkId { get; set; }
    }
}
