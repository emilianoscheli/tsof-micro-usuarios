namespace UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal
{
    public class CreateDelegacionTemporalDTO
    {
        public string? DelegadorIdRed { get; set; }

        public string? DelegadoIdRed { get; set; }

        public DateOnly FechaDesde { get; set; }

        public DateOnly FechaHasta { get; set; }

        public int? MotivoId { get; set; } = 0;

        public int TiempoLimite { get; set; }

        public string? Observacion { get; set; }
    }
}
