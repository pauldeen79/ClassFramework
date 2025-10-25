namespace ClassFramework.TemplateFramework.ViewModels;

public class CodeGenerationHeaderViewModel : CsharpClassGeneratorViewModelBase<CodeGenerationHeaderModel>
{
    public string Version
        => !string.IsNullOrEmpty(Model.EnvironmentVersion)
            ? Model.EnvironmentVersion!
            : Environment.Version.ToString();

    public bool CreateCodeGenerationHeader
        => Model.CreateCodeGenerationHeader;
}
