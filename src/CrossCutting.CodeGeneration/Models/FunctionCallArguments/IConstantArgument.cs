namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;

internal interface IConstantArgument : IFunctionCallArgument
{
    object? Value { get; }
}

internal interface ITypedConstantArgument<T> : IFunctionCallArgument, ITypedFunctionCallArgument<T>
{
    T Value { get; }
}
