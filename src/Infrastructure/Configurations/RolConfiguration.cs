namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.Rol;

    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descripcion)
                .HasMaxLength(250);

            builder
                .HasMany(x => x.Permisos);
        }
    }
}
