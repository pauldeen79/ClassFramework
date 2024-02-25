namespace ClassFramework.TemplateFramework.Models;

public class CodeGenerationHeaderModel
{
    public CodeGenerationHeaderModel(bool createCodeGenerationHeader, string? environmentVersion)
    {
        CreateCodeGenerationHeader = createCodeGenerationHeader;
        EnvironmentVersion = environmentVersion;
    }

    public bool CreateCodeGenerationHeader { get; }
    public string? EnvironmentVersion { get; }
}
