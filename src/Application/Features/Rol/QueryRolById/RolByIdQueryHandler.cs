namespace UsuariosAPI.Application.Features.Rol.QueryRolById
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Rol;

    public class RolByIdQueryHandler : IQueryHandler<RolByIdQuery, RolDTO>
    {
        public RolByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IResponse<RolDTO>> Handle(RolByIdQuery request, CancellationToken cancellationToken)
        {
            var entidad = await this.UnitOfWork.RolRepository.GetByIdAsync(request.dto.Id, cancellationToken);

            if (entidad is null)
            {
                return Response.Failure<RolDTO>(UsuariosError.EntidadNoEncontrada(nameof(Rol)));
            }

            var entidadResponse = this.AutoMapper.Map<RolDTO>(entidad);

            return Response.Success(entidadResponse);
        }
    }
}
