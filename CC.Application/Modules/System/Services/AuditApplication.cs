using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.Modules.System.Dtos;
using CC.Application.Modules.System.Interfaces;
using CC.Domain.Repositories;
using CC.Utilities.Static;

namespace CC.Application.Modules.System.Services;

public class AuditApplication : IAuditApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServiceData _serviceData;

    public AuditApplication(IUnitOfWork unitOfWork, ServiceData serviceData)
    {
        _unitOfWork = unitOfWork;
        _serviceData = serviceData;
    }

    public async Task<BaseResponse<IEnumerable<SystemAuditDto>>> GetPagedAuditsAsync(int page, int size, string? search = null)
    {
        var pagedResult = await _unitOfWork.SystemAudit.GetPagedAsync(
            page,
            size,
            filter: x => (string.IsNullOrEmpty(search) ||
                         x.UserEmail.Contains(search) ||
                         x.Module.Contains(search) ||
                         x.Operation.Contains(search)),
            orderBy: x => x.OrderByDescending(f => f.CreatedAt) // Los más recientes primero
        );

        var dtos = pagedResult.Items.Select(a => new SystemAuditDto
        {
            Id = a.Id,
            UserEmail = a.UserEmail,
            Operation = a.Operation,
            Module = a.Module,
            Action = a.Action,
            Endpoint = a.Endpoint,
            ResponseCode = a.ResponseCode,
            RequestData = a.RequestData,
            ExecutionTime = a.ExecutionTime,
            CreatedAt = a.CreatedAt,
            UserIp = a.UserIp
        });

        return _serviceData.CreateResponse(dtos, ReplyMessage.MESSAGE_QUERY, 200, pagedResult.TotalCount);
    }
}