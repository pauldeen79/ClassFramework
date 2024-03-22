namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IEnumsContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IEnumeration> Enums { get; }
}
