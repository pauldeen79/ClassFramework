namespace ClassFramework.Domain.Builders.CodeStatements;

public partial class StringCodeStatementBuilder
{
    public StringCodeStatementBuilder(string statement)
    {
        ArgumentGuard.IsNotNull(statement, nameof(statement));

        _statement = statement;
    }
}
