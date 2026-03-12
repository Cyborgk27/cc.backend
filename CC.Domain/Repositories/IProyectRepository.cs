using CC.Domain.Entities.Project;

namespace CC.Domain.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project, Guid>
    {
        Task<Project?> GetProjectDetailsAsync(Guid id);
    }
}
