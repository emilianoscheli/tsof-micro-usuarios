namespace UsuariosAPI.Application.Features.Usuario.CommandAsegurarUsuarioInterno
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed class AsegurarUsuarioInternoCommandHandler : ICommandHandler<AsegurarUsuarioInternoCommand, UsuarioDTO>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IExternalDirectoryService externalDirectoryService;
        private readonly IMapper mapper;

        public AsegurarUsuarioInternoCommandHandler(
            IUnitOfWork unitOfWork,
            IExternalDirectoryService externalDirectoryService,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.externalDirectoryService = externalDirectoryService;
            this.mapper = mapper;
        }

        public async Task<IResponse<UsuarioDTO>> Handle(
            AsegurarUsuarioInternoCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.dto.IdRed))
            {
                return Response.Failure<UsuarioDTO>(UsuariosError.RequestInvalido);
            }

            var idRed = request.dto.IdRed.Trim();
            var codigoUo = request.dto.CodigoUO?.Trim();

            var usuarioExistente = await this.unitOfWork.UsuarioSRCRepository
                .GetByIdRedWithAllDataAsync(idRed, cancellationToken);

            if (usuarioExistente is not null)
            {
                if (!usuarioExistente.Activo)
                {
                    usuarioExistente.Activo = true;
                }

                if (!string.IsNullOrWhiteSpace(codigoUo) &&
                    !string.Equals(usuarioExistente.CodigoUO, codigoUo, StringComparison.InvariantCultureIgnoreCase))
                {
                    usuarioExistente.CodigoUO = codigoUo;
                }

                await this.unitOfWork.SaveAsync(cancellationToken);

                return Response.Success(this.mapper.Map<UsuarioDTO>(usuarioExistente));
            }

            var (usuariosAd, adError) = await this.externalDirectoryService.Search(idRed);

            if (adError != null)
            {
                return Response.Failure<UsuarioDTO>(adError);
            }

            var usuarioAd = usuariosAd.FirstOrDefault(x =>
                string.Equals(x.IdRed, idRed, StringComparison.InvariantCultureIgnoreCase))
                ?? usuariosAd.FirstOrDefault();

            if (usuarioAd is null)
            {
                return Response.Failure<UsuarioDTO>(UsuariosError.EntidadNoEncontrada("Usuario en AdHelper"));
            }

            var nuevoUsuario = new UsuarioSRC
            {
                IdRed = usuarioAd.IdRed,
                NombreCompleto = $"{usuarioAd.FirstName} {usuarioAd.LastName}".Trim(),
                CorreoElectronico = usuarioAd.Email,
                CodigoUO = codigoUo,
                EsExterno = false,
                Activo = true,
            };

            await this.unitOfWork.UsuarioSRCRepository.AddAsync(nuevoUsuario, cancellationToken);
            await this.unitOfWork.SaveAsync(cancellationToken);

            return Response.Success(this.mapper.Map<UsuarioDTO>(nuevoUsuario));
        }
    }
}