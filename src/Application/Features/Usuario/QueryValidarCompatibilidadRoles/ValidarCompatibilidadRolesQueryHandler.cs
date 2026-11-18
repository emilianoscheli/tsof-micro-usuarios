namespace UsuariosAPI.Application.Features.Usuario.QueryValidarCompatibilidadRoles
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;

    public class ValidarCompatibilidadRolesQueryHandler : IQueryHandler<ValidarCompatibilidadRolesQuery, bool>
    {
        public ValidarCompatibilidadRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        public Task<IResponse<bool>> Handle(ValidarCompatibilidadRolesQuery request, CancellationToken cancellationToken)
        {
            var rolesAValidar = request.dto.RolesDestinoIds
                .Where(id => !request.dto.RolesOrigenIds.Contains(id))
                .ToList();

            if (rolesAValidar.Count == 0)
            {
                IResponse<bool> respuestaDirecta = Response.Success(true);
                return Task.FromResult(respuestaDirecta);
            }

            var incompatibles = this.UnitOfWork.CompatibilidadRolRepository
                .GetRolesIncompatibles(request.dto.RolesOrigenIds, rolesAValidar);

            var esCompatible = incompatibles.Count == 0;

            IResponse<bool> response = Response.Success(esCompatible);
            return Task.FromResult(response);
        }
    }
}
