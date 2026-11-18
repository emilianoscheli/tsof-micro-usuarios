namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosSelect
{
    using APICore.Enums;
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using ContratosAPI.SDK.Contracts.DTOs.Etapas.EtapaAfectacion;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuariosSelectQueryHandler : IQueryPaginatedHandler<UsuariosSelectQuery, UsuarioAcotadoDTO>
    {
        public UsuariosSelectQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISociedadServiceConsumer sociedadServiceConsumer,
            IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
            IContratistaServiceConsumer contratistaServiceConsumer,
            IAuthenticatedUserService authenticatedUserService)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.SociedadServiceConsumer = sociedadServiceConsumer;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
            this.AuthenticatedUserService = authenticatedUserService;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private ISociedadServiceConsumer SociedadServiceConsumer { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        private IAuthenticatedUserService AuthenticatedUserService { get; }

        public async Task<IPaginatedResponse<UsuarioAcotadoDTO>> Handle(UsuariosSelectQuery request, CancellationToken cancellationToken)
        {
            var filtersDto = this.AutoMapper.Map<UsuariosSRCFilterAndOrderDTO>(request.dto);

            // Si es EstudioAuditor o SaludOcupacional: solo usuarios vinculados a su estudio y activos
            var rolesRestringidos = new List<RolesEnum> { RolesEnum.EstudioAuditor, RolesEnum.SaludOcupacional };
            if (this.AuthenticatedUserService.AuthenticatedUserTieneAlgunRol(rolesRestringidos))
            {
                var estudioAuditorId = this.AuthenticatedUserService.GetAuthenticatedUserEstudioAuditorId();
                if (estudioAuditorId > 0)
                {
                    filtersDto.EstudioAuditorId = estudioAuditorId;
                }
            }

            var (usuarios, pagination) = await this.UnitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuarios(filtersDto)
                .ProjectTo<UsuarioAcotadoDTO>(this.AutoMapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.dto.PageNumber, request.dto.PageSize);

            return PaginatedResponse<UsuarioAcotadoDTO>.Success(usuarios, pagination);
        }
    }
}
