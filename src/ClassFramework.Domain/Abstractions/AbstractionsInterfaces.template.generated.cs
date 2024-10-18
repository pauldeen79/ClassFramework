﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.10
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace ClassFramework.Domain.Abstractions
{
    public partial interface IAttributesContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Attribute> Attributes
        {
            get;
        }
    }
    public partial interface IBaseClassContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string BaseClass
        {
            get;
        }
    }
    public partial interface ICodeStatementsContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.CodeStatementBase> CodeStatements
        {
            get;
        }
    }
    public partial interface IConcreteType : ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer, ClassFramework.Domain.Abstractions.IConstructorsContainer, ClassFramework.Domain.Abstractions.IRecordContainer, ClassFramework.Domain.Abstractions.IBaseClassContainer
    {
    }
    public partial interface IConstructorsContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Constructor> Constructors
        {
            get;
        }
    }
    public partial interface IDefaultValueContainer
    {
        object? DefaultValue
        {
            get;
        }
    }
    public partial interface IEnumsContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Enumeration> Enums
        {
            get;
        }
    }
    public partial interface IExplicitInterfaceNameContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string ExplicitInterfaceName
        {
            get;
        }
    }
    public partial interface IGenericTypeArgumentsContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<string> GenericTypeArguments
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<string> GenericTypeArgumentConstraints
        {
            get;
        }
    }
    public partial interface IModifiersContainer : ClassFramework.Domain.Abstractions.IVisibilityContainer
    {
        bool Static
        {
            get;
        }

        bool Virtual
        {
            get;
        }

        bool Abstract
        {
            get;
        }

        bool Protected
        {
            get;
        }

        bool Override
        {
            get;
        }

        bool New
        {
            get;
        }
    }
    public partial interface INameContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        string Name
        {
            get;
        }
    }
    public partial interface IParametersContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Parameter> Parameters
        {
            get;
        }
    }
    public partial interface IParentTypeContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string ParentTypeFullName
        {
            get;
        }
    }
    public partial interface IRecordContainer
    {
        bool Record
        {
            get;
        }
    }
    public partial interface IReferenceType : ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer
    {
        bool Static
        {
            get;
        }

        bool Sealed
        {
            get;
        }

        bool Abstract
        {
            get;
        }
    }
    public partial interface ISubClassesContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.TypeBase> SubClasses
        {
            get;
        }
    }
    public partial interface ISuppressWarningCodesContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        System.Collections.Generic.IReadOnlyCollection<string> SuppressWarningCodes
        {
            get;
        }
    }
    public partial interface IType : ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string Namespace
        {
            get;
        }

        bool Partial
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<string> Interfaces
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Field> Fields
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Property> Properties
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Method> Methods
        {
            get;
        }
    }
    public partial interface ITypeContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        string TypeName
        {
            get;
        }

        bool IsNullable
        {
            get;
        }

        bool IsValueType
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Abstractions.ITypeContainer> GenericTypeArguments
        {
            get;
        }
    }
    public partial interface IValueType : ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer
    {
    }
    public partial interface IVisibilityContainer
    {
        ClassFramework.Domain.Domains.Visibility Visibility
        {
            get;
        }
    }
}
#nullable disable
