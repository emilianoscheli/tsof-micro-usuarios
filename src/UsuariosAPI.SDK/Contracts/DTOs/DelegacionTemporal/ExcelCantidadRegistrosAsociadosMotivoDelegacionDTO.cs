namespace UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal
{
    public class ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO
    {
        public int CodigoDelegacion { get; set; }

        public string Delegado { get; set; } = string.Empty;

        public string FechaDesde { get; set; } = string.Empty;

        public string FechaHasta { get; set; } = string.Empty;
    }
}
