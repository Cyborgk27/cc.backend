using CC.Domain.Entities;

namespace CC.Domain.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project, Guid>
    {
        Task<Project?> GetProjectDetailsAsync(Guid id);
    }
}
