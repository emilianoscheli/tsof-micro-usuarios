namespace UsuariosAPI.Application.Features.Rol.QueryRolesByIdBatch
{
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed class RolesByIdBatchQueryHandler : IQueryPaginatedHandler<RolesByIdBatchQuery, RolDetailsDTO>
    {
        public RolesByIdBatchQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IPaginatedResponse<RolDetailsDTO>> Handle(RolesByIdBatchQuery request, CancellationToken cancellationToken)
        {
            var (roles, pagination) = await this.UnitOfWork.RolRepository
                .GetByIds(request.dto.Ids)
                .ProjectTo<RolDetailsDTO>(this.AutoMapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.dto.PageNumber, request.dto.PageSize);

            if ((roles is null || roles.Count == 0) && request.dto.Ids.Count > 0)
            {
                return PaginatedResponse<RolDetailsDTO>.Failure(UsuariosError.EntidadNoEncontrada(nameof(Rol)));
            }

            return PaginatedResponse<RolDetailsDTO>.Success(roles, pagination);
        }
    }
}
