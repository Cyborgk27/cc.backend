using CC.Application.Common.Bases;
using CC.Application.Common.Helpers; // Importante para ServiceData
using CC.Domain.Repositories;
using System.Text.Json;

namespace CC.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Inyectamos ServiceData junto con IUnitOfWork en el InvokeAsync
        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork, ServiceData serviceData)
        {
            // 1. Intentamos obtener la cabecera X-API-KEY
            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
            {
                await _next(context);
                return;
            }

            var apiKeyRecord = await unitOfWork.ProjectApiKeys.GetActiveKeyWithProjectAsync(extractedApiKey.ToString());

            // 2. Validación de existencia y vigencia
            if (apiKeyRecord == null || !apiKeyRecord.IsValid())
            {
                await ReturnUnauthorized(context, serviceData, "Invalid or expired API Key.");
                return;
            }

            // 3. Validación de IP
            var remoteIp = context.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(apiKeyRecord.AllowedIp) && remoteIp != apiKeyRecord.AllowedIp && remoteIp != "::1")
            {
                await ReturnUnauthorized(context, serviceData, $"Acceso denegado para la IP: {remoteIp}");
                return;
            }

            // 4. Éxito: Guardamos el ProjectId para el controlador
            context.Items["ProjectId"] = apiKeyRecord.ProjectId;

            await _next(context);
        }

        private async Task ReturnUnauthorized(HttpContext context, ServiceData serviceData, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            // Usamos CreateResponse de ServiceData para estandarizar el error 401
            var response = serviceData.CreateResponse<object>(
                data: null!,
                message: message,
                statusCode: StatusCodes.Status401Unauthorized
            );

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}