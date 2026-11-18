namespace UsuariosAPI.Application.Features.Usuario.QueryExportarUsuarios
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
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class ExportarUsuariosQueryHandler : IQueryHandler<ExportarUsuariosQuery, FileResponseDTO>
    {
        private const int BatchSize = 500;

        public ExportarUsuariosQueryHandler(
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IFileAdministrator fileAdministrator,
          ISociedadServiceConsumer sociedadServiceConsumer,
          IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
          IContratistaServiceConsumer contratistaServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.FileAdministrator = fileAdministrator;
            this.SociedadServiceConsumer = sociedadServiceConsumer;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IFileAdministrator FileAdministrator { get; }

        private ISociedadServiceConsumer SociedadServiceConsumer { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        public async Task<IResponse<FileResponseDTO>> Handle(ExportarUsuariosQuery request, CancellationToken cancellationToken)
        {
            var usuariosList = new List<UsuariosDTO>();
            var filtersDto = this.AutoMapper.Map<UsuariosSRCFilterAndOrderDTO>(request.dto);

            var usuarios = await this.UnitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuarios(filtersDto)
                .ToListAsync(cancellationToken);

            if (usuarios is not null && usuarios.Count > 0)
            {
                var contratistasId = usuarios.Where(x => x.ContratistaFkId is not null).Select(x => x.ContratistaFkId ?? 0).Distinct().ToList();
                var estudiosId = usuarios.Where(x => x.EstudioAuditorFkId is not null).Select(x => x.EstudioAuditorFkId ?? 0).Distinct().ToList();
                var sedesId = usuarios.Where(x => x.SedeFkId is not null).Select(x => x.SedeFkId ?? 0).Distinct().ToList();
                var sociedadesId = usuarios.Where(x => x.Sociedades is not null).SelectMany(x => x.Sociedades!).Select(y => y.SociedadFkId).Distinct().ToList();

                var contratistasMap = new Dictionary<int, ContratistaUsuarioDTO>();
                var estudiosMap = new Dictionary<int, EstudioAuditorUsuarioDTO>();
                var sedesMap = new Dictionary<int, SedeUsuarioDTO>();
                var sociedadesMap = new Dictionary<int, SociedadUsuarioDTO>();

                if (contratistasId.Count > 0)
                {
                    foreach (var batch in contratistasId.Chunk(BatchSize))
                    {
                        var (contratistas, errorContratistas) = await this.ContratistaServiceConsumer.GetContratistasByIds([.. batch], int.MaxValue);

                        if (errorContratistas is null)
                        {
                            foreach (var c in contratistas)
                            {
                                contratistasMap.TryAdd(c.Id, c);
                            }
                        }
                    }
                }

                if (estudiosId.Count > 0)
                {
                    foreach (var batch in estudiosId.Chunk(BatchSize))
                    {
                        var (estudios, errorEstudios) = await this.EstudioSedeServiceConsumer.GetEstudiosByIds([.. batch], int.MaxValue);

                        if (errorEstudios is null)
                        {
                            foreach (var e in estudios)
                            {
                                estudiosMap.TryAdd(e.Id, e);
                            }
                        }
                    }
                }

                if (sedesId.Count > 0)
                {
                    foreach (var batch in sedesId.Chunk(BatchSize))
                    {
                        var (sedes, errorSedes) = await this.EstudioSedeServiceConsumer.GetSedesByIds([.. batch], int.MaxValue);

                        if (errorSedes is null)
                        {
                            foreach (var s in sedes)
                            {
                                sedesMap.TryAdd(s.Id, s);
                            }
                        }
                    }
                }

                if (sociedadesId.Count > 0)
                {
                    foreach (var batch in sociedadesId.Chunk(BatchSize))
                    {
                        var (sociedades, errorSociedades) = await this.SociedadServiceConsumer.GetSociedadesByIds([.. batch], int.MaxValue);

                        if (errorSociedades is null)
                        {
                            foreach (var s in sociedades)
                            {
                                sociedadesMap.TryAdd(s.Id, s);
                            }
                        }
                    }
                }

                foreach (var item in usuarios)
                {
                    var usuarioDTO = this.AutoMapper.Map<UsuariosDTO>(item);

                    if (contratistasMap.TryGetValue(item.ContratistaFkId ?? 0, out var contratista))
                    {
                        usuarioDTO.Contratista = contratista.RazonSocial;
                    }

                    foreach (var usuarioSociedad in item.Sociedades ?? [])
                    {
                        if (sociedadesMap.TryGetValue(usuarioSociedad.SociedadFkId, out var sociedad))
                        {
                            usuarioDTO.Sociedades += $"{sociedad.Nombre},";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(usuarioDTO.Sociedades))
                    {
                        usuarioDTO.Sociedades = usuarioDTO.Sociedades[..^1];
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
                { nameof(UsuariosDTO.Codigo), "Codigo" },
                { nameof(UsuariosDTO.NombreCompleto), "Nombre y Apellido" },
                { nameof(UsuariosDTO.IdRed), "Id de Red" },
                { nameof(UsuariosDTO.CorreoElectronico), "Correo" },
                { nameof(UsuariosDTO.Roles), "Roles asignados" },
                { nameof(UsuariosDTO.Sociedades), "Sociedad" },
                { nameof(UsuariosDTO.Contratista), "Contratista/Empresa" },
                { nameof(UsuariosDTO.SedeEstudioAuditor), "Estudio y Sede" },
            };

            var fileContent = this.FileAdministrator.ExportToExcel(usuariosList, "Usuarios", columnHeaders);

            if (fileContent is null || fileContent.Length == 0)
            {
                return Response.Failure<FileResponseDTO>(UsuariosError.ArchivoVacio);
            }

            var responseData = new FileResponseDTO
            {
                FileContent = fileContent,
                FileName = "Usuarios.xlsx",
            };

            return Response.Success(responseData);
        }
    }
}