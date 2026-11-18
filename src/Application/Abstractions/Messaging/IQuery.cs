namespace UsuariosAPI.Application.Abstractions.Messaging
{
    using APICore.Interfaces.Responses;
    using MediatR;

    public interface IQuery<TResponse> : IRequest<IResponse<TResponse>>
    {
    }
}
