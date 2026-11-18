namespace UsuariosAPI.Application.Features.Directorio.CommandResolverUsuarioConRol
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;

    public sealed class ResolverUsuarioConRolCommandHandler
        : ICommandHandler<ResolverUsuarioConRolCommand, UsuarioDirectorioResueltoDTO>
    {
        private readonly IDirectorioUsuariosService directorioUsuariosService;

        public ResolverUsuarioConRolCommandHandler(IDirectorioUsuariosService directorioUsuariosService)
        {
            this.directorioUsuariosService = directorioUsuariosService;
        }

        public async Task<IResponse<UsuarioDirectorioResueltoDTO>> Handle(
            ResolverUsuarioConRolCommand request,
            CancellationToken cancellationToken)
        {
            var (usuario, error) = await this.directorioUsuariosService
                .ResolverConRolAsync(request.dto, cancellationToken);

            return error is not null || usuario is null
                ? Response.Failure<UsuarioDirectorioResueltoDTO>(error ?? Error.InvalidAPIResponse)
                : Response.Success(usuario);
        }
    }
}
