namespace UsuariosAPI.Domain.DelegacionTemporal
{
    using APICore.Responses;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario;

    public class DelegacionTemporal : LegacyEntity
    {
        public UsuarioSRC? UsuarioDelegado { get; set; }

        public UsuarioSRC? UsuarioDelegador { get; set; }

        required public int UsuarioDelegadorId { get; set; }

        required public int UsuarioDelegadoId { get; set; }

        required public int MotivoDelegacionFkId { get; set; }

        required public int TiempoLimite { get; set; }

        required public DateOnly FechaDesde { get; set; }

        required public DateOnly FechaHasta { get; set; }

        public string? Observacion { get; set; }

        public bool Estado { get; set; } = true;

        public List<RolTemporal>? RolesTemporales { get; set; }

        public virtual ResultDomain Desactivar()
        {
            this.Estado = false;

            return ResultDomain.Success();
        }
    }
}
