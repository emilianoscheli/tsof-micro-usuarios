namespace UsuariosAPI.Application.Abstractions.Messaging
{
    using APICore.Interfaces.Responses;
    using MediatR;

    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IResponse<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
