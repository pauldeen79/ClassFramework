namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface ISubClassesContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<ITypeBase> SubClasses { get; }
}
