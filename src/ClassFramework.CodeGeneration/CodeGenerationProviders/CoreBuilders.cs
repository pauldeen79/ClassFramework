namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders : ClassFrameworkCSharpClassBase
{
    public override string Path => Constants.Paths.DomainBuilders;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetCoreModels(),
            Constants.Namespaces.Domain,
            Constants.Namespaces.DomainBuilders)
        .OfType<ModelFramework.Objects.Contracts.IClass>()
        .Select(x => new ModelFramework.Objects.Builders.ClassBuilder(x)
            .Chain(y => y.Methods.RemoveAll(z => IsInterfacedMethod(z.Name, y)))
            .Build()
        ).ToArray();
}
