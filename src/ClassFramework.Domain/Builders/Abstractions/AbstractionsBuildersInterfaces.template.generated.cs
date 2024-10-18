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
namespace ClassFramework.Domain.Builders.Abstractions
{
    public partial interface IAttributesContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.AttributeBuilder> Attributes
        {
            get;
            set;
        }
    }
    public partial interface IBaseClassContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string BaseClass
        {
            get;
            set;
        }
    }
    public partial interface ICodeStatementsContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.CodeStatementBaseBuilder> CodeStatements
        {
            get;
            set;
        }
    }
    public partial interface IConcreteTypeBuilder : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IRecordContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IBaseClassContainerBuilder
    {
    }
    public partial interface IConstructorsContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder> Constructors
        {
            get;
            set;
        }
    }
    public partial interface IDefaultValueContainerBuilder
    {
        object? DefaultValue
        {
            get;
            set;
        }
    }
    public partial interface IEnumsContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.EnumerationBuilder> Enums
        {
            get;
            set;
        }
    }
    public partial interface IExplicitInterfaceNameContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string ExplicitInterfaceName
        {
            get;
            set;
        }
    }
    public partial interface IGenericTypeArgumentsContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<string> GenericTypeArguments
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<string> GenericTypeArgumentConstraints
        {
            get;
            set;
        }
    }
    public partial interface IModifiersContainerBuilder : ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder
    {
        bool Static
        {
            get;
            set;
        }

        bool Virtual
        {
            get;
            set;
        }

        bool Abstract
        {
            get;
            set;
        }

        bool Protected
        {
            get;
            set;
        }

        bool Override
        {
            get;
            set;
        }

        bool New
        {
            get;
            set;
        }
    }
    public partial interface INameContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        string Name
        {
            get;
            set;
        }
    }
    public partial interface IParametersContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ParameterBuilder> Parameters
        {
            get;
            set;
        }
    }
    public partial interface IParentTypeContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string ParentTypeFullName
        {
            get;
            set;
        }
    }
    public partial interface IRecordContainerBuilder
    {
        bool Record
        {
            get;
            set;
        }
    }
    public partial interface IReferenceTypeBuilder : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder
    {
        bool Static
        {
            get;
            set;
        }

        bool Sealed
        {
            get;
            set;
        }

        bool Abstract
        {
            get;
            set;
        }
    }
    public partial interface ISubClassesContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.TypeBaseBuilder> SubClasses
        {
            get;
            set;
        }
    }
    public partial interface ISuppressWarningCodesContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        System.Collections.ObjectModel.ObservableCollection<string> SuppressWarningCodes
        {
            get;
            set;
        }
    }
    public partial interface ITypeBuilder : ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string Namespace
        {
            get;
            set;
        }

        bool Partial
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<string> Interfaces
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.FieldBuilder> Fields
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.PropertyBuilder> Properties
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.MethodBuilder> Methods
        {
            get;
            set;
        }
    }
    public partial interface ITypeContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        string TypeName
        {
            get;
            set;
        }

        bool IsNullable
        {
            get;
            set;
        }

        bool IsValueType
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Abstractions.ITypeContainer> GenericTypeArguments
        {
            get;
            set;
        }
    }
    public partial interface IValueTypeBuilder : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder
    {
    }
    public partial interface IVisibilityContainerBuilder
    {
        ClassFramework.Domain.Domains.Visibility Visibility
        {
            get;
            set;
        }
    }
}
#nullable disable
