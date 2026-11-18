namespace UsuariosAPI.Domain.DelegacionTemporal.DTOs
{
    public class DelegacionTemporalFilterAndOrderDTO
    {
        public int? DelegadorODelegadoId { get; set; }

        public string? NombreCompletoDelegador { get; set; }

        public string? DelegadorIdRed { get; set; }

        public string? NombreCompletoDelegado { get; set; }

        public string? DelegadoIdRed { get; set; }

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }

        public string? Motivo { get; set; }

        public int? MotivoId { get; set; }

        public bool? Estado { get; set; }

        public bool? Vigente { get; set; }

        public string? SortBy { get; set; }

        public bool? OrderAsc { get; set; }
    }
}
