namespace UsuariosAPI.Test.Application.UsuarioSRC
{
    using APICore.Interfaces.Services;
    using APICore.Requests.DTOs;
    using AutoMapper;
    using Microsoft.Extensions.Configuration;
    using MockQueryable.Moq;
    using Moq;
    using UsuariosAPI.Application.Abstractions.Mappers;
    using UsuariosAPI.Application.Features.Usuario.QueryExportarUsuarios;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarioById;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarioByIdRed;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuarios;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.Test.FactoryMethods;

    public class GetUsuariosTests
    {
        private IMapper mapper;
        private MapperConfiguration config;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<IMapper> mockMapper;
        private Mock<IFileAdministrator> mockFileAdministrator;
        private Mock<IConfiguration> mockConfiguration;
        private Mock<ISociedadServiceConsumer> sociedadServiceConsumerMock;
        private Mock<IContratistaServiceConsumer> contratistaServiceConsumerMock;
        private Mock<IEstudioSedeServiceConsumer> estudioSedeServiceConsumerMock;

        [SetUp]
        public void Setup()
        {
            this.config = new MapperConfiguration(cfg => cfg.AddProfile<UsuarioSRCMapperProfile>());
            this.mapper = this.config.CreateMapper();
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockUnitOfWork.Setup(m => m.UsuarioSRCRepository).Returns(() => new Mock<IUsuarioSRCRepository>().Object);
            this.mockMapper = new Mock<IMapper>();
            this.mockFileAdministrator = new Mock<IFileAdministrator>();
            this.mockConfiguration = new Mock<IConfiguration>();
            this.sociedadServiceConsumerMock = new Mock<ISociedadServiceConsumer>();
            this.contratistaServiceConsumerMock = new Mock<IContratistaServiceConsumer>();
            this.estudioSedeServiceConsumerMock = new Mock<IEstudioSedeServiceConsumer>();
        }

        [Test]
        public void ConfigurationTest()
        {
            this.config.AssertConfigurationIsValid();
        }

        [Test]
        public void UsuariosMapperProfile_MapUsuarioSRCToUsuariosDTO()
        {
            // Arrange
            var usuarioSRC = UsuarioSRCFactory.CreateAllData();

            // Act
            var result = this.mapper.Map<UsuariosDTO>(usuarioSRC);

            // Assert
            Assert.That(result.Codigo, Is.EqualTo(usuarioSRC.Id));
            Assert.That(result.NombreCompleto, Is.EqualTo(usuarioSRC.NombreCompleto));
            Assert.That(result.IdRed, Is.EqualTo(usuarioSRC.IdRed));
            Assert.That(result.Roles, Is.EqualTo("Rol 1 - M"));
        }

        [Test]
        public void UsuariosMapperProfile_MapUsuarioSRCToUsuarioDetalleDTOList()
        {
            // Arrange
            var usuario = UsuarioSRCFactory.CreateAllData();

            var usuarioList = new List<UsuarioSRC> { usuario };

            // Act
            var result = this.mapper.Map<List<UsuarioDTO>>(usuarioList);

            // Assert
            Assert.That(result.First().Codigo, Is.EqualTo(usuarioList.First().Id));
            Assert.That(result.First().NombreCompleto, Is.EqualTo(usuarioList.First().NombreCompleto));
            Assert.That(result.First().IdRed, Is.EqualTo(usuarioList.First().IdRed));
            Assert.That(result.First().Roles, Is.EqualTo("Rol 1 - M"));
        }

        [Test]
        public async Task GetUsuarioByIdQuery_Should_Return_Usuario()
        {
            // Prepare
            var dto = new GetByIdDTO { Id = 1 };

            var usuarioSRC = UsuarioSRCFactory.CreateAllData(1, estudioAuditorId: null, sedeId: null);

            var usuarioSRCDTO = UsuarioSRCFactory.CreateUsuarioDetalleDTO(1);

            var query = new UsuarioByIdQuery(dto, true);

            this.mockMapper.Setup(m => m.Map<UsuarioDTO>(It.IsAny<UsuarioSRC>())).Returns(usuarioSRCDTO);
            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetByIdWithAllDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioSRC);

            this.sociedadServiceConsumerMock
                .Setup(x => x.GetSociedadesByIds(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(([UsuarioSRCFactory.CreateSociedadDTO()], null));

            this.contratistaServiceConsumerMock
                .Setup(x => x.ValidateContratista(It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync((UsuarioSRCFactory.CreateContratistaDTO(), null));

            var handler = new UsuarioByIdQueryHandler(
                this.mockUnitOfWork.Object,
                this.mockMapper.Object,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            // Assert
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Data!.Codigo, Is.EqualTo(usuarioSRC.Id));
            Assert.That(resultado.Data!.Codigo, Is.EqualTo(usuarioSRCDTO.Codigo));
        }

        [Test]
        public async Task GetUsuarioByIdQuery_NotFound_Should_Return_Error()
        {
            // Prepare
            var dto = new GetByIdDTO { Id = 1 };

            var query = new UsuarioByIdQuery(dto, true);

            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetByIdWithAllDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as UsuarioSRC);

            var handler = new UsuarioByIdQueryHandler(
                this.mockUnitOfWork.Object,
                this.mockMapper.Object,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            // Assert
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Data, Is.Null);
        }

        [Test]
        public async Task GetUsuarioByIdRedQuery_Should_Return_Usuario()
        {
            // Prepare
            var dto = new GetUsuarioByIdRedDTO { IdRed = "SExxxxx" };

            var usuarioSRC = UsuarioSRCFactory.CreateAllData(1, estudioAuditorId: null, sedeId: null);

            var usuarioSRCDTO = UsuarioSRCFactory.CreateUsuarioDetalleDTO(1);

            var query = new UsuarioByIdRedQuery(dto);

            this.mockMapper.Setup(m => m.Map<UsuarioDTO>(It.IsAny<UsuarioSRC>())).Returns(usuarioSRCDTO);
            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetByIdRedWithAllDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioSRC);

            this.sociedadServiceConsumerMock
                .Setup(x => x.GetSociedadesByIds(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(([UsuarioSRCFactory.CreateSociedadDTO()], null));

            this.contratistaServiceConsumerMock
                .Setup(x => x.ValidateContratista(It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync((UsuarioSRCFactory.CreateContratistaDTO(), null));

            var handler = new UsuarioByIdRedQueryHandler(
                this.mockUnitOfWork.Object,
                this.mockMapper.Object,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            // Assert
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Data!.Codigo, Is.EqualTo(usuarioSRC.Id));
            Assert.That(resultado.Data!.Codigo, Is.EqualTo(usuarioSRCDTO.Codigo));
        }

        [Test]
        public async Task GetUsuarioByIdRedQuery_NotFound_Should_Return_Error()
        {
            // Prepare
            var dto = new GetUsuarioByIdRedDTO { IdRed = "SExxxxx" };

            var query = new UsuarioByIdRedQuery(dto);

            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetByIdRedWithAllDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as UsuarioSRC);

            var handler = new UsuarioByIdRedQueryHandler(
                this.mockUnitOfWork.Object,
                this.mockMapper.Object,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            // Assert
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Data, Is.Null);
        }

        [Test]
        public async Task GetUsuariosFilterQuery_Should_Return_Success()
        {
            // Prepare
            var dto = new GetPaginatedUsuariosDTO();

            // Mock EF operations
            // 1 - create a List<T> with test items
            var usuarios = new List<UsuarioSRC>
            {
                UsuarioSRCFactory.CreateAllData(1),
                UsuarioSRCFactory.CreateAllData(2),
                UsuarioSRCFactory.CreateAllData(3),
                UsuarioSRCFactory.CreateAllData(4),
            };

            // 2 - build mock by extension
            var mock = usuarios.AsQueryable().BuildMock();

            // 3 - setup the mock as Queryable for Moq
            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetFilteredAndOrderedUsuarios(It.IsAny<UsuariosSRCFilterAndOrderDTO>()))
                .Returns(mock);

            var query = new UsuariosQuery(dto);

            var handler = new UsuariosQueryHandler(
                this.mockUnitOfWork.Object,
                this.mapper,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Data, Is.Not.Null);
            Assert.That(resultado.PaginationData, Is.Not.Null);
            Assert.That(resultado.Data, Has.Count.EqualTo(4));
        }

        [Test]
        public async Task GetUsuariosFilterQuery_EmptyResult_Should_Return_Success()
        {
            // Prepare
            var dto = new GetPaginatedUsuariosDTO();

            // Mock EF operations
            // 1 - create a List<T> with test items
            var list = new List<UsuarioSRC>();

            // 2 - build mock by extension
            var mock = list.AsQueryable().BuildMock();

            // 3 - setup the mock as Queryable for Moq
            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetFilteredAndOrderedUsuarios(It.IsAny<UsuariosSRCFilterAndOrderDTO>()))
                .Returns(mock);

            var query = new UsuariosQuery(dto);

            var handler = new UsuariosQueryHandler(
                this.mockUnitOfWork.Object,
                this.mapper,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            // Assert
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Data, Is.Empty);
            Assert.That(resultado.PaginationData, Is.Not.Null);
        }

        [Test]
        public async Task DescargarUsuariosQuery_Should_Filter_And_ExportToFile_Then_Return_Success()
        {
            // Prepare
            var dto = new GetUsuariosDTO();

            // Mock EF operations
            // 1 - create a List<T> with test items
            var usuarios = new List<UsuarioSRC>
            {
                UsuarioSRCFactory.CreateAllData(1),
                UsuarioSRCFactory.CreateAllData(2),
                UsuarioSRCFactory.CreateAllData(3),
                UsuarioSRCFactory.CreateAllData(4),
            };

            // 2 - build mock by extension
            var mock = usuarios.AsQueryable().BuildMock();

            // 3 - setup the mock as Queryable for Moq
            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetFilteredAndOrderedUsuarios(It.IsAny<UsuariosSRCFilterAndOrderDTO>()))
                .Returns(mock);

            this.mockFileAdministrator.Setup(x =>
                x.ExportToExcel(It.IsAny<IEnumerable<UsuariosDTO>>(), It.IsAny<string?>(), It.IsAny<Dictionary<string, string>?>(), It.IsAny<bool>()))
                .Returns([1]);

            var query = new ExportarUsuariosQuery(dto);

            var handler = new ExportarUsuariosQueryHandler(
                this.mockUnitOfWork.Object,
                this.mapper,
                this.mockFileAdministrator.Object,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            // Assert
            Assert.That(resultado.IsSuccess, Is.True);
            Assert.That(resultado.Data, Is.Not.Null);
            Assert.That(resultado.Data!.FileName, Is.Not.Null);
            Assert.That(resultado.Data.FileContent, Is.Not.Null);
        }

        [Test]
        public async Task DescargarUsuariosQuery_EmptyResult_Should_Return_Failure()
        {
            // Prepare
            var dto = new GetUsuariosDTO();

            // Mock EF operations
            // 1 - create a List<T> with test items
            var list = new List<UsuarioSRC>();

            // 2 - build mock by extension
            var mock = list.AsQueryable().BuildMock();

            // 3 - setup the mock as Queryable for Moq
            this.mockUnitOfWork.Setup(x => x.UsuarioSRCRepository.GetFilteredAndOrderedUsuarios(It.IsAny<UsuariosSRCFilterAndOrderDTO>()))
                .Returns(mock);

            this.mockFileAdministrator.Setup(x =>
                x.ExportToExcel(It.IsAny<IEnumerable<UsuariosDTO>>(), It.IsAny<string?>(), It.IsAny<Dictionary<string, string>?>(), It.IsAny<bool>()))
                .Returns([]);

            var query = new ExportarUsuariosQuery(dto);

            var handler = new ExportarUsuariosQueryHandler(
                this.mockUnitOfWork.Object,
                this.mapper,
                this.mockFileAdministrator.Object,
                this.sociedadServiceConsumerMock.Object,
                this.estudioSedeServiceConsumerMock.Object,
                this.contratistaServiceConsumerMock.Object);

            // Execute
            var resultado = await handler.Handle(query, default);

            // Assert
            Assert.That(resultado.IsSuccess, Is.False);
            Assert.That(resultado.Errors, Is.Not.Null);
            Assert.That(resultado.Errors!.First(), Is.EqualTo(UsuariosError.ArchivoVacio));
        }
    }
}
