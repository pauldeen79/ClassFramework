namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;

internal interface ITypedConstantArgument<T> : ITypedFunctionCallArgument<T>
{
    T Value { get; }
}
