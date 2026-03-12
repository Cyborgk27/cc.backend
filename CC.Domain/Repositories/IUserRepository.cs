using CC.Domain.Entities.Identity;

namespace CC.Domain.Repositories
{
    public interface IUserRepository : IGenericRepository<User, Guid>
    {
        Task<bool> HasPermission(Guid userId, string featureName, string actionName);
        Task<User?> GetUserWithRoleAsync(Guid id);
    }
}
