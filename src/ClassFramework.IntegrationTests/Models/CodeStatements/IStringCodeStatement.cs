namespace ClassFramework.IntegrationTests.Models.CodeStatements;

internal interface IStringCodeStatement : ICodeStatementBase
{
    [Required] string Statement { get; set; }
}
