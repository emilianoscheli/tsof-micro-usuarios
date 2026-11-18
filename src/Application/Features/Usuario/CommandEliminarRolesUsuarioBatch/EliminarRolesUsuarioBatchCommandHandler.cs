namespace UsuariosAPI.Application.Features.Usuario.CommandEliminarRolesUsuarioBatch
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;

    public class EliminarRolesUsuarioBatchCommandHandler : ICommandHandler<EliminarRolesUsuarioBatchCommand>
    {
        public EliminarRolesUsuarioBatchCommandHandler(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        public async Task<IResponse> Handle(EliminarRolesUsuarioBatchCommand request, CancellationToken cancellationToken)
        {
            // Este endpoint solo se usa internamente entre APIs. El frontend usa el de ModificarRolesSociedades
            var dto = request.dto;

            // Validar que no sean usuarios externos
            var usuariosAfectados = this.UnitOfWork.UsuarioSRCRepository
                .GetByIds(dto.UsuariosIds)
                .Where(u => u.EsExterno)
                .Any();

            if (usuariosAfectados)
            {
                return Response.Failure(UsuariosError.UsuarioEsExterno);
            }

            var usuariosAEliminar = this.UnitOfWork.UsuarioRolRepository
                .AsQueryable()
                .Where(ur => dto.UsuariosIds.Contains(ur.UsuarioSRCId) && ur.RolId == dto.RolId);

            /*if (dto.RolId == (int)RolesEnum.Contratista)
            {
                var usuariosContratistasAEliminar = this.UnitOfWork.UsuarioSRCRepository
                    .AsQueryable()
                    .Where(ur => dto.UsuariosIds.Contains(ur.Id) && ur.Roles!.Any(r => r.RolId == (int)RolesEnum.Contratista));

                foreach (var item in usuariosContratistasAEliminar)
                {
                    item.ContratistaFkId = null;
                }
            }*/
            if (dto.RolId == (int)RolesEnum.EstudioAuditor)
            {
                var usuariosEstudioAuditorAEliminar = this.UnitOfWork.UsuarioSRCRepository
                    .AsQueryable()
                    .Where(ur => dto.UsuariosIds.Contains(ur.Id) && ur.Roles!.Any(r => r.RolId == (int)RolesEnum.EstudioAuditor));

                foreach (var item in usuariosEstudioAuditorAEliminar)
                {
                    item.EstudioAuditorFkId = null;
                    item.SedeFkId = null;
                }
            }

            this.UnitOfWork.UsuarioRolRepository.RemoveRange(usuariosAEliminar, cancellationToken);

            await this.UnitOfWork.SaveAsync(cancellationToken);

            return Response.Success();
        }
    }
}
