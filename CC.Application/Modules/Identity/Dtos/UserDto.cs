using CC.Application.Common.Bases;

namespace CC.Application.Modules.Identity.Dtos
{
    public class UserDto : BaseEntityDto<Guid>
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Password { get; set; } // Opcional en Update, Obligatorio en Create
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; } // Solo para visualización (Response)
    }
}
