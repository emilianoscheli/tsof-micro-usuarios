namespace UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal
{
    public class ExcelDelegacionTemporalDTO
    {
        public string? NombreCompletoDelegador { get; set; }

        public string? DelegadorIdRed { get; set; }

        public string? NombreCompletoDelegado { get; set; }

        public string? DelegadoIdRed { get; set; }

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }

        public int? MotivoId { get; set; }

        public string? Motivo { get; set; }

        public string? TiempoLimite { get; set; }

        public string? Observaciones { get; set; }

        public string? Estado { get; set; }
    }
}
