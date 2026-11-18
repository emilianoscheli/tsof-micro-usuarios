namespace UsuariosAPI.Application.Features.AdUsuarios.QueryObtenerAdUsuarioPorIdRed
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;

    public class ObtenerAdUsuarioPorIdRedQueryHandler : IQueryHandler<ObtenerAdUsuarioPorIdRedQuery, AdUsuarioDto>
    {
        private readonly IExternalDirectoryService externalDirectoryService;

        public ObtenerAdUsuarioPorIdRedQueryHandler(IExternalDirectoryService externalDirectoryService)
        {
            this.externalDirectoryService = externalDirectoryService;
        }

        public async Task<IResponse<AdUsuarioDto>> Handle(ObtenerAdUsuarioPorIdRedQuery request, CancellationToken cancellationToken)
        {
            var (resultados, adError) = await this.externalDirectoryService.Search(request.IdRed);

            if (adError != null)
            {
                return Response.Failure<AdUsuarioDto>(adError);
            }

            var usuario = resultados.FirstOrDefault(u =>
                string.Equals(u.IdRed, request.IdRed, StringComparison.OrdinalIgnoreCase));

            if (usuario is null)
            {
                return Response.Failure<AdUsuarioDto>(UsuariosError.EntidadNoEncontrada("Usuario AD"));
            }

            return Response.Success(usuario);
        }
    }
}
