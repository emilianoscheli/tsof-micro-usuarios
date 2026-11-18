namespace UsuariosAPI.WebAPI.Controllers
{
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Client;

    [Route("api/usuarios/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment env;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHostEnvironment environment, IConfiguration configuration, ILogger<AuthController> logger)
        {
            this.env = environment;
            this._configuration = configuration;
            this._logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token()
        {
            if (!this.env.IsDevelopment())
            {
                return this.BadRequest();
            }

            var authHeader = this.Request.Headers.Authorization;
            var authHeaderInfo = authHeader.ToString().Split(' ');
            var base64TokenInfo = Convert.FromBase64String(authHeaderInfo[1]);
            var base64DecodedClientAndSecret = Encoding.UTF8.GetString(base64TokenInfo);
            var app = ConfidentialClientApplicationBuilder
                .Create(base64DecodedClientAndSecret.Split(':')[0])
                .WithClientSecret(base64DecodedClientAndSecret.Split(':')[1])
                .WithAuthority($"{this._configuration.GetValue<string>("AzureAd:Instance")}{this._configuration.GetValue<string>("AzureAd:TenantId")}")
                .Build();

            var tokenResult = await app.AcquireTokenForClient(new List<string> { this._configuration.GetValue<string>("AzureAd:RequiredScope")! }).ExecuteAsync();

            return this.Ok(new { access_token = tokenResult.AccessToken });
        }
    }
}
