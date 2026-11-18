namespace UsuariosAPI.Application.Features.Rol.QueryRoles
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;

    public class RolesQueryHandler : IQueryHandler<RolesQuery, List<RolDTO>>
    {
        public RolesQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IResponse<List<RolDTO>>> Handle(RolesQuery request, CancellationToken cancellationToken)
        {
            var queryable = this.UnitOfWork.RolRepository.GetAllByFilters(request.dto.Nombre, request.dto.AsignaSociedad, request.dto.EsManual, request.dto.SortBy, request.dto.OrderAsc);

            var rolesDTO = await queryable
                .ProjectTo<RolDTO>(this.AutoMapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Response.Success(rolesDTO);
        }
    }
}
