namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuilderInterfaces : ClassFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Paths.DomainBuilders}/Abstractions";

    public override object CreateModel()
        => GetType().Assembly.GetTypes()
            .Where(x => x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions")
            .Select(x => x.ToInterfaceBuilder().Chain(y => y.Properties.RemoveAll(z => z.ParentTypeFullName != x.FullName)))
            .Select(CreateBuilderInterface)
            .ToArray();
}
