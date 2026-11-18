namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarios
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Extensions;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class UsuariosQueryHandler : IQueryPaginatedHandler<UsuariosQuery, UsuariosDTO>
    {
        public UsuariosQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISociedadServiceConsumer sociedadServiceConsumer,
            IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
            IContratistaServiceConsumer contratistaServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.SociedadServiceConsumer = sociedadServiceConsumer;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private ISociedadServiceConsumer SociedadServiceConsumer { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        public async Task<IPaginatedResponse<UsuariosDTO>> Handle(UsuariosQuery request, CancellationToken cancellationToken)
        {
            var filtersDto = this.AutoMapper.Map<UsuariosSRCFilterAndOrderDTO>(request.dto);

            // Contratista, Sociedad y Estudio/Sede se resuelven contra otros microservicios: no son ordenables en la DB.
            var sortByContratista = string.Equals(filtersDto.SortBy, "contratista", StringComparison.OrdinalIgnoreCase);
            var sortBySociedad = string.Equals(filtersDto.SortBy, "sociedad", StringComparison.OrdinalIgnoreCase);
            var sortByEstudioSede = string.Equals(filtersDto.SortBy, "estudioSede", StringComparison.OrdinalIgnoreCase);
            var needsPrePaginationSort = sortByContratista || sortBySociedad || sortByEstudioSede;
            var sortOrderAsc = filtersDto.OrderAsc;
            if (needsPrePaginationSort)
            {
                filtersDto.SortBy = null;
            }

            var queryUsuarios = this.UnitOfWork.UsuarioSRCRepository.GetFilteredAndOrderedUsuarios(filtersDto);

            List<UsuarioSRC> usuarios;
            PaginationData pagination;

            if (needsPrePaginationSort)
            {
                var allResults = await queryUsuarios.ToListAsync(cancellationToken);
                var sortKeys = await this.BuildSortKeysAsync(allResults, sortByContratista, sortBySociedad, request.gdiAuth);

                var sorted = sortOrderAsc == true
                    ? allResults.OrderBy(u => sortKeys.TryGetValue(u.Id, out var k) ? k : "\uffff")
                    : allResults.OrderByDescending(u => sortKeys.TryGetValue(u.Id, out var k) ? k : string.Empty);

                var totalCount = allResults.Count;
                usuarios = sorted
                    .Skip((request.dto.PageNumber - 1) * request.dto.PageSize)
                    .Take(request.dto.PageSize)
                    .ToList();

                pagination = new PaginationData
                {
                    TotalCount = totalCount,
                    PageNumber = request.dto.PageNumber,
                    PageSize = request.dto.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.dto.PageSize),
                };
            }
            else
            {
                (usuarios, pagination) = await queryUsuarios
                    .ToPaginatedListAsync(request.dto.PageNumber, request.dto.PageSize);
            }

            if (usuarios is null || usuarios.Count == 0)
            {
                return PaginatedResponse<UsuariosDTO>.Success([], pagination);
            }

            var contratistasId = usuarios.Where(x => x.ContratistaFkId is not null).Select(x => x.ContratistaFkId ?? 0).ToList() ?? [];
            var estudiosId = usuarios.Where(x => x.EstudioAuditorFkId is not null).Select(x => x.EstudioAuditorFkId ?? 0).ToList() ?? [];
            var sedesId = usuarios.Where(x => x.SedeFkId is not null).Select(x => x.SedeFkId ?? 0).ToList() ?? [];
            var sociedadesId = usuarios.Where(x => x.Sociedades is not null).SelectMany(x => x.Sociedades!).Select(y => y.SociedadFkId).Distinct().ToList() ?? [];

            var usuariosList = new List<UsuariosDTO>();
            var contratistasMap = new Dictionary<int, ContratistaUsuarioDTO>();
            var estudiosMap = new Dictionary<int, EstudioAuditorUsuarioDTO>();
            var sedesMap = new Dictionary<int, SedeUsuarioDTO>();
            var sociedadesMap = new Dictionary<int, SociedadUsuarioDTO>();

            if (contratistasId.Count > 0)
            {
                var (contratistas, errorContratistas) = await this.ContratistaServiceConsumer.GetContratistasByIds(contratistasId, contratistasId.Count, gdiAuth: request.gdiAuth);

                if (errorContratistas is null)
                {
                    contratistasMap = contratistas.ToDictionary(r => r.Id);
                }
            }

            if (estudiosId.Count > 0)
            {
                var (estudios, errorEstudios) = await this.EstudioSedeServiceConsumer.GetEstudiosByIds(estudiosId, estudiosId.Count, gdiAuth: request.gdiAuth);

                if (errorEstudios is null)
                {
                    estudiosMap = estudios.ToDictionary(r => r.Id);
                }
            }

            if (sedesId.Count > 0)
            {
                var (sedes, errorSedes) = await this.EstudioSedeServiceConsumer.GetSedesByIds(sedesId, sedesId.Count, gdiAuth: request.gdiAuth);

                if (errorSedes is null)
                {
                    sedesMap = sedes.ToDictionary(r => r.Id);
                }
            }

            if (sociedadesId.Count > 0)
            {
                var (sociedades, errorSociedades) = await this.SociedadServiceConsumer.GetSociedadesByIds(sociedadesId, sociedadesId.Count, gdiAuth: request.gdiAuth);

                if (errorSociedades is null)
                {
                    sociedadesMap = sociedades.ToDictionary(r => r.Id);
                }
            }

            foreach (var item in usuarios)
            {
                var usuarioDTO = this.AutoMapper.Map<UsuariosDTO>(item);

                if (contratistasMap.TryGetValue(item.ContratistaFkId ?? 0, out var contratista))
                {
                    usuarioDTO.Contratista = contratista.RazonSocial;
                }

                var addedSociedadesIds = new HashSet<int>();
                foreach (var usuarioSociedad in item.Sociedades ?? [])
                {
                    if (addedSociedadesIds.Add(usuarioSociedad.SociedadFkId)
                        && sociedadesMap.TryGetValue(usuarioSociedad.SociedadFkId, out var sociedad))
                    {
                        usuarioDTO.Sociedades += $"{sociedad.Nombre}|";
                    }
                }

                // sacar el último separador
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

            return PaginatedResponse<UsuariosDTO>.Success(usuariosList!, pagination);
        }

        private async Task<Dictionary<int, string>> BuildSortKeysAsync(
            List<UsuarioSRC> usuarios,
            bool sortByContratista,
            bool sortBySociedad,
            bool gdiAuth)
        {
            var sortKeys = new Dictionary<int, string>();

            if (sortByContratista)
            {
                var contratistasId = usuarios.Where(x => x.ContratistaFkId is not null).Select(x => x.ContratistaFkId!.Value).Distinct().ToList();
                var nombres = new Dictionary<int, string>();

                if (contratistasId.Count > 0)
                {
                    var (contratistas, error) = await this.ContratistaServiceConsumer.GetContratistasByIds(contratistasId, contratistasId.Count, gdiAuth: gdiAuth);
                    if (error is null)
                    {
                        nombres = contratistas.ToDictionary(r => r.Id, r => r.RazonSocial ?? string.Empty);
                    }
                }

                foreach (var usuario in usuarios)
                {
                    sortKeys[usuario.Id] = nombres.TryGetValue(usuario.ContratistaFkId ?? 0, out var n) ? n : string.Empty;
                }

                return sortKeys;
            }

            if (sortBySociedad)
            {
                var sociedadesId = usuarios.Where(x => x.Sociedades is not null).SelectMany(x => x.Sociedades!).Select(y => y.SociedadFkId).Distinct().ToList();
                var nombres = new Dictionary<int, string>();

                if (sociedadesId.Count > 0)
                {
                    var (sociedades, error) = await this.SociedadServiceConsumer.GetSociedadesByIds(sociedadesId, sociedadesId.Count, gdiAuth: gdiAuth);
                    if (error is null)
                    {
                        nombres = sociedades.ToDictionary(r => r.Id, r => r.Nombre ?? string.Empty);
                    }
                }

                foreach (var usuario in usuarios)
                {
                    // La celda concatena varias sociedades: se ordena por la primera del texto mostrado.
                    var primera = (usuario.Sociedades ?? [])
                        .Select(s => nombres.TryGetValue(s.SociedadFkId, out var n) ? n : string.Empty)
                        .Where(n => !string.IsNullOrEmpty(n))
                        .OrderBy(n => n)
                        .FirstOrDefault();

                    sortKeys[usuario.Id] = primera ?? string.Empty;
                }

                return sortKeys;
            }

            var estudiosId = usuarios.Where(x => x.EstudioAuditorFkId is not null).Select(x => x.EstudioAuditorFkId!.Value).Distinct().ToList();
            var sedesId = usuarios.Where(x => x.SedeFkId is not null).Select(x => x.SedeFkId!.Value).Distinct().ToList();
            var estudioNombres = new Dictionary<int, string>();
            var sedeNombres = new Dictionary<int, string>();

            if (estudiosId.Count > 0)
            {
                var (estudios, error) = await this.EstudioSedeServiceConsumer.GetEstudiosByIds(estudiosId, estudiosId.Count, gdiAuth: gdiAuth);
                if (error is null)
                {
                    estudioNombres = estudios.ToDictionary(r => r.Id, r => r.Nombre ?? string.Empty);
                }
            }

            if (sedesId.Count > 0)
            {
                var (sedes, error) = await this.EstudioSedeServiceConsumer.GetSedesByIds(sedesId, sedesId.Count, gdiAuth: gdiAuth);
                if (error is null)
                {
                    sedeNombres = sedes.ToDictionary(r => r.Id, r => r.Nombre ?? string.Empty);
                }
            }

            foreach (var usuario in usuarios)
            {
                var tieneEstudio = estudioNombres.TryGetValue(usuario.EstudioAuditorFkId ?? 0, out var estudioNombre);
                var tieneSede = sedeNombres.TryGetValue(usuario.SedeFkId ?? 0, out var sedeNombre);

                sortKeys[usuario.Id] = tieneEstudio && tieneSede
                    ? $"{estudioNombre} ; {sedeNombre}"
                    : string.Empty;
            }

            return sortKeys;
        }
    }
}
