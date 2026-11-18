namespace UsuariosAPI.Domain.DelegacionTemporal.DTOs
{
    public class DelegacionTemporalRowDTO
    {
        public int Id { get; set; }

        public string IdUsuarioDelegador { get; set; } = string.Empty;

        public string ApellidoNombreUsuarioDelegador { get; set; } = string.Empty;

        public string IdUsuarioDelegado { get; set; } = string.Empty;

        public string ApellidoNombreUsuarioDelegado { get; set; } = string.Empty;

        public int MotivoDelegacionFkId { get; set; }

        public string MotivoDelegacion { get; set; } = string.Empty;

        public DateOnly FechaDesde { get; set; }

        public DateOnly FechaHasta { get; set; }

        public bool Vigente { get; set; }

        public List<string> RolesDelegados { get; set; } = new ();
    }
}