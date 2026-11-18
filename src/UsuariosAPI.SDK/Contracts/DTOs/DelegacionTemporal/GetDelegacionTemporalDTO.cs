namespace UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal
{
    using APICore.Requests;

    public class GetDelegacionTemporalDTO : OrderedRequest
    {
        public string? DelegacionTemporalId { get; set; }

        public string? NombreCompletoDelegador { get; set; }

        public string? DelegadorId { get; set; }

        public string? NombreCompletoDelegado { get; set; }

        public int? DelegadoId { get; set; } = 0;

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }

        public int? MotivoId { get; set; }

        public string? TiempoLimite { get; set; }

        public string? Observacion { get; set; }

        public bool? Estado { get; set; }
    }
}
