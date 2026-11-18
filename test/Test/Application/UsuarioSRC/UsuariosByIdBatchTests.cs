namespace UsuariosAPI.Test.Application.UsuarioSRC
{
    using APICore.Requests.DTOs;
    using AutoMapper;
    using MockQueryable.Moq;
    using Moq;
    using UsuariosAPI.Application.Abstractions.Mappers;
    using UsuariosAPI.Application.Features.Usuario.QueryUsuariosByIdBatch;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;

    public class UsuariosByIdBatchTests
    {
        private Mock<IUnitOfWork> mockUnitOfWork;
        private MapperConfiguration config;
        private IMapper mapper;

        [SetUp]
        public void Setup()
        {
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.config = new MapperConfiguration(cfg => cfg.AddProfile<UsuarioSRCMapperProfile>());
            this.mapper = this.config.CreateMapper();
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenContratistaNotFound()
        {
            // Arrange
            var query = new UsuariosByIdBatchQuery(new GetByIdsListDTO { Ids = [1, 2] });
            var mockQueryable = new List<UsuarioSRC>().AsQueryable().BuildMock();

            this.mockUnitOfWork
                .Setup(u => u.UsuarioSRCRepository.GetByIdsWithAllData(It.IsAny<List<int>>()))
                .Returns(mockQueryable);

            var handler = new UsuariosByIdBatchQueryHandler(this.mockUnitOfWork.Object, this.mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)), Is.EqualTo(result.Errors!.First()));
            });
        }
    }
}
