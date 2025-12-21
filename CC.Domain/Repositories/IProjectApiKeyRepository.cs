using CC.Domain.Entities;

namespace CC.Domain.Repositories
{
    public interface IProjectApiKeyRepository : IGenericRepository<ProjectApiKey, int>
    {
        Task<ProjectApiKey?> GetActiveKeyWithProjectAsync(string key);
    }
}
