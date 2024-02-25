namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface INameContainer
{
    [Required] string Name { get; }
}
