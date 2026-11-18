namespace UsuariosAPI.WebAPI
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Reflection;
    using APICore.Interfaces.Services;
    using APICore.Middlewares;
    using APICore.Providers;
    using ElmahCore.Mvc;
    using ElmahCore.Sql;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using Serilog.Ui.Core.Extensions;
    using Serilog.Ui.MsSqlServerProvider.Extensions;
    using Serilog.Ui.Web.Extensions;
    using SixLabors.ImageSharp;
    using Swashbuckle.AspNetCore.SwaggerUI;
    using UsuariosAPI.Application;
    using UsuariosAPI.Infrastructure;
    using UsuariosAPI.WebAPI.Extensions;
    using UsuariosAPI.WebAPI.Middlewares;
    using UsuariosAPI.WebAPI.Services;
    using YPFSDK.ADHelper;
    using YPFSDK.Core;

    public class Program
    {
        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            config.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: true);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirTodo", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("Authorization");
                });
            });

            builder.Services.AddInfrastructure(config);
            builder.Services.AddApplication();
            builder.Services.AddYPFSDK(config).AddADHelper();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Host.UseSerilog();
            var logger = Log.ForContext<Program>();
            logger.Information("Serilog esta integrado correctamente");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Services.AddSerilogUi(options => options
            .UseSqlServer(opts => opts
                .WithConnectionString(builder.Configuration.GetConnectionString("DBConnectionString"))
                .WithTable("Logs")));

            var credentialsSection = config.GetSection("SerilogUiCredentials");
            builder.Services.Configure<SerilogUiCredentialOptions>(credentialsSection);

            var identityProvider = config["IdentityProvider"] ?? "GDI";
            if (identityProvider != "GDI" && identityProvider != "IBMVerify")
            {
                throw new InvalidOperationException("IdentityProvider must be GDI or IBMVerify.");
            }

            var idpConfig = config.GetSection(identityProvider == "GDI" ? "Auth" : "IBMVerify");
            if (identityProvider == "IBMVerify" &&
                (string.IsNullOrWhiteSpace(idpConfig["Issuer"]) ||
                 string.IsNullOrWhiteSpace(idpConfig["Audience"]) ||
                 string.IsNullOrWhiteSpace(idpConfig["UserIdClaim"])))
            {
                throw new InvalidOperationException("IBMVerify issuer, audience and user claim must be configured.");
            }

            // Scopes para GDI
            var gdiScopes = new Dictionary<string, string>
            {
                { "oob", "oob" },
                { "interno", "interno" },
                { "proveedor", "proveedor" },
                { "openid", "openid" },
                { "profile", "profile" },
                { "email", "emai1" },
                { "jwt", "jwt" },
            };

            // Configuraci�n Azure AD
            var azureAdConfig = builder.Configuration.GetSection("AzureAd");
            var tokenEndpoint = builder.Configuration.GetSection("ServiceUrls");
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TSRC YPF API", Version = "v1" });

                // Definici�n de seguridad para GDI
                var idpFlow = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(idpConfig["AuthorizationUrl"] ?? string.Empty),
                    TokenUrl = new Uri(idpConfig["TokenUrl"] ?? string.Empty),
                    Scopes = identityProvider == "GDI"
                        ? gdiScopes
                        : idpConfig.GetSection("Scopes").Get<string[]>()?
                            .ToDictionary(scope => scope, scope => scope) ?? new Dictionary<string, string>(),
                };
                opt.AddSecurityDefinition(identityProvider, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = $"Por favor ingrese un token {identityProvider}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.OAuth2,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    OpenIdConnectUrl = identityProvider == "GDI"
                        ? new Uri(idpConfig["ServerUrl"] ?? string.Empty)
                        : null,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = identityProvider == "GDI" ? idpFlow : null,
                        AuthorizationCode = identityProvider == "IBMVerify" ? idpFlow : null,
                    },
                });

                // Definici�n de seguridad para Azure AD
                opt.AddSecurityDefinition("AzureAD", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "OAuth2 Authorization Code Flow (requiere cliente p�blico)",
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{tokenEndpoint["Usuarios:BaseAddress"]}auth/token"),
                        },
                    },
                });

                // Requerimiento de seguridad para ambos esquemas
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = identityProvider,
                            },
                        },
                        new string[] { }
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "AzureAD",
                            },
                        },
                        new[] { azureAdConfig["RequiredScope"] }
                    },
                });
            });

            IList<SecurityKey>? keys = null;
            if (identityProvider == "GDI")
            {
                var jwksJson = new HttpClient().GetStringAsync(idpConfig["JsonKeysUrl"]).Result;
                keys = new JsonWebKeySet(jwksJson).GetSigningKeys();
            }

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = identityProvider; // o el que uses m�s
                options.DefaultChallengeScheme = identityProvider;
            })
            .AddJwtBearer(identityProvider, options =>
            {
                if (identityProvider == "IBMVerify")
                {
                    options.Authority = !string.IsNullOrWhiteSpace(idpConfig["Authority"])
                        ? idpConfig["Authority"] : idpConfig["Issuer"];
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = idpConfig["Issuer"],
                    ValidAudience = idpConfig[identityProvider == "GDI" ? "ClientId" : "Audience"],
                    IssuerSigningKeys = keys,
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },
                };
            })
            .AddJwtBearer("AzureAD", options =>
            {
                options.Authority = $"{config["AzureAd:Instance"]}{config["AzureAd:TenantId"]}/v2.0";
                options.Audience = config["AzureAd:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = new[]
                    {
                        $"{config["AzureAd:Instance"]}{config["AzureAd:TenantId"]}/v2.0",
                        $"https://sts.windows.net/{config["AzureAd:TenantId"]}/",
                    },
                    ValidateAudience = true,
                    ValidAudience = config["AzureAd:Audience"],
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    NameClaimType = "name",
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        var roles = ctx.Principal!.Claims
                            .Where(c => c.Type == "roles")
                            .Select(c => c.Value);
                        return Task.CompletedTask;
                    },
                };
            });

            // Permite que HttpContext est� disponible en cualquier parte del c�digo
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            builder.Services.AddScoped<IRolesPermisosService, RolesPermisosService>();

            builder.Services.AddElmah<SqlErrorLog>(options =>
            {
                options.Path = @"elmah";
                options.ConnectionString = builder.Configuration.GetConnectionString("DBConnectionString");
                options.LogPath = "~/errors.xml";
            });

            // Se deshabilita limpieza de logs
            // builder.Services.AddHostedService<LogCleanupService>();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    opt.OAuthClientId(idpConfig["ClientId"]);
                    if (identityProvider == "GDI")
                    {
                        opt.OAuthClientSecret(azureAdConfig["ClientSecret"]);
                    }

                    // Configuraciones adicionales
                    opt.OAuthRealm("your-realm");
                    opt.OAuthAppName("API Documentation");
                    opt.OAuthScopeSeparator(" ");
                    if (identityProvider == "GDI" || idpConfig.GetValue<bool>("UsePkce"))
                    {
                        opt.OAuthUsePkce();
                    }

                    opt.DocExpansion(DocExpansion.None);
                });
            }

            var credentials = app.Services.GetRequiredService<IOptions<SerilogUiCredentialOptions>>().Value;
            app.MapProtectedSerilogUi(credentials);

            app.ApplyMigration();

            app.UseCors("PermitirTodo");

            // El middleware de Elmah se ejecuta antes que UseAuthentication y UseAuthorization
            app.UseMiddleware<ElmahAuthMiddleware>();

            app.UseAuthentication();

            // En cada API
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // En el nuget API Core
            app.UseMiddleware<AuthMiddleware>();
            app.UseMiddleware<UnauthorizedMiddleware>();

            app.UseAuthorization();

            app.UseElmah();
            app.MapHealthChecks("/health");

            app.MapControllers();

            // Temporal: log de las librer�as internas cargadas (SDKs de otras APIs/API Core)
            var loaded = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name?.StartsWith("TSRC") ?? true);

            foreach (var asm in loaded)
            {
                var name = asm.GetName();
                var infoVer = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                var location = asm.Location;

                logger.Information(
                    "Lib interna de TSRC: {Name} v{Version} (InfoVer: {InfoVer}) @ {Location}",
                    name.Name,
                    name.Version,
                    infoVer ?? "(none)",
                    location);
            }

            app.Run();
        }
    }
}