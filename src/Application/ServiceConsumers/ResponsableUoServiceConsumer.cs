namespace UsuariosAPI.Application.ServiceConsumers
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Responses;
    using AutoMapper;
    using MaestrosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.SDK.Contracts.DTOs.ResponsableUO;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public class ResponsableUoServiceConsumer : IResponsableUoServiceConsumer
    {
        public ResponsableUoServiceConsumer(
            IMapper mapper,
            IResponsableUoService responsableUoService)
        {
            this.AutoMapper = mapper;
            this.ResponsableUoService = responsableUoService;
        }

        private IMapper AutoMapper { get; }

        private IResponsableUoService ResponsableUoService { get; }

        public async Task<(ResponsableUoMaestroDTO?, Error?)> ValidateResponsableUoByCodigo(string codigo, CancellationToken cancellationToken)
        {
            var responsableUoResponse = await this.ResponsableUoService.GetResponsableUoByCodigo(codigo);

            if (responsableUoResponse is null || !responsableUoResponse.IsSuccess || responsableUoResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("ResponsableUO"));
            }

            var response = this.AutoMapper.Map<ResponsableUoMaestroDTO>(responsableUoResponse.Data);
            response.Rdi = this.AutoMapper.Map<UsuarioDTO>(responsableUoResponse.Data.Rdi);
            response.Bp1 = this.AutoMapper.Map<UsuarioDTO>(responsableUoResponse.Data.Bp1);
            response.Bp2 = this.AutoMapper.Map<UsuarioDTO>(responsableUoResponse.Data.Bp2);

            return (response, null);
        }
    }
}
