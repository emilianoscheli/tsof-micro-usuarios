namespace UsuariosAPI.Infrastructure.Repositories
{
    using APICore.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.CompatibilidadRol;
    using UsuariosAPI.Domain.DelegacionTemporal.Repository;
    using UsuariosAPI.Domain.LogTecnico;
    using UsuariosAPI.Domain.Permisos;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.RolPermiso;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;
    using UsuariosAPI.Infrastructure.Repositories.CompatibilidadRol;
    using UsuariosAPI.Infrastructure.Repositories.DelegacionTemporal;
    using UsuariosAPI.Infrastructure.Repositories.LogTecnico;
    using UsuariosAPI.Infrastructure.Repositories.Permiso;
    using UsuariosAPI.Infrastructure.Repositories.Rol;
    using UsuariosAPI.Infrastructure.Repositories.RolPermiso;
    using UsuariosAPI.Infrastructure.Repositories.UsuarioRol;
    using UsuariosAPI.Infrastructure.Repositories.UsuarioSociedad;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private ILogTecnicoRepository? logTecnicoRepository = null;
        private IUsuarioRolRepository? usuarioRolRepository = null;
        private IUsuarioSRCRepository? usuarioSRCRepository = null;
        private IRolRepository? rolRepository = null;
        private IPermisoRepository? permisoRepository = null;
        private IRolPermisoRepository? rolPermisoRepository = null;
        private IUsuarioSociedadRepository? usuarioSociedadRepository = null;
        private IRolTemporalRepository? rolTemporalRepository = null;
        private IDelegacionTemporalRepository? delegacionTemporalRepository = null;
        private ICompatibilidadRolRepository? compatibilidadRolRepository = null;

        public UnitOfWork(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ILogTecnicoRepository LogTecnicoRepository => this.logTecnicoRepository ??= new LogTecnicoRepository(this.context, this.httpContextAccessor);

        public IUsuarioRolRepository UsuarioRolRepository => this.usuarioRolRepository ??= new UsuarioRolRepository(this.context);

        public IUsuarioSRCRepository UsuarioSRCRepository => this.usuarioSRCRepository ??= new UsuarioSRCRepository(this.context);

        public IRolRepository RolRepository => this.rolRepository ??= new RolRepository(this.context);

        public IPermisoRepository PermisoRepository => this.permisoRepository ??= new PermisoRepository(this.context);

        public IUsuarioSociedadRepository UsuarioSociedadRepository => this.usuarioSociedadRepository ??= new UsuarioSociedadRepository(this.context);

        public IRolPermisoRepository RolPermisoRepository => this.rolPermisoRepository ??= new RolPermisoRepository(this.context);

        public IRolTemporalRepository RolTemporalRepository => this.rolTemporalRepository ??= new RolTemporalRepository(this.context);

        public IDelegacionTemporalRepository DelegacionTemporalRepository => this.delegacionTemporalRepository ??= new DelegacionTemporalRepository(this.context);

        public ICompatibilidadRolRepository CompatibilidadRolRepository => this.compatibilidadRolRepository ??= new CompatibilidadRolRepository(this.context);

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default, bool withLogTecnico = true, string? logTecnicoComment = null)
        {
            foreach (var entry in this.context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is Entity entity)
                    {
                        entity.UsuarioId = this.httpContextAccessor.HttpContext?.GetUsuarioId();
                        entity.NombreUsuario = this.httpContextAccessor.HttpContext?.GetNombreUsuario();
                    }
                }
            }

            if (withLogTecnico)
            {
                await this.LogTecnicoRepository.CreateLog(logTecnicoComment);
            }

            return await this.context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
