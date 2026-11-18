namespace UsuariosAPI.Application.Features.AdUsuarios.QueryBuscarAdUsuarioExistente
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario.DTO;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class BuscarAdUsuarioExistenteQueryHandler : IQueryPaginatedHandler<BuscarAdUsuarioExistenteQuery, UsuariosDTO>
    {
        private const int MaxResultados = 10;
        private const int MaxConcurrenciaAD = 10;

        private readonly IExternalDirectoryService externalDirectoryService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public BuscarAdUsuarioExistenteQueryHandler(
            IExternalDirectoryService externalDirectoryService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.externalDirectoryService = externalDirectoryService;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IPaginatedResponse<UsuariosDTO>> Handle(BuscarAdUsuarioExistenteQuery request, CancellationToken cancellationToken)
        {
            var filtersDto = new UsuariosSRCFilterAndOrderDTO
            {
                PalabraClave = request.palabraClave,
                RolId = request.rolId,
                SortBy = "NombreCompleto",
                OrderAsc = true,
            };

            // 1. Buscar en SRC filtrando por palabraClave y por Rol
            var usuariosSrc = await this.unitOfWork.UsuarioSRCRepository
                .GetFilteredAndOrderedUsuarios(filtersDto)
                .Take(MaxResultados)
                .ToListAsync(cancellationToken);

            if (usuariosSrc.Count == 0)
            {
                return PaginatedResponse<UsuariosDTO>.Success([], new PaginationData { TotalCount = 0 });
            }

            // 2. Verificar en AD cuáles de esos IdReds existen, con concurrencia controlada
            var idRedsEnAd = await this.BuscarIdRedsEnAdAsync(usuariosSrc, cancellationToken);

            if (idRedsEnAd.error != null)
            {
                return PaginatedResponse<UsuariosDTO>.Failure(idRedsEnAd.error);
            }

            usuariosSrc = usuariosSrc
                .Where(u => idRedsEnAd.encontrados.Contains(u.IdRed.Trim()))
                .ToList();

            if (usuariosSrc.Count == 0)
            {
                return PaginatedResponse<UsuariosDTO>.Success([], new PaginationData { TotalCount = 0 });
            }

            var usuariosList = usuariosSrc.Select(this.mapper.Map<UsuariosDTO>).ToList<UsuariosDTO?>();

            var pagination = new PaginationData
            {
                TotalCount = usuariosList.Count,
                PageSize = usuariosList.Count,
                TotalPages = 1,
            };

            return PaginatedResponse<UsuariosDTO>.Success(usuariosList, pagination);
        }

        private async Task<(HashSet<string> encontrados, Error? error)> BuscarIdRedsEnAdAsync(
            List<Domain.Usuario.UsuarioSRC> usuariosSrc,
            CancellationToken cancellationToken)
        {
            var idRedsSrc = usuariosSrc
                .Select(u => u.IdRed)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var encontrados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int erroresCount = 0;
            Error? ultimoError = null;

            using var semaphore = new SemaphoreSlim(MaxConcurrenciaAD);

            var tareas = idRedsSrc.Select(async idRed =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    var resultado = await this.externalDirectoryService.Search(idRed);
                    return resultado;
                }
                catch
                {
                    return (Data: new List<UsuariosAPI.SDK.Contracts.DTOs.AdUsuario.AdUsuarioDto>(), Error: (Error?)Error.GenericError($"Error al consultar AD para {idRed}"));
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            var resultados = await Task.WhenAll(tareas);

            foreach (var resultado in resultados)
            {
                if (resultado.Error != null)
                {
                    erroresCount++;
                    ultimoError = resultado.Error;
                    continue;
                }

                foreach (var usuario in resultado.Data.Where(u => u?.IdRed != null))
                {
                    encontrados.Add(usuario.IdRed.Trim());
                }
            }

            // Solo fallar si TODAS las llamadas fallaron (error sistémico, ej: credenciales)
            if (erroresCount == idRedsSrc.Count && ultimoError != null)
            {
                return (encontrados, ultimoError);
            }

            return (encontrados, null);
        }
    }
}