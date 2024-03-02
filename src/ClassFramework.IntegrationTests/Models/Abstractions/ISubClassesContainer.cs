namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface ISubClassesContainer
{
    [Required] IReadOnlyCollection<ITypeBase> SubClasses { get; }
}
