namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelinesEntities : ClassFrameworkCSharpClassBase
{
    public override string Path => Constants.Paths.Pipelines;
    protected override ModelFramework.Objects.Settings.ArgumentValidationType ValidateArgumentsInConstructor => ModelFramework.Objects.Settings.ArgumentValidationType.DomainOnly;

    public override object CreateModel()
        => GetImmutableClasses(GetPipelinesModels(), Constants.Namespaces.Pipelines)
            .OfType<ModelFramework.Objects.Contracts.IClass>()
            .Select(x => new ModelFramework.Objects.Builders.ClassBuilder(x)
                .AddMethods(new ModelFramework.Objects.Builders.ClassMethodBuilder()
                    .WithName("ToBuilder")
                    .WithTypeName($"{Constants.Namespaces.PipelinesBuilders}.{x.Name}Builder")
                    .AddLiteralCodeStatements($"return new {Constants.Namespaces.PipelinesBuilders}.{x.Name}Builder(this);")
                )
                .BuildTyped()
            )
            .ToArray();
}
