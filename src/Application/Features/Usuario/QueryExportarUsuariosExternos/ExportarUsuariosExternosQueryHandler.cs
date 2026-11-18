namespace UsuariosAPI.Application.Features.Usuario.QueryExportarUsuariosExternos
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Interfaces.Services;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class ExportarUsuariosExternosQueryHandler : IQueryHandler<ExportarUsuariosExternosQuery, FileResponseDTO>
    {
        public ExportarUsuariosExternosQueryHandler(
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IFileAdministrator fileAdministrator,
          IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
          IContratistaServiceConsumer contratistaServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.FileAdministrator = fileAdministrator;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IFileAdministrator FileAdministrator { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        public async Task<IResponse<FileResponseDTO>> Handle(ExportarUsuariosExternosQuery request, CancellationToken cancellationToken)
        {
            var usuariosList = new List<UsuariosExternosDTO>();
            var filtersDto = this.AutoMapper.Map<UsuariosExternosFilterAndOrderDTO>(request.dto);

            var usuarios = await this.UnitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuariosExternos(filtersDto)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (usuarios is not null && usuarios.Count > 0)
            {
                var contratistasId = usuarios.Where(x => x.ContratistaFkId is not null).Select(x => x.ContratistaFkId ?? 0).Distinct().ToList();
                var estudiosId = usuarios.Where(x => x.EstudioAuditorFkId is not null).Select(x => x.EstudioAuditorFkId ?? 0).Distinct().ToList();
                var sedesId = usuarios.Where(x => x.SedeFkId is not null).Select(x => x.SedeFkId ?? 0).Distinct().ToList();

                var contratistasMap = new Dictionary<int, ContratistaUsuarioDTO>();
                var estudiosMap = new Dictionary<int, EstudioAuditorUsuarioDTO>();
                var sedesMap = new Dictionary<int, SedeUsuarioDTO>();

                var contratistaTask = contratistasId.Count > 0
                    ? this.ContratistaServiceConsumer.GetContratistasByIds(contratistasId, contratistasId.Count, gdiAuth: request.gdiAuth)
                    : Task.FromResult<(IEnumerable<ContratistaUsuarioDTO>, Error?)>(([], null));

                var estudiosTask = estudiosId.Count > 0
                    ? this.EstudioSedeServiceConsumer.GetEstudiosByIds(estudiosId, estudiosId.Count, gdiAuth: request.gdiAuth)
                    : Task.FromResult<(IEnumerable<EstudioAuditorUsuarioDTO>, Error?)>(([], null));

                var sedesTask = sedesId.Count > 0
                    ? this.EstudioSedeServiceConsumer.GetSedesByIds(sedesId, sedesId.Count, gdiAuth: request.gdiAuth)
                    : Task.FromResult<(IEnumerable<SedeUsuarioDTO>, Error?)>(([], null));

                await Task.WhenAll(contratistaTask, estudiosTask, sedesTask);

                var (contratistas, errorContratistas) = await contratistaTask;
                if (errorContratistas is null)
                {
                    contratistasMap = contratistas.ToDictionary(r => r.Id);
                }

                var (estudios, errorEstudios) = await estudiosTask;
                if (errorEstudios is null)
                {
                    estudiosMap = estudios.ToDictionary(r => r.Id);
                }

                var (sedes, errorSedes) = await sedesTask;
                if (errorSedes is null)
                {
                    sedesMap = sedes.ToDictionary(r => r.Id);
                }

                foreach (var item in usuarios)
                {
                    var usuarioDTO = this.AutoMapper.Map<UsuariosExternosDTO>(item);

                    if (contratistasMap.TryGetValue(item.ContratistaFkId ?? 0, out var contratista))
                    {
                        usuarioDTO.Contratista = contratista.RazonSocial;
                    }

                    if (estudiosMap.TryGetValue(item.EstudioAuditorFkId ?? 0, out var estudio)
                        && sedesMap.TryGetValue(item.SedeFkId ?? 0, out var sede))
                    {
                        usuarioDTO.SedeEstudioAuditor = $"{estudio.Nombre} ; {sede.Nombre}";
                    }

                    usuariosList.Add(usuarioDTO);
                }
            }

            var columnHeaders = new Dictionary<string, string>
            {
                { nameof(UsuariosExternosDTO.Codigo), "Código" },
                { nameof(UsuariosExternosDTO.NombreCompleto), "Nombre y Apellido" },
                { nameof(UsuariosExternosDTO.IdRed), "Id de Red" },
                { nameof(UsuariosExternosDTO.CorreoElectronico), "Correo" },
                { nameof(UsuariosExternosDTO.Rol), "Rol asignado" },
                { nameof(UsuariosExternosDTO.Contratista), "Contratista/Empresa" },
                { nameof(UsuariosExternosDTO.SedeEstudioAuditor), "Estudio y Sede" },
                { nameof(UsuariosExternosDTO.Estado), "Estado" },
            };

            var fileContent = this.FileAdministrator.ExportToExcel(usuariosList, "Usuarios Externos", columnHeaders);

            if (fileContent is null || fileContent.Length == 0)
            {
                return Response.Failure<FileResponseDTO>(UsuariosError.ArchivoVacio);
            }

            var responseData = new FileResponseDTO
            {
                FileContent = fileContent,
                FileName = "Usuarios_Externos",
            };

            return Response.Success(responseData);
        }
    }
}
