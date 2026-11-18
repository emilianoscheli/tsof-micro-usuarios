namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class RolDetailsDTO
    {
        public RolDetailsDTO()
        {
        }

        public int Codigo { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool AsignaSociedad { get; set; }

        public bool EsManual { get; set; }
    }
}
