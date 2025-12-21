namespace CC.Application.DTOs.Auth
{
    public record NavigationDto(
    string Name,      // Nombre técnico (ej: "ProjectModule")
    string ShowName,  // Nombre para mostrar (ej: "Gestión de Proyectos")
    string Path,      // Ruta del frontend
    string Icon       // Icono visual
);
}
