namespace UsuariosAPI.Application.Features.Usuario.CommandModificarRolesSociedades
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;

    public class ModificarRolesSociedadesCommandHandler : ICommandHandler<ModificarRolesSociedadesCommand>
    {
        public ModificarRolesSociedadesCommandHandler(
           IUnitOfWork unitOfWork,
           ISociedadServiceConsumer sociedadServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.SociedadServiceConsumer = sociedadServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private ISociedadServiceConsumer SociedadServiceConsumer { get; }

        public async Task<IResponse> Handle(ModificarRolesSociedadesCommand request, CancellationToken cancellationToken)
        {
            var usuario = await this.UnitOfWork.UsuarioSRCRepository
                .GetByIdWithAllDataAsync(request.dto.UsuarioSRCId, cancellationToken);

            if (usuario is null)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            // Validar que no sea usuario externo
            if (usuario.EsExterno)
            {
                return Response.Failure(UsuariosError.UsuarioEsExterno);
            }

            // Un usuario sin roles se considera inactivo (HU04 RN)
            // Se actualiza al final del flujo según los roles resultantes
            if (SonListasNulasOVacias(request.dto.RolesIds, request.dto.SociedadesIds))
            {
                return Response.Success("No se realizaron cambios, ya que las listas están vacías.");
            }

            // Validar intentos de eliminar roles automáticos
            var validacionRoles = ValidarRolesEliminados(usuario, request.dto.RolesIds);
            if (!validacionRoles.IsSuccess)
            {
                return validacionRoles;
            }

            // Gestionar roles del usuario
            var gestionRolesResult = await this.GestionarRoles(usuario, request.dto.RolesIds, cancellationToken);
            if (!gestionRolesResult.IsSuccess)
            {
                return gestionRolesResult;
            }

            // Gestionar sociedades asignadas al usuario, solo si el tipo de rol lo permite
            var gestionSociedadesResult = await this.GestionarSociedades(usuario, request.dto.SociedadesIds);
            if (!gestionSociedadesResult.IsSuccess)
            {
                return gestionSociedadesResult;
            }

            // Un usuario sin roles manuales ni automáticos es considerado inactivo (HU04)
            usuario.Activo = usuario.Roles!.Count > 0;

            await this.UnitOfWork.SaveAsync();
            return Response.Success();
        }

        private static bool SonListasNulasOVacias(List<int>? rolesIds, List<int>? sociedadesIds)
        {
            return (rolesIds == null || rolesIds.Count == 0) && (sociedadesIds == null || sociedadesIds.Count == 0);
        }

        private static IResponse ValidarRolesEliminados(UsuarioSRC usuario, List<int>? nuevosRolesIds)
        {
            var rolesAutomaticosEliminados = usuario.Roles!
                .Where(ur => nuevosRolesIds != null && !nuevosRolesIds.Contains(ur.RolId) && !ur.Rol!.EsManual)
                .ToList();

            return rolesAutomaticosEliminados.Count > 0
                ? Response.Failure(UsuariosError.OperacionNoPermitida("No se pueden eliminar roles automáticos."))
                : Response.Success();
        }

        private static bool CambiaSociedades(List<int>? nuevasSociedadesIds, List<UsuarioSociedad>? usuarioSociedades)
        {
            if ((nuevasSociedadesIds is null || nuevasSociedadesIds.Count == 0)
                && (usuarioSociedades is null || usuarioSociedades.Count == 0))
            {
                return false;
            }

            var idsActuales = usuarioSociedades!.Select(m => m.SociedadFkId).ToHashSet() ?? [];
            var idsNuevos = nuevasSociedadesIds?.ToHashSet() ?? [];

            return !idsActuales.SetEquals(idsNuevos);
        }

        private async Task<IResponse> GestionarRoles(UsuarioSRC usuario, List<int>? nuevosRolesIds, CancellationToken cancellationToken)
        {
            if (nuevosRolesIds == null || nuevosRolesIds.Count == 0)
            {
                return Response.Success();
            }

            var rolesActuales = usuario.Roles!.Select(r => r.RolId).ToList();
            var rolesParaAgregar = nuevosRolesIds.Where(id => !rolesActuales.Contains(id)).ToList();

            if (rolesParaAgregar.Count > 0 && rolesActuales.Count > 0)
            {
                var rolesIncompatibles = this.UnitOfWork.CompatibilidadRolRepository
                    .GetRolesIncompatibles(rolesActuales, rolesParaAgregar);

                if (rolesIncompatibles.Count > 0)
                {
                    return Response.Failure(UsuariosError.RolesIncompatibles);
                }
            }

            if (rolesParaAgregar.Count > 0)
            {
                var rolesParaValidar = await this.UnitOfWork.RolRepository
                    .GetByIds(rolesParaAgregar)
                    .ToListAsync(cancellationToken);

                var rolesAutomaticos = rolesParaValidar.Where(r => !r.EsManual).ToList();

                if (rolesAutomaticos.Count > 0)
                {
                    return Response.Failure(UsuariosError.OperacionNoPermitida("No se pueden asignar roles automáticos."));
                }

                // HU04: No se pueden asignar Contratista ni Estudio Auditor desde este módulo
                var rolesRestringidosHU04 = new[] { (int)RolesEnum.Contratista, (int)RolesEnum.EstudioAuditor };
                if (rolesParaAgregar.Any(id => rolesRestringidosHU04.Contains(id)))
                {
                    return Response.Failure(UsuariosError.OperacionNoPermitida(
                        "Los roles Contratista y Estudio Auditor se asignan exclusivamente desde el módulo de Gestión de Usuarios Externos."));
                }
            }

            var rolesRestringidos = new[] { (int)RolesEnum.Compras, (int)RolesEnum.RRLL, (int)RolesEnum.Auditoria };

            if (nuevosRolesIds.Where(x => rolesRestringidos.Contains(x)).Count() > 1)
            {
                return Response.Failure(Error.GenericError("No se puede asignar más de uno de los roles RRLL, Compras o Auditoria al mismo usuario"));
            }

            foreach (var rolId in rolesParaAgregar)
            {
                var rol = await this.UnitOfWork.RolRepository.GetByIdAsync(rolId, cancellationToken);

                if (rol is null)
                {
                    return Response.Failure(UsuariosError.EntidadNoEncontrada(nameof(rol)));
                }

                usuario.Roles!.Add(new UsuarioRol
                {
                    UsuarioSRCId = usuario.Id,
                    RolId = rol.Id,
                    UsuarioSRC = usuario,
                    Rol = rol,
                });
            }

            var rolesParaEliminar = usuario.Roles!
                .Where(ur => nuevosRolesIds != null && !nuevosRolesIds.Contains(ur.RolId) && ur.Rol!.EsManual)
                .ToList();

            foreach (var rolParaEliminar in rolesParaEliminar)
            {
                /*if (rolParaEliminar.RolId == (int)RolesEnum.Contratista)
                {
                    usuario.ContratistaFkId = null;
                }*/

                if (rolParaEliminar.RolId == (int)RolesEnum.EstudioAuditor)
                {
                    usuario.EstudioAuditorFkId = null;
                    usuario.SedeFkId = null;
                }

                usuario.Roles!.Remove(rolParaEliminar);
                this.UnitOfWork.UsuarioRolRepository.Remove(rolParaEliminar);
            }

            return Response.Success();
        }

        private async Task<IResponse> GestionarSociedades(UsuarioSRC usuario, List<int>? nuevasSociedadesIds)
        {
            var noTieneRolQuePermitaSociedades = !usuario.Roles!.Any(r => r.Rol!.AsignaSociedad);
            if (noTieneRolQuePermitaSociedades)
            {
                var sociedadesAEliminar = usuario.Sociedades!.ToList();
                usuario.Sociedades!.Clear();
                this.UnitOfWork.UsuarioSociedadRepository.RemoveRange(sociedadesAEliminar);
                return Response.Success();
            }

            if (nuevasSociedadesIds == null)
            {
                return Response.Failure(Error.GenericError("Sociedades no puede estar vacio"));
            }

            if (!CambiaSociedades(nuevasSociedadesIds, usuario.Sociedades))
            {
                return Response.Success();
            }

            foreach (var sociedadId in nuevasSociedadesIds)
            {
                var (sociedad, errorSociedad) = await this.SociedadServiceConsumer.ValidateSociedad(sociedadId);

                if (errorSociedad is not null)
                {
                    return Response.Failure(errorSociedad);
                }

                if (!usuario.Sociedades!.Any(us => us.SociedadFkId == sociedad!.Id))
                {
                    usuario.Sociedades!.Add(new UsuarioSociedad
                    {
                        UsuarioSRCId = usuario.Id,
                        SociedadFkId = sociedad!.Id,
                        UsuarioSRC = usuario,
                    });
                }
            }

            var sociedadesParaEliminar = usuario.Sociedades!
                .Where(us => nuevasSociedadesIds != null && !nuevasSociedadesIds.Contains(us.SociedadFkId))
                .ToList();

            foreach (var sociedadParaEliminar in sociedadesParaEliminar)
            {
                usuario.Sociedades!.Remove(sociedadParaEliminar);
                this.UnitOfWork.UsuarioSociedadRepository.Remove(sociedadParaEliminar);
            }

            if (usuario.Sociedades!.Count == 0)
            {
                return Response.Failure(UsuariosError.OperacionNoPermitida(
                    "El usuario tiene un rol que requiere al menos una sociedad asignada."));
            }

            return Response.Success();
        }
    }
}
