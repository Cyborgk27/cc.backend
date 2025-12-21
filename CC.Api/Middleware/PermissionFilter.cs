using CC.Application.Common.Bases;
using CC.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CC.Api.Middleware
{
    public class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _feature;
        private readonly string _action;
        private readonly IUnitOfWork _unitOfWork; // O el servicio que maneje seguridad

        public PermissionFilter(string feature, string action, IUnitOfWork unitOfWork)
        {
            _feature = feature;
            _action = action;
            _unitOfWork = unitOfWork;
        }

        // Dentro de PermissionFilter.cs
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.Items["UserId"]?.ToString();

            // Validamos que sea un GUID válido
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userGuid))
            {
                ReturnUnauthorized(context);
                return;
            }

            // Pasamos el Guid al repositorio
            var hasPermission = await _unitOfWork.Users.HasPermission(userGuid, _feature, _action);

            if (!hasPermission)
            {
                context.Result = new ObjectResult(new BaseResponse<object>
                {
                    IsSuccess = false,
                    Message = $"No tienes permiso para ejecutar '{_action}' en la función '{_feature}'.",
                    StatusCode = 403,
                    StatusCodeCat = "https://http.cat/403"
                })
                {
                    StatusCode = 403
                };
            }
        }

        private void ReturnUnauthorized(AuthorizationFilterContext context)
        {
            context.Result = new ObjectResult(new BaseResponse<object>
            {
                IsSuccess = false,
                Message = "Usuario no identificado.",
                StatusCode = 401,
                StatusCodeCat = "https://http.cat/401"
            })
            {
                StatusCode = 401
            };
        }
    }
}
