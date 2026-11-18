namespace UsuariosAPI.WebAPI.Controllers
{
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosByIdBatch;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    [Route("api/usuarios/v1/integraciones")]
    public class IntegracionesController : ControllerBase
    {
        private readonly IMediator mediator;

        public IntegracionesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("batch/usuarios")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
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
    }
}
