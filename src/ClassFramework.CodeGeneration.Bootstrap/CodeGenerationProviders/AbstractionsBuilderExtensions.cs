namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuilderExtensions : ClassFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Paths.DomainBuilders}/Extensions";

    public override object CreateModel()
        => GetType().Assembly.GetTypes()
            .Where(x => x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions")
            .Select(x => x.ToInterfaceBuilder().Chain(y =>
            {
                y.Properties.RemoveAll(z => z.ParentTypeFullName != x.FullName);
                FixImmutableBuilderProperties(y);
            }))
            .Select(x => CreateBuilderExtensions(x.Build(), CurrentNamespace).WithName($"{x.Name[1..]}BuilderExtensions").BuildTyped())
            .ToArray();
}
