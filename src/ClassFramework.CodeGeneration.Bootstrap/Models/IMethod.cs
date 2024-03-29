﻿namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IMethod : IExtendedVisibilityContainer, INameContainer, IAttributesContainer, ICodeStatementsContainer, IParametersContainer, IExplicitInterfaceNameContainer, IParentTypeContainer, IGenericTypeArgumentsContainer, ISuppressWarningCodesContainer
{
    [Required(AllowEmptyStrings = true)] string ReturnTypeName { get; }
    bool ReturnTypeIsNullable { get; }
    bool ReturnTypeIsValueType { get; }

    bool Partial { get; }
    bool ExtensionMethod { get; }
    bool Operator { get; }
    bool Async { get; }
}
