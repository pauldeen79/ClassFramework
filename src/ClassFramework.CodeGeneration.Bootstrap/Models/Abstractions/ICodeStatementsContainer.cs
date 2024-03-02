namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface ICodeStatementsContainer
{
    [Required] IReadOnlyCollection<ICodeStatementBase> CodeStatements { get; }
}
