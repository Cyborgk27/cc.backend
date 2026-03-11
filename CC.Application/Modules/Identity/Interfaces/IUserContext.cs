namespace CC.Application.Modules.Identity.Interfaces
{
    public interface IUserContext
    {
        Guid UserId { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
        IEnumerable<string> Roles { get; }
        IEnumerable<string> Permissions { get; }
        bool HasPermission(string permission);
        bool IsInRole(string role);
    }
}
