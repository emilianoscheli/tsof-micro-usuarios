namespace UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal
{
    using APICore.Requests;

    public class GetDelegacionTemporalPaginatedDTO : PaginatedOrderedRequest
    {
        public string? NombreCompletoDelegador { get; set; }

        public string? DelegadorIdRed { get; set; }

        public string? NombreCompletoDelegado { get; set; }

        public string? DelegadoIdRed { get; set; }

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }

        public int? MotivoId { get; set; }

        public bool? Estado { get; set; }

        public bool? Vigente { get; set; }
    }
}
