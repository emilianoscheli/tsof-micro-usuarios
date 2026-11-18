namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.RolPermiso;

    public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
    {
        public void Configure(EntityTypeBuilder<RolPermiso> builder)
        {
            builder.HasIndex(x => new { x.PermisoId, x.RolId }).IsUnique();

            builder.HasOne(x => x.Permiso)
                    .WithMany()
                    .HasForeignKey(x => x.PermisoId);

            builder.HasOne(x => x.Rol)
                   .WithMany(y => y.Permisos)
                   .HasForeignKey(x => x.RolId);
        }
    }
}
