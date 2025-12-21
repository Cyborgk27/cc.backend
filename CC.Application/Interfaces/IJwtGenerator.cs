namespace CC.Application.Interfaces
{
    public interface IJwtGenerator
    {
        string GenerateToken(
            string userId, 
            string email, 
            IEnumerable<string> roles, 
            IEnumerable<string> permissions
        );
    }
}
