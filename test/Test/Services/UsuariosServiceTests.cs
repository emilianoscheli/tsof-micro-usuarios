namespace UsuariosAPI.Test.Services
{
    using APICore.Abstractions;
    using APICore.Interfaces.Services;
    using APICore.Responses;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.SDK.Services;

    public class UsuariosServiceTests
    {
        [Test]
        public async Task GetUsuarioById_OnSuccess_Should_Return_Data()
        {
            // Prepare
            var mockHttpClient = new Mock<IHttpClientService>();
            var mockConfig = new Mock<IConfiguration>();
            Mock<IConfigurationSection> mockSection = new Mock<IConfigurationSection>();

            var response = Response.Success(new UsuarioDTO());

            mockHttpClient.Setup(x => x.GetResourceAsync<UsuarioDTO>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<KeyValuePair<string, object?>>>(),
                It.IsAny<TokenResponse>()))
                .ReturnsAsync(response);

            mockSection.Setup(x => x.Value).Returns("https://localhost:7081/api/v1");
            mockConfig.Setup(x => x.GetSection("ServiceUrls:Usuarios:BaseAddress")).Returns(mockSection.Object);

            var service = this.CreateUsuariosService(mockHttpClient.Object, mockConfig.Object);

            // Execute
            var result = await service.GetUsuarioById(1);

            // Asserts
            Assert.That(result is not null);
            Assert.That(result!.IsSuccess);
            Assert.That(result!.Data, Is.EqualTo(response.Data));
        }

        [Test]
        public async Task GetUsuarioById_OnFailure_Should_Return_Failure()
        {
            // Prepare
            var mockHttpClient = new Mock<IHttpClientService>();
            var mockConfig = new Mock<IConfiguration>();
            Mock<IConfigurationSection> mockSection = new Mock<IConfigurationSection>();

            var response = Response.Failure<UsuarioDTO>(Error.NullValue);

            mockHttpClient.Setup(x => x.GetResourceAsync<UsuarioDTO>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<KeyValuePair<string, object?>>>(),
                It.IsAny<TokenResponse>()))
                .ReturnsAsync(response);

            mockSection.Setup(x => x.Value).Returns("https://localhost:7081/api/v1");
            mockConfig.Setup(x => x.GetSection("ServiceUrls:Usuarios:BaseAddress")).Returns(mockSection.Object);

            var service = this.CreateUsuariosService(mockHttpClient.Object, mockConfig.Object);

            // Execute
            var result = await service.GetUsuarioById(1);

            // Asserts
            Assert.That(result is not null);
            Assert.That(result!.IsSuccess, Is.False);
            Assert.That(result!.Errors is not null);
        }

        [Test]
        public async Task GetUsuarios_OnSuccess_Should_Return_Paginated_Usuarios()
        {
            // Prepare
            var mockHttpClient = new Mock<IHttpClientService>();
            var mockConfig = new Mock<IConfiguration>();
            Mock<IConfigurationSection> mockSection = new Mock<IConfigurationSection>();

            var response = PaginatedResponse<UsuariosDTO>.Success(new List<UsuariosDTO?>(), new PaginationData());

            mockHttpClient.Setup(x => x.GetResourcesAsync<UsuariosDTO>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<KeyValuePair<string, object?>>>(),
                It.IsAny<TokenResponse>()))
                .ReturnsAsync(response);

            mockSection.Setup(x => x.Value).Returns("https://localhost:7081/api/v1");
            mockConfig.Setup(x => x.GetSection("ServiceUrls:Usuarios:BaseAddress")).Returns(mockSection.Object);

            var service = this.CreateUsuariosService(mockHttpClient.Object, mockConfig.Object);

            var dto = new GetPaginatedUsuariosDTO
            {
                NombreCompleto = "unNombre",
            };

            // Execute
            var result = await service.GetUsuarios(dto);

            // Asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess);
            Assert.That(result!.Data, Is.EqualTo(response.Data));
            Assert.That(result!.PaginationData, Is.Not.Null);
        }

        [Test]
        public async Task GetUsuarios_OnFailure_Should_Return_Failure()
        {
            // Prepare
            var mockHttpClient = new Mock<IHttpClientService>();
            var mockConfig = new Mock<IConfiguration>();
            Mock<IConfigurationSection> mockSection = new Mock<IConfigurationSection>();

            var response = PaginatedResponse<UsuariosDTO>.Failure(Error.InvalidAPIResponse);

            mockHttpClient.Setup(x => x.GetResourcesAsync<UsuariosDTO>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<KeyValuePair<string, object?>>>(),
                It.IsAny<TokenResponse>()))
                .ReturnsAsync(response);

            mockSection.Setup(x => x.Value).Returns("https://localhost:7081/api/v1");
            mockConfig.Setup(x => x.GetSection("ServiceUrls:Usuarios:BaseAddress")).Returns(mockSection.Object);

            var service = this.CreateUsuariosService(mockHttpClient.Object, mockConfig.Object);

            var dto = new GetPaginatedUsuariosDTO
            {
                NombreCompleto = "unNombre",
            };

            // Execute
            var result = await service.GetUsuarios(dto);

            // Asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.False);
            Assert.That(result!.Errors, Is.Not.Null);
        }

        protected UsuariosService CreateUsuariosService(
            IHttpClientService? httpClientService = null,
            IConfiguration? configuration = null,
            IAzureAdAuthClient? azureAdAuthClient = null)
        {
            httpClientService ??= new Mock<IHttpClientService>().Object;
            configuration ??= new Mock<IConfiguration>().Object;
            azureAdAuthClient ??= new Mock<IAzureAdAuthClient>().Object;

            return new UsuariosService(httpClientService, configuration, azureAdAuthClient);
        }
    }
}
