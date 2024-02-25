namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface ISubClassesContainer
{
    [Required] IReadOnlyCollection<ITypeBase> SubClasses { get; }
}
