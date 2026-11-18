namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Responses;
    using MaestrosAPI.SDK.Contracts.DTOs.MotivosDelegacion;

    public interface IMotivosDelegacionServiceConsumer
    {
        Task<(MotivoDelegacionDTO?, Error?)> ValidateMotivoDelegacion(int bdOrigenId);

        Task<(IEnumerable<MotivoDelegacionDTO>, Error?)> GetMotivosDelegacionByIds(List<int> bdOrigenIds, bool gdiAuth = true);
    }
}
