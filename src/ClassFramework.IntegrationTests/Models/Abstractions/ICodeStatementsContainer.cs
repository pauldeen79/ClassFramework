namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface ICodeStatementsContainer
{
    [Required] IReadOnlyCollection<ICodeStatementBase> CodeStatements { get; }
}
