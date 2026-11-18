namespace UsuariosAPI.Domain.DelegacionTemporal.Repository
{
    using System.Diagnostics.Contracts;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.DelegacionTemporal.DTOs;

    public interface IDelegacionTemporalRepository : IBaseRepository<DelegacionTemporal>
    {
        IQueryable<DelegacionTemporal> GetAllByFilters(DelegacionTemporalFilterAndOrderDTO dto);

        Task<DelegacionTemporal?> GetByIdWithUsuariosDataAsync(int id, CancellationToken cancellationToken = default);

        Task<List<int>> SyncRoles(int delegadorId, int delegadoId, CancellationToken cancellationToken);

        Task<bool> ExisteSuperposicionComoDelegadoAsync(
        int usuarioId,
        DateOnly fechaDesde,
        DateOnly fechaHasta,
        CancellationToken token);

        IQueryable<DelegacionTemporalRowDTO> GetDelegacionesTemporalesReporte(GetByFiltersReporteDelegacionTemporalDTO filtros);

        Task<List<DelegacionTemporal>> GetByMotivoDelegacionId(int motivoId, CancellationToken cancellationToken);
    }
}
