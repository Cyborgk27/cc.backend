using CC.Domain.Entities.Project;

namespace CC.Domain.Repositories
{
    public interface IProjectApiKeyRepository : IGenericRepository<ProjectApiKey, int>
    {
        Task<ProjectApiKey?> GetActiveKeyWithProjectAsync(string key);
    }
}
