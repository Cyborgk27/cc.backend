namespace CC.Application.DTOs.Features
{
    public record FeatureDto(int Id, string Name, string ShowName, string Path, string Icon);

    public record CreateFeatureRequest(string Name, string ShowName, string Path, string Icon);

    public record UpdateFeatureRequest(int Id, string ShowName, string Path, string Icon);
}
