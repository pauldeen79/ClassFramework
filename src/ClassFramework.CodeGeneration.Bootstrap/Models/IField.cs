﻿namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IField : IMetadataContainer, IExtendedVisibilityContainer, INameContainer, IAttributesContainer, ITypeContainer, IDefaultValueContainer, IParentTypeContainer
{
    bool ReadOnly { get; }
    bool Constant { get; }
    bool Event { get; }
}
