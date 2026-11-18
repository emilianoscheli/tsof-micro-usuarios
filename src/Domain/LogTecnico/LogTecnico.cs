namespace UsuariosAPI.Domain.LogTecnico
{
    using UsuariosAPI.Domain.Abstractions;

    public class LogTecnico : Entity
    {
        public LogTecnico()
        {
        }

        required public string Entidad { get; set; }

        required public int EntidadId { get; set; }

        required public string ValorOriginal { get; set; }

        required public string ValorNuevo { get; set; }

        public string? Comentario { get; set; }
    }
}
