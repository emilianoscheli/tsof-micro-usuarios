namespace UsuariosAPI.WebAPI.Controllers
{
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UsuariosAPI.Application.Features.Usuario.CommandActivarUsuarioExterno;
    using UsuariosAPI.Application.Features.Usuario.CommandCambiarRolUsuarioExterno;
    using UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioExterno;
    using UsuariosAPI.Application.Features.Usuario.CommandDesactivarUsuarioExterno;
    using UsuariosAPI.Application.Features.Usuario.CommandEditarDatosPersonalesUsuarioExterno;
    using UsuariosAPI.Application.Features.Usuario.QueryExportarUsuariosExternos;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarioExternoById;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosExternos;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosExternosIds;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    [ApiController]
    [Route("api/usuarios/v1/usuarios-externos")]
    [Authorize]
    public class UsuariosExternosController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsuariosExternosController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "UsuariosExternos_VisualizarListado")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuariosExternosDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosExternos([FromQuery] GetPaginatedUsuariosExternosDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosExternosQuery(dto ?? new GetPaginatedUsuariosExternosDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("exportar")]
        [Authorize(Policy = "UsuariosExternos_ExportarListado")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<byte[]>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArchivoUsuariosExternos([FromQuery] GetUsuariosExternosDTO dto, CancellationToken cancellationToken)
        {
            var query = new ExportarUsuariosExternosQuery(dto ?? new GetUsuariosExternosDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.File(
                result.Data!.FileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.Data!.FileName)
                 : this.File([], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NoData");
        }

        [HttpGet("{Id}")]
        [Authorize(Policy = "UsuariosExternos_VisualizarListado")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioExternoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarioExternoById([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuarioExternoByIdQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("ids")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostUsuariosExternosIds([FromBody] GetUsuariosExternosIdsDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosExternosIdsQuery(dto ?? new GetUsuariosExternosIdsDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost("azureAD/ids")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostUsuariosExternosIdsAzureAD([FromBody] GetUsuariosExternosIdsDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosExternosIdsQuery(dto ?? new GetUsuariosExternosIdsDTO());

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost]
        [Authorize(Policy = "UsuariosExternos_Crear")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CrearUsuarioExterno([FromBody] CrearUsuarioExternoDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new CrearUsuarioExternoCommand(dto);

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

        [HttpPut("editar-datos-personales")]
        [Authorize(Policy = "UsuariosExternos_Editar")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditarDatosPersonales([FromBody] EditarDatosPersonalesUsuarioExternoDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new EditarDatosPersonalesUsuarioExternoCommand(dto);

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

        [HttpPut("cambiar-rol")]
        [Authorize(Policy = "UsuariosExternos_Editar")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CambiarRol([FromBody] CambiarRolUsuarioExternoDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new CambiarRolUsuarioExternoCommand(dto);

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

        [HttpPatch("activar")]
        [Authorize(Policy = "UsuariosExternos_EditarEstado")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activar([FromBody] ActivarUsuarioExternoDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new ActivarUsuarioExternoCommand(dto);

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

        [HttpPatch("desactivar")]
        [Authorize(Policy = "UsuariosExternos_EditarEstado")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Desactivar([FromBody] DesactivarUsuarioExternoDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(APICore.Responses.Response.Failure(Error.NullValue));
            }

            var command = new DesactivarUsuarioExternoCommand(dto);

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

        [HttpGet("azureAD")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuariosExternosDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuariosExternosAzureAD([FromQuery] GetPaginatedUsuariosExternosDTO dto, CancellationToken cancellationToken)
        {
            var query = new UsuariosExternosQuery(dto ?? new GetPaginatedUsuariosExternosDTO(), false);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/exportar")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<byte[]>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArchivoUsuariosExternosAzureAD([FromQuery] GetUsuariosExternosDTO dto, CancellationToken cancellationToken)
        {
            var query = new ExportarUsuariosExternosQuery(dto ?? new GetUsuariosExternosDTO(), false);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.File(
                result.Data!.FileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.Data!.FileName)
                 : this.File([], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NoData");
        }

        [HttpGet("azureAD/{Id}")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<UsuarioExternoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsuarioExternoByIdAzureAD([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new UsuarioExternoByIdQuery(dto, false);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }
    }
}
