namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IParentTypeContainer
{
    [Required(AllowEmptyStrings = true)] string ParentTypeFullName { get; }
}
