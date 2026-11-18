namespace UsuariosAPI.WebAPI.Middlewares
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    public class ElmahAuthMiddleware
    {
        private readonly string username;
        private readonly string password;
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public ElmahAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
            this.username = configuration["ElmahCredentials:Username"] ?? throw new Exception("Elmah Username no está configurado.");
            this.password = configuration["ElmahCredentials:Password"] ?? throw new Exception("Elmah Password no está configurado.");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/elmah"))
            {
                var authHeader = context.Request.Headers.Authorization.ToString();

                if (string.IsNullOrEmpty(authHeader))
                {
                    PromptForCredentials(context);
                    return;
                }

                var authValue = authHeader.Split(' ')[1];
                var authValueBytes = Convert.FromBase64String(authValue);
                var credentials = Encoding.UTF8.GetString(authValueBytes).Split(':');

                if (credentials.Length == 2 && credentials[0] == this.username && credentials[1] == this.password)
                {
                    await this.next(context);
                }
                else
                {
                    PromptForCredentials(context);
                }
            }
            else
            {
                await this.next(context);
            }
        }

        private static void PromptForCredentials(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.WWWAuthenticate = "Basic realm=\"Elmah\"";
        }
    }
}