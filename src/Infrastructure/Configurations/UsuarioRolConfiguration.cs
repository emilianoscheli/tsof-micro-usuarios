namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.UsuarioRol;

    public sealed class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
    {
        public void Configure(EntityTypeBuilder<UsuarioRol> builder)
        {
            builder.HasIndex(x => new { x.UsuarioSRCId, x.RolId }).IsUnique();

            builder.HasOne(x => x.UsuarioSRC)
                    .WithMany(y => y.Roles)
                    .HasForeignKey(x => x.UsuarioSRCId);

            builder.HasOne(x => x.Rol)
                   .WithMany()
                   .HasForeignKey(x => x.RolId);
        }
    }
}
