namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;

internal interface ITypedConstantArgument<T> : IFunctionCallArgument, ITypedFunctionCallArgument<T>
{
    T Value { get; }
}
