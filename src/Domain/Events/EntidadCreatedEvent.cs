namespace UsuariosAPI.Domain.Events
{
    using MediatR;

    public class EntidadCreatedEvent : INotification
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Status { get; set; }
    }
}
