namespace CrossCutting.CodeGeneration.Models.FunctionCallArguments;
internal interface IExpressionArgument : IFunctionCallArgument
{
    [Required] string Value { get; }
}
