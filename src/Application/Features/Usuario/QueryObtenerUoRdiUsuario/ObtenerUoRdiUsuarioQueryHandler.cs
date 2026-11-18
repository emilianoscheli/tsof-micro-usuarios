namespace UsuariosAPI.Application.Features.Usuario.QueryObtenerUoRdiUsuario
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class ObtenerUoRdiUsuarioQueryHandler : IQueryHandler<ObtenerUoRdiUsuarioQuery, UsuarioSolicitanteDTO>
    {
        public ObtenerUoRdiUsuarioQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IResponsableUoServiceConsumer responsableUOServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.ResponsableUoServiceConsumer = responsableUOServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IResponsableUoServiceConsumer ResponsableUoServiceConsumer { get; }

        public async Task<IResponse<UsuarioSolicitanteDTO>> Handle(ObtenerUoRdiUsuarioQuery request, CancellationToken cancellationToken)
        {
            var entidad = await this.UnitOfWork.UsuarioSRCRepository.GetByIdWithAllDataAsync(request.dto.Id, cancellationToken);

            if (entidad is null)
            {
                return Response.Failure<UsuarioSolicitanteDTO>(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            var (responsableUo, errorUo) = await this.ResponsableUoServiceConsumer.ValidateResponsableUoByCodigo(entidad.CodigoUO!, cancellationToken);

            if (errorUo is not null || responsableUo is null)
            {
                return Response.Failure<UsuarioSolicitanteDTO>(errorUo ?? Error.InvalidAPIResponse);
            }

            var entidadResponse = new UsuarioSolicitanteDTO
            {
                UsuarioCodigo = entidad.Id,
                UsuarioNombreCompleto = entidad.NombreCompleto,
                UoUsuario = responsableUo.UnidadOrganizativa,
                RDIUo = responsableUo.Rdi!.NombreCompleto,
            };

            return Response.Success(entidadResponse);
        }
    }
}
