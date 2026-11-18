namespace UsuariosAPI.Domain.DelegacionTemporal.DTOs
{
    public class GetByFiltersReporteDelegacionTemporalDTO
    {
        public int? UsuarioDelegadorId { get; set; }

        public int? UsuarioDelegadoId { get; set; }

        public List<int>? MotivosDelegacionIds { get; set; }

        public bool? Vigente { get; set; }

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }

        public int UsuarioId { get; set; }
    }
}