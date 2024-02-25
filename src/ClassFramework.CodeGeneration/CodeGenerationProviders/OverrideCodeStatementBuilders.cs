namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementBuilders : ClassFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Paths.DomainBuilders}/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override ModelFramework.Objects.Contracts.IClass? BaseClass => CreateBaseclass(typeof(ICodeStatementBase), Constants.Namespaces.Domain);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.DomainBuilders;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(ICodeStatementBase)),
            $"{Constants.Namespaces.Domain}.CodeStatements",
            $"{Constants.Namespaces.DomainBuilders}.CodeStatements");
}
