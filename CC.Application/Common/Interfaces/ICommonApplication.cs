using CC.Application.DTOs.Common;

namespace CC.Application.Common.Interfaces
{
    public interface ICommonApplication
    {
        public Task<IEnumerable<LookupDto>> GetRoles();
        Task<IEnumerable<LookupDto>> GetCatalogsParents();
    }
}
