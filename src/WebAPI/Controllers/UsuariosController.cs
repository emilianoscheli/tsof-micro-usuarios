namespace UsuariosAPI.WebAPI.Controllers
{
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UsuariosAPI.Application.Features.Directorio.CommandResolverUsuarioConRol;
    using UsuariosAPI.Application.Features.Directorio.QueryBuscarUsuariosDirectorio;
    using UsuariosAPI.Application.Features.Rol.QueryPermisosByRoles;
    using UsuariosAPI.Application.Features.Rol.QueryRolById;
    using UsuariosAPI.Application.Features.Rol.QueryRoles;
    using UsuariosAPI.Application.Features.Rol.QueryRolesByIdBatch;
    using UsuariosAPI.Application.Features.Usuario.CommandActualizarLegajos;
    using UsuariosAPI.Application.Features.Usuario.CommandAgregarRoles;
    using UsuariosAPI.Application.Features.Usuario.CommandAsegurarUsuarioInterno;
    using UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioInterno;
    using UsuariosAPI.Application.Features.Usuario.CommandEliminarRolesUsuarioBatch;
    using UsuariosAPI.Application.Features.Usuario.CommandModificarRolesSociedades;
    using UsuariosAPI.Application.Features.Usuario.QueryExportarUsuarios;
    using UsuariosAPI.Application.Features.Usuario.QueryObtenerUoRdiPorUsuarios;
    using UsuariosAPI.Application.Features.Usuario.QueryObtenerUoRdiUsuario;
    using UsuariosAPI.Application.Features.Usuario.QueryReporteUsuariosRolesSRC;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarioById;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarioByIdRed;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarioPermisos;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarios;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosByIdBatch;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosByRolIdBatch;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosIds;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosSelect;
    using UsuariosAPI.Application.Features.Usuario.QueryValidarCompatibilidadRoles;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.CompatibilidadRol;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol;

    [ApiController]
    [Route("api/usuarios/v1/usuarios")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsuariosController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("permisos-usuario")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioRolesPermisosDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermisosUsuario([FromQuery] GetPermisosUsuarioDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuarioPermisosQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuariosDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarios([FromQuery] GetPaginatedUsuariosDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosQuery(dto ?? new GetPaginatedUsuariosDTO(), true);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("exportar")]
        [Authorize(Policy = "Usuarios_IngresarModulo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<byte[]>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArchivoUsuarios([FromQuery] GetUsuariosDTO dto, CancellationToken cancellationToken)
        {
            var query = new ExportarUsuariosQuery(dto ?? new GetUsuariosDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.File(
                result.Data!.FileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.Data!.FileName)
                 : this.File([], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NoData");
        }

        [HttpGet("idred/{IdRed}")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarioByIdRed([FromRoute] GetUsuarioByIdRedDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.IdRed))
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuarioByIdRedQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("{Id}")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarioById([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuarioByIdQuery(dto, true);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/{Id}")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarioByIdAzureAD([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuarioByIdQuery(dto, false);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("batch")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosByIds([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuariosByIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/batch")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosByIdsAzureAD([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuariosByIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("batch")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostUsuariosByIds([FromBody] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuariosByIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("azureAD/batch")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostUsuariosByIdsAzureAD([FromBody] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuariosByIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("por-rol/batch")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosByRolesId([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuariosByRolIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/por-rol/batch")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosByRolesIdAzureAD([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuariosByRolIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("ids")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostUsuariosIds([FromBody] GetUsuariosIdsDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosIdsQuery(dto ?? new GetUsuariosIdsDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("azureAD/ids")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostUsuariosIdsAzureAD([FromBody] GetUsuariosIdsDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosIdsQuery(dto ?? new GetUsuariosIdsDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost]
        [Authorize(Policy = "Usuarios_Crear")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearUsuarioInterno([FromBody] CrearUsuarioInternoDTO request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new CrearUsuarioInternoCommand(request);

            var result = await this.mediator.Send(command, cancellationToken);

            if (result is null)
            {
                return this.BadRequest();
            }

            if (!result.IsSuccess)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        [HttpPut]
        [Authorize(Policy = "Usuarios_Editar")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUsuario([FromBody] UpdateUsuarioRolesSociedadesDTO request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new ModificarRolesSociedadesCommand(request);

            var result = await this.mediator.Send(command, cancellationToken);

            if (result is null)
            {
                return this.BadRequest();
            }

            if (!result.IsSuccess)
            {
                var isNotFound = result.Errors!.First()!.code == UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)).code;
                return isNotFound ? this.NotFound(result) : this.BadRequest(result);
            }

            return this.Ok(result);
        }

        [HttpPatch("agregar-rol")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AgregarRolAUsuario([FromBody] AgregarRolesUsuarioDTO request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new AgregarRolesCommand(request);

            var result = await this.mediator.Send(command, cancellationToken);

            if (result is null)
            {
                return this.BadRequest();
            }

            if (!result.IsSuccess)
            {
                var isNotFound = result.Errors!.First()!.code == UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)).code;
                return isNotFound ? this.NotFound(result) : this.BadRequest(result);
            }

            return this.Ok(result);
        }

        [HttpGet("roles")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<RolDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoles([FromQuery] GetRolDTO dto, CancellationToken cancellationToken)
        {
            var query = new RolesQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("roles/{Id}")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<RolDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRolById([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new RolByIdQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("permisos-por-roles")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<int>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermisosByRoles([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new PermisosByRolesQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("solicitud-rdi-uo/{Id}")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioSolicitanteDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUoRdiUsuario([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new ObtenerUoRdiUsuarioQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("solicitud-rdi-uo")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<UsuarioSolicitanteDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUoRdiPorUsuarios([FromBody] GetUoRdiPorUsuariosDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new ObtenerUoRdiPorUsuariosQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPatch("eliminar-rol-usuarios")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarRoleBatchUsuarios([FromBody] UpdateUsuarioRolesBatchDTO request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new EliminarRolesUsuarioBatchCommand(request);

            var result = await this.mediator.Send(command, cancellationToken);

            if (result is null)
            {
                return this.BadRequest();
            }

            if (!result.IsSuccess)
            {
                var isNotFound = result.Errors!.First()!.code == UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)).code;
                return isNotFound ? this.NotFound(result) : this.BadRequest(result);
            }

            return this.Ok(result);
        }

        [HttpGet("reportes/usuarios-roles-src")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<ReporteUsuariosRolesSRCDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReporteUsuariosRolesSRC([FromQuery] GetPaginatedReporteUsuariosRolesSRCDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new ReporteUsuariosRolesSRCQuery(dto, true);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/reportes/usuarios-roles-src")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(PaginatedResponse<ReporteUsuariosRolesSRCDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReporteUsuariosRolesSRCAzureAD([FromQuery] GetPaginatedReporteUsuariosRolesSRCDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new ReporteUsuariosRolesSRCQuery(dto, false);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("roles/batch")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<RolDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRolesByIds([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new RolesByIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("select-usuarios")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosSelect([FromQuery] GetPaginatedUsuariosDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosSelectQuery(dto ?? new GetPaginatedUsuariosDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuariosDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosAzureAD([FromQuery] GetPaginatedUsuariosDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosQuery(dto ?? new GetPaginatedUsuariosDTO(), false);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/idred/{IdRed}")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(Response<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarioByIdRedAzureAD([FromRoute] GetUsuarioByIdRedDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.IdRed))
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuarioByIdRedQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/roles")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(Response<List<RolDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRolesAzureAD([FromQuery] GetRolDTO dto, CancellationToken cancellationToken)
        {
            var query = new RolesQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/roles/{Id}")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(Response<RolDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRolByIdAzureAD([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new RolByIdQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/roles/batch")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(PaginatedResponse<RolDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRolesByIdsAzureAD([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new RolesByIdBatchQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/permisos-por-roles")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(Response<List<int>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermisosByRolesAzureAD([FromQuery] GetByIdsListDTO dto, CancellationToken cancellationToken)
        {
            if (dto?.Ids is null || dto.Ids.Count == 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new PermisosByRolesQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/select-usuarios")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuarioAcotadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosSelectAzureAD([FromQuery] GetPaginatedUsuariosDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosSelectQuery(dto ?? new GetPaginatedUsuariosDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("validar-compatibilidad-roles")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidarCompatibilidadRoles([FromQuery] ValidarCompatibilidadRolesDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.RolesOrigenIds is null || dto.RolesDestinoIds is null )
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new ValidarCompatibilidadRolesQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return this.Ok(result);
        }

        [HttpPost("azureAD/internos/asegurar")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AsegurarUsuarioInterno(
                        [FromBody] AsegurarUsuarioInternoDTO dto,
                        CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var command = new AsegurarUsuarioInternoCommand(dto);

            var result = await this.mediator.Send(command, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.BadRequest(result);
        }

        [HttpPost("sync-legajos")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> SyncLegajos(CancellationToken cancellationToken)
        {
            var result = await this.mediator.Send(new ActualizarLegajosCommand(), cancellationToken);
            return result.IsSuccess ? this.Ok(result) : this.BadRequest(result);
        }

        /// <summary>
        /// Búsqueda unificada de usuarios internos: BD de usuarios SRC combinada con AdHelper.
        /// </summary>
        [HttpGet("directorio/buscar")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<UsuarioDirectorioDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarUsuariosDirectorio(
            [FromQuery] BuscarUsuarioDirectorioDTO dto,
            CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var result = await this.mediator.Send(new BuscarUsuariosDirectorioQuery(dto), cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.BadRequest(result);
        }

        /// <inheritdoc cref="BuscarUsuariosDirectorio"/>
        [HttpGet("azureAD/directorio/buscar")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<UsuarioDirectorioDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarUsuariosDirectorioAzureAD(
            [FromQuery] BuscarUsuarioDirectorioDTO dto,
            CancellationToken cancellationToken)
        {
            return await this.BuscarUsuariosDirectorio(dto, cancellationToken);
        }

        /// <summary>
        /// Resuelve el usuario (SRC o AdHelper) y garantiza que tenga asignado el rol indicado.
        /// </summary>
        [HttpPost("directorio/resolver-con-rol")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioDirectorioResueltoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResolverUsuarioConRol(
            [FromBody] ResolverUsuarioConRolDTO dto,
            CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var result = await this.mediator.Send(new ResolverUsuarioConRolCommand(dto), cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.BadRequest(result);
        }

        /// <summary>
        /// Versión Azure AD de <see cref="EliminarRoleBatchUsuarios"/>, para llamadas entre APIs sin usuario logueado
        /// (por ejemplo, procesos programados disparados por Azure Service Bus).
        /// </summary>
        [HttpPatch("azureAD/eliminar-rol-usuarios")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarRoleBatchUsuariosAzureAD([FromBody] UpdateUsuarioRolesBatchDTO request, CancellationToken cancellationToken)
        {
            return await this.EliminarRoleBatchUsuarios(request, cancellationToken);
        }

        /// <inheritdoc cref="ResolverUsuarioConRol"/>
        [HttpPost("azureAD/directorio/resolver-con-rol")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioDirectorioResueltoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResolverUsuarioConRolAzureAD(
            [FromBody] ResolverUsuarioConRolDTO dto,
            CancellationToken cancellationToken)
        {
            return await this.ResolverUsuarioConRol(dto, cancellationToken);
        }
    }
}
