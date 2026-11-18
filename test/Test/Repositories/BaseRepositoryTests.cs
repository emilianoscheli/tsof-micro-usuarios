namespace UsuariosAPI.Test.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Infrastructure;
    using UsuariosAPI.Infrastructure.Repositories.UsuarioRol;
    using UsuariosAPI.Test.FactoryMethods;

    public class BaseRepositoryTests
    {
        private ApplicationDbContext dbContext;
        private IUsuarioRolRepository entityRepository; // BaseRepository espera un generic, por eso se usa uno de ejemplo para probar solo funciones Base

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("BaseTestDB").Options;

            this.dbContext = new ApplicationDbContext(dbContextOptions);

            this.entityRepository = new UsuarioRolRepository(this.dbContext);

            this.SeedDatabase();
        }

        /*
        [Test]
        public async Task GetByIdAsync_ExistingEntity_Should_Return_Entity()
        {
            // Execute
            var result = await this.entityRepository.GetByIdAsync(1);

            // Asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_NonExistingEntity_Should_Return_Null()
        {
            // Execute
            var result = await this.entityRepository.GetByIdAsync(10);

            // Asserts
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            var entity = UsuarioRolFactory.Create();

            // Act
            await this.entityRepository.AddAsync(entity);

            // Assert
            var result = await this.dbContext.Entidad.FindAsync(entity.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UsuarioRed, Is.EqualTo(entity.UsuarioRed));
        }

        [Test]
        public async Task Update_ShouldUpdateEntity()
        {
            // Arrange
            var entity = UsuarioRolFactory.Create();
            await this.entityRepository.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();

            // Act
            entity.UsuarioRed = "Otro";
            this.entityRepository.Update(entity);

            // Assert
            var result = await this.dbContext.Entidad.FindAsync(entity.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UsuarioRed, Is.EqualTo("Otro"));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Act
            var result = await this.entityRepository.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task FindOneAsync_ShouldReturnEntity_WhenPredicateMatches()
        {
            // Act
            var result = await this.entityRepository.FindOneAsync(e => e.Id == 2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(2));
        }

        [Test]
        public async Task FindOneAsync_ShouldReturnNull_WhenPredicateDoesNotMatch()
        {
            // Act
            var result = await this.entityRepository.FindOneAsync(e => e.Id == 999);

            // Assert
            Assert.That(result, Is.Null);
        }
        */

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

            /*
            var entity1 = UsuarioRolFactory.CreateWithId(1, nombre: "nombre 1");
            var entity2 = UsuarioRolFactory.CreateWithId(2, nombre: "nombre 2");
            var entity3 = UsuarioRolFactory.CreateWithId(3, nombre: "nombre 3");

            var entities = new List<UsuarioRol>
            {
                entity1, entity2, entity3,
            };

            */
            var entities = new List<UsuarioRol>();
            this.dbContext.UsuarioRol.AddRange(entities);
            this.dbContext.SaveChanges();
        }
    }
}
