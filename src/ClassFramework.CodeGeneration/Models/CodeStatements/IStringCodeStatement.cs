namespace ClassFramework.CodeGeneration.Models.CodeStatements;

internal interface IStringCodeStatement : ICodeStatementBase
{
    [Required] string Statement { get; set; }
}
