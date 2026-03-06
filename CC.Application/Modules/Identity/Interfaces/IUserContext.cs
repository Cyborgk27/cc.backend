namespace CC.Application.Modules.Identity.Interfaces
{
    public interface IUserContext
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
    }
}
