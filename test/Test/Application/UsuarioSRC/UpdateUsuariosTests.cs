namespace UsuariosAPI.Test.Application.UsuarioSRC
{
    using MockQueryable.Moq;
    using Moq;
    using UsuariosAPI.Application.Features.Usuario.CommandModificarRolesSociedades;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.CompatibilidadRol;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.Test.FactoryMethods;

    public class UpdateUsuariosTests
    {
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<IUsuarioSRCRepository> mockUsuarioRepository;
        private Mock<IRolRepository> mockRolRepository;
        private Mock<IUsuarioRolRepository> mockUsuarioRolRepository;
        private Mock<IUsuarioSociedadRepository> mockUsuarioSociedadRepository;
        private Mock<ISociedadServiceConsumer> sociedadServiceConsumerMock;
        private Mock<ICompatibilidadRolRepository> mockCompatibilidadRolRepository;
        private ModificarRolesSociedadesCommandHandler modificarRolesSociedadesCommandHandler;

        [SetUp]
        public void Setup()
        {
            this.mockUsuarioRepository = new Mock<IUsuarioSRCRepository>();
            this.mockRolRepository = new Mock<IRolRepository>();
            this.mockUsuarioRolRepository = new Mock<IUsuarioRolRepository>();
            this.mockUsuarioSociedadRepository = new Mock<IUsuarioSociedadRepository>();
            this.sociedadServiceConsumerMock = new Mock<ISociedadServiceConsumer>();
            this.mockCompatibilidadRolRepository = new Mock<ICompatibilidadRolRepository>();

            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockUnitOfWork.Setup(m => m.UsuarioSRCRepository)
                .Returns(this.mockUsuarioRepository.Object);
            this.mockUnitOfWork.Setup(m => m.RolRepository).Returns(this.mockRolRepository.Object);
            this.mockUnitOfWork.Setup(m => m.UsuarioRolRepository).Returns(this.mockUsuarioRolRepository.Object);
            this.mockUnitOfWork.Setup(m => m.UsuarioSociedadRepository).Returns(this.mockUsuarioSociedadRepository.Object);
            this.mockUnitOfWork.Setup(m => m.CompatibilidadRolRepository).Returns(this.mockCompatibilidadRolRepository.Object);

            this.modificarRolesSociedadesCommandHandler = new ModificarRolesSociedadesCommandHandler(this.mockUnitOfWork.Object, this.sociedadServiceConsumerMock.Object);
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UsuarioNotFound()
        {
            // Arrange
            var dto = new UpdateUsuarioRolesSociedadesDTO { UsuarioSRCId = 1 };
            var command = new ModificarRolesSociedadesCommand(dto);

            this.mockUsuarioRepository
                .Setup(repo => repo.GetByIdWithAllDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UsuarioSRC)null!);

            // Act
            var result = await this.modificarRolesSociedadesCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors!.First(), Is.EqualTo(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC))));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_RolesAutomaticosEliminados()
        {
            // Arrange
            var usuario = UsuarioSRCFactory.CreateAllData();
            usuario.Roles!.First().Rol!.EsManual = false;

            var dto = new UpdateUsuarioRolesSociedadesDTO { UsuarioSRCId = 1, RolesIds = new List<int> { 2 } };
            var command = new ModificarRolesSociedadesCommand(dto);

            this.mockUsuarioRepository
                .Setup(repo => repo.GetByIdWithAllDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await this.modificarRolesSociedadesCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors!.First(), Is.EqualTo(UsuariosError.OperacionNoPermitida("No se pueden eliminar roles automáticos.")));
            this.mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Handle_Should_AddAndRemoveRolesSuccessfully()
        {
            // Arrange
            var usuario = UsuarioSRCFactory.CreateAllData();
            var dto = new UpdateUsuarioRolesSociedadesDTO { UsuarioSRCId = 1, RolesIds = new List<int> { 2 } };
            var command = new ModificarRolesSociedadesCommand(dto);

            this.mockUsuarioRepository
                .Setup(repo => repo.GetByIdWithAllDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            var rolToAdd = new Rol { Id = 2, Nombre = "GIP", AsignaSociedad = false, EsManual = true };

            this.mockRolRepository
                .Setup(repo => repo.GetByIds(It.IsAny<List<int>>()))
                .Returns(new List<Rol> { rolToAdd }.AsQueryable().BuildMock());

            this.mockRolRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rolToAdd);

            this.mockCompatibilidadRolRepository
                .Setup(repo => repo.GetRolesIncompatibles(It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .Returns(new List<int>());

            // Act
            var result = await this.modificarRolesSociedadesCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(usuario.Roles!.Count, Is.EqualTo(1));
            Assert.That(usuario.Roles!.First().RolId, Is.EqualTo(2));
            this.mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_ManageSociedadesSuccessfully()
        {
            // Arrange
            var usuario = UsuarioSRCFactory.CreateAllData();

            var dto = new UpdateUsuarioRolesSociedadesDTO { UsuarioSRCId = 1, SociedadesIds = new List<int> { 2 }, RolesIds = new List<int> { 2 } };
            var command = new ModificarRolesSociedadesCommand(dto);

            this.mockUsuarioRepository
                .Setup(repo => repo.GetByIdWithAllDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            var rolToAdd = new Rol { Id = 2, Nombre = "GIP", AsignaSociedad = true, EsManual = true };

            this.mockRolRepository
                .Setup(repo => repo.GetByIds(It.IsAny<List<int>>()))
                .Returns(new List<Rol> { rolToAdd }.AsQueryable().BuildMock());

            this.mockRolRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rolToAdd);

            this.mockCompatibilidadRolRepository
                .Setup(repo => repo.GetRolesIncompatibles(It.IsAny<List<int>>(), It.IsAny<List<int>>()))
                .Returns(new List<int>());

            this.sociedadServiceConsumerMock
                .Setup(repo => repo.ValidateSociedad(It.IsAny<int>()))
                .ReturnsAsync((UsuarioSRCFactory.CreateSociedadDTO(id: 2), null));

            // Act
            var result = await this.modificarRolesSociedadesCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(usuario.Sociedades!.Count, Is.EqualTo(1));
            Assert.That(usuario.Sociedades!.First().SociedadFkId, Is.EqualTo(2));
            this.mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_HandleNullRolesIdsAndSociedadesIds()
        {
            // Arrange
            var usuario = UsuarioSRCFactory.CreateAllData();

            var dto = new UpdateUsuarioRolesSociedadesDTO { UsuarioSRCId = 1, RolesIds = null, SociedadesIds = null };
            var command = new ModificarRolesSociedadesCommand(dto);

            this.mockUsuarioRepository
                .Setup(repo => repo.GetByIdWithAllDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await this.modificarRolesSociedadesCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            this.mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
        }
    }
}
