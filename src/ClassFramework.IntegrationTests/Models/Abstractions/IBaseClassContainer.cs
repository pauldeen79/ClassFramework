namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IBaseClassContainer
{
    [Required(AllowEmptyStrings = true)] string BaseClass { get; }
}
