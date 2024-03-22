namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IType : IVisibilityContainer, INameContainer, IAttributesContainer, IGenericTypeArgumentsContainer, ISuppressWarningCodesContainer
{
    [Required(AllowEmptyStrings = true)] string Namespace { get; }
    bool Partial { get; }
    [Required] [ValidateObject] IReadOnlyCollection<string> Interfaces { get; }
    [Required] [ValidateObject] IReadOnlyCollection<IField> Fields { get; }
    [Required] [ValidateObject] IReadOnlyCollection<IProperty> Properties { get; }
    [Required] [ValidateObject] IReadOnlyCollection<IMethod> Methods { get; }
}
