namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IEnumsContainer
{
    [Required][ValidateObject] IReadOnlyCollection<IEnumeration> Enums { get; }
}
