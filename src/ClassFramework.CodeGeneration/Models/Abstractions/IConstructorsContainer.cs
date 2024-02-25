namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IConstructorsContainer
{
    [Required] IReadOnlyCollection<IConstructor> Constructors { get; }
}
