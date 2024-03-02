﻿namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IParameter : ITypeContainer, IAttributesContainer, IMetadataContainer, INameContainer, IDefaultValueContainer
{
    bool IsParamArray { get; }
    bool IsOut { get; }
    bool IsRef { get; }
}
