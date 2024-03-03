namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface ISubClassesContainer
{
    [Required] IReadOnlyCollection<ITypeBase> SubClasses { get; }
}
