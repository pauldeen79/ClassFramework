namespace CrossCutting.CodeGeneration.Models;

internal interface IFunctionCall
{
    [Required] string Name { get; }
    [Required][ValidateObject] IReadOnlyCollection<IFunctionCallArgument> Arguments { get; }
}
