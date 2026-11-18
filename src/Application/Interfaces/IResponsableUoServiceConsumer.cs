namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Responses;
    using UsuariosAPI.SDK.Contracts.DTOs.ResponsableUO;

    public interface IResponsableUoServiceConsumer
    {
        Task<(ResponsableUoMaestroDTO?, Error?)> ValidateResponsableUoByCodigo(string codigo, CancellationToken cancellationToken);
    }
}
