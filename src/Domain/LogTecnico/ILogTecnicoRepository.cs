namespace UsuariosAPI.Domain.LogTecnico
{
    public interface ILogTecnicoRepository
    {
        Task CreateLog(string? comment = null);
    }
}
