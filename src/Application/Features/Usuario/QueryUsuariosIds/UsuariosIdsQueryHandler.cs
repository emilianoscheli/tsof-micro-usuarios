namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosIds
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario.DTO;

    public sealed class UsuariosIdsQueryHandler : IQueryHandler<UsuariosIdsQuery, List<int>>
    {
        public UsuariosIdsQueryHandler(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        public async Task<IResponse<List<int>>> Handle(UsuariosIdsQuery request, CancellationToken cancellationToken)
        {
            var dto = request.dto;

            var filterDto = new UsuariosSRCFilterAndOrderDTO
            {
                Id = dto.Codigo?.ToString(),
                NombreCompleto = dto.NombreCompleto,
                IdRed = dto.IdRed,
                CorreoElectronico = dto.CorreoElectronico,
                RolId = dto.RolId,
                SociedadId = dto.SociedadId,
                ContratistaId = dto.ContratistaId,
                ContratistasIds = dto.ContratistasIds,
                EstudioAuditorId = dto.EstudioAuditorId,
                SedeId = dto.SedeId,
                CodigoUO = dto.CodigoUO,
                PalabraClave = dto.PalabraClave,
            };

            var ids = await this.UnitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuarios(filterDto)
                .Select(u => u.Id)
                .Distinct()
                .ToListAsync(cancellationToken);

            return Response.Success(ids);
        }
    }
}
