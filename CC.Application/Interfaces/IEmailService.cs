namespace CC.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendConfirmationEmailAsync(string to, string userName, string token);
    }
}
