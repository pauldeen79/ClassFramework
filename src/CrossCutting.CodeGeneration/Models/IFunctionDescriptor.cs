namespace CrossCutting.CodeGeneration.Models;

internal interface IFunctionDescriptor
{
    [Required] string Name { get; }
    [Required] Type FunctionType { get; }
    Type? ReturnValueType { get; }
    [Required(AllowEmptyStrings = true)] string Description { get; }
    [Required][ValidateObject] IReadOnlyCollection<IFunctionDescriptorArgument> Arguments { get; }
    [Required][ValidateObject] IReadOnlyCollection<IFunctionDescriptorResult> Results { get; }
}
