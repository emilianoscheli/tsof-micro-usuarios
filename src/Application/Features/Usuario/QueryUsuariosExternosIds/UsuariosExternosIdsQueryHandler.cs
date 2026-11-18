namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosExternosIds
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario.DTO;

    public sealed class UsuariosExternosIdsQueryHandler : IQueryHandler<UsuariosExternosIdsQuery, List<int>>
    {
        public UsuariosExternosIdsQueryHandler(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        public async Task<IResponse<List<int>>> Handle(UsuariosExternosIdsQuery request, CancellationToken cancellationToken)
        {
            var dto = request.dto;

            var filterDto = new UsuariosExternosFilterAndOrderDTO
            {
                Id = dto.Codigo?.ToString(),
                NombreCompleto = dto.NombreCompleto,
                IdRed = dto.IdRed,
                CorreoElectronico = dto.CorreoElectronico,
                RolId = dto.RolId,
                ContratistaId = dto.ContratistaId,
                ContratistasIds = dto.ContratistasIds,
                CodigoIncidencia = dto.CodigoIncidencia,
                EstudioAuditorId = dto.EstudioAuditorId,
                SedeId = dto.SedeId,
                Activo = dto.Activo,
            };

            var ids = await this.UnitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuariosExternos(filterDto)
                .Select(u => u.Id)
                .Distinct()
                .ToListAsync(cancellationToken);

            return Response.Success(ids);
        }
    }
}
