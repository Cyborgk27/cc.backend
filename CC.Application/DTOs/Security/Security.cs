namespace CC.Application.DTOs.Security
{
    // Para crear la Feature (el módulo/ruta)
    public record FeatureDto(string Name, string ShowName, string Path, string Icon);

    // Para crear el Permiso ligado a una Feature
    public record PermissionDto(string Name, string ShowName, int FeatureId);

    // Para crear el Rol con sus permisos iniciales
    public record RoleDto(string Name, string ShowName, List<int> PermissionIds);
}
