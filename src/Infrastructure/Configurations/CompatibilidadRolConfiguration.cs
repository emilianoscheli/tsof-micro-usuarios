namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.CompatibilidadRol;

    public class CompatibilidadRolConfiguration : IEntityTypeConfiguration<CompatibilidadRol>
    {
        public void Configure(EntityTypeBuilder<CompatibilidadRol> builder)
        {
            builder.ToTable("CompatibilidadRol");

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => new { c.RolOrigenId, c.RolDestinoId }).IsUnique();

            builder.HasOne(c => c.RolOrigen)
                .WithMany()
                .HasForeignKey(c => c.RolOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.RolDestino)
                .WithMany()
                .HasForeignKey(c => c.RolDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
