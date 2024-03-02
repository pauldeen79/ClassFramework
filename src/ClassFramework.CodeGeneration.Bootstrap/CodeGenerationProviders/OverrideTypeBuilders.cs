namespace ClassFramework.CodeGeneration.Bootstrap.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeBuilders : ClassFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Paths.DomainBuilders}/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override ModelFramework.Objects.Contracts.IClass? BaseClass => CreateBaseclass(typeof(ITypeBase), Constants.Namespaces.Domain);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.DomainBuilders;

    // Do not generate 'With' methods. Do this on the interfaces instead.
    protected override string SetMethodNameFormatString => string.Empty;
    protected override string AddMethodNameFormatString => string.Empty;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(ITypeBase)),
            $"{Constants.Namespaces.Domain}.Types",
            $"{Constants.Namespaces.DomainBuilders}.Types")
        .OfType<ModelFramework.Objects.Contracts.IClass>()
        .Select(x => new ModelFramework.Objects.Builders.ClassBuilder(x)
            .Chain(y => y.Methods.RemoveAll(z => IsInterfacedMethod(z.Name, y)))
            .Build()
        ).ToArray();
}
