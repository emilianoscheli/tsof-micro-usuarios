namespace UsuariosAPI.Application.ServiceConsumers
{
    using System.Threading.Tasks;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using AutoMapper;
    using MaestrosAPI.SDK.Contracts.DTOs.MotivosDelegacion;
    using MaestrosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions.Errors;

    public class MotivosDelegacionServiceConsumer : IMotivosDelegacionServiceConsumer
    {
        public MotivosDelegacionServiceConsumer(
            IMapper mapper,
            IMotivosDelegacionService service)
        {
            this.AutoMapper = mapper;
            this.MotivosDelegacionService = service;
        }

        private IMapper AutoMapper { get; }

        private IMotivosDelegacionService MotivosDelegacionService { get; }

        public async Task<(MotivoDelegacionDTO?, Error?)> ValidateMotivoDelegacion(int bdOrigenId)
        {
            var apiResponse = await this.MotivosDelegacionService.GetMotivoDelegacionById(bdOrigenId);

            if (apiResponse is null || !apiResponse.IsSuccess || apiResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Motivo Delegacion"));
            }

            return (apiResponse.Data, null);
        }

        public async Task<(IEnumerable<MotivoDelegacionDTO>, Error?)> GetMotivosDelegacionByIds(List<int> bdOrigenIds, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var areaResponse = await this.MotivosDelegacionService.GetMotivosDelegacionByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageSize = bdOrigenIds.Count, PageNumber = 1 }, gdiAuth);

            if (areaResponse is null || !areaResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Motivo Delegacion"));
            }

            var response = areaResponse.Data?.Where(x => x != null).ToList() ?? [];

            return (response!, null);
        }
    }
}
