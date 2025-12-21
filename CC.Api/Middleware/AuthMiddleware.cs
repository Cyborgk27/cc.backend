using CC.Application.Common.Bases;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;

namespace CC.Api.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var enableAuth = _configuration.GetValue<bool>("SecuritySettings:EnableJwtAuthentication");

            if (enableAuth)
            {
                // 1. Omitir validación para rutas de autenticación (Register/Login)
                var path = context.Request.Path.Value?.ToLower();
                if (path != null && path.Contains("/api/auth"))
                {
                    await _next(context);
                    return;
                }

                // 2. Si NO está autenticado, devolvemos el formato BaseResponse (401)
                // Verificamos si la ruta actual tiene el atributo [AllowAnonymous] como respaldo
                var endpoint = context.GetEndpoint();
                var isAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;

                if (!isAnonymous && (!context.User.Identity?.IsAuthenticated ?? true))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new BaseResponse<object>
                    {
                        IsSuccess = false,
                        Message = "No estás autorizado. Debes proporcionar un token válido.",
                        StatusCode = 401,
                        StatusCodeCat = "https://http.cat/401"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

                // 3. Si SI está autenticado, guardamos el ID
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    context.Items["UserId"] = userId;
                }
            }

            await _next(context);
        }
    }
}