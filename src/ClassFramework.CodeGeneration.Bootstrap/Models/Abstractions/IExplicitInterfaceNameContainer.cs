namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IExplicitInterfaceNameContainer
{
    [Required(AllowEmptyStrings = true)] string ExplicitInterfaceName { get; }
}
