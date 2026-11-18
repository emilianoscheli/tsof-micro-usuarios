namespace UsuariosAPI.SDK.Contracts.Interfaces
{
    using APICore.Interfaces.Responses;
    using APICore.Responses.DTOs;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public interface IDelegacionTemporalService
    {
        Task<IPaginatedResponse<ReporteDelegacionTemporalDTO>> GetReporteDelegacionTemporal(GetPaginatedReporteDelegacionTemporalDTO dto, bool gdiAuth = true);

        Task<IPaginatedResponse<DelegacionTemporalDTO>> GetDelegacionesTemporales(GetDelegacionTemporalPaginatedDTO dto);

        Task<IResponse<DelegacionTemporalDTO>> GetDelegacionTemporalById(int id);

        Task<IResponse<CantidadRecursosAfectadosDTO>> GetCantidadRecursosAfectadosPorMotivoDelegacion(int id);

        Task<IResponse<List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>> GetExcelRecursosAfectadosPorMotivoDelegacion(int id);
    }
}
