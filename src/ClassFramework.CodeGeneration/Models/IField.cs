namespace ClassFramework.CodeGeneration.Models;

internal interface IField : Abstractions.IModifiersContainer, Abstractions.INameContainer, Abstractions.IAttributesContainer, Abstractions.ITypeContainer, Abstractions.IDefaultValueContainer, Abstractions.IParentTypeContainer
{
    bool ReadOnly { get; }
    bool Constant { get; }
    bool Event { get; }
}
