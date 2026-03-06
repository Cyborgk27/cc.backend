using CC.Application.DTOs.Common;
using CC.Application.Interfaces;
using CC.Domain.Repositories;

namespace CC.Application.Services
{
    public class CommonApplication : ICommonApplication
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommonApplication(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LookupDto>> GetRoles()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            return roles.Select(r => 
            new LookupDto { 
                value = r.Id.ToString(), 
                Name = r.ShowName }
            );
        }

        public async Task<IEnumerable<LookupDto>> GetCatalogsParents()
        {
            var catalogs = await _unitOfWork.Catalogs.GetAllAsync();

            return catalogs.Select(c => new LookupDto { 
                value = c.Id.ToString(), 
                Name = c.ShowName}
            );
        }
    }
}
