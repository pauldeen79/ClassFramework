namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IConstructorsContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IConstructor> Constructors { get; }
}
