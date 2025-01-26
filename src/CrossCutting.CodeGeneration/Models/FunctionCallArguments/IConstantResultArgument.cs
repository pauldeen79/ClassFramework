namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;

internal interface IConstantResultArgument : IFunctionCallArgument
{
    [Required] Result<object?> Result { get; }
}
