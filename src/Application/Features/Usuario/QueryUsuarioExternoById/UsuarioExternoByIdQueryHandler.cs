namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarioExternoById
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using ContratosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuarioExternoByIdQueryHandler : IQueryHandler<UsuarioExternoByIdQuery, UsuarioExternoDTO>
    {
        public UsuarioExternoByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
            IContratistasService contratistasService,
            ITipoDocumentoServiceConsumer tipoDocumentoServiceConsumer,
            IUbicacionServiceConsumer ubicacionServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistasService = contratistasService;
            this.TipoDocumentoServiceConsumer = tipoDocumentoServiceConsumer;
            this.UbicacionServiceConsumer = ubicacionServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistasService ContratistasService { get; }

        private ITipoDocumentoServiceConsumer TipoDocumentoServiceConsumer { get; }

        private IUbicacionServiceConsumer UbicacionServiceConsumer { get; }

        public async Task<IResponse<UsuarioExternoDTO>> Handle(UsuarioExternoByIdQuery request, CancellationToken cancellationToken)
        {
            var entidad = await this.UnitOfWork.UsuarioSRCRepository.GetByIdWithAllDataAsync(request.dto.Id, cancellationToken);

            if (entidad is null)
            {
                return Response.Failure<UsuarioExternoDTO>(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            if (!entidad.EsExterno)
            {
                return Response.Failure<UsuarioExternoDTO>(UsuariosError.UsuarioNoEsExterno);
            }

            var usuarioResponse = this.AutoMapper.Map<UsuarioExternoDTO>(entidad);

            // Setear TipoDocumento
            if (entidad.TipoDocumentoFkId.HasValue)
            {
                var (tipoDocumento, errorTipoDoc) = await this.TipoDocumentoServiceConsumer.GetTipoDocumentoById(entidad.TipoDocumentoFkId.Value, request.gdiAuth);
                if (errorTipoDoc is null && tipoDocumento is not null)
                {
                    usuarioResponse.TipoDocumento = tipoDocumento.Nombre ?? string.Empty;
                }
            }

            // Setear NumeroDocumento
            usuarioResponse.NumeroDocumento = entidad.NumeroDocumento ?? string.Empty;

            // Setear CUIL
            usuarioResponse.CUIL = entidad.Cuil ?? string.Empty;

            // Setear País
            if (entidad.PaisFkId.HasValue)
            {
                var (pais, errorPais) = await this.UbicacionServiceConsumer.GetPaisesByIds([entidad.PaisFkId.Value], gdiAuth: request.gdiAuth);
                if (errorPais is null && pais is not null)
                {
                    usuarioResponse.Pais = pais.FirstOrDefault()!.ISOName ?? string.Empty;
                }
            }

            // Setear Contratista
            if (entidad.ContratistaFkId.HasValue)
            {
                var contratistaResponse = await this.ContratistasService.GetContratistaById(entidad.ContratistaFkId.Value, request.gdiAuth);

                if (contratistaResponse is null || !contratistaResponse.IsSuccess || contratistaResponse.Data is null)
                {
                    return Response.Failure<UsuarioExternoDTO>(UsuariosError.EntidadNoEncontrada("Contratista"));
                }

                usuarioResponse.Contratista = $"{contratistaResponse!.Data.RazonSocial} (CUIT: {contratistaResponse.Data.NumeroIdentificacion})";
            }

            // Setear EstudioAuditor
            if (entidad.EstudioAuditorFkId.HasValue)
            {
                var (estudio, errorEstudio) = await this.EstudioSedeServiceConsumer.ValidateEstudioAuditor(entidad.EstudioAuditorFkId.Value, request.gdiAuth);

                if (errorEstudio is not null)
                {
                    return Response.Failure<UsuarioExternoDTO>(errorEstudio);
                }

                usuarioResponse.EstudioAuditor = estudio!.Nombre;
            }

            // Setear Sede
            if (entidad.SedeFkId.HasValue)
            {
                var (sede, errorSede) = await this.EstudioSedeServiceConsumer.ValidateSede(entidad.SedeFkId.Value, request.gdiAuth);

                if (errorSede is not null)
                {
                    return Response.Failure<UsuarioExternoDTO>(errorSede);
                }

                usuarioResponse.Sede = sede!.Nombre;
            }

            return Response.Success(usuarioResponse);
        }
    }
}
