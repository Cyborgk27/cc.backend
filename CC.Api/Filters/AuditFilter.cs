using CC.Application.Modules.Identity.Interfaces;
using CC.Domain.Entities.System;
using CC.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Text.Json;

namespace CC.Api.Filters
{
    public class AuditFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;

        public AuditFilter(IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Iniciar cronómetro
            var timer = Stopwatch.StartNew();

            // 2. Capturar datos de la petición (antes de ejecutar el controlador)
            var request = context.HttpContext.Request;
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Serializamos los argumentos que recibió el método (evitando datos sensibles si es necesario)
            var payload = JsonSerializer.Serialize(context.ActionArguments);

            // 3. Ejecutar el proceso (el controlador hace su trabajo)
            var executedContext = await next();
            timer.Stop();

            // 4. Crear el registro de auditoría
            var audit = new SystemAudit
            {
                UserId = _userContext.UserId != Guid.Empty ? _userContext.UserId : null,
                UserEmail = _userContext.Email ?? "Anónimo",
                UserIp = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Operation = request.Method,
                Module = controller,
                Action = action,
                Endpoint = request.Path,
                RequestData = payload,
                ResponseCode = context.HttpContext.Response.StatusCode,
                ExecutionTime = (int)timer.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow
            };

            // Si ocurrió una excepción durante la ejecución, la capturamos aquí también
            if (executedContext.Exception != null)
            {
                audit.ErrorMessage = executedContext.Exception.Message;
                // Nota: El ExceptionMiddleware seguirá funcionando para dar la respuesta JSON,
                // pero aquí ya dejamos constancia en la auditoría del fallo.
            }

            // 5. Guardar en DB
            await _unitOfWork.SystemAudit.AddAsync(audit);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
