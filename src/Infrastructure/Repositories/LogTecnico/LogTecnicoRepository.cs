namespace UsuariosAPI.Infrastructure.Repositories.LogTecnico
{
    using System.Threading.Tasks;
    using APICore.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Newtonsoft.Json;
    using UsuariosAPI.Domain.LogTecnico;

    public sealed class LogTecnicoRepository : BaseRepository<LogTecnico>, ILogTecnicoRepository
    {
        public LogTecnicoRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
            : base(dbContext)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        private IHttpContextAccessor HttpContextAccessor { get; }

        public async Task CreateLog(string? comment = null)
        {
            var entries = this.DbContext.ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Modified || e.State == EntityState.Deleted)
                        && e.Entity.GetType().Name != nameof(LogTecnico))
                .ToList();

            foreach (var entry in entries)
            {
                var entidadName = entry.Entity.GetType().Name;
                var currentValuesJson = entry.State == EntityState.Deleted ? string.Empty : PropertiesToJsonString(entry.CurrentValues);
                var originalValuesJson = PropertiesToJsonString(entry.OriginalValues);
                var idEntidad = (int)entry.OriginalValues["Id"] !;
                var logTecnico = new LogTecnico()
                {
                    Entidad = entidadName,
                    EntidadId = idEntidad,
                    ValorOriginal = originalValuesJson,
                    ValorNuevo = currentValuesJson,
                    UsuarioId = this.HttpContextAccessor.HttpContext?.GetUsuarioId(),
                    NombreUsuario = this.HttpContextAccessor.HttpContext?.GetNombreUsuario(),
                    Comentario = comment,
                };

                await this.AddAsync(logTecnico);
            }
        }

        private static string PropertiesToJsonString(PropertyValues values)
        {
            var valuesDictionary = values.Properties.ToDictionary(p => p.Name, p => values[p]);
            return JsonConvert.SerializeObject(valuesDictionary);
        }
    }
}
