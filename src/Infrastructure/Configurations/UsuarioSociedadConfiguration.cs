namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.UsuarioSociedad;

    public sealed class UsuarioSociedadConfiguration : IEntityTypeConfiguration<UsuarioSociedad>
    {
        public void Configure(EntityTypeBuilder<UsuarioSociedad> builder)
        {
            builder.HasIndex(x => new { x.UsuarioSRCId, x.SociedadFkId }).IsUnique();

            builder.HasOne(x => x.UsuarioSRC)
                    .WithMany(y => y.Sociedades)
                    .HasForeignKey(x => x.UsuarioSRCId);
        }
    }
}
