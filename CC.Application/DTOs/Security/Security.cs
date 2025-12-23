namespace CC.Application.DTOs.Security
{
    public record FeatureDto(
        int? Id,
        string Name,
        string ShowName,
        string Path,
        string Icon
    );

    public record PermissionDto(
        int? Id,
        string Name,
        string ShowName,
        int FeatureId
    );

    public record RoleDto(
        Guid? Id,
        string Name,
        string ShowName,
        List<int> PermissionIds
    );
}