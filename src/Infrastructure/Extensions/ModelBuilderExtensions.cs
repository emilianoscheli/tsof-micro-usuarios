namespace UsuariosAPI.Infrastructure.Extensions
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.Abstractions;

    public static class ModelBuilderExtensions
    {
        public static void ApplyEntityConfiguration(this ModelBuilder modelBuilder)
        {
            // Get all entity types that inherit from the base class 'LegacyEntity'
            var legacyEntityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(LegacyEntity).IsAssignableFrom(e.ClrType) && e.ClrType != typeof(LegacyEntity));

            // Get all entity types that inherit from the base class 'Entity'
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(Entity).IsAssignableFrom(e.ClrType) && e.ClrType != typeof(Entity));

            // Apply configuration to all derived types
            foreach (var legacyEntityType in legacyEntityTypes)
            {
                modelBuilder.Entity(legacyEntityType.ClrType).HasIndex(nameof(LegacyEntity.LegacyId)).IsUnique();
            }

            foreach (var entityType in entityTypes)
            {
                modelBuilder.Entity(entityType.ClrType).Property(nameof(Entity.FechaCreacion)).HasPrecision(0);  // hasta segundos, ejemplo 2024-10-22 14:35:21

                modelBuilder.Entity(entityType.ClrType).HasKey(nameof(Entity.Id));

                modelBuilder.Entity(entityType.ClrType).Property(nameof(Entity.NombreUsuario)).HasMaxLength(100);
            }
        }
    }
}
