namespace UsuariosAPI.Test.Controllers.UsuarioSRC
{
    using System.Net;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using UsuariosAPI.Application.Features.Usuario.CommandModificarRolesSociedades;
    using UsuariosAPI.Application.Features.Usuario.QueryExportarUsuarios;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarioById;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarios;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosByIdBatch;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.WebAPI.Controllers;

    public class UsuarioSRCControllerTests
    {
        [Test]
        public async Task GetUsuarioById_Success_Returns_Ok()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = Response.Success(new UsuarioDTO());

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuarioByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuarioById(new GetByIdDTO { Id = 1 }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetUsuarioById_Failure_Returns_NotFound()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = Response.Failure<UsuarioDTO>(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuarioByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuarioById(new GetByIdDTO { Id = 1 }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetUsuarioById_Failure_Returns_BadRequest()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuarioById(null!, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetUsuariosByIds_Success_Returns_Ok()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();
            var mockResultado = PaginatedResponse<UsuarioAcotadoDTO>.Success([], new PaginationData());

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuariosByIdBatchQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuariosByIds(new GetByIdsListDTO { Ids = [1] }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetUsuariosByIds_Failure_Returns_NotFound()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();
            var mockResultado = PaginatedResponse<UsuarioAcotadoDTO>.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuariosByIdBatchQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuariosByIds(new GetByIdsListDTO { Ids = [999] }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetUsuariosByIds_NullDto_Returns_BadRequest()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuariosByIds(null!, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PostUsuariosByIds_Success_Returns_Ok()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();
            var mockResultado = PaginatedResponse<UsuarioAcotadoDTO>.Success([], new PaginationData());

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuariosByIdBatchQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.PostUsuariosByIds(new GetByIdsListDTO { Ids = [1] }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task PostUsuariosByIds_Failure_Returns_NotFound()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();
            var mockResultado = PaginatedResponse<UsuarioAcotadoDTO>.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuariosByIdBatchQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.PostUsuariosByIds(new GetByIdsListDTO { Ids = [999] }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task PostUsuariosByIds_NullDto_Returns_BadRequest()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.PostUsuariosByIds(null!, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetUsuarios_Success_Returns_Ok()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = PaginatedResponse<UsuariosDTO>
                .Success(new List<UsuariosDTO?>(), new PaginationData());

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuarios(new GetPaginatedUsuariosDTO(), default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetUsuarios_Failure_Returns_NotFound()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = PaginatedResponse<UsuariosDTO>
                .Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));

            mediatorMock.Setup(x => x.Send(It.IsAny<UsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.GetUsuarios(new GetPaginatedUsuariosDTO(), default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task UpdateUsuario_Success_Returns_Ok()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = Response.Success();

            mediatorMock.Setup(x => x.Send(It.IsAny<ModificarRolesSociedadesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.UpdateUsuario(new UpdateUsuarioRolesSociedadesDTO() { UsuarioSRCId = 1 }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task UpdateUsuario_NullDto_Returns_BadRequest()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (BadRequestResult)await controller.UpdateUsuario(new UpdateUsuarioRolesSociedadesDTO() { UsuarioSRCId = 1 }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUsuario_Failure_Returns_BadRequest()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = Response.Failure(UsuariosError.OperacionNoPermitida("No se pueden eliminar roles automáticos."));

            mediatorMock.Setup(x => x.Send(It.IsAny<ModificarRolesSociedadesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.UpdateUsuario(new UpdateUsuarioRolesSociedadesDTO() { UsuarioSRCId = 1 }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateUsuario_Returns_NotFound()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = Response.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));

            mediatorMock.Setup(x => x.Send(It.IsAny<ModificarRolesSociedadesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (ObjectResult)await controller.UpdateUsuario(new UpdateUsuarioRolesSociedadesDTO() { UsuarioSRCId = 1 }, default);

            // Assert
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetArchivoUsuarios_Success_Returns_CorrectFileName()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();
            var data = new FileResponseDTO();

            var mockResultado = Response.Success(data);

            mediatorMock.Setup(x => x.Send(It.IsAny<ExportarUsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (FileContentResult)await controller.GetArchivoUsuarios(new GetUsuariosDTO(), default);

            // Assert
            Assert.That(result.FileDownloadName, Is.EqualTo(data.FileName));
        }

        [Test]
        public async Task GetArchivoUsuarios_Failure_Returns_NoDataFileName()
        {
            // Prepare
            var mediatorMock = new Mock<IMediator>();

            var mockResultado = Response.Failure<FileResponseDTO>(UsuariosError.ArchivoVacio);

            mediatorMock.Setup(x => x.Send(It.IsAny<ExportarUsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado);

            var controller = new UsuariosController(mediatorMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            // Execute
            var result = (FileContentResult)await controller.GetArchivoUsuarios(new GetUsuariosDTO(), default);

            // Assert
            Assert.That(result.FileDownloadName, Is.EqualTo("NoData"));
        }
    }
}
