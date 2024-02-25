namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IBaseClassContainer
{
    [Required(AllowEmptyStrings = true)] string BaseClass { get; }
}
