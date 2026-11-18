namespace UsuariosAPI.Test.Repositories
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using UsuariosAPI.Domain.LogTecnico;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Infrastructure;
    using UsuariosAPI.Infrastructure.Repositories.LogTecnico;
    using UsuariosAPI.Test.FactoryMethods;

    [TestFixture]
    public class LogTecnicoRepositoryTests
    {
        private static readonly string UserName = "Test";
        private static readonly string UserId = "1";
        private ApplicationDbContext dbContext;
        private ILogTecnicoRepository logTecnicoRepo;
        private Mock<IHttpContextAccessor> httpContextAccessorMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            this.dbContext = new ApplicationDbContext(options);
            this.httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpContext = CreateHttpContextWithClaims(
            [
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.NameIdentifier, UserId)
            ]);

            this.httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

            this.logTecnicoRepo = new LogTecnicoRepository(this.dbContext, this.httpContextAccessorMock.Object);

            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Database.EnsureCreated();
        }

        [Test]
        public async Task CreateLog_ShouldGenerateLogForModifiedEntities()
        {
            // Arrange
            var entidad = UsuarioSRCFactory.Create(1, "Nombre Original");
            this.dbContext.UsuarioSRC.Add(entidad);
            await this.dbContext.SaveChangesAsync();

            // Simulamos una modificación
            entidad.NombreCompleto = "Nombre Updated";
            this.dbContext.Entry(entidad).State = EntityState.Modified;

            // Act
            await this.logTecnicoRepo.CreateLog();
            await this.dbContext.SaveChangesAsync();

            // Assert
            var log = await this.dbContext.LogTecnico.FirstOrDefaultAsync();

            Assert.That(log, Is.Not.Null);
            Assert.That(log!.Entidad, Is.EqualTo(nameof(UsuarioSRC)));
            Assert.That(log.EntidadId, Is.EqualTo(1));
            Assert.That(log.ValorOriginal, Is.Not.Null & Is.Not.Empty);
            Assert.That(log.ValorNuevo, Is.Not.Null & Is.Not.Empty);
            Assert.That(log.ValorNuevo, Is.Not.EqualTo(log.ValorOriginal));
            Assert.That(log.ValorOriginal.Contains("Nombre Original") & !log.ValorOriginal.Contains("Nombre Updated"));
            Assert.That(!log.ValorNuevo.Contains("Nombre Original") & log.ValorNuevo.Contains("Nombre Updated"));
            Assert.That(log.UsuarioId.ToString(), Is.EqualTo(UserId));
            Assert.That(log.NombreUsuario, Is.EqualTo(UserName));
        }

        [Test]
        public async Task CreateLog_ShouldNotCreateLogWhenStateIsAdded()
        {
            var entidad = UsuarioSRCFactory.Create(1, "Nombre Original");
            this.dbContext.UsuarioSRC.Add(entidad);
            this.dbContext.Entry(entidad).State = EntityState.Added;
            await this.logTecnicoRepo.CreateLog();
            await this.dbContext.SaveChangesAsync();

            var log = await this.dbContext.LogTecnico.FirstOrDefaultAsync();

            Assert.That(log, Is.Null);
        }

        [Test]
        public async Task CreateLog_ShouldNotCreateLogWhenStateIsUnchanged()
        {
            var entidad = UsuarioSRCFactory.Create(1, "Nombre Original");
            this.dbContext.UsuarioSRC.Add(entidad);
            await this.dbContext.SaveChangesAsync();

            this.dbContext.Entry(entidad).State = EntityState.Unchanged;
            await this.logTecnicoRepo.CreateLog();
            await this.dbContext.SaveChangesAsync();

            var log = await this.dbContext.LogTecnico.FirstOrDefaultAsync();

            Assert.That(log, Is.Null);
        }

        [Test]
        public async Task CreateLog_ShouldCreateLogWhenStateIsDeleted()
        {
            var entidad = UsuarioSRCFactory.Create(1, "Nombre Original");
            this.dbContext.UsuarioSRC.Add(entidad);
            await this.dbContext.SaveChangesAsync();

            this.dbContext.Remove(entidad);
            this.dbContext.Entry(entidad).State = EntityState.Deleted;
            await this.logTecnicoRepo.CreateLog();
            await this.dbContext.SaveChangesAsync();

            var log = await this.dbContext.LogTecnico.FirstOrDefaultAsync();

            Assert.That(log, Is.Not.Null);
            Assert.That(log!.Entidad, Is.EqualTo(nameof(UsuarioSRC)));
            Assert.That(log.EntidadId, Is.EqualTo(1));
            Assert.That(log.ValorOriginal, Is.Not.Null & Is.Not.Empty);
            Assert.That(log.ValorNuevo, Is.Empty);
        }

        [TearDown]
        public void Cleanup()
        {
            this.dbContext.Database.EnsureDeleted(); // Remove from memory
            this.dbContext.Dispose();
        }

        private static DefaultHttpContext CreateHttpContextWithClaims(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            return new DefaultHttpContext { User = principal };
        }
    }
}