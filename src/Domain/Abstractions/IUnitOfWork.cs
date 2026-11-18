namespace UsuariosAPI.Domain.Abstractions
{
    using UsuariosAPI.Domain.CompatibilidadRol;
    using UsuariosAPI.Domain.DelegacionTemporal.Repository;
    using UsuariosAPI.Domain.LogTecnico;
    using UsuariosAPI.Domain.Permisos;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.RolPermiso;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;

    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRolRepository UsuarioRolRepository { get; }

        IUsuarioSRCRepository UsuarioSRCRepository { get; }

        ILogTecnicoRepository LogTecnicoRepository { get; }

        IRolPermisoRepository RolPermisoRepository { get; }

        IPermisoRepository PermisoRepository { get; }

        IRolRepository RolRepository { get; }

        IUsuarioSociedadRepository UsuarioSociedadRepository { get; }

        IDelegacionTemporalRepository DelegacionTemporalRepository { get; }

        IRolTemporalRepository RolTemporalRepository { get; }

        ICompatibilidadRolRepository CompatibilidadRolRepository { get; }

        Task<int> SaveAsync(CancellationToken cancellationToken = default, bool withLogTecnico = true, string? logTecnicoComment = null);
    }
}
