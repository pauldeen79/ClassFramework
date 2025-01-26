namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;

internal interface IConstantArgument : IFunctionCallArgument
{
    object? Value { get; }
}
