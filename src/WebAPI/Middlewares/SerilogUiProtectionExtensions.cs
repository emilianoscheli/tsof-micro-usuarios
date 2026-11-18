namespace UsuariosAPI.WebAPI.Middlewares
{
    using System.Text;
    using Serilog.Ui.Web.Extensions;
    using UsuariosAPI.WebAPI.Extensions;

    public static class SerilogUiProtectionExtensions
    {
        public static void MapProtectedSerilogUi(this WebApplication app, SerilogUiCredentialOptions credentials)
        {
            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/serilog-ui"), appBranch =>
            {
                appBranch.Use(async (context, next) =>
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Basic "))
                    {
                        context.Response.Headers["WWW-Authenticate"] = "Basic";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Acceso no autorizado");
                        return;
                    }

                    var encoded = authHeader["Basic ".Length..].Trim();
                    var credentialBytes = Convert.FromBase64String(encoded);
                    var parsed = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    var username = parsed[0];
                    var password = parsed[1];

                    if (username != credentials.Username || password != credentials.Password)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Credenciales invalidas");
                        return;
                    }

                    await next.Invoke();
                });

                appBranch.UseStaticFiles();
                appBranch.UseSerilogUi();
            });
        }
    }
}
