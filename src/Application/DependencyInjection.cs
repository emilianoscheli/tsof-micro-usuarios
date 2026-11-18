namespace UsuariosAPI.Application
{
    using System.Reflection;
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using UsuariosAPI.Application.Abstractions.Behaviors;
    using UsuariosAPI.Application.CommonServices;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Application.ServiceConsumers;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
                configuration.AddOpenBehavior(typeof(ValidationBahavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddScoped<IResponsableUoServiceConsumer, ResponsableUoServiceConsumer>();
            services.AddScoped<ISociedadServiceConsumer, SociedadServiceConsumer>();
            services.AddScoped<IEstudioSedeServiceConsumer, EstudioSedeServiceConsumer>();
            services.AddScoped<IContratistaServiceConsumer, ContratistaServiceConsumer>();
            services.AddScoped<IMotivosDelegacionServiceConsumer, MotivosDelegacionServiceConsumer>();
            services.AddScoped<ITipoDocumentoServiceConsumer, TipoDocumentoServiceConsumer>();
            services.AddScoped<IUbicacionServiceConsumer, UbicacionServiceConsumer>();

            // Common Services
            services.AddScoped<IUsuariosRolesPermisosService, UsuariosRolesPermisosService>();
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddScoped<IDirectorioUsuariosService, DirectorioUsuariosService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
