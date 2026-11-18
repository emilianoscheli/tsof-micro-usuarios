namespace UsuariosAPI.SDK.Contracts.DTOs.Directorio
{
    /// <summary>
    /// Resultado unificado de la búsqueda de usuarios internos (BD SRC + AdHelper).
    /// </summary>
    public class UsuarioDirectorioDTO
    {
        /// <summary>
        /// Id del usuario en la BD SRC. Null cuando el usuario sólo existe en AdHelper.
        /// </summary>
        public int? UsuarioId { get; set; }

        public string IdRed { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string? CorreoElectronico { get; set; }

        public string? CodigoUO { get; set; }

        public string? Legajo { get; set; }

        public bool ExisteEnSrc { get; set; }

        public bool Activo { get; set; } = true;

        public List<int> RolesIds { get; set; } = [];

        /// <summary>
        /// True si el usuario ya posee el rol solicitado en la búsqueda.
        /// </summary>
        public bool TieneRolDestino { get; set; }

        /// <summary>
        /// True si el rol solicitado puede asignarse al usuario sin conflicto de compatibilidad.
        /// </summary>
        public bool EsCompatibleConRolDestino { get; set; } = true;
    }
}
