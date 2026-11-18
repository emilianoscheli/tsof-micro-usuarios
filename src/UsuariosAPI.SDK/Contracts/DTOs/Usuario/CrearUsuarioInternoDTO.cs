namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class CrearUsuarioInternoDTO
    {
        /// <summary>
        /// ID de red del usuario interno (proveniente de GDI ADHelper).
        /// Los datos de nombre completo y correo electrónico se obtienen automáticamente desde el servicio ADHelper de Contratos.
        /// </summary>
        public string IdRed { get; set; } = string.Empty;

        /// <summary>
        /// Roles manuales a asignar al usuario (opcional, se puede crear sin roles).
        /// </summary>
        public List<int>? RolesIds { get; set; }

        /// <summary>
        /// Sociedades a asignar al usuario (requerido si algún rol las requiere).
        /// </summary>
        public List<int>? SociedadesIds { get; set; }
    }
}