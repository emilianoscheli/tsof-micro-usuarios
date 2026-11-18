namespace UsuariosAPI.WebAPI.Controllers
{
    using APICore.Responses;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UsuariosAPI.Application.Features.AdUsuarios.QueryBuscarAdUsuario;
    using UsuariosAPI.Application.Features.AdUsuarios.QueryBuscarAdUsuarioExistente;
    using UsuariosAPI.Application.Features.AdUsuarios.QueryObtenerAdUsuarioPorIdRed;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    [ApiController]
    [Route("api/usuarios/v1/ad-usuarios")]
    [Authorize]
    public class AdUsuarioController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdUsuarioController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        /*[Authorize(Policy = "Acceso_General")]*/
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<AdUsuarioDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarUsuarios([FromQuery] string palabraClave, CancellationToken cancellationToken)
        {
            var query = new BuscarAdUsuarioQuery(palabraClave);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.BadRequest(result);
        }

        [HttpGet("{idRed}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<AdUsuarioDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerUsuarioPorIdRed([FromRoute] string idRed, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idRed))
            {
                return this.BadRequest(Error.InvalidInput(nameof(idRed)));
            }

            var result = await this.mediator.Send(new ObtenerAdUsuarioPorIdRedQuery(idRed), cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("existentes")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<UsuariosDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarUsuariosExistentes([FromQuery] string? palabraClave, [FromQuery] int? rolId, CancellationToken cancellationToken)
        {
            var query = new BuscarAdUsuarioExistenteQuery(palabraClave, rolId);

            var result = await this.mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return this.BadRequest(result);
            }

            if (result.Data == null || result.Data.All(x => x == null))
            {
                var message = result.PaginationData?.TotalCount > 0
                    ? "Se encontraron usuarios en la base de datos, pero no fueron hallados en el servicio de directorio externo (AD)."
                    : "No se encontraron usuarios en la base de datos.";

                return this.Ok(new { result.IsSuccess, result.Data, result.PaginationData, Message = message });
            }

            return this.Ok(result);
        }
    }
}
