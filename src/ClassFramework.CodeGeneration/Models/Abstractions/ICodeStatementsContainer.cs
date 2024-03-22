namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface ICodeStatementsContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<ICodeStatementBase> CodeStatements { get; }
}
