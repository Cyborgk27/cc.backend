using CC.Application.Common.Helpers;
using CC.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace CC.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ServiceData serviceData)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, serviceData);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ServiceData serviceData)
        {
            context.Response.ContentType = "application/json";

            string message;
            int statusCode;

            // Determinamos el mensaje y código según el tipo de excepción
            if (exception is UserFriendlyException userEx)
            {
                statusCode = userEx.StatusCode;
                message = userEx.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Forbidden;
                message = "No tienes permisos para realizar esta acción.";
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "Ocurrió un error inesperado en el servidor.";
                // Opcional: Loggear exception.Message aquí
            }

            context.Response.StatusCode = statusCode;

            // USAMOS TU SERVICEDATA PARA CREAR LA RESPUESTA
            // Pasamos 'null' o 'false' como data ya que es una respuesta de error
            var response = serviceData.CreateResponse<object?>(
                data: null,
                message: message,
                statusCode: statusCode
            );

            // Aseguramos que IsSuccess sea false (aunque ServiceData lo calcula por el status)
            response.IsSuccess = false;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(json);
        }
    }
}