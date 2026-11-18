namespace UsuariosAPI.Application.CommonServices
{
    using APICore.Responses;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed class DirectorioUsuariosService : IDirectorioUsuariosService
    {
        private const int MaxResultados = 10;

        private readonly IUnitOfWork unitOfWork;
        private readonly IExternalDirectoryService externalDirectoryService;
        private readonly IMapper mapper;
        private readonly ILogger<DirectorioUsuariosService> logger;

        public DirectorioUsuariosService(
            IUnitOfWork unitOfWork,
            IExternalDirectoryService externalDirectoryService,
            IMapper mapper,
            ILogger<DirectorioUsuariosService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.externalDirectoryService = externalDirectoryService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<(List<UsuarioDirectorioDTO> Usuarios, Error? Error)> BuscarAsync(
            BuscarUsuarioDirectorioDTO dto,
            CancellationToken cancellationToken)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.PalabraClave))
            {
                return ([], Error.InvalidInput(nameof(dto.PalabraClave)));
            }

            var palabraClave = dto.PalabraClave.Trim();

            var usuariosSrc = await this.unitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuarios(new UsuariosSRCFilterAndOrderDTO
                {
                    PalabraClave = palabraClave,
                    SortBy = "NombreCompleto",
                    OrderAsc = true,
                })
                .Take(MaxResultados)
                .ToListAsync(cancellationToken);

            var resultado = usuariosSrc
                .Where(u => !u.EsExterno)
                .Select(u => this.MapDesdeSrc(u, dto.RolDestinoId))
                .ToList();

            var idRedsResueltos = resultado
                .Select(x => x.IdRed)
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            var (usuariosAd, errorAd) = await this.externalDirectoryService.Search(palabraClave);

            if (errorAd is not null)
            {
                this.logger.LogWarning(
                    "Directorio - AdHelper no respondió correctamente para PalabraClave={PalabraClave}. Error={Error}",
                    palabraClave,
                    errorAd.description);

                // Si AdHelper falla, se devuelve lo encontrado en SRC para no bloquear la operación.
                return (resultado, resultado.Count > 0 ? null : errorAd);
            }

            foreach (var usuarioAd in usuariosAd
                .Where(x => !string.IsNullOrWhiteSpace(x.IdRed))
                .Where(x => !idRedsResueltos.Contains(x.IdRed.Trim())))
            {
                if (resultado.Count >= MaxResultados)
                {
                    break;
                }

                idRedsResueltos.Add(usuarioAd.IdRed.Trim());
                resultado.Add(MapDesdeAd(usuarioAd));
            }

            return (resultado, null);
        }

        public async Task<(UsuarioDirectorioResueltoDTO? Usuario, Error? Error)> ResolverConRolAsync(
            ResolverUsuarioConRolDTO dto,
            CancellationToken cancellationToken)
        {
            if (dto is null || dto.RolDestinoId <= 0)
            {
                return (null, Error.InvalidInput(nameof(dto.RolDestinoId)));
            }

            if (string.IsNullOrWhiteSpace(dto.IdRed) && !dto.UsuarioId.HasValue)
            {
                return (null, Error.InvalidInput(nameof(dto.IdRed)));
            }

            var rol = await this.unitOfWork.RolRepository.GetByIdAsync(dto.RolDestinoId, cancellationToken);

            if (rol is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada(nameof(Domain.Rol.Rol)));
            }

            var codigoUo = string.IsNullOrWhiteSpace(dto.CodigoUO) ? null : dto.CodigoUO.Trim();
            var idRed = string.IsNullOrWhiteSpace(dto.IdRed) ? null : dto.IdRed.Trim();
            var usuario = await this.ObtenerUsuarioExistenteAsync(idRed, dto.UsuarioId, cancellationToken);
            var usuarioCreado = false;

            this.logger.LogInformation(
                "Directorio - Resolver usuario. IdRed={IdRed} UsuarioId={UsuarioId} RolDestinoId={RolDestinoId} Encontrado={Encontrado}",
                idRed,
                dto.UsuarioId,
                dto.RolDestinoId,
                usuario is not null);

            if (usuario is null)
            {
                if (idRed is null)
                {
                    return (null, UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
                }

                var (usuarioNuevo, errorCreacion) = await this.CrearDesdeAdHelperAsync(idRed, codigoUo, cancellationToken);

                if (errorCreacion is not null || usuarioNuevo is null)
                {
                    return (null, errorCreacion ?? Error.InvalidAPIResponse);
                }

                usuario = usuarioNuevo;
                usuarioCreado = true;
            }

            if (usuario.EsExterno)
            {
                return (null, UsuariosError.UsuarioEsExterno);
            }

            this.ActualizarDatosBasicos(usuario, codigoUo);

            var yaTeniaRol = usuario.Roles?.Any(r => r.RolId == dto.RolDestinoId) == true;
            var rolAsignado = false;

            if (!yaTeniaRol)
            {
                var rolesActuales = usuario.Roles?.Select(r => r.RolId).ToList() ?? [];
                var incompatibles = this.unitOfWork.CompatibilidadRolRepository
                    .GetRolesIncompatibles(rolesActuales, [dto.RolDestinoId]);

                if (incompatibles.Count > 0)
                {
                    return (null, UsuariosError.RolesIncompatibles);
                }

                var usuarioRol = new UsuarioRol { UsuarioSRCId = usuario.Id, RolId = dto.RolDestinoId };

                await this.unitOfWork.UsuarioRolRepository.AddAsync(usuarioRol, cancellationToken);
                usuario.Roles ??= [];
                usuario.Roles.Add(usuarioRol);
                rolAsignado = true;
            }

            await this.unitOfWork.SaveAsync(cancellationToken);

            var usuarioCompleto = await this.unitOfWork.UsuarioSRCRepository
                .GetByIdWithAllDataAsync(usuario.Id, cancellationToken) ?? usuario;

            return (
                new UsuarioDirectorioResueltoDTO
                {
                    Usuario = this.mapper.Map<UsuarioDTO>(usuarioCompleto),
                    UsuarioCreado = usuarioCreado,
                    YaTeniaRol = yaTeniaRol,
                    RolAsignado = rolAsignado,
                },
                null);
        }

        private static UsuarioDirectorioDTO MapDesdeAd(AdUsuarioDto usuarioAd)
        {
            var nombre = $"{usuarioAd.FirstName} {usuarioAd.LastName}".Trim();

            return new UsuarioDirectorioDTO
            {
                UsuarioId = null,
                IdRed = usuarioAd.IdRed.Trim(),
                NombreCompleto = string.IsNullOrWhiteSpace(nombre)
                    ? usuarioAd.DisplayName ?? usuarioAd.IdRed
                    : nombre,
                CorreoElectronico = usuarioAd.Email,
                ExisteEnSrc = false,
                Activo = true,
                RolesIds = [],
                TieneRolDestino = false,
                EsCompatibleConRolDestino = true,
            };
        }

        private UsuarioDirectorioDTO MapDesdeSrc(UsuarioSRC usuario, int? rolDestinoId)
        {
            var rolesIds = usuario.Roles?.Select(r => r.RolId).ToList() ?? [];
            var tieneRol = rolDestinoId.HasValue && rolesIds.Contains(rolDestinoId.Value);

            var esCompatible = !rolDestinoId.HasValue
                || tieneRol
                || this.unitOfWork.CompatibilidadRolRepository
                    .GetRolesIncompatibles(rolesIds, [rolDestinoId.Value]).Count == 0;

            return new UsuarioDirectorioDTO
            {
                UsuarioId = usuario.Id,
                IdRed = usuario.IdRed.Trim(),
                NombreCompleto = usuario.NombreCompleto,
                CorreoElectronico = usuario.CorreoElectronico,
                CodigoUO = usuario.CodigoUO,
                Legajo = usuario.Legajo,
                ExisteEnSrc = true,
                Activo = usuario.Activo,
                RolesIds = rolesIds,
                TieneRolDestino = tieneRol,
                EsCompatibleConRolDestino = esCompatible,
            };
        }

        private async Task<UsuarioSRC?> ObtenerUsuarioExistenteAsync(
            string? idRed,
            int? usuarioId,
            CancellationToken cancellationToken)
        {
            if (idRed is not null)
            {
                var porIdRed = await this.unitOfWork.UsuarioSRCRepository
                    .GetByIdRedWithAllDataAsync(idRed, cancellationToken);

                if (porIdRed is not null)
                {
                    return porIdRed;
                }
            }

            if (usuarioId is null || usuarioId.Value <= 0)
            {
                return null;
            }

            return await this.unitOfWork.UsuarioSRCRepository
                .GetByIdWithAllDataAsync(usuarioId.Value, cancellationToken);
        }

        private async Task<(UsuarioSRC?, Error?)> CrearDesdeAdHelperAsync(
            string idRed,
            string? codigoUo,
            CancellationToken cancellationToken)
        {
            var (usuariosAd, errorAd) = await this.externalDirectoryService.Search(idRed);

            if (errorAd is not null)
            {
                return (null, errorAd);
            }

            var usuarioAd = usuariosAd.FirstOrDefault(x =>
                string.Equals(x.IdRed, idRed, StringComparison.InvariantCultureIgnoreCase))
                ?? (usuariosAd.Count == 1 ? usuariosAd[0] : null);

            if (usuarioAd is null)
            {
                this.logger.LogWarning(
                    "Directorio - AdHelper no devolvió coincidencia para IdRed={IdRed}. Resultados={Cantidad}",
                    idRed,
                    usuariosAd.Count);

                return (null, UsuariosError.UsuarioNoEncontrado(idRed));
            }

            var nuevoUsuario = new UsuarioSRC
            {
                IdRed = usuarioAd.IdRed.Trim(),
                NombreCompleto = $"{usuarioAd.FirstName} {usuarioAd.LastName}".Trim(),
                CorreoElectronico = usuarioAd.Email ?? string.Empty,
                CodigoUO = codigoUo,
                EsExterno = false,
                Activo = true,
            };

            await this.unitOfWork.UsuarioSRCRepository.AddAsync(nuevoUsuario, cancellationToken);
            await this.unitOfWork.SaveAsync(cancellationToken);

            return (nuevoUsuario, null);
        }

        private void ActualizarDatosBasicos(UsuarioSRC usuario, string? codigoUo)
        {
            if (!usuario.Activo)
            {
                usuario.Activo = true;
            }

            if (!string.IsNullOrWhiteSpace(codigoUo) &&
                !string.Equals(usuario.CodigoUO, codigoUo, StringComparison.InvariantCultureIgnoreCase))
            {
                usuario.CodigoUO = codigoUo;
            }
        }
    }
}
