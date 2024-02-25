namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IParentTypeContainer
{
    [Required(AllowEmptyStrings = true)] string ParentTypeFullName { get; }
}
