namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.Usuario;

    public class UsuarioSRCConfiguration : IEntityTypeConfiguration<UsuarioSRC>
    {
        public void Configure(EntityTypeBuilder<UsuarioSRC> builder)
        {
            builder.Property(x => x.NombreCompleto)
                .IsRequired()
                .HasMaxLength(100);

            // Nota: el endpoint de integraciones debería validar que "permite letras y números, signo @ y puntos. No permite caracteres especiales además de los mencionados."
            builder.Property(x => x.CorreoElectronico)
                .IsRequired();

            builder.Property(x => x.IdRed)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(true);

            builder.HasIndex(x => x.IdRed)
                .IsUnique();

            builder.Property(x => x.CodigoUO).HasMaxLength(100);

            // Campos para usuarios externos
            builder.Property(x => x.EsExterno)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.NumeroDocumento)
                .HasMaxLength(80);

            builder.Property(x => x.Cuil)
                .HasMaxLength(15);

            builder.Property(x => x.TelefonoParticular)
                .HasMaxLength(80);

            builder.Property(x => x.CodigoIncidencia)
                .HasMaxLength(200);

            builder.Property(x => x.Legajo)
                .HasMaxLength(20);

            builder.Property(x => x.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasMany(x => x.DelegacionTemporales)
                .WithOne(d => d.UsuarioDelegado)
                .HasForeignKey(d => d.UsuarioDelegadoId);
        }
    }
}
