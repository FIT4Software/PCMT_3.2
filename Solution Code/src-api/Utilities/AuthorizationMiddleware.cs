using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace src_api.Utilities
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private ILogger _logger;

        public AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<AuthorizationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null && endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Authorization", out var tokenString))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                _logger.LogError("Authorization header is missing.");
                await context.Response.WriteAsync("Authorization header is missing.");
                return;
            }

            try
            {
                var jwtToken = new JwtSecurityToken(tokenString);
                var validationParameters = new TokenValidationParameters();

                string encodedSecret = _configuration["secret"];
                byte[] secretInBytes = Convert.FromBase64String(encodedSecret);
                string secret = Encoding.Unicode.GetString(ProtectedData.Unprotect(secretInBytes, null, DataProtectionScope.LocalMachine));

                if (jwtToken.Issuer == "PCMT")
                {
                    validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                    validationParameters.ValidateAudience = false;
                    validationParameters.ValidateIssuer = false;

                    var handler = new JwtSecurityTokenHandler();
                    _logger.LogInformation("Successfully validated token: " + handler.ToString());
                    handler.ValidateToken(tokenString, validationParameters, out var validatedToken);
                }
                else
                {
                    _logger.LogError("Invalid token issuer");
                    throw new SecurityTokenException("Invalid token issuer.");
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authentication failed: " + ex.Message);
            }
        }
    }
}

