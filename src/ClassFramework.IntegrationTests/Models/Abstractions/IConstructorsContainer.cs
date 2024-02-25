namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IConstructorsContainer
{
    [Required] IReadOnlyCollection<IConstructor> Constructors { get; }
}
