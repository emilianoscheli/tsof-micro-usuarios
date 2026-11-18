namespace UsuariosAPI.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using UsuariosAPI.Domain.LogTecnico;

    public class LogTecnicoConfiguration : IEntityTypeConfiguration<LogTecnico>
    {
        public void Configure(EntityTypeBuilder<LogTecnico> builder)
        {
            builder.Property(x => x.Entidad).HasMaxLength(100);
            builder.Property(x => x.Comentario).HasMaxLength(255);
        }
    }
}
