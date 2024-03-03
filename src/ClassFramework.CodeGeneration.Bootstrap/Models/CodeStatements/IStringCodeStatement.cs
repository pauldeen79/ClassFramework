namespace ClassFramework.CodeGeneration.Bootstrap.Models.CodeStatements;

internal interface IStringCodeStatement : ICodeStatementBase
{
    [Required] string Statement { get; set; }
}
