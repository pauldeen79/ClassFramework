namespace ClassFramework.Domain.Builders;

public partial class PropertyBuilder
{
    public PropertyBuilder WithParentType(Type parentType) => this.WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).FullName.FixTypeName());
    public PropertyBuilder WithParentType(IType parentType) => this.WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).GetFullName());
    public PropertyBuilder WithParentType(ITypeBuilder parentType) => this.WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).GetFullName());

    public PropertyBuilder AddGetterStringCodeStatements(params string[] statements) => AddGetterCodeStatements(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder(x)));
    public PropertyBuilder AddGetterStringCodeStatements(IEnumerable<string> statements) => AddGetterStringCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());
    public PropertyBuilder GetterNotImplemented() => AddGetterStringCodeStatements(WellKnownCodeStatements.ThrowNotImplementedException);

    public PropertyBuilder AddSetterStringCodeStatements(params string[] statements) => AddSetterCodeStatements(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder(x)));
    public PropertyBuilder AddSetterStringCodeStatements(IEnumerable<string> statements) => AddSetterStringCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());
    public PropertyBuilder SetterNotImplemented() => AddSetterStringCodeStatements(WellKnownCodeStatements.ThrowNotImplementedException);

    public PropertyBuilder AddInitializerStringCodeStatements(params string[] statements) => AddInitializerCodeStatements(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder(x)));
    public PropertyBuilder AddInitializerStringCodeStatements(IEnumerable<string> statements) => AddInitializerStringCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());
    public PropertyBuilder InitializerNotImplemented() => AddInitializerStringCodeStatements(WellKnownCodeStatements.ThrowNotImplementedException);
}
