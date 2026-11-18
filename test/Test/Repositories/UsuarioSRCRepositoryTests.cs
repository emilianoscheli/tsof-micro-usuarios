namespace UsuariosAPI.Test.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.Infrastructure;
    using UsuariosAPI.Infrastructure.Repositories.UsuarioRol;

    public class UsuarioSRCRepositoryTests
    {
        private ApplicationDbContext dbContext;
        private IUsuarioSRCRepository usuarioRepository;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("TipoUnidadTestDB").Options;

            this.dbContext = new ApplicationDbContext(dbContextOptions);

            this.usuarioRepository = new UsuarioSRCRepository(this.dbContext);

            this.SeedDatabase();
        }

        [Test]
        public void GetAllByFilters_NoFilter_Should_Return_CompleteList()
        {
            var filterDTO = new UsuariosSRCFilterAndOrderDTO()
            {
                Id = null,
            };

            // Execute
            var result = this.usuarioRepository.GetFilteredAndOrderedUsuarios(filterDTO);

            // Asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(this.dbContext.UsuarioSRC.Count()));
        }

        [Test]
        public void GetAllByFilters_Filter_Should_Return_FilteredList()
        {
            var filterDTO = new UsuariosSRCFilterAndOrderDTO()
            {
                Id = "1",
            };

            // Execute
            var result = this.usuarioRepository.GetFilteredAndOrderedUsuarios(filterDTO);

            // Asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.Not.EqualTo(this.dbContext.UsuarioSRC.Count()));
            Assert.That(result.First().Id, Is.EqualTo(1));
        }

        [TearDown]
        public void Cleanup()
        {
            this.dbContext.Dispose();
        }

        private void SeedDatabase()
        {
            // Clear existing data if any
            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Database.EnsureCreated();

            var usuarios = new List<UsuarioSRC>
            {
                new UsuarioSRC { Id = 1, NombreCompleto = "Maria", CorreoElectronico = "maria@mail.com", IdRed = "SE12345" },
                new UsuarioSRC { Id = 2, NombreCompleto = "Luis", CorreoElectronico = "maria@mail.com", IdRed = "SE12346" },
                new UsuarioSRC { Id = 3, NombreCompleto = "Pedro", CorreoElectronico = "maria@mail.com", IdRed = "SE12347" },
                new UsuarioSRC { Id = 4, NombreCompleto = "Lucia", CorreoElectronico = "maria@mail.com", IdRed = "SE12348" },
            };

            this.dbContext.UsuarioSRC.AddRange(usuarios);
            this.dbContext.SaveChanges();
        }
    }
}
