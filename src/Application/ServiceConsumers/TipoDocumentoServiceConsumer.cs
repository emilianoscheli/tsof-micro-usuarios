namespace UsuariosAPI.Application.ServiceConsumers
{
    using System.Threading.Tasks;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using AutoMapper;
    using MaestrosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions.Errors;

    public class TipoDocumentoServiceConsumer : ITipoDocumentoServiceConsumer
    {
        public TipoDocumentoServiceConsumer(
            IMapper mapper,
            ITipoDocumentoService tipoDocumentoService)
        {
            this.AutoMapper = mapper;
            this.TipoDocumentoService = tipoDocumentoService;
        }

        private IMapper AutoMapper { get; }

        private ITipoDocumentoService TipoDocumentoService { get; }

        public async Task<(EnumDTO?, Error?)> GetTipoDocumentoById(int id, bool gdiAuth = true)
        {
            var tipoDocumentoResponse = await this.TipoDocumentoService.GetTipoDocumentoById(id, gdiAuth);

            if (tipoDocumentoResponse is null || !tipoDocumentoResponse.IsSuccess || tipoDocumentoResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("TipoDocumento"));
            }

            return (tipoDocumentoResponse.Data, null);
        }

        public async Task<(List<EnumDTO?>?, Error?)> GetAllTiposDocumentosAsync(bool gdiAuth = true)
        {
            var dto = new GetEnumPaginadoDTO();

            // NOTE: gdiAuth not passed to SDK until TSRC.MaestrosAPI.SDK is updated (currently v1.0.116)
            var tiposDocumentosResponse = await this.TipoDocumentoService.GetTiposDocumentos(dto);
            if (tiposDocumentosResponse is null || !tiposDocumentosResponse.IsSuccess || tiposDocumentosResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("TipoDocumento"));
            }

            var tiposDocList = tiposDocumentosResponse.Data.ToList();

            return (tiposDocList, null);
        }
    }
}
