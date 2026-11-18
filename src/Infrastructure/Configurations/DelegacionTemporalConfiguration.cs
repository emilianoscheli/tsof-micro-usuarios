namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.DelegacionTemporal;

    public class DelegacionTemporalConfiguration : IEntityTypeConfiguration<DelegacionTemporal>
    {
        public void Configure(EntityTypeBuilder<DelegacionTemporal> builder)
        {
            builder.Property(x => x.Observacion)
                .HasMaxLength(250);

            builder.Property(x => x.FechaDesde).HasPrecision(0);
            builder.Property(x => x.FechaHasta).HasPrecision(0);

            builder.HasOne(x => x.UsuarioDelegado)
                .WithMany(d => d.DelegacionTemporales)
                .HasForeignKey(x => x.UsuarioDelegadoId)
                .HasPrincipalKey(u => u.Id);

            builder.HasOne(x => x.UsuarioDelegador)
                .WithMany()
                .HasForeignKey(x => x.UsuarioDelegadorId);

            builder.HasMany(c => c.RolesTemporales)
                .WithOne()
                .HasForeignKey(d => d.DelegacionTemporalId);
        }
    }
}
