namespace UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal
{
    public class ReporteDelegacionTemporalDTO
    {
        public string IdUsuarioDelegador { get; set; } = string.Empty;

        public string ApellidoNombreUsuarioDelegador { get; set; } = string.Empty;

        public string IdUsuarioDelegado { get; set; } = string.Empty;

        public string ApellidoNombreUsuarioDelegado { get; set; } = string.Empty;

        public string MotivoDelegacion { get; set; } = string.Empty;

        public string FechaDesde { get; set; } = string.Empty;

        public string FechaHasta { get; set; } = string.Empty;

        public bool Vigente { get; set; }

        public string RolesDelegados { get; set; } = string.Empty;
    }
}