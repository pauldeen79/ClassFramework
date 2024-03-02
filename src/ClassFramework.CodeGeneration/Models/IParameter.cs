namespace ClassFramework.CodeGeneration.Models;

internal interface IParameter : Abstractions.ITypeContainer, Abstractions.IAttributesContainer, Abstractions.IMetadataContainer, Abstractions.INameContainer, Abstractions.IDefaultValueContainer
{
    bool IsParamArray { get; }
    bool IsOut { get; }
    bool IsRef { get; }
}
