namespace ClassFramework.CodeGeneration.Bootstrap.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsInterfaces : ClassFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Paths.Domain}/Abstractions";

    public override object CreateModel()
        => GetType().Assembly.GetTypes()
            .Where(x => x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions")
            .Select(x => x.ToInterfaceBuilder().Chain(y => y.Properties.RemoveAll(z => z.ParentTypeFullName != x.FullName)))
            .Select(CreateInterface)
            .ToArray();
}
