namespace UsuariosAPI.Test.FactoryMethods
{
    using UsuariosAPI.Domain.LogTecnico;

    public static class LogTecnicoFactory
    {
        public static LogTecnico Create(
            int id = 1,
            string entidad = "",
            int entidadId = 1,
            string valorOriginal = "",
            string valorNuevo = "")
        {
            return new LogTecnico
            {
                Id = id,
                Entidad = entidad,
                EntidadId = entidadId,
                ValorOriginal = valorOriginal,
                ValorNuevo = valorNuevo,
            };
        }
    }
}
