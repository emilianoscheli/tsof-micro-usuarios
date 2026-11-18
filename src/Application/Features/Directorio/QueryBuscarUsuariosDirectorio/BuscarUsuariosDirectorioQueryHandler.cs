namespace UsuariosAPI.Application.Features.Directorio.QueryBuscarUsuariosDirectorio
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;

    public sealed class BuscarUsuariosDirectorioQueryHandler
        : IQueryHandler<BuscarUsuariosDirectorioQuery, List<UsuarioDirectorioDTO>>
    {
        private readonly IDirectorioUsuariosService directorioUsuariosService;

        public BuscarUsuariosDirectorioQueryHandler(IDirectorioUsuariosService directorioUsuariosService)
        {
            this.directorioUsuariosService = directorioUsuariosService;
        }

        public async Task<IResponse<List<UsuarioDirectorioDTO>>> Handle(
            BuscarUsuariosDirectorioQuery request,
            CancellationToken cancellationToken)
        {
            var (usuarios, error) = await this.directorioUsuariosService
                .BuscarAsync(request.dto, cancellationToken);

            return error is not null
                ? Response.Failure<List<UsuarioDirectorioDTO>>(error)
                : Response.Success(usuarios);
        }
    }
}
