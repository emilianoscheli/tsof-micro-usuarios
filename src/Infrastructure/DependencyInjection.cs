namespace UsuariosAPI.Infrastructure
{
    using APICore.Extensions;
    using APICore.Interfaces.Services;
    using APICore.Interfaces.Services.AzureServiceBus;
    using APICore.Services;
    using APICore.Services.AzureServiceBus;
    using ContratosAPI.SDK.Contracts.Interfaces;
    using ContratosAPI.SDK.Contracts.Services;
    using IntegracionesAPI.SDK.Contracts.Interfaces;
    using IntegracionesAPI.SDK.Services;
    using MaestrosAPI.SDK.Contracts.Interfaces;
    using MaestrosAPI.SDK.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using UsuariosAPI.Application.Abstractions.CoreServices;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Application.ServiceConsumers;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.DelegacionTemporal.Repository;
    using UsuariosAPI.Domain.LogTecnico;
    using UsuariosAPI.Domain.Permisos;
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.RolPermiso;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;
    using UsuariosAPI.Infrastructure.CoreServices;
    using UsuariosAPI.Infrastructure.CoreServices.AzureBusServices;
    using UsuariosAPI.Infrastructure.Repositories;
    using UsuariosAPI.Infrastructure.Repositories.DelegacionTemporal;
    using UsuariosAPI.Infrastructure.Repositories.LogTecnico;
    using UsuariosAPI.Infrastructure.Repositories.Permiso;
    using UsuariosAPI.Infrastructure.Repositories.Rol;
    using UsuariosAPI.Infrastructure.Repositories.RolPermiso;
    using UsuariosAPI.Infrastructure.Repositories.UsuarioRol;
    using UsuariosAPI.Infrastructure.Repositories.UsuarioSociedad;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddHttpContextAccessor();

            // APIs externas
            services.AddScoped<ISociedadService, SociedadService>();
            services.AddScoped<IEstudioAuditorService, EstudiosAuditoresService>();
            services.AddScoped<ContratosAPI.SDK.Contracts.Interfaces.IContratistasService, ContratistasService>();
            services.AddScoped<IResponsableUoService, ResponsableUoService>();
            services.AddScoped<IMotivosDelegacionService, MotivosDelegacionService>();
            services.AddScoped<IAzureAdAuthClient, AzureAdAuthClient>();
            services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();
            services.AddScoped<IUbicacionService, UbicacionService>();
            services.AddScoped<IGDIEControllerService, GDIEControllerService>();
            services.AddScoped<IGDIEServiceConsumer, GDIEServiceConsumer>();
            services.AddScoped<IExternalDirectoryService, YpfAdDirectoryAdapter>();
            services.AddScoped<IGdiUsuarioInternoService, GdiUsuarioInternoService>();

            services.AddScoped<IFileAdministrator, FileAdministrator>();

            // BD
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DBConnectionString"),
                    sqlOptions => sqlOptions.CommandTimeout(120));
            });

            services.AddScoped<IUsuarioRolRepository, UsuarioRolRepository>();
            services.AddScoped<ILogTecnicoRepository, LogTecnicoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsuarioSRCRepository, UsuarioSRCRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IPermisoRepository, PermisoRepository>();
            services.AddScoped<IRolPermisoRepository, RolPermisoRepository>();
            services.AddScoped<IUsuarioSociedadRepository, UsuarioSociedadRepository>();
            services.AddScoped<IDelegacionTemporalRepository, DelegacionTemporalRepository>();
            services.AddScoped<IRolTemporalRepository, RolTemporalRepository>();

            // Azure Service Bus
            var azureConnectionString = configuration["ServiceBus:ConnectionStrings"];
            if (string.IsNullOrEmpty(azureConnectionString))
            {
                services.AddSingleton<IServiceBusSubscriptionProcessor, NullAzureBusReceiverService>();
            }
            else
            {
                services.AddServiceBusSetup(configuration);
                services.AddSingleton<IServiceBusSubscriptionProcessor, ServiceBusSubscriptionProcessor>();
            }

            return services;
        }
    }
}
