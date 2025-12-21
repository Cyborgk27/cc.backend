using CC.Application.Common.Bases;
using CC.Domain.Common;
using CC.Utilities.Static;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CC.Application.Common.Helpers
{
    public class ServiceData
    {
        private const int DefaultStatusCode = 200;
        private static readonly List<int> SuccessStatusCodes = [200, 201, 204];
        public BaseResponse<T> CreateResponse<T>(T data, string? message = ReplyMessage.MESSAGE_QUERY, int? statusCode = DefaultStatusCode, int? count = 0)
        {
            count = data is IEnumerable enumerableData && !(data is string) ? enumerableData.Cast<object>().Count() : 1;
            var status = statusCode ?? DefaultStatusCode;
            var response = new BaseResponse<T>()
            {
                IsSuccess = statusCode is null || SuccessStatusCodes.Contains(statusCode.Value),
                Message = message ?? ReplyMessage.MESSAGE_QUERY,
                StatusCode = status,
                StatusCodeCat = $"https://http.cat/{status}",
                Data = data,
                Count = (int)count
            };

            return response;
        }

        public BaseResponse<IEnumerable<T>> CreatePagedResponse<T>(PagedResult<T> pagedResult, string? message = null, int? statusCode = 200)
        {
            var status = statusCode ?? DefaultStatusCode;
            return new BaseResponse<IEnumerable<T>>
            {
                IsSuccess = SuccessStatusCodes.Contains(status),
                Message = message ?? "Consulta exitosa",
                StatusCode = status,
                StatusCodeCat = $"https://http.cat/{status}",
                Data = pagedResult.Items,      // Solo los 10 items de la página
                Count = pagedResult.TotalCount // ¡El total real de la DB! (ej: 150)
            };
        }

    }
}
