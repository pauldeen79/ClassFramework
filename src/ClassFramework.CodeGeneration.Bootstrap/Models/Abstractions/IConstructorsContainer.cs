namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IConstructorsContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IConstructor> Constructors { get; }
}
