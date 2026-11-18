namespace UsuariosAPI.Test.Application.DelegacionTemporal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Enums;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using UsuariosAPI.Application.Features.DelegacionTemporal.CommandCreateDelegacionTemporal;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.DelegacionTemporal.Repository;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;
    using UsuariosAPI.Test.FactoryMethods;              // 👈  Factory de usuarios

    public class CreateDelegacionTemporalHandlerTests
    {
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IMapper> mapperMock;
        private Mock<IDelegacionTemporalRepository> delegacionRepoMock;
        private Mock<IUsuarioSRCRepository> usuarioRepoMock;
        private Mock<IAuthenticatedUserService> authenticatedUserServiceMock;

        private CreateDelegacionTemporalCommandHandler handler;

        [SetUp]
        public void SetUp()
        {
            this.unitOfWorkMock = new Mock<IUnitOfWork>();
            this.mapperMock = new Mock<IMapper>();
            this.delegacionRepoMock = new Mock<IDelegacionTemporalRepository>();
            this.usuarioRepoMock = new Mock<IUsuarioSRCRepository>();
            this.authenticatedUserServiceMock = new Mock<IAuthenticatedUserService>();

            this.unitOfWorkMock.SetupGet(u => u.DelegacionTemporalRepository)
                               .Returns(this.delegacionRepoMock.Object);
            this.unitOfWorkMock.SetupGet(u => u.UsuarioSRCRepository)
                               .Returns(this.usuarioRepoMock.Object);
            this.unitOfWorkMock.Setup(u => u.SaveAsync(
                                           It.IsAny<CancellationToken>(),
                                           It.IsAny<bool>(),
                                           It.IsAny<string?>()))
                               .ReturnsAsync(1);

            this.handler = new CreateDelegacionTemporalCommandHandler(
                               this.unitOfWorkMock.Object,
                               this.mapperMock.Object,
                               this.authenticatedUserServiceMock.Object);
        }

        // ----- 1) cuando el delegador YA es delegado activo ------------------
        [Test]
        public async Task Handle_ShouldFail_WhenDelegadorIsAlreadyDelegated()
        {
            // Arrange
            var delegador = UsuarioSRCFactory.Create(                       // id = 10
                id: 10,
                nombreApellido: "Delegador",
                idRed: "SE46005",
                correo: "delegador@mail.com");

            var delegado = UsuarioSRCFactory.Create(                       // id = 20
                id: 20,
                nombreApellido: "Delegado",
                idRed: "ET104230",
                correo: "delegado@mail.com");

            this.usuarioRepoMock.Setup(r => r.GetByIdRedWithDependenciesAsync(
                                            "SE46005", It.IsAny<CancellationToken>()))
                                .ReturnsAsync(delegador);
            this.usuarioRepoMock.Setup(r => r.GetByIdRedWithDependenciesAsync(
                                            "ET104230", It.IsAny<CancellationToken>()))
                                .ReturnsAsync(delegado);

            this.delegacionRepoMock.Setup(r => r.ExisteSuperposicionComoDelegadoAsync(
                                                delegador.Id,
                                                It.IsAny<DateOnly>(),
                                                It.IsAny<DateOnly>(),
                                                It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(true);                     // 👈  dispara la regla

            var dto = new CreateDelegacionTemporalDTO
            {
                DelegadorIdRed = "SE46005",
                DelegadoIdRed = "ET104230",
                FechaDesde = new DateOnly(2025, 5, 20),
                FechaHasta = new DateOnly(2025, 6, 15),
                Observacion = "test motivo",
                MotivoId = 1,
                TiempoLimite = 1,
            };

            var command = new CreateDelegacionTemporalCommand(dto);

            // Act
            var response = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(response.IsSuccess, Is.False);
        }

        // ----- 2) cuando NO hay delegaciones superpuestas --------------------
        [Test]
        public async Task Handle_ShouldSucceed_WhenDelegadorHasNoActiveDelegations()
        {
            // Arrange
            var delegador = UsuarioSRCFactory.Create(
                id: 11,
                nombreApellido: "Delegador 2",
                idRed: "A1",
                correo: "d2@mail.com");

            var delegado = UsuarioSRCFactory.Create(
                id: 22,
                nombreApellido: "Delegado 2",
                idRed: "B1",
                correo: "de2@mail.com");

            this.authenticatedUserServiceMock.Setup(r => r.AuthenticatedUserEsRol(It.IsAny<RolesEnum>()))
                 .Returns(true);
            this.authenticatedUserServiceMock.Setup(r => r.GetAuthenticatedUserId())
                 .Returns(11);

            this.usuarioRepoMock.Setup(r => r.GetByIdRedWithDependenciesAsync("A1", It.IsAny<CancellationToken>()))
                                .ReturnsAsync(delegador);
            this.usuarioRepoMock.Setup(r => r.GetByIdRedWithDependenciesAsync("B1", It.IsAny<CancellationToken>()))
                                .ReturnsAsync(delegado);

            this.delegacionRepoMock.Setup(r => r.ExisteSuperposicionComoDelegadoAsync(
                                                delegador.Id,
                                                It.IsAny<DateOnly>(),
                                                It.IsAny<DateOnly>(),
                                                It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(false);

            // El mapper necesita devolver una entidad con TODOS los 'required'
            this.mapperMock.Setup(m => m.Map<DelegacionTemporal>(It.IsAny<CreateDelegacionTemporalDTO>()))
                           .Returns(new DelegacionTemporal
                           {
                               UsuarioDelegadorId = 11,
                               UsuarioDelegadoId = 22,
                               MotivoDelegacionFkId = 1,
                               TiempoLimite = 10,
                               FechaDesde = new DateOnly(2025, 5, 20),
                               FechaHasta = new DateOnly(2025, 6, 15),
                               Observacion = "ok",
                           });

            this.delegacionRepoMock.Setup(r => r.SyncRoles(delegador.Id, delegado.Id, It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(new List<int>());

            var dto = new CreateDelegacionTemporalDTO
            {
                DelegadorIdRed = "A1",
                DelegadoIdRed = "B1",
                FechaDesde = new DateOnly(2025, 5, 20),
                FechaHasta = new DateOnly(2025, 6, 15),
                Observacion = "ok",
                MotivoId = 1,
                TiempoLimite = 1,
            };

            var command = new CreateDelegacionTemporalCommand(dto);

            // Act
            var response = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(response.IsSuccess, Is.True);
        }
    }
}
