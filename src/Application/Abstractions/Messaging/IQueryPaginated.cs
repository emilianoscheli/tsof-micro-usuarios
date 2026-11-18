namespace UsuariosAPI.Application.Abstractions.Messaging
{
    using APICore.Interfaces.Responses;
    using MediatR;

    public interface IQueryPaginated<TResponse> : IRequest<IPaginatedResponse<TResponse>>
    {
    }
}
