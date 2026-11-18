namespace UsuariosAPI.Application.Abstractions.Messaging
{
    using APICore.Interfaces.Responses;
    using MediatR;

    public interface IQueryPaginatedHandler<TQuery, TResponse> : IRequestHandler<TQuery, IPaginatedResponse<TResponse>>
        where TQuery : IQueryPaginated<TResponse>
    {
    }
}
