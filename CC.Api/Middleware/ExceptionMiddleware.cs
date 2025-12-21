using CC.Application.Common.Bases;
using CC.Domain.Exceptions;
using System.Text.Json;

namespace CC.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (Exception ex) { await HandleExceptionAsync(context, ex); }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new BaseResponse<object> { IsSuccess = false };

            switch (exception)
            {
                case EntityNotFoundException ex:
                    context.Response.StatusCode = 404;
                    response.Message = ex.Message;
                    response.StatusCode = 404;
                    break;
                case DomainException ex:
                    context.Response.StatusCode = 400;
                    response.Message = ex.Message;
                    response.StatusCode = 400;
                    break;
                default:
                    context.Response.StatusCode = 500;
                    response.Message = "Error interno del servidor.";
                    response.StatusCode = 500;
                    break;
            }

            response.StatusCodeCat = $"https://http.cat/{context.Response.StatusCode}";
            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
