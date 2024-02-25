namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface ICodeStatementsContainer
{
    [Required] IReadOnlyCollection<ICodeStatementBase> CodeStatements { get; }
}
