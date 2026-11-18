namespace UsuariosAPI.Application.Features.Usuario.QueryObtenerUoRdiPorUsuarios
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using MediatR;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class ObtenerUoRdiPorUsuariosQueryHandler : IQueryHandler<ObtenerUoRdiPorUsuariosQuery, List<UsuarioSolicitanteDTO>>
    {
        public ObtenerUoRdiPorUsuariosQueryHandler(
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

        public async Task<IResponse<List<UsuarioSolicitanteDTO>>> Handle(ObtenerUoRdiPorUsuariosQuery request, CancellationToken cancellationToken)
        {
            if (request.dto.Usuarios.Count != 0)
            {
                var usuariosSolicitantesList = new List<UsuarioSolicitanteDTO>();
                foreach (var usuario in request.dto.Usuarios)
                {
                    var entidad = await this.UnitOfWork.UsuarioSRCRepository.GetByIdWithAllDataAsync(usuario, cancellationToken);

                    if (entidad is null)
                    {
                        return Response.Failure<List<UsuarioSolicitanteDTO>>(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
                    }

                    var (responsableUo, errorUo) = await this.ResponsableUoServiceConsumer.ValidateResponsableUoByCodigo(entidad.CodigoUO!, cancellationToken);

                    if (errorUo is not null || responsableUo is null)
                    {
                        return Response.Failure<List<UsuarioSolicitanteDTO>>(errorUo is not null ? errorUo : Error.InvalidAPIResponse);
                    }

                    var entidadResponse = new UsuarioSolicitanteDTO
                    {
                        UsuarioCodigo = entidad.Id,
                        UsuarioNombreCompleto = entidad.NombreCompleto,
                        UoUsuario = responsableUo.UnidadOrganizativa,
                        RDIUo = responsableUo.Rdi!.NombreCompleto,
                    };

                    usuariosSolicitantesList.Add(entidadResponse);
                }

                return Response.Success(usuariosSolicitantesList);
            }
            else
            {
                return Response.Failure<List<UsuarioSolicitanteDTO>>(UsuariosError.RequestInvalido);
            }
        }
    }
}
