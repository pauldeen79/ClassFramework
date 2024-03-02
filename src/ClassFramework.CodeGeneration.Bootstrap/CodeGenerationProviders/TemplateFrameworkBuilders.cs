namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TemplateFrameworkBuilders : ClassFrameworkCSharpClassBase
{
    public override string Path => Constants.Paths.TemplateFrameworkBuilders;
    protected override ModelFramework.Objects.Settings.ArgumentValidationType ValidateArgumentsInConstructor => ModelFramework.Objects.Settings.ArgumentValidationType.DomainOnly;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetTemplateFrameworkModels(),
            Constants.Namespaces.TemplateFramework,
            Constants.Namespaces.TemplateFrameworkBuilders);
}
