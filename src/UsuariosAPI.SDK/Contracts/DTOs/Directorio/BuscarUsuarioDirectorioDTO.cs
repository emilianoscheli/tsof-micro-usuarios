namespace UsuariosAPI.SDK.Contracts.DTOs.Directorio
{
    public class BuscarUsuarioDirectorioDTO
    {
        public string PalabraClave { get; set; } = string.Empty;

        /// <summary>
        /// Rol que se pretende asignar. Se usa para informar compatibilidad, no para filtrar.
        /// </summary>
        public int? RolDestinoId { get; set; }
    }
}
