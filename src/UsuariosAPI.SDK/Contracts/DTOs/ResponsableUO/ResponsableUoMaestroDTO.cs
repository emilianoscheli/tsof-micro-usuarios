namespace UsuariosAPI.SDK.Contracts.DTOs.ResponsableUO
{
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class ResponsableUoMaestroDTO
    {
        public ResponsableUoMaestroDTO()
        {
        }

        public int BdOrigenId { get; set; } = 0;

        public string UnidadOrganizativa { get; set; } = string.Empty;

        public string CodigoIntegracion { get; set; } = string.Empty;

        public UsuarioDTO? Rdi { get; set; }

        public UsuarioDTO? Bp1 { get; set; }

        public UsuarioDTO? Bp2 { get; set; }

        public List<UsuarioDTO?> Gip { get; set; } = new List<UsuarioDTO?>() { };

        public bool? Bp1ParticipaFlujo { get; set; }
    }
}
