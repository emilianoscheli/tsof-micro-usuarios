namespace UsuariosAPI.Application.Features.Rol.QueryPermisosByRoles
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;

    public sealed class PermisosByRolesQueryHandler : IQueryHandler<PermisosByRolesQuery, List<int>>
    {
        public PermisosByRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        public async Task<IResponse<List<int>>> Handle(PermisosByRolesQuery request, CancellationToken cancellationToken)
        {
            if (request.dto.Ids is null || !request.dto.Ids.Any())
            {
                return Response.Failure<List<int>>(Error.CampoObligatorio);
            }

            var permisosIds = await this.UnitOfWork.RolPermisoRepository
                .AsQueryable()
                .Where(rp => request.dto.Ids.Contains(rp.RolId))
                .Where(rp => rp.Permiso != null && rp.Permiso.Codigo.ToLower().Contains("reporte"))
                .Select(rp => rp.PermisoId)
                .Distinct()
                .ToListAsync(cancellationToken);

            return Response.Success(permisosIds);
        }
    }
}
