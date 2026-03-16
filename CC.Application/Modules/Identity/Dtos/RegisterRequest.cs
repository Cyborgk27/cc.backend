namespace CC.Application.Modules.Identity.Dtos
{
    public class RegisterRequest
    {
        public Guid? Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Password { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? ConfirmPassword { get; set; } = default!;
        public Guid RoleId { get; set; }
    }
}
