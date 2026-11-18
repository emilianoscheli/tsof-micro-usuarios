namespace UsuariosAPI.SDK.Contracts.DTOs.Directorio
{
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuarioDirectorioResueltoDTO
    {
        public UsuarioDTO Usuario { get; set; } = new UsuarioDTO();

        /// <summary>
        /// True cuando el usuario no existía en SRC y fue creado a partir de AdHelper.
        /// </summary>
        public bool UsuarioCreado { get; set; }

        /// <summary>
        /// True cuando el usuario ya tenía el rol solicitado.
        /// </summary>
        public bool YaTeniaRol { get; set; }

        /// <summary>
        /// True cuando el rol fue asignado durante esta operación.
        /// </summary>
        public bool RolAsignado { get; set; }
    }
}
