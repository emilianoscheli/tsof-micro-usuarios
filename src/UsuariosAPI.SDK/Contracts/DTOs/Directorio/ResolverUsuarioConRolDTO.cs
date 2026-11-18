namespace UsuariosAPI.SDK.Contracts.DTOs.Directorio
{
    public class ResolverUsuarioConRolDTO
    {
        /// <summary>
        /// IdRed del usuario. Tiene prioridad sobre <see cref="UsuarioId"/>.
        /// </summary>
        public string? IdRed { get; set; }

        /// <summary>
        /// Id del usuario en la BD SRC, cuando la UI ya trabaja con usuarios existentes.
        /// </summary>
        public int? UsuarioId { get; set; }

        public int RolDestinoId { get; set; }

        public string? CodigoUO { get; set; }
    }
}
