namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;

internal interface IDelegateArgument : IFunctionCallArgument
{
    [Required] Func<object?> Delegate { get; }
    Func<Type>? ValidationDelegate { get; }
}
