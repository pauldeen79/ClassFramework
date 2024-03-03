namespace ClassFramework.CodeGeneration.Bootstrap.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TemplateFrameworkEntities : ClassFrameworkCSharpClassBase
{
    public override string Path => Constants.Paths.TemplateFramework;
    protected override ModelFramework.Objects.Settings.ArgumentValidationType ValidateArgumentsInConstructor => ModelFramework.Objects.Settings.ArgumentValidationType.DomainOnly;

    public override object CreateModel()
        => GetImmutableClasses(GetTemplateFrameworkModels(), Constants.Namespaces.TemplateFramework)
            .OfType<ModelFramework.Objects.Contracts.IClass>()
            .Select(x => new ModelFramework.Objects.Builders.ClassBuilder(x)
                .AddMethods(new ModelFramework.Objects.Builders.ClassMethodBuilder()
                    .WithName("ToBuilder")
                    .WithTypeName($"{Constants.Namespaces.TemplateFrameworkBuilders}.{x.Name}Builder")
                    .AddLiteralCodeStatements($"return new {Constants.Namespaces.TemplateFrameworkBuilders}.{x.Name}Builder(this);")
                )
                .BuildTyped()
            )
            .ToArray();
}
