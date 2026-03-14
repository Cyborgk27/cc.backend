using CC.Application.Common.Bases;
using CC.Application.Modules.System.Dtos;

namespace CC.Application.Modules.System.Interfaces
{
    public interface IAuditApplication
    {
        Task<BaseResponse<IEnumerable<SystemAuditDto>>> GetPagedAuditsAsync(int page, int size, string? search = null);
    }
}
