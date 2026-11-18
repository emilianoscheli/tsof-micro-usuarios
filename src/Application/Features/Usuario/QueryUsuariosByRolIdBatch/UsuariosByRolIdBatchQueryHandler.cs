namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosByRolIdBatch
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuariosByRolIdBatchQueryHandler : IQueryPaginatedHandler<UsuariosByRolIdBatchQuery, UsuarioAcotadoDTO>
    {
        public UsuariosByRolIdBatchQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IPaginatedResponse<UsuarioAcotadoDTO>> Handle(UsuariosByRolIdBatchQuery request, CancellationToken cancellationToken)
        {
            var (usuarios, pagination) = await this.UnitOfWork.UsuarioSRCRepository
                .GetByRolesIdWithAllData(request.dto.Ids)
                .ProjectTo<UsuarioAcotadoDTO>(this.AutoMapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.dto.PageNumber, request.dto.PageSize);

            if ((usuarios is null || usuarios.Count == 0) && request.dto.Ids.Count > 0)
            {
                return PaginatedResponse<UsuarioAcotadoDTO>.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            return PaginatedResponse<UsuarioAcotadoDTO>.Success(usuarios, pagination);
        }
    }
}
