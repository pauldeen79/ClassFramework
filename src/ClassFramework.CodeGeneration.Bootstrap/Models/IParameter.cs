namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IParameter : ITypeContainer, IAttributesContainer, INameContainer, IDefaultValueContainer
{
    bool IsParamArray { get; }
    bool IsOut { get; }
    bool IsRef { get; }
}
