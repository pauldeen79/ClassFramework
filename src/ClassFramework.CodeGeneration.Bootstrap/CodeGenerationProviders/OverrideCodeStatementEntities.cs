namespace ClassFramework.CodeGeneration.Bootstrap.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementEntities : ClassFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Paths.Domain}/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override ModelFramework.Objects.Contracts.IClass? BaseClass => CreateBaseclass(typeof(ICodeStatementBase), Constants.Namespaces.Domain);

    public override object CreateModel()
        => GetImmutableClasses(GetOverrideModels(typeof(ICodeStatementBase)), $"{Constants.Namespaces.Domain}.CodeStatements")
            .OfType<ModelFramework.Objects.Contracts.IClass>()
            .Select(x => FixOverrideEntity(x, "CodeStatement", $"{Constants.Namespaces.DomainBuilders}.CodeStatements"))
            .ToArray();

}
