namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarioByIdRed
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
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioSociedad;

    public class UsuarioByIdRedQueryHandler : IQueryHandler<UsuarioByIdRedQuery, UsuarioDTO>
    {
        public UsuarioByIdRedQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISociedadServiceConsumer sociedadServiceConsumer,
            IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
            IContratistaServiceConsumer contratistaServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.SociedadServiceConsumer = sociedadServiceConsumer;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private ISociedadServiceConsumer SociedadServiceConsumer { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        public async Task<IResponse<UsuarioDTO>> Handle(UsuarioByIdRedQuery request, CancellationToken cancellationToken)
        {
            var entidad = await this.UnitOfWork.UsuarioSRCRepository.GetByIdRedWithAllDataAsync(request.dto.IdRed, cancellationToken);

            if (entidad is null)
            {
                return Response.Failure<UsuarioDTO>(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            var usuarioResponse = this.AutoMapper.Map<UsuarioDTO>(entidad);

            if (entidad.ContratistaFkId.HasValue)
            {
                var (contratista, errorContratista) = await this.ContratistaServiceConsumer.ValidateContratista(entidad.ContratistaFkId.Value);

                if (errorContratista is not null)
                {
                    return Response.Failure<UsuarioDTO>(errorContratista);
                }

                usuarioResponse.Contratista!.RazonSocial = contratista!.RazonSocial;
            }

            if (entidad.EstudioAuditorFkId.HasValue)
            {
                var (estudio, errorEstudio) = await this.EstudioSedeServiceConsumer.ValidateEstudioAuditor(entidad.EstudioAuditorFkId.Value);

                if (errorEstudio is not null)
                {
                    return Response.Failure<UsuarioDTO>(errorEstudio);
                }

                usuarioResponse.EstudioAuditor!.Nombre = estudio!.Nombre;
            }

            if (entidad.SedeFkId.HasValue)
            {
                var (sede, errorSede) = await this.EstudioSedeServiceConsumer.ValidateSede(entidad.SedeFkId.Value);

                if (errorSede is not null)
                {
                    return Response.Failure<UsuarioDTO>(errorSede);
                }

                usuarioResponse.Sede!.Nombre = sede!.Nombre;
            }

            if (entidad.Sociedades is not null && entidad.Sociedades.Count != 0)
            {
                var sociedadesId = entidad.Sociedades.Select(y => y.SociedadFkId).Distinct().ToList() ?? [];
                var (sociedades, errorSociedades) = await this.SociedadServiceConsumer.GetSociedadesByIds(sociedadesId);

                if (errorSociedades is not null)
                {
                    return Response.Failure<UsuarioDTO>(errorSociedades);
                }

                usuarioResponse.Sociedades = string.Join(",", sociedades.Select(x => x.Nombre));
                usuarioResponse.SociedadesList = sociedades.Select(x => new UsuarioSociedadDTO
                {
                    UsuarioSRCId = entidad.Id,
                    Sociedad = x,
                }).ToList();
            }

            return Response.Success(usuarioResponse);
        }
    }
}
