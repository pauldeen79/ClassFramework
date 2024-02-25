namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IExplicitInterfaceNameContainer
{
    [Required(AllowEmptyStrings = true)] string ExplicitInterfaceName { get; }
}
