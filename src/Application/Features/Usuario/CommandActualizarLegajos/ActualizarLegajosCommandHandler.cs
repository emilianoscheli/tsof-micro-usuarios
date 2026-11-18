namespace UsuariosAPI.Application.Features.Usuario.CommandActualizarLegajos
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using IntegracionesAPI.SDK.Contracts.DTOs.GDIInterno;
    using IntegracionesAPI.SDK.Contracts.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario;

    public sealed class ActualizarLegajosCommandHandler : ICommandHandler<ActualizarLegajosCommand>
    {
        private const int BatchSize = 10;

        private readonly IUsuarioSRCRepository usuarioRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGdiUsuarioInternoService gdiService;
        private readonly ILogger<ActualizarLegajosCommandHandler> logger;

        public ActualizarLegajosCommandHandler(
            IUsuarioSRCRepository usuarioRepository,
            IUnitOfWork unitOfWork,
            IGdiUsuarioInternoService gdiService,
            ILogger<ActualizarLegajosCommandHandler> logger)
        {
            this.usuarioRepository = usuarioRepository;
            this.unitOfWork = unitOfWork;
            this.gdiService = gdiService;
            this.logger = logger;
        }

        public async Task<IResponse> Handle(ActualizarLegajosCommand request, CancellationToken cancellationToken)
        {
            var usuarios = await this.usuarioRepository
                .AsQueryable()
                .Where(u => u.Legajo == null && u.IdRed != null && !u.EsExterno)
                .Select(u => new { u.Id, u.IdRed })
                .ToListAsync(cancellationToken);

            this.logger.LogInformation("[SYNC_LEGAJOS] Procesando {Count} usuarios sin legajo", usuarios.Count);

            var actualizados = 0;

            for (var i = 0; i < usuarios.Count; i += BatchSize)
            {
                var batch = usuarios.Skip(i).Take(BatchSize).ToList();

                var tasks = batch.Select(async u =>
                {
                    try
                    {
                        var response = await this.gdiService.GetUnidadOrganizativaAsync(
                            new GetGdiUnidadOrganizativaRequestDTO { Uid = u.IdRed });

                        if (response?.IsSuccess == true && !string.IsNullOrWhiteSpace(response.Data?.LegajoUsuario))
                        {
                            return (u.Id, Legajo: response.Data.LegajoUsuario);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogWarning(ex, "[SYNC_LEGAJOS] Error al consultar GDI para IdRed={IdRed}", u.IdRed);
                    }

                    return (u.Id, Legajo: (string?)null);
                });

                var results = await Task.WhenAll(tasks);

                foreach (var result in results.Where(r => r.Legajo != null))
                {
                    var entity = await this.usuarioRepository.GetByIdAsync(result.Id);
                    if (entity != null)
                    {
                        entity.Legajo = result.Legajo;
                        actualizados++;
                    }
                }

                await this.unitOfWork.SaveAsync(cancellationToken);
            }

            this.logger.LogInformation("[SYNC_LEGAJOS] Finalizado. {Actualizados}/{Total} legajos actualizados", actualizados, usuarios.Count);

            return Response.Success();
        }
    }
}
