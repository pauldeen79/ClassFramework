namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface ICodeStatementsContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<ICodeStatementBase> CodeStatements { get; }
}
