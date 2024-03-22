namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface ISubClassesContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<ITypeBase> SubClasses { get; }
}
