namespace UsuariosAPI.Domain.Usuario
{
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;

    public sealed class UsuarioSRC : Entity
    {
        public UsuarioSRC()
        {
        }

        required public string NombreCompleto { get; set; }

        required public string IdRed { get; set; }

        required public string CorreoElectronico { get; set; }

        public List<UsuarioRol>? Roles { get; set; } = new List<UsuarioRol>();

        public List<UsuarioSociedad>? Sociedades { get; set; } = new List<UsuarioSociedad>();

        public int? EstudioAuditorFkId { get; set; }

        public int? SedeFkId { get; set; }

        public int? ContratistaFkId { get; set; }

        public string? CodigoUO { get; set; }

        public List<DelegacionTemporal>? DelegacionTemporales { get; set; } = new List<DelegacionTemporal>();

        // Campos para usuarios externos
        public bool EsExterno { get; set; } = false;

        public int? TipoDocumentoFkId { get; set; }

        public string? NumeroDocumento { get; set; }

        public string? Cuil { get; set; }

        public int? PaisFkId { get; set; } // es del contratista

        public string? TelefonoParticular { get; set; } // del usuario

        public string? CodigoIncidencia { get; set; }

        public string? Legajo { get; set; }

        public bool Activo { get; set; } = true;
    }
}
