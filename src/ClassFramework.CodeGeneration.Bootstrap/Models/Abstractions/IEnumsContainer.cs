namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IEnumsContainer
{
    [Required] IReadOnlyCollection<IEnumeration> Enums { get; }
}
