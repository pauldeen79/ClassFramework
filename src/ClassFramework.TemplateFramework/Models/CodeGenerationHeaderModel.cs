namespace ClassFramework.TemplateFramework.Models;

public class CodeGenerationHeaderModel(bool createCodeGenerationHeader, string? environmentVersion)
{
    public bool CreateCodeGenerationHeader { get; } = createCodeGenerationHeader;
    public string? EnvironmentVersion { get; } = environmentVersion;
}
