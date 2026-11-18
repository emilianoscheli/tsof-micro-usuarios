namespace UsuariosAPI.Application.CommonServices
{
    using System.Threading;
    using APICore.Responses.DTOs;
    using AutoMapper;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions;

    public class UsuariosRolesPermisosService : IUsuariosRolesPermisosService
    {
        public UsuariosRolesPermisosService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<UsuarioRolesPermisosDTO?> GetRolesPermisosUsuarios(string idRed, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(idRed))
            {
                return null;
            }

            var usuarioSRC = await this.UnitOfWork.UsuarioSRCRepository.GetByIdRedWithDependenciesAsync(idRed, cancellationToken);

            if (usuarioSRC is null || !usuarioSRC.Activo)
            {
                return null;
            }
            else
            {
                return this.AutoMapper.Map<UsuarioRolesPermisosDTO>(usuarioSRC);
            }
        }
    }
}
