namespace UsuariosAPI.Test.Repositories
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using UsuariosAPI.Domain.LogTecnico;
    using UsuariosAPI.Infrastructure;
    using UsuariosAPI.Infrastructure.Repositories;
    using UsuariosAPI.Test.FactoryMethods;

    public class UnitOfWorkTests
    {
        private static readonly string UserName = "Test";
        private static readonly string UserId = "1";
        private ApplicationDbContext dbContext;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("UnitOfWorkTestDB").Options;

            this.dbContext = new ApplicationDbContext(dbContextOptions);
        }

        [Test]
        public async Task SaveAsync_ShouldSetUsuarioData_WhenEntityIsAdded()
        {
            var entidad = UsuarioSRCFactory.Create(1, "Nombre Original");
            this.dbContext.UsuarioSRC.Add(entidad);

            var unitOfWork = this.CreateUnitOfWork(this.dbContext);

            await unitOfWork.SaveAsync();

            Assert.That(entidad.UsuarioId.ToString(), Is.EqualTo(UserId));
            Assert.That(entidad.NombreUsuario, Is.EqualTo(UserName));
        }

        [TearDown]
        public void Cleanup()
        {
            this.dbContext.Database.EnsureDeleted(); // Remove from memory
            this.dbContext.Dispose();
        }

        protected UnitOfWork CreateUnitOfWork(
           ApplicationDbContext? dbContext = null,
           ILogTecnicoRepository? logTecnicoRepository = null)
        {
            dbContext ??= new Mock<ApplicationDbContext>().Object;
            logTecnicoRepository ??= new Mock<ILogTecnicoRepository>().Object;

            var httpContext = CreateHttpContextWithClaims(
            [
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.NameIdentifier, UserId)
            ]);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

            return new UnitOfWork(dbContext, httpContextAccessorMock.Object);
        }

        private static DefaultHttpContext CreateHttpContextWithClaims(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            return new DefaultHttpContext { User = principal };
        }
    }
}