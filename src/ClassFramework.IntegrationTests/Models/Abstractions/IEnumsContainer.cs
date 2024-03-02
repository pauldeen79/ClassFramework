namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IEnumsContainer
{
    [Required] IReadOnlyCollection<IEnumeration> Enums { get; }
}
