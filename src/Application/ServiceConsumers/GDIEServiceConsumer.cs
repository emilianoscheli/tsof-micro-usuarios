namespace UsuariosAPI.Application.ServiceConsumers
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using IntegracionesAPI.SDK.Contracts.DTOs.GDIE;
    using IntegracionesAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;

    public sealed class GDIEServiceConsumer : IGDIEServiceConsumer
    {
        private readonly IGDIEControllerService gdieControllerService;

        public GDIEServiceConsumer(IGDIEControllerService gdieControllerService)
        {
            this.gdieControllerService = gdieControllerService;
        }

        public async Task<(GDIEResponseDTO?, Error?)> RegistrarUsuarioAsync(CrearUsuarioGDIEDTO dto)
        {
            var response = await this.gdieControllerService.RegistrarUsuarioAsync(dto, gdiAuth: false);

            if (response is null || response.Data is null)
            {
                return (null, ObtenerErrorGDIE(response));
            }

            if (!response.IsSuccess)
            {
                // Se preserva response.Data para que el handler pueda evaluar el Codigo de GDIE
                // (ej: -16 = email ya existe como EP → debe crear localmente sin error)
                return (response.Data, ObtenerErrorGDIE(response));
            }

            return (response.Data, null);
        }

        public async Task<(GDIEResponseDTO?, Error?)> ModificarDatosPersonalesAsync(ModificarUsuarioGDIEDTO dto)
        {
            var response = await this.gdieControllerService.ModificarDatosPersonalesAsync(dto, gdiAuth: false);

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return (null, ObtenerErrorGDIE(response));
            }

            return (response.Data, null);
        }

        public async Task<(GDIEResponseDTO?, Error?)> SolicitarAccesoAsync(GestionarAccesoDTO dto)
        {
            var response = await this.gdieControllerService.SolicitarAccesoAsync(dto, gdiAuth: false);

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return (null, ObtenerErrorGDIE(response));
            }

            return (response.Data, null);
        }

        public async Task<(GDIEResponseDTO?, Error?)> CancelarAccesoAsync(GestionarAccesoDTO dto)
        {
            var response = await this.gdieControllerService.CancelarAccesoAsync(dto, gdiAuth: false);

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return (null, ObtenerErrorGDIE(response));
            }

            return (response.Data, null);
        }

        public async Task<(GDIEResponseDTO?, Error?)> ObtenerUsuarioPorEmailAsync(string email)
        {
            var response = await this.gdieControllerService.ObtenerUsuarioPorEmailAsync(email, gdiAuth: false);

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return (null, ObtenerErrorGDIE(response));
            }

            return (response.Data, null);
        }

        public async Task<(GDIEResponseDTO?, Error?)> ObtenerUsuarioPorUserIdAsync(string userId)
        {
            var response = await this.gdieControllerService.ObtenerUsuarioPorUserIdAsync(userId, gdiAuth: false);

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return (null, ObtenerErrorGDIE(response));
            }

            return (response.Data, null);
        }

        private static Error ObtenerErrorGDIE(IResponse<GDIEResponseDTO>? response)
        {
            if (response is null)
            {
                return Error.GenericError("No se recibió respuesta desde Integraciones al consultar GDIE.");
            }

            if (response.Errors is not null)
            {
                foreach (var error in response.Errors)
                {
                    if (error is not null)
                    {
                        return error;
                    }
                }
            }

            return Error.GenericError("Integraciones no pudo completar la operación contra GDIE.");
        }
    }
}