namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;

internal interface IFunctionArgument : IFunctionCallArgument
{
    [Required][ValidateObject] IFunctionCall Function { get; }
}
