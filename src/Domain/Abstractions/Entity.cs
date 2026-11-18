namespace UsuariosAPI.Domain.Abstractions
{
    public abstract class Entity
    {
        protected Entity()
        {
        }

        protected Entity(int? usuarioId, string? nombreUsuario)
        {
            this.UsuarioId = usuarioId;
            this.NombreUsuario = nombreUsuario;
        }

        public int Id { get; init; }

        public DateTime FechaCreacion { get; init; } = DateTime.Now;

        public int? UsuarioId { get; set; }

        public string? NombreUsuario { get; set; }
    }
}