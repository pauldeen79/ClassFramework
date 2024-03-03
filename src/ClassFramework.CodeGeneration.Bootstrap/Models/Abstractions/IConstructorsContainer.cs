namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IConstructorsContainer
{
    [Required] IReadOnlyCollection<IConstructor> Constructors { get; }
}
