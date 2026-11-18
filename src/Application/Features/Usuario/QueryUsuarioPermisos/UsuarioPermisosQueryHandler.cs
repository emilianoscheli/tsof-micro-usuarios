namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarioPermisos
{
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions.Errors;

    public class UsuarioPermisosQueryHandler : IQueryHandler<UsuarioPermisosQuery, UsuarioRolesPermisosDTO>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsuarioPermisosQueryHandler(
            IUsuariosRolesPermisosService usuariosRolesPermisosService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.UsuariosRolesPermisosService = usuariosRolesPermisosService;
            this.httpContextAccessor = httpContextAccessor;
        }

        private IUsuariosRolesPermisosService UsuariosRolesPermisosService { get; }

        public async Task<IResponse<UsuarioRolesPermisosDTO>> Handle(UsuarioPermisosQuery request, CancellationToken cancellationToken)
        {
            var httpContext = this.httpContextAccessor.HttpContext;

            var idRed = httpContext.GetIdRed();

            var response = await this.UsuariosRolesPermisosService.GetRolesPermisosUsuarios(idRed, cancellationToken);

            if (response is null)
            {
                return Response.Failure<UsuarioRolesPermisosDTO>(UsuariosError.UsuarioNoEncontrado(idRed));
            }

            return Response.Success(response);
        }
    }
}