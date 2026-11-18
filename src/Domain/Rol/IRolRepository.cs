namespace UsuariosAPI.Domain.Rol
{
    using UsuariosAPI.Domain.Abstractions;

    public interface IRolRepository : IBaseRepository<Rol>
    {
        IQueryable<Rol> GetAllByFilters(string? nombre, bool? asignaSociedad, bool? esManual, string? sortBy, bool? orderAsc);
    }
}
