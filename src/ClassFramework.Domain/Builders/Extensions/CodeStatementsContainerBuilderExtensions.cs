namespace ClassFramework.Domain.Builders.Extensions;

public static partial class CodeStatementsContainerBuilderExtensions
{
    public static T AddCodeStatements<T>(this T instance, params string[] statements) where T : ICodeStatementsContainerBuilder
        => instance.AddCodeStatements(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder(x)));

    public static T AddCodeStatements<T>(this T instance, IEnumerable<string> statements) where T : ICodeStatementsContainerBuilder
        => instance.AddCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());

    public static T NotImplemented<T>(this T instance) where T : ICodeStatementsContainerBuilder
        => instance.AddCodeStatements(WellKnownCodeStatements.ThrowNotImplementedException);
}
