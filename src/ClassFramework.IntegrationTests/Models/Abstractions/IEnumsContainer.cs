namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IEnumsContainer
{
    [Required] IReadOnlyCollection<IEnumeration> Enums { get; }
}
