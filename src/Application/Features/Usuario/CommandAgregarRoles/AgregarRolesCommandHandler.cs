namespace UsuariosAPI.Application.Features.Usuario.CommandAgregarRoles
{
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;

    public class AgregarRolesCommandHandler : ICommandHandler<AgregarRolesCommand>
    {
        public AgregarRolesCommandHandler(
           IMapper mapper,
           IUnitOfWork unitOfWork)
        {
            this.AutoMapper = mapper;
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IResponse> Handle(AgregarRolesCommand request, CancellationToken cancellationToken)
        {
            // Este endpoint solo se usa internamente entre APIs. El frontend usa el de ModificarRolesSociedades
            var rolesRestringidos = new[] { (int)RolesEnum.Compras, (int)RolesEnum.RRLL, (int)RolesEnum.Auditoria };

            var usuariosConMultiplesRestringidos = request.dto.UsuariosRoles
                .GroupBy(x => x.UsuarioId)
                .Where(g => g.Count(x => rolesRestringidos.Contains(x.RolId)) > 1)
                .Select(g => g.Key)
                .ToList();

            if (usuariosConMultiplesRestringidos.Count > 0)
            {
                return Response.Failure(Error.GenericError("No se puede asignar más de uno de los roles RRLL, Compras o Auditoria al mismo usuario en la misma petición."));
            }

            var usuariosIds = request.dto.UsuariosRoles.Select(x => x.UsuarioId).Distinct().ToList();
            var usuarios = this.UnitOfWork.UsuarioSRCRepository.GetByIdsWithRoles(usuariosIds);

            if (usuarios is null || usuarios.Count() < usuariosIds.Count)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            // Validar que no sean usuarios externos
            if (usuarios.Any(u => u.EsExterno))
            {
                return Response.Failure(UsuariosError.UsuarioEsExterno);
            }

            // Asegurar que usuarios internos estén siempre activos (no deberia darse este caso)
            //foreach (var usuario in usuarios)
            //{
            //    usuario.Activo = true;
            //}
            var rolesIds = request.dto.UsuariosRoles.Select(x => x.RolId).Distinct().ToList();
            var roles = this.UnitOfWork.RolRepository.GetByIds(rolesIds);

            if (roles is null || roles.Count() < rolesIds.Count)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada(nameof(Rol)));
            }

            // Validar restricción de roles ya existentes
            var usuariosDict = usuarios.ToDictionary(u => u.Id);
            var rolesAPorUsuario = request.dto.UsuariosRoles
                .Where(x => rolesRestringidos.Contains(x.RolId))
                .GroupBy(x => x.UsuarioId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.RolId).ToHashSet());

            foreach (var (usuarioId, nuevosRolesRestringidos) in rolesAPorUsuario)
            {
                var usuario = usuariosDict[usuarioId];
                var rolesRestringidosActuales = usuario.Roles!
                    .Where(r => rolesRestringidos.Contains(r.RolId))
                    .Select(r => r.RolId)
                    .ToHashSet();

                // Si ya tiene un rol restringido y se intenta agregar uno diferente, rechazar
                if (rolesRestringidosActuales.Count > 0 &&
                    nuevosRolesRestringidos.Any(rn => !rolesRestringidosActuales.Contains(rn)))
                {
                    return Response.Failure(Error.GenericError(
                        "No se puede asignar más de uno de los roles RRLL, Compras o Auditoria a un mismo usuario."));
                }
            }

            var usuariosRolesExistentes = usuarios
               .SelectMany(u => u.Roles!)
               .ToList();

            var existentes = usuariosRolesExistentes
                .Select(ur => (ur.UsuarioSRCId, ur.RolId))
                .ToHashSet();

            var nuevos = request.dto.UsuariosRoles
                .Where(x => !existentes.Contains((x.UsuarioId, x.RolId)))
                .Select(this.AutoMapper.Map<UsuarioRol>)
                .ToHashSet();

            if (nuevos != null && nuevos.Count != 0)
            {
                await this.UnitOfWork.UsuarioRolRepository.AddRangeAsync(nuevos, cancellationToken);
            }

            await this.UnitOfWork.SaveAsync(cancellationToken);

            return Response.Success();
        }
    }
}
