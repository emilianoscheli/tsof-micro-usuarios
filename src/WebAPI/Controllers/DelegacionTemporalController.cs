namespace UsuariosAPI.WebAPI.Controllers
{
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using UsuariosAPI.Application.Features.DelegacionTemporal.CommandCreateDelegacionTemporal;
    using UsuariosAPI.Application.Features.DelegacionTemporal.CommandDesactivarDelegacionTemporal;
    using UsuariosAPI.Application.Features.DelegacionTemporal.QueryArchivoRecursosAfectacosPorMotivoDelegacion;
    using UsuariosAPI.Application.Features.DelegacionTemporal.QueryCantidadRecursosAfectadosPorMotivoDelegacion;
    using UsuariosAPI.Application.Features.DelegacionTemporal.QueryDelegacionTemporal;
    using UsuariosAPI.Application.Features.DelegacionTemporal.QueryDelegacionTemporalById;
    using UsuariosAPI.Application.Features.DelegacionTemporal.QueryExportDelegacionTemporal;
    using UsuariosAPI.Application.Features.DelegacionTemporal.QueryReporteDelegacionTemporal;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    [ApiController]
    [Route("api/usuarios/v1/delegacion-temporal")]
    [Authorize]
    public class DelegacionTemporalController : ControllerBase
    {
        private readonly IMediator mediator;

        public DelegacionTemporalController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<DelegacionTemporalDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDelegacionTemporal([FromQuery] GetDelegacionTemporalPaginatedDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new DelegacionTemporalQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("{Id}")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<DelegacionTemporalDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDelegacionTemporalById([FromRoute] GetByIdDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new DelegacionTemporalByIdQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpPost]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDelegacionTemporal([FromBody] CreateDelegacionTemporalDTO dto, CancellationToken cancellationToken)
        {
            var command = new CreateDelegacionTemporalCommand(dto);

            var result = await this.mediator.Send(command, cancellationToken);

            if (result is null || !result.IsSuccess)
            {
                return this.BadRequest(result);
            }

            return this.Created(this.HttpContext.Request.GetDisplayUrl(), result);
        }

        [HttpPatch]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        public async Task<IActionResult> DesactivarDelegacionTemporal([FromQuery] CambiarEstadoConMotivoDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new DesactivarDelegacionTemporalCommand(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            if (result is null)
            {
                return this.BadRequest();
            }

            if (!result.IsSuccess)
            {
                var isNotFound = result.Errors!.First()!.code == UsuariosError.EntidadNoEncontrada(nameof(DelegacionTemporal)).code;
                return isNotFound ? this.NotFound(result) : this.BadRequest(result);
            }

            return this.Ok(result);
        }

        [HttpGet("exportar-delegacion-temporal")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<byte[]>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportarDelegacionesTemporales([FromQuery] GetDelegacionTemporalPaginatedDTO dto, CancellationToken cancellationToken)
        {
            var query = new ExportDelegacionTemporalQuery(dto);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.File(
                result.Data!.FileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.Data!.FileName)
                 : this.File([], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NoData");
        }

        [HttpGet("reportes/delegaciones-temporales-usuarios")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<ReporteDelegacionTemporalDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReporteDelegacionesTemporales([FromQuery] GetPaginatedReporteDelegacionTemporalDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new ReporteDelegacionTemporalQuery(dto, true);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("azureAD/reportes/delegaciones-temporales-usuarios")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        [ProducesResponseType(typeof(PaginatedResponse<ReporteDelegacionTemporalDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReporteDelegacionesTemporalesAzureAD([FromQuery] GetPaginatedReporteDelegacionTemporalDTO dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return this.BadRequest(Error.InvalidInput("dto"));
            }

            var query = new ReporteDelegacionTemporalQuery(dto, false);

            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("recursos-asociados/motivo-delegacion/{Id}")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<CantidadRecursosAfectadosDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCantidadRecursosAfectadosPorMotivoDelegacion(
            [FromRoute] GetByIdDTO dto,
            CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("id"));
            }

            var query = new CantidadRecursosAfectadosPorMotivoDelegacionQuery(dto);
            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }

        [HttpGet("recursos-asociados/motivo-delegacion/excel/{Id}")]
        [Authorize(Policy = "Acceso_General")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Response<List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExcelRecursosAfectadosPorMotivoDelegacion(
            [FromRoute] GetByIdDTO dto,
            CancellationToken cancellationToken)
        {
            if (dto is null || dto.Id <= 0)
            {
                return this.BadRequest(Error.InvalidInput("id"));
            }

            var query = new ArchivoRecursosAfectadosPorMotivoDelegacionQuery(dto);
            var result = await this.mediator.Send(query, cancellationToken);

            return result.IsSuccess ? this.Ok(result) : this.NotFound(result);
        }
    }
}
