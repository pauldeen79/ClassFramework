namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeEntities : ClassFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Paths.Domain}/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override ModelFramework.Objects.Contracts.IClass? BaseClass => CreateBaseclass(typeof(ITypeBase), Constants.Namespaces.Domain);

    public override object CreateModel()
        => GetImmutableClasses(GetOverrideModels(typeof(ITypeBase)), $"{Constants.Namespaces.Domain}.Types")
            .OfType<ModelFramework.Objects.Contracts.IClass>()
            .Select(x => FixOverrideEntity(x, "Type", $"{Constants.Namespaces.DomainBuilders}.Types"))
            .ToArray();
}
