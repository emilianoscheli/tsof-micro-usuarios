namespace UsuariosAPI.Infrastructure.Repositories.Rol
{
    using System.Linq.Expressions;
    using APICore.Abstractions;
    using APICore.Extensions;
    using APICore.Helpers;
    using UsuariosAPI.Domain.Rol;

    public sealed class RolRepository : BaseRepository<Rol>, IRolRepository
    {
        public RolRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<Rol> GetAllByFilters(string? nombre, bool? asignaSociedad, bool? esManual, string? sortBy, bool? orderAsc)
        {
            var (sortExpressions, defaultExpression) = this.GetElementsToOrder();

            return this.DbContext.Rol
                .AddFilter(r => r.Nombre, FilterOperation.Contains, nombre)
                .AddFilter(r => r.AsignaSociedad, FilterOperation.Equals, asignaSociedad)
                .AddFilter(r => r.EsManual, FilterOperation.Equals, esManual)
                .AddOrder(sortExpressions, sortBy, orderAsc, defaultExpression)
                .AsQueryable();
        }

        private (Dictionary<string, LambdaExpression>, LambdaExpression) GetElementsToOrder()
        {
            var sortExpressions = new Dictionary<string, LambdaExpression>();

            QueryHelpers.AddSortExpression(sortExpressions, "Id", (Rol p) => p.Id);
            QueryHelpers.AddSortExpression(sortExpressions, "Nombre", (Rol p) => p.Nombre);

            LambdaExpression defaultExpression = (Rol e) => e.Id;

            return (sortExpressions, defaultExpression);
        }
    }
}
