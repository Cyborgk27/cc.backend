namespace CC.Application.DTOs.User
{
    public record UserDto(
        Guid? Id,                 // Null = Nuevo Usuario
        string UserName,
        string Email,
        string? Password,         // Opcional en Update, Obligatorio en Create
        string FirstName,
        string LastName,
        Guid RoleId,               // ID del Rol asignado
        string? RoleName,         // Solo para visualización (Response)
        bool IsDeleted = false    // Para borrado lógico
    );
}
