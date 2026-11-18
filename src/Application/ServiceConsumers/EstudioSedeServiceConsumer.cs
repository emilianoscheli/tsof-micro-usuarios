namespace UsuariosAPI.Application.ServiceConsumers
{
    using System.Threading.Tasks;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using AutoMapper;
    using MaestrosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;

    public class EstudioSedeServiceConsumer : IEstudioSedeServiceConsumer
    {
        public EstudioSedeServiceConsumer(
            IMapper mapper,
            IEstudioAuditorService service)
        {
            this.AutoMapper = mapper;
            this.EstudioSedeService = service;
        }

        private IMapper AutoMapper { get; }

        private IEstudioAuditorService EstudioSedeService { get; }

        public async Task<(EstudioAuditorUsuarioDTO?, Error?)> ValidateEstudioAuditor(int bdOrigenId, bool gdiAuth = true)
        {
            var estudioResponse = await this.EstudioSedeService.GetEstudioAuditorById(bdOrigenId, gdiAuth);

            if (estudioResponse is null || !estudioResponse.IsSuccess || estudioResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Estudio Auditor"));
            }

            var response = this.AutoMapper.Map<EstudioAuditorUsuarioDTO>(estudioResponse.Data);

            return (response, null);
        }

        public async Task<(SedeUsuarioDTO?, Error?)> ValidateSede(int bdOrigenId, bool gdiAuth = true)
        {
            var sedeResponse = await this.EstudioSedeService.GetSedeById(bdOrigenId, gdiAuth);

            if (sedeResponse is null || !sedeResponse.IsSuccess || sedeResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Sede"));
            }

            var response = this.AutoMapper.Map<SedeUsuarioDTO>(sedeResponse.Data);

            return (response, null);
        }

        public async Task<(IEnumerable<EstudioAuditorUsuarioDTO>, Error?)> GetEstudiosByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var estudiosResponse = await this.EstudioSedeService.GetEstudiosAuditoresByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageNumber = pageNumber, PageSize = pageSize }, gdiAuth);

            if (estudiosResponse is null || !estudiosResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Estudio Auditor"));
            }

            var response = estudiosResponse.Data?.Select(this.AutoMapper.Map<EstudioAuditorUsuarioDTO>)?.ToList() ?? [];

            return (response, null);
        }

        public async Task<(IEnumerable<SedeUsuarioDTO>, Error?)> GetSedesByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var sedesResponse = await this.EstudioSedeService.GetSedesByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageNumber = pageNumber, PageSize = pageSize }, gdiAuth);

            if (sedesResponse is null || !sedesResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Sede"));
            }

            var response = sedesResponse.Data?.Select(this.AutoMapper.Map<SedeUsuarioDTO>)?.ToList() ?? [];

            return (response, null);
        }
    }
}
