namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.Permisos;

    public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
    {
        public void Configure(EntityTypeBuilder<Permiso> builder)
        {
            builder.Property(x => x.Codigo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descripcion)
                .HasMaxLength(250);

            builder.HasIndex(x => x.Codigo)
                .IsUnique();
        }
    }
}
