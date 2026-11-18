namespace UsuariosAPI.Application.Interfaces.CommonServices
{
    using APICore.Responses;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;

    /// <summary>
    /// Punto único de resolución de usuarios internos: combina la BD de usuarios SRC con AdHelper
    /// y centraliza la creación del usuario y la asignación del rol correspondiente.
    /// </summary>
    public interface IDirectorioUsuariosService
    {
        /// <summary>
        /// Busca usuarios internos combinando la BD SRC y AdHelper. Los resultados de SRC tienen prioridad
        /// y se completan con los de AdHelper que aún no existen en SRC.
        /// </summary>
        Task<(List<UsuarioDirectorioDTO> Usuarios, Error? Error)> BuscarAsync(
            BuscarUsuarioDirectorioDTO dto,
            CancellationToken cancellationToken);

        /// <summary>
        /// Resuelve el usuario y garantiza que tenga el rol destino:
        /// existe con rol -> continúa; existe sin rol -> valida compatibilidad y lo asigna;
        /// no existe -> lo busca en AdHelper, lo crea y le asigna el rol.
        /// </summary>
        Task<(UsuarioDirectorioResueltoDTO? Usuario, Error? Error)> ResolverConRolAsync(
            ResolverUsuarioConRolDTO dto,
            CancellationToken cancellationToken);
    }
}
