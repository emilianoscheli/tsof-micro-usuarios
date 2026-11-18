namespace UsuariosAPI.Infrastructure.Repositories.UsuarioRol
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using APICore.Abstractions;
    using APICore.Extensions;
    using APICore.Helpers;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.Usuario.DTO;

    public sealed class UsuarioSRCRepository : BaseRepository<UsuarioSRC>, IUsuarioSRCRepository
    {
        public UsuarioSRCRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<List<string>> GetIdRedsExistentesAsync(List<string> idReds, CancellationToken cancellationToken = default)
        {
            var idRedsUpper = idReds.Select(id => id.Trim().ToUpper()).ToList();

            return await this.DbContext.UsuarioSRC
                .AsNoTracking()
                .Where(u => idRedsUpper.Contains(u.IdRed.Trim().ToUpper()))
                .Select(u => u.IdRed.Trim().ToUpper())
                .ToListAsync(cancellationToken);
        }

        public async Task<UsuarioSRC?> GetByIdRedWithDependenciesAsync(string idRed, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.UsuarioSRC
                .Include(r => r.DelegacionTemporales!)
                    .ThenInclude(r => r.RolesTemporales!)
                        .ThenInclude(r => r.Rol)
                .Include(u => u.Roles!)
                    .ThenInclude(r => r.Rol).ThenInclude(rp => rp!.Permisos).ThenInclude(p => p.Permiso)
                .Include(r => r.Sociedades)
                .AsSplitQuery()
                .FirstOrDefaultAsync(entity => entity.IdRed.Trim().ToUpper() == idRed.Trim().ToUpper(), cancellationToken);
        }

        public IQueryable<UsuariosRolesSRCRowDTO> GetUsuariosRolesSRC(GetByFiltersReporteUsuariosRolesSRCDTO filtros)
        {
            var usuarioIds = (filtros.UsuariosIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();

            var rolIds = (filtros.RolesIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();

            var contratistaIds = (filtros.ContratistasIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();

            var estudioIds = (filtros.EstudiosAuditoresIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();

            bool? activo = filtros.Activo;

            var query = this.DbContext.UsuarioSRC
                .AsNoTracking()
                .AsSplitQuery()
                .Include(r => r.DelegacionTemporales!)
                    .ThenInclude(r => r.RolesTemporales!)
                        .ThenInclude(r => r.Rol)
                .Include(u => u.Roles!)
                    .ThenInclude(r => r.Rol).ThenInclude(rp => rp!.Permisos).ThenInclude(p => p.Permiso)
                .Include(r => r.Sociedades)
                .AsQueryable();

            if (usuarioIds.Count > 0)
            {
                query = query.Where(u => usuarioIds.Contains(u.Id));
            }

            // ver el filtro activo porque no hay para habilitar o deshabilitar
            if (rolIds.Count > 0)
            {
                query = query.Where(x => x.Roles != null && x.Roles.Any(r => rolIds.Contains(r.RolId)));
            }

            if (contratistaIds.Count > 0)
            {
                query = query.Where(c => c.ContratistaFkId.HasValue && contratistaIds.Contains(c.ContratistaFkId.Value));
            }

            if (estudioIds.Count > 0)
            {
                query = query.Where(c => c.EstudioAuditorFkId.HasValue && estudioIds.Contains(c.EstudioAuditorFkId.Value));
            }

            var permanentes = query
                .SelectMany(u => u.Roles!.Select(ur => new UsuariosRolesSRCRowDTO
                {
                    IdUsuario = u.IdRed,
                    ApellidoNombreUsuario = u.NombreCompleto,
                    Email = u.CorreoElectronico,
                    RolId = ur.RolId,
                    RolEnSRC = ur.Rol != null ? ur.Rol.Nombre : string.Empty,
                    TipoAsignacionDelRol = ur.Rol != null && ur.Rol.EsManual ? "Administrador de seguridad" : "Proceso SRC",
                    SociedadesIds = u.Sociedades!.Select(s => s.SociedadFkId).ToList(),
                    ContratistaFkId = u.ContratistaFkId,
                    EstudioAuditorFkId = u.EstudioAuditorFkId,
                }));

            permanentes = permanentes.OrderBy(r => r.IdUsuario);

            return permanentes;
        }

        public async Task<UsuarioSRC?> GetByIdRedWithAllDataAsync(string idRed, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.UsuarioSRC
                .Include(r => r.DelegacionTemporales!)
                    .ThenInclude(r => r.RolesTemporales!)
                        .ThenInclude(r => r.Rol)
                .Include(u => u.Roles!)
                    .ThenInclude(r => r.Rol)
                .Include(r => r.Sociedades)
                .FirstOrDefaultAsync(entity => entity.IdRed.Trim().ToUpper() == idRed.Trim().ToUpper(), cancellationToken);
        }

        public async Task<UsuarioSRC?> GetByIdWithAllDataAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.UsuarioSRC
                .Include(r => r.DelegacionTemporales!)
                    .ThenInclude(r => r.RolesTemporales!).ThenInclude(r => r.Rol)
                .Include(x => x.Roles!)
                    .ThenInclude(r => r.Rol)
                .Include(r => r.Sociedades)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<UsuarioSRC?> GetByIdWithRolesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.UsuarioSRC
                .Include(x => x.Roles!)
                    .ThenInclude(r => r.Rol)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public IQueryable<UsuarioSRC> GetByIdsWithAllData(List<int> ids)
        {
            return this.GetByIds(ids)
                .Include(r => r.DelegacionTemporales!)
                    .ThenInclude(r => r.RolesTemporales!).ThenInclude(r => r.Rol)
                .Include(x => x.Roles!)
                    .ThenInclude(r => r.Rol)
                .Include(r => r.Sociedades)
                .AsSplitQuery()
                .AsNoTracking();
        }

        public IQueryable<UsuarioSRC> GetByRolesIdWithAllData(List<int> rolesIds)
        {
            return this.DbContext.UsuarioSRC
                .Include(r => r.DelegacionTemporales!)
                    .ThenInclude(r => r.RolesTemporales!).ThenInclude(r => r.Rol)
                .Include(x => x.Roles!)
                    .ThenInclude(r => r.Rol)
                .Include(r => r.Sociedades)
                .Where(x => x.Roles!.Any(y => rolesIds.Contains(y.RolId)) || (x.DelegacionTemporales != null && x.DelegacionTemporales.Any(z => z.FechaHasta >= DateOnly.FromDateTime(DateTime.Now) && z.RolesTemporales!.Any(w => rolesIds.Contains(w.RolId)))));
        }

        public IQueryable<UsuarioSRC> GetByIdsWithRoles(List<int> ids)
        {
            return this.GetByIds(ids)
                .Include(x => x.Roles!)
                    .ThenInclude(r => r.Rol);
        }

        public IQueryable<UsuarioSRC> GetFilteredAndOrderedUsuarios(UsuariosSRCFilterAndOrderDTO dto)
        {
            var (sortExpressions, defaultExpression) = this.GetElementsToOrder();

            return this.DbContext.UsuarioSRC
                .AsNoTracking()
                .AsSplitQuery()
                .Include(u => u.Roles!
                        .OrderBy(ur => ur.Rol!.Nombre))
                        .ThenInclude(ur => ur.Rol)
                .Include(x => x.Sociedades!)
                .Where(u => dto.EstudioAuditorId.HasValue
                    || (!u.EsExterno && dto.ContratistaId == null)
                    || dto.ContratistaId != null)
                .WhereIf(!string.IsNullOrEmpty(dto.Id), c => c.Id.ToString().Contains(dto.Id!))
                .AddFilter(r => r.NombreCompleto, FilterOperation.Contains, dto.NombreCompleto)
                .AddFilter(r => r.CodigoUO, FilterOperation.Contains, dto.CodigoUO)
                .AddFilter(r => r.IdRed, FilterOperation.Contains, dto.IdRed)
                .WhereIf(!string.IsNullOrEmpty(dto.PalabraClave), r => r.NombreCompleto.Contains(dto.PalabraClave!) || r.IdRed.Contains(dto.PalabraClave!))
                .AddFilter(r => r.CorreoElectronico, FilterOperation.Contains, dto.CorreoElectronico)
                .WhereIf(dto.ContratistaId.HasValue, c => c.ContratistaFkId.Equals(dto.ContratistaId))
                .WhereIf(dto.ContratistasIds != null && dto.ContratistasIds.Count > 0, c => c.ContratistaFkId.HasValue && c.ContratistaFkId != null && dto.ContratistasIds!.Contains(c.ContratistaFkId!.Value))
                .WhereIf(dto.EstudioAuditorId.HasValue, c => c.EstudioAuditorFkId == dto.EstudioAuditorId)
                .WhereIf(dto.EstudioAuditorId.HasValue, c => c.Activo)
                .WhereIf(dto.SedeId.HasValue, c => c.SedeFkId.Equals(dto.SedeId))
                .WhereIf(dto.RolId.HasValue, c => c.Roles!.Any(ts => ts.RolId == dto.RolId))
                .WhereIf(dto.SociedadId.HasValue, c => c.Sociedades!.Any(ts => ts.SociedadFkId == dto.SociedadId))
                .AddOrder(sortExpressions, dto.SortBy, dto.OrderAsc, defaultExpression)
                .AsQueryable();
        }

        public IQueryable<UsuarioSRC> GetFilteredAndOrderedUsuariosExternos(UsuariosExternosFilterAndOrderDTO dto)
        {
            var (sortExpressions, defaultExpression) = this.GetElementsToOrderExternos();

            return this.DbContext.UsuarioSRC
                .Include(u => u.Roles!)
                    .ThenInclude(ur => ur.Rol)
                .Where(u => u.EsExterno) // Solo usuarios externos
                .WhereIf(!string.IsNullOrEmpty(dto.Id), c => c.Id.ToString().Contains(dto.Id!))
                .AddFilter(r => r.NombreCompleto, FilterOperation.Contains, dto.NombreCompleto)
                .AddFilter(r => r.IdRed, FilterOperation.Contains, dto.IdRed)

                //.WhereIf(!string.IsNullOrEmpty(dto.PalabraClave), r => r.NombreCompleto.Contains(dto.PalabraClave!) || r.IdRed.Contains(dto.PalabraClave!))
                .AddFilter(r => r.CorreoElectronico, FilterOperation.Contains, dto.CorreoElectronico)
                .AddFilter(r => r.CodigoIncidencia, FilterOperation.Contains, dto.CodigoIncidencia)
                .WhereIf(dto.ContratistaId.HasValue, c => c.ContratistaFkId.Equals(dto.ContratistaId))
                .WhereIf(dto.ContratistasIds != null && dto.ContratistasIds.Count > 0, c => c.ContratistaFkId.HasValue && c.ContratistaFkId != null && dto.ContratistasIds!.Contains(c.ContratistaFkId!.Value))
                .WhereIf(dto.EstudioAuditorId.HasValue, c => c.EstudioAuditorFkId.Equals(dto.EstudioAuditorId))
                .WhereIf(dto.SedeId.HasValue, c => c.SedeFkId.Equals(dto.SedeId))
                .WhereIf(dto.RolId.HasValue, c => c.Roles!.Any(ts => ts.RolId == dto.RolId))
                .WhereIf(dto.Activo.HasValue, c => c.Activo == dto.Activo!.Value)
                .AddOrder(sortExpressions, dto.SortBy, dto.OrderAsc, defaultExpression)
                .AsQueryable();
        }

        private (Dictionary<string, LambdaExpression>, LambdaExpression) GetElementsToOrder()
        {
            var sortExpressions = new Dictionary<string, LambdaExpression>();

            QueryHelpers.AddSortExpression(sortExpressions, "Codigo", (UsuarioSRC p) => p.Id);
            QueryHelpers.AddSortExpression(sortExpressions, "NombreCompleto", (UsuarioSRC p) => p.NombreCompleto);
            QueryHelpers.AddSortExpression(sortExpressions, "IdRed", (UsuarioSRC p) => p.IdRed);
            QueryHelpers.AddSortExpression(sortExpressions, "Email", (UsuarioSRC p) => p.CorreoElectronico);
            QueryHelpers.AddSortExpression(sortExpressions, "RolesAsignados", (UsuarioSRC p) => p.Roles!
            .OrderBy(s => s.Rol!.Nombre)
                .Select(s => s.Rol!.Nombre)
                .FirstOrDefault());
            LambdaExpression defaultExpression = (UsuarioSRC e) => e.Id;

            return (sortExpressions, defaultExpression);
        }

        private (Dictionary<string, LambdaExpression>, LambdaExpression) GetElementsToOrderExternos()
        {
            var sortExpressions = new Dictionary<string, LambdaExpression>();

            QueryHelpers.AddSortExpression(sortExpressions, "Codigo", (UsuarioSRC p) => p.Id);
            QueryHelpers.AddSortExpression(sortExpressions, "NombreCompleto", (UsuarioSRC p) => p.NombreCompleto);
            QueryHelpers.AddSortExpression(sortExpressions, "IdRed", (UsuarioSRC p) => p.IdRed);
            QueryHelpers.AddSortExpression(sortExpressions, "Email", (UsuarioSRC p) => p.CorreoElectronico);
            QueryHelpers.AddSortExpression(sortExpressions, "Rol", (UsuarioSRC p) => p.Roles!
                .OrderBy(s => s.Rol!.Nombre)
                .Select(s => s.Rol!.Nombre)
                .FirstOrDefault());
            QueryHelpers.AddSortExpression(sortExpressions, "Estado", (UsuarioSRC p) => p.Activo);
            LambdaExpression defaultExpression = (UsuarioSRC e) => e.Id;

            return (sortExpressions, defaultExpression);
        }
    }
}
