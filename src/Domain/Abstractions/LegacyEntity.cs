namespace UsuariosAPI.Domain.Abstractions
{
    public abstract class LegacyEntity : Entity
    {
        public int? LegacyId { get; init; }
    }
}
