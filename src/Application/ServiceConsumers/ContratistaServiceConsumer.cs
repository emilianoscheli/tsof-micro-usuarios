namespace UsuariosAPI.Application.ServiceConsumers
{
    using System.Threading.Tasks;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using AutoMapper;
    using ContratosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;

    public class ContratistaServiceConsumer : IContratistaServiceConsumer
    {
        public ContratistaServiceConsumer(
            IMapper mapper,
            IContratistasService service)
        {
            this.AutoMapper = mapper;
            this.ContratistaService = service;
        }

        private IMapper AutoMapper { get; }

        private IContratistasService ContratistaService { get; }

        public async Task<(ContratistaUsuarioDTO?, Error?)> ValidateContratista(int bdOrigenId, bool gdiAuth = true)
        {
            var contratistaResponse = await this.ContratistaService.GetContratistaById(bdOrigenId, gdiAuth);

            if (contratistaResponse is null || !contratistaResponse.IsSuccess || contratistaResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Contratista"));
            }

            var response = this.AutoMapper.Map<ContratistaUsuarioDTO>(contratistaResponse.Data);

            return (response, null);
        }

        public async Task<(IEnumerable<ContratistaUsuarioDTO>, Error?)> GetContratistasByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var contratistasResponse = await this.ContratistaService.GetContratistasByIdBatchPost(new GetByIdsListDTO { Ids = bdOrigenIds, PageNumber = pageNumber, PageSize = pageSize }, gdiAuth);

            if (contratistasResponse is null || !contratistasResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Contratista"));
            }

            var response = contratistasResponse.Data?.Select(this.AutoMapper.Map<ContratistaUsuarioDTO>)?.ToList() ?? [];

            return (response, null);
        }
    }
}
