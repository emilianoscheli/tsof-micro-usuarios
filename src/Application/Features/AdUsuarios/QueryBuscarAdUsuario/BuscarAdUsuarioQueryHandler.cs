namespace UsuariosAPI.Application.Features.AdUsuarios.QueryBuscarAdUsuario
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;

    public class BuscarAdUsuarioQueryHandler : IQueryHandler<BuscarAdUsuarioQuery, List<AdUsuarioDto>>
    {
        private readonly IExternalDirectoryService externalDirectoryService;
        private readonly IUnitOfWork unitOfWork;

        public BuscarAdUsuarioQueryHandler(
            IExternalDirectoryService externalDirectoryService,
            IUnitOfWork unitOfWork)
        {
            this.externalDirectoryService = externalDirectoryService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IResponse<List<AdUsuarioDto>>> Handle(BuscarAdUsuarioQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.palabraClave))
            {
                return Response.Failure<List<AdUsuarioDto>>(Error.InvalidInput("Debe proporcionar una palabra clave."));
            }

            var (usuariosAd, adError) = await this.externalDirectoryService.Search(request.palabraClave);

            if (adError != null)
            {
                return Response.Failure<List<AdUsuarioDto>>(adError);
            }

            if (usuariosAd.Count == 0)
            {
                return Response.Success(usuariosAd);
            }

            // Obtener los IdRed que ya están registrados en SRC para filtrarlos
            var idRedsAd = usuariosAd.Select(u => u.IdRed).ToList();

            var idRedsExistentes = await this.unitOfWork.UsuarioSRCRepository
                .GetIdRedsExistentesAsync(idRedsAd, cancellationToken);

            var usuariosFiltrados = usuariosAd
                .Where(u => !idRedsExistentes.Contains(u.IdRed.Trim().ToUpper()))
                .ToList();

            return Response.Success(usuariosFiltrados);
        }
    }
}
