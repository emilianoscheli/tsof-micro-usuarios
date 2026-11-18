namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.DelegacionTemporal;

    public class RolTemporalConfiguration : IEntityTypeConfiguration<RolTemporal>
    {
        public void Configure(EntityTypeBuilder<RolTemporal> builder)
        {
            builder.HasOne(x => x.DelegacionTemporal)
                .WithMany(x => x.RolesTemporales)
                .HasForeignKey(x => x.DelegacionTemporalId);
        }
    }
}
