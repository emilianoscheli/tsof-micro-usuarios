namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosExternos
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuariosExternosQueryHandler : IQueryPaginatedHandler<UsuariosExternosQuery, UsuariosExternosDTO>
    {
        public UsuariosExternosQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
            IContratistaServiceConsumer contratistaServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        public async Task<IPaginatedResponse<UsuariosExternosDTO>> Handle(UsuariosExternosQuery request, CancellationToken cancellationToken)
        {
            var filtersDto = this.AutoMapper.Map<UsuariosExternosFilterAndOrderDTO>(request.dto);

            var (usuarios, pagination) = await this.UnitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuariosExternos(filtersDto)
                .ToPaginatedListAsync(request.dto.PageNumber, request.dto.PageSize);

            if (usuarios is null || usuarios.Count == 0)
            {
                return PaginatedResponse<UsuariosExternosDTO>.Success([], pagination);
            }

            var contratistasId = usuarios.Where(x => x.ContratistaFkId is not null).Select(x => x.ContratistaFkId ?? 0).ToList() ?? [];
            var estudiosId = usuarios.Where(x => x.EstudioAuditorFkId is not null).Select(x => x.EstudioAuditorFkId ?? 0).ToList() ?? [];
            var sedesId = usuarios.Where(x => x.SedeFkId is not null).Select(x => x.SedeFkId ?? 0).ToList() ?? [];

            var usuariosList = new List<UsuariosExternosDTO>();
            var contratistasMap = new Dictionary<int, ContratistaUsuarioDTO>();
            var estudiosMap = new Dictionary<int, EstudioAuditorUsuarioDTO>();
            var sedesMap = new Dictionary<int, SedeUsuarioDTO>();

            if (contratistasId.Count > 0)
            {
                var (contratistas, errorContratistas) = await this.ContratistaServiceConsumer.GetContratistasByIds(contratistasId, request.dto.PageSize, gdiAuth: request.gdiAuth);

                if (errorContratistas is null)
                {
                    contratistasMap = contratistas.ToDictionary(r => r.Id);
                }
            }

            if (estudiosId.Count > 0)
            {
                var (estudios, errorEstudios) = await this.EstudioSedeServiceConsumer.GetEstudiosByIds(estudiosId, request.dto.PageSize, gdiAuth: request.gdiAuth);

                if (errorEstudios is null)
                {
                    estudiosMap = estudios.ToDictionary(r => r.Id);
                }
            }

            if (sedesId.Count > 0)
            {
                var (sedes, errorSedes) = await this.EstudioSedeServiceConsumer.GetSedesByIds(sedesId, request.dto.PageSize, gdiAuth: request.gdiAuth);

                if (errorSedes is null)
                {
                    sedesMap = sedes.ToDictionary(r => r.Id);
                }
            }

            foreach (var item in usuarios)
            {
                var usuarioDTO = this.AutoMapper.Map<UsuariosExternosDTO>(item);

                if (contratistasMap.TryGetValue(item.ContratistaFkId ?? 0, out var contratista))
                {
                    usuarioDTO.Contratista = contratista.RazonSocial;
                }

                if (estudiosMap.TryGetValue(item.EstudioAuditorFkId ?? 0, out var estudio)
                    && sedesMap.TryGetValue(item.SedeFkId ?? 0, out var sede))
                {
                    usuarioDTO.SedeEstudioAuditor = $"{estudio.Nombre} ; {sede.Nombre}";
                }

                usuariosList.Add(usuarioDTO);
            }

            return PaginatedResponse<UsuariosExternosDTO>.Success(usuariosList!, pagination);
        }
    }
}
