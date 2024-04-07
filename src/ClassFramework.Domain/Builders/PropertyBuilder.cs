﻿namespace ClassFramework.Domain.Builders;

[CustomValidation(typeof(PropertyValidator), nameof(PropertyValidator.Validate))]
public partial class PropertyBuilder
{
    public PropertyBuilder WithParentType(Type parentType) => WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).FullName.FixTypeName());
    public PropertyBuilder WithParentType(IType parentType) => WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).GetFullName());
    public PropertyBuilder WithParentType(ITypeBuilder parentType) => WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).GetFullName());

    public PropertyBuilder AddGetterStringCodeStatements(params string[] statements) => AddGetterCodeStatements(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder().WithStatement(x)));
    public PropertyBuilder AddGetterStringCodeStatements(IEnumerable<string> statements) => AddGetterStringCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());
    public PropertyBuilder GetterNotImplemented() => AddGetterStringCodeStatements(WellKnownCodeStatements.ThrowNotImplementedException);

    public PropertyBuilder AddSetterStringCodeStatements(params string[] statements) => AddSetterCodeStatements(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder().WithStatement(x)));
    public PropertyBuilder AddSetterStringCodeStatements(IEnumerable<string> statements) => AddSetterStringCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());
    public PropertyBuilder SetterNotImplemented() => AddSetterStringCodeStatements(WellKnownCodeStatements.ThrowNotImplementedException);

    public PropertyBuilder AddInitializerStringCodeStatements(params string[] statements) => AddInitializerCodeStatements(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder().WithStatement(x)));
    public PropertyBuilder AddInitializerStringCodeStatements(IEnumerable<string> statements) => AddInitializerStringCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());
    public PropertyBuilder InitializerNotImplemented() => AddInitializerStringCodeStatements(WellKnownCodeStatements.ThrowNotImplementedException);
}
