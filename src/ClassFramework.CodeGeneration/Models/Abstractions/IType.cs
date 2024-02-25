namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IType : IMetadataContainer, IVisibilityContainer, INameContainer, IAttributesContainer, IGenericTypeArgumentsContainer
{
    [Required(AllowEmptyStrings = true)] string Namespace { get; }
    bool Partial { get; }
    [Required] IReadOnlyCollection<string> Interfaces { get; }
    [Required] IReadOnlyCollection<IField> Fields { get; }
    [Required] IReadOnlyCollection<IProperty> Properties { get; }
    [Required] IReadOnlyCollection<IMethod> Methods { get; }
    [Required] IReadOnlyCollection<string> SuppressWarningCodes { get; }
}
