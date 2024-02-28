namespace ClassFramework.Domain.Builders.Extensions;

public static partial class CodeStatementsContainerBuilderExtensions
{
    public static T AddStringCodeStatements<T>(this T instance, params string[] statements) where T : ICodeStatementsContainerBuilder
    {
        instance.CodeStatements.AddRange(statements.IsNotNull(nameof(statements)).Select(x => new StringCodeStatementBuilder().WithStatement(x)));
        return instance;
    }

    public static T AddStringCodeStatements<T>(this T instance, IEnumerable<string> statements) where T : ICodeStatementsContainerBuilder
    {
        return instance.AddStringCodeStatements(statements.IsNotNull(nameof(statements)).ToArray());
    }
}
