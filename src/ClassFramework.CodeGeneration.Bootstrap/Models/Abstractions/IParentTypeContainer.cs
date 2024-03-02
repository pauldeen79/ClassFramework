namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IParentTypeContainer
{
    [Required(AllowEmptyStrings = true)] string ParentTypeFullName { get; }
}
