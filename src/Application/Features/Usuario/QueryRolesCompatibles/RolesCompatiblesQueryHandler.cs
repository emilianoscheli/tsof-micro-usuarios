namespace UsuariosAPI.Application.Features.Usuario.QueryRolesCompatibles
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
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;

    public class RolesCompatiblesQueryHandler : IQueryHandler<RolesCompatiblesQuery, List<RolDTO>>
    {
        public RolesCompatiblesQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IResponse<List<RolDTO>>> Handle(RolesCompatiblesQuery request, CancellationToken cancellationToken)
        {
            var usuario = await this.UnitOfWork.UsuarioSRCRepository
                .GetByIdWithAllDataAsync(request.dto.Id, cancellationToken);

            if (usuario is null)
            {
                return Response.Failure<List<RolDTO>>(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            var rolesActualesIds = usuario.Roles!.Select(r => r.RolId).ToList();

            var todosLosRoles = await this.UnitOfWork.RolRepository
                .GetAllByFilters(null, null, true, null, null)
                .ToListAsync(cancellationToken);

            var rolesCompatibles = todosLosRoles
                .Where(r => !rolesActualesIds.Contains(r.Id) &&
                            this.UnitOfWork.CompatibilidadRolRepository.EsCompatible(rolesActualesIds, r.Id))
                .ToList();

            var rolesDTO = this.AutoMapper.Map<List<RolDTO>>(rolesCompatibles);

            return Response.Success(rolesDTO);
        }
    }
}
