namespace UsuariosAPI.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.CompatibilidadRol;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.LogTecnico;
    using UsuariosAPI.Domain.Permisos;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.RolPermiso;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;
    using UsuariosAPI.Infrastructure.Extensions;

    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<LogTecnico> LogTecnico { get; set; }

        public DbSet<UsuarioSRC> UsuarioSRC { get; set; }

        public DbSet<UsuarioRol> UsuarioRol { get; set; }

        public DbSet<Rol> Rol { get; set; }

        public DbSet<RolPermiso> RolPermiso { get; set; }

        public DbSet<Permiso> Permiso { get; set; }

        public DbSet<UsuarioSociedad> UsuarioSociedad { get; set; }

        public DbSet<RolTemporal> RolTemporal { get; set; }

        public DbSet<DelegacionTemporal> DelegacionTemporal { get; set; }

        public DbSet<CompatibilidadRol> CompatibilidadRol { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Entity>();

            modelBuilder.ApplyEntityConfiguration();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
