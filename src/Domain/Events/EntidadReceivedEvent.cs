namespace UsuariosAPI.Domain.Events
{
    using MediatR;

    public class EntidadReceivedEvent : INotification
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Status { get; set; }
    }
}
