namespace UsuariosAPI.Infrastructure.CoreServices
{
    using APICore.Responses;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;
    using YPFSDK.ADHelper;

    public class YpfAdDirectoryAdapter : IExternalDirectoryService
    {
        private readonly ADHelperService adHelper;

        public YpfAdDirectoryAdapter(ADHelperService adHelper)
        {
            this.adHelper = adHelper;
        }

        public async Task<(List<AdUsuarioDto> Data, Error? Error)> Search(string keyword)
        {
            var response = await this.adHelper.Usuarios.Search(keyword);

            if (response.status.code == System.Net.HttpStatusCode.Unauthorized
                || response.status.code == System.Net.HttpStatusCode.Forbidden)
            {
                return ([], Error.GenericError("El servicio de directorio externo (AD) denegó el acceso. Verifique las credenciales."));
            }

            if (response.status.code == System.Net.HttpStatusCode.NotFound)
            {
                return ([], null);
            }

            if (response.status.code != System.Net.HttpStatusCode.OK)
            {
                return ([], Error.GenericError("El servicio de directorio externo (AD) no respondió correctamente."));
            }

            var data = response.data.Select(user => new AdUsuarioDto
            {
                IdRed = user.SamAccountName,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.EmailAddress,
            }).ToList();

            return (data, null);
        }
    }
}
