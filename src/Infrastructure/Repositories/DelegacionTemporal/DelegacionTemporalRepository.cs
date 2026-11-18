namespace UsuariosAPI.Infrastructure.Repositories.DelegacionTemporal
{
    using System.Linq.Expressions;
    using APICore.Abstractions;
    using APICore.Extensions;
    using APICore.Helpers;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.DelegacionTemporal.DTOs;
    using UsuariosAPI.Domain.DelegacionTemporal.Repository;

    public class DelegacionTemporalRepository : BaseRepository<DelegacionTemporal>, IDelegacionTemporalRepository
    {
        public DelegacionTemporalRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<bool> ExisteSuperposicionComoDelegadoAsync(
        int usuarioId, DateOnly fd, DateOnly fh, CancellationToken ct)
        {
            var minDate = DateOnly.MinValue;

            return await this.DbContext.DelegacionTemporal
                .AsNoTracking()
                .AnyAsync(
                    d =>
                       d.Estado
                    && d.UsuarioDelegadoId == usuarioId
                    && d.FechaDesde <= fh
                    && (d.FechaHasta == minDate || d.FechaHasta >= fd),
                    ct);
        }

        // Método para obtener la información del reporte HU09
        public IQueryable<DelegacionTemporalRowDTO> GetDelegacionesTemporalesReporte(GetByFiltersReporteDelegacionTemporalDTO filtros)
        {
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

            var query = this.DbContext.DelegacionTemporal
                .Include(x => x.UsuarioDelegador!)
                .Include(x => x.UsuarioDelegado!)
                .Include(x => x.RolesTemporales!).ThenInclude(rt => rt.Rol)
                .AsQueryable();

            if (filtros.UsuarioDelegadorId.HasValue)
            {
                query = query.Where(x => x.UsuarioDelegadorId == filtros.UsuarioDelegadorId);
            }

            if (filtros.UsuarioDelegadoId.HasValue)
            {
                query = query.Where(x => x.UsuarioDelegadoId == filtros.UsuarioDelegadoId);
            }

            if (filtros.MotivosDelegacionIds != null && filtros.MotivosDelegacionIds.Any())
            {
                query = query.Where(x => filtros.MotivosDelegacionIds.Contains(x.MotivoDelegacionFkId));
            }

            if (filtros.Vigente.HasValue)
            {
                query = query.Where(x => x.Estado == filtros.Vigente.Value);
            }

            if (filtros.FechaDesde.HasValue)
            {
                query = query.Where(x => x.FechaDesde >= filtros.FechaDesde.Value);
            }

            if (filtros.FechaHasta.HasValue)
            {
                query = query.Where(x => x.FechaHasta <= filtros.FechaHasta.Value);
            }

            return query.Select(x => new DelegacionTemporalRowDTO
            {
                Id = x.Id,
                IdUsuarioDelegador = x.UsuarioDelegador != null ? x.UsuarioDelegador.IdRed : string.Empty,
                ApellidoNombreUsuarioDelegador = x.UsuarioDelegador != null ? x.UsuarioDelegador.NombreCompleto : string.Empty,
                IdUsuarioDelegado = x.UsuarioDelegado != null ? x.UsuarioDelegado.IdRed : string.Empty,
                ApellidoNombreUsuarioDelegado = x.UsuarioDelegado != null ? x.UsuarioDelegado.NombreCompleto : string.Empty,
                MotivoDelegacionFkId = x.MotivoDelegacionFkId,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                Vigente = x.Estado && x.FechaDesde <= hoy && x.FechaHasta >= hoy,
                RolesDelegados = x.RolesTemporales != null ? x.RolesTemporales.Select(rt => rt.Rol != null ? rt.Rol.Nombre : string.Empty).ToList() : new List<string>(),
                MotivoDelegacion = string.Empty,
            });
        }

        public async Task<DelegacionTemporal?> GetByIdWithUsuariosDataAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.DelegacionTemporal
                .Include(x => x.UsuarioDelegado)
                .Include(x => x.UsuarioDelegador)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public IQueryable<DelegacionTemporal> GetAllByFilters(DelegacionTemporalFilterAndOrderDTO dto)
        {
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

            var (sortExpressions, defaultExpression) = this.GetElementsToOrder();

            return this.DbContext.DelegacionTemporal
                .Include(x => x.UsuarioDelegador)
                .Include(x => x.UsuarioDelegado)
                .AddFilter(r => r.UsuarioDelegado!.IdRed, FilterOperation.Contains, dto.DelegadoIdRed)
                .AddFilter(r => r.UsuarioDelegador!.IdRed, FilterOperation.Contains, dto.DelegadorIdRed)
                .AddFilter(r => r.UsuarioDelegador!.NombreCompleto, FilterOperation.Contains, dto.NombreCompletoDelegador)
                .AddFilter(r => r.UsuarioDelegado!.NombreCompleto, FilterOperation.Contains, dto.NombreCompletoDelegado)
                .AddFilter(r => r.FechaDesde, FilterOperation.Equals, dto.FechaDesde)
                .AddFilter(r => r.FechaHasta, FilterOperation.Equals, dto.FechaHasta)
                .AddFilter(r => r.Estado, FilterOperation.Equals, dto.Estado)
                .AddFilter(r => r.MotivoDelegacionFkId, FilterOperation.Equals, dto.MotivoId)
                .WhereIf(dto.DelegadorODelegadoId.HasValue, x => x.UsuarioDelegadorId == dto.DelegadorODelegadoId || x.UsuarioDelegadoId == dto.DelegadorODelegadoId)
                .WhereIf(
                    dto.Vigente.HasValue,
                    x => (dto.Vigente == true && x.Estado && x.FechaDesde <= hoy && x.FechaHasta >= hoy)
                    || (dto.Vigente == false && (!x.Estado || x.FechaDesde > hoy || x.FechaHasta < hoy)))
                .AddOrder(sortExpressions, dto.SortBy, dto.OrderAsc, defaultExpression);
        }

        public async Task<List<int>> SyncRoles(int delegadorId, int delegadoId, CancellationToken cancellationToken)
        {
            var rolesDelegador = await this.DbContext.UsuarioRol
                .Where(usr => usr.UsuarioSRCId == delegadorId)
                .Select(usr => usr.RolId)
                .ToListAsync(cancellationToken);

            var rolesDelegado = await this.DbContext.UsuarioRol
                .Where(usr => usr.UsuarioSRCId == delegadoId)
                .Select(usr => usr.RolId)
                .ToListAsync(cancellationToken);

            var rolesAgregar = rolesDelegador.Except(rolesDelegado).ToList();

            return rolesAgregar;
        }

        public Task<List<DelegacionTemporal>> GetByMotivoDelegacionId(int motivoId, CancellationToken cancellationToken)
        {
            return this.DbContext.DelegacionTemporal
                .Include(x => x.UsuarioDelegador)
                .Include(x => x.UsuarioDelegado)
                .Where(x => x.MotivoDelegacionFkId == motivoId)
                .ToListAsync(cancellationToken);
        }

        private (Dictionary<string, LambdaExpression>, LambdaExpression) GetElementsToOrder()
        {
            var sortExpressions = new Dictionary<string, LambdaExpression>();

            QueryHelpers.AddSortExpression(sortExpressions, "Id", (DelegacionTemporal p) => p.Id);
            QueryHelpers.AddSortExpression(sortExpressions, "fechaDesde", (DelegacionTemporal p) => p.FechaDesde);
            QueryHelpers.AddSortExpression(sortExpressions, "fechaHasta", (DelegacionTemporal p) => p.FechaHasta);
            QueryHelpers.AddSortExpression(sortExpressions, "estado", (DelegacionTemporal p) => p.Estado);
            QueryHelpers.AddSortExpression(sortExpressions, "nombreCompletoDelegador", (DelegacionTemporal p) => p.UsuarioDelegador!.NombreCompleto);
            QueryHelpers.AddSortExpression(sortExpressions, "nombreCompletoDelegado", (DelegacionTemporal p) => p.UsuarioDelegado!.NombreCompleto);
            QueryHelpers.AddSortExpression(sortExpressions, "delegadorIdRed", (DelegacionTemporal p) => p.UsuarioDelegador!.IdRed);
            QueryHelpers.AddSortExpression(sortExpressions, "delegadoIdRed", (DelegacionTemporal p) => p.UsuarioDelegado!.IdRed);

            // "motivo" no se ordena acá: el nombre se resuelve contra MaestrosAPI en el handler.

            // Claves legacy mantenidas por compatibilidad con la exportación.
            QueryHelpers.AddSortExpression(sortExpressions, "Delegador", (DelegacionTemporal p) => p.UsuarioDelegador!.NombreCompleto);
            QueryHelpers.AddSortExpression(sortExpressions, "Delegado", (DelegacionTemporal p) => p.UsuarioDelegado!.NombreCompleto);
            QueryHelpers.AddSortExpression(sortExpressions, "IDDelegador", (DelegacionTemporal p) => p.UsuarioDelegador!.IdRed);
            QueryHelpers.AddSortExpression(sortExpressions, "IDDelegado", (DelegacionTemporal p) => p.UsuarioDelegado!.IdRed);

            LambdaExpression defaultExpression = (DelegacionTemporal e) => e.Id;

            return (sortExpressions, defaultExpression);
        }
    }
}
