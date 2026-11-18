namespace UsuariosAPI.Application.ServiceConsumers
{
    using System.Threading.Tasks;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using AutoMapper;
    using MaestrosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;

    public class SociedadServiceConsumer : ISociedadServiceConsumer
    {
        public SociedadServiceConsumer(
            IMapper mapper,
            ISociedadService service)
        {
            this.AutoMapper = mapper;
            this.SociedadService = service;
        }

        private IMapper AutoMapper { get; }

        private ISociedadService SociedadService { get; }

        public async Task<(SociedadUsuarioDTO?, Error?)> ValidateSociedad(int bdOrigenId)
        {
            var sociedadResponse = await this.SociedadService.GetSociedadById(bdOrigenId);

            if (sociedadResponse is null || !sociedadResponse.IsSuccess || sociedadResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Sociedad"));
            }

            var response = this.AutoMapper.Map<SociedadUsuarioDTO>(sociedadResponse.Data);

            return (response, null);
        }

        public async Task<(IEnumerable<SociedadUsuarioDTO>, Error?)> GetSociedadesByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var sociedadesResponse = await this.SociedadService.GetSociedadesByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageNumber = pageNumber, PageSize = pageSize }, gdiAuth);

            if (sociedadesResponse is null || !sociedadesResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Sociedad"));
            }

            var response = sociedadesResponse.Data?.Select(this.AutoMapper.Map<SociedadUsuarioDTO>)?.ToList() ?? [];

            return (response, null);
        }
    }
}
