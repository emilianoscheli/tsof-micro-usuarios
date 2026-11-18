namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosByIdBatch
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuariosByIdBatchQueryHandler : IQueryPaginatedHandler<UsuariosByIdBatchQuery, UsuarioAcotadoDTO>
    {
        public UsuariosByIdBatchQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IPaginatedResponse<UsuarioAcotadoDTO>> Handle(UsuariosByIdBatchQuery request, CancellationToken cancellationToken)
        {
            // No uso ToPaginatedListAsync para mejorar la performance, este endpoint no se usa en grillas, no necesita verdadera paginacion
            var usuarios = await this.UnitOfWork.UsuarioSRCRepository
                .GetByIdsWithAllData(request.dto.Ids)
                .ToListAsync();

            if ((usuarios is null || usuarios.Count == 0) && request.dto.Ids.Count > 0)
            {
                return PaginatedResponse<UsuarioAcotadoDTO>.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            var usuariosDto = usuarios!.Select(u => this.AutoMapper.Map<UsuarioAcotadoDTO>(u)).ToList();

            return PaginatedResponse<UsuarioAcotadoDTO>.Success(usuariosDto!, new PaginationData { TotalPages = 1, TotalCount = usuariosDto.Count, PageSize = request.dto.PageSize, PageNumber = 1 });
        }
    }
}
