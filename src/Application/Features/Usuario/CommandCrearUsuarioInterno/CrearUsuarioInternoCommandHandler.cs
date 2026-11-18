namespace UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioInterno
{
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;

    public class CrearUsuarioInternoCommandHandler : ICommandHandler<CrearUsuarioInternoCommand>
    {
        public CrearUsuarioInternoCommandHandler(
            IUnitOfWork unitOfWork,
            ISociedadServiceConsumer sociedadServiceConsumer,
            IExternalDirectoryService adUsuarioServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.SociedadServiceConsumer = sociedadServiceConsumer;
            this.AdUsuarioServiceConsumer = adUsuarioServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private ISociedadServiceConsumer SociedadServiceConsumer { get; }

        private IExternalDirectoryService AdUsuarioServiceConsumer { get; }

        public async Task<IResponse> Handle(CrearUsuarioInternoCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;

            // Verificar que el usuario no exista ya en SRC
            var usuarioExistente = await this.UnitOfWork.UsuarioSRCRepository
                .GetByIdRedWithAllDataAsync(dto.IdRed, cancellationToken);

            if (usuarioExistente is not null)
            {
                return Response.Failure(UsuariosError.OperacionNoPermitida($"El usuario con IdRed '{dto.IdRed}' ya existe en SRC."));
            }

            var (adUsuarios, adError) = await this.AdUsuarioServiceConsumer.Search(dto.IdRed);

            if (adError != null)
            {
                return Response.Failure(adError);
            }

            if (adUsuarios.Count == 0)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada("Usuario en AdHelper"));
            }

            // Crear el usuario interno con los datos obtenidos del AD
            var nuevoUsuario = new UsuarioSRC
            {
                IdRed = adUsuarios.FirstOrDefault()!.IdRed,
                NombreCompleto = $"{adUsuarios.FirstOrDefault()!.FirstName} {adUsuarios.FirstOrDefault()!.LastName}".Trim(),
                CorreoElectronico = adUsuarios.FirstOrDefault()!.Email,
                EsExterno = false,
                Activo = true,
            };

            await this.UnitOfWork.UsuarioSRCRepository.AddAsync(nuevoUsuario, cancellationToken);

            // Asignar roles manuales (si se proporcionaron)
            if (dto.RolesIds is not null && dto.RolesIds.Count > 0)
            {
                var rolesRestringidos = new[] { (int)RolesEnum.Contratista, (int)RolesEnum.EstudioAuditor };

                var rolesParaValidar = await this.UnitOfWork.RolRepository
                    .GetByIds(dto.RolesIds)
                    .ToListAsync(cancellationToken);

                // No se pueden asignar roles de usuarios externos
                if (dto.RolesIds.Any(id => rolesRestringidos.Contains(id)))
                {
                    return Response.Failure(UsuariosError.OperacionNoPermitida(
                        "No se pueden asignar los roles Contratista o Estudio Auditor en este módulo."));
                }

                // Solo roles manuales
                var rolesAutomaticos = rolesParaValidar.Where(r => !r.EsManual).ToList();
                if (rolesAutomaticos.Count > 0)
                {
                    return Response.Failure(UsuariosError.OperacionNoPermitida("No se pueden asignar roles automáticos."));
                }

                // No más de uno de los roles exclusivos (Compras, RRLL, Auditoria)
                var rolesExclusivos = new[] { (int)RolesEnum.Compras, (int)RolesEnum.RRLL, (int)RolesEnum.Auditoria };
                if (dto.RolesIds.Count(id => rolesExclusivos.Contains(id)) > 1)
                {
                    return Response.Failure(Error.GenericError("No se puede asignar más de uno de los roles RRLL, Compras o Auditoria al mismo usuario."));
                }

                foreach (var rolId in dto.RolesIds)
                {
                    var rol = rolesParaValidar.FirstOrDefault(r => r.Id == rolId);
                    if (rol is null)
                    {
                        return Response.Failure(UsuariosError.EntidadNoEncontrada("Rol"));
                    }

                    nuevoUsuario.Roles!.Add(new UsuarioRol
                    {
                        UsuarioSRC = nuevoUsuario,
                        UsuarioSRCId = nuevoUsuario.Id,
                        RolId = rol.Id,
                    });
                }

                // Verificar si algún rol requiere sociedad
                var requiereSociedad = rolesParaValidar.Any(r => r.AsignaSociedad);
                if (requiereSociedad)
                {
                    if (dto.SociedadesIds is null || dto.SociedadesIds.Count == 0)
                    {
                        return Response.Failure(Error.GenericError("Se deben asignar sociedades para el rol seleccionado."));
                    }

                    foreach (var sociedadId in dto.SociedadesIds)
                    {
                        var (sociedad, errorSociedad) = await this.SociedadServiceConsumer.ValidateSociedad(sociedadId);
                        if (errorSociedad is not null)
                        {
                            return Response.Failure(errorSociedad);
                        }

                        nuevoUsuario.Sociedades!.Add(new UsuarioSociedad
                        {
                            UsuarioSRC = nuevoUsuario,
                            SociedadFkId = sociedad!.Id,
                        });
                    }
                }
            }

            await this.UnitOfWork.SaveAsync(cancellationToken);

            return Response.Success();
        }
    }
}
