﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 9.0.1
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
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

        ClassFramework.Domain.Abstractions.IAttributesContainer Build();
    }
    public partial interface IBaseClassContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string BaseClass
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.IBaseClassContainer Build();
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

        ClassFramework.Domain.Abstractions.ICodeStatementsContainer Build();
    }
    public partial interface IConcreteTypeBuilder : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IRecordContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IBaseClassContainerBuilder
    {
        new ClassFramework.Domain.Abstractions.IConcreteType Build();
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

        ClassFramework.Domain.Abstractions.IConstructorsContainer Build();
    }
    public partial interface IDefaultValueContainerBuilder
    {
        object? DefaultValue
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.IDefaultValueContainer Build();
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

        ClassFramework.Domain.Abstractions.IEnumsContainer Build();
    }
    public partial interface IExplicitInterfaceNameContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string ExplicitInterfaceName
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.IExplicitInterfaceNameContainer Build();
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

        ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer Build();
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

        new ClassFramework.Domain.Abstractions.IModifiersContainer Build();
    }
    public partial interface INameContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        string Name
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.INameContainer Build();
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

        ClassFramework.Domain.Abstractions.IParametersContainer Build();
    }
    public partial interface IParentTypeContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string ParentTypeFullName
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.IParentTypeContainer Build();
    }
    public partial interface IRecordContainerBuilder
    {
        bool Record
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.IRecordContainer Build();
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

        new ClassFramework.Domain.Abstractions.IReferenceType Build();
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

        ClassFramework.Domain.Abstractions.ISubClassesContainer Build();
    }
    public partial interface ISuppressWarningCodesContainerBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        System.Collections.ObjectModel.ObservableCollection<string> SuppressWarningCodes
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer Build();
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

        new ClassFramework.Domain.Abstractions.IType Build();
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
        System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder> GenericTypeArguments
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.ITypeContainer Build();
    }
    public partial interface IValueTypeBuilder : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder
    {
        new ClassFramework.Domain.Abstractions.IValueType Build();
    }
    public partial interface IVisibilityContainerBuilder
    {
        ClassFramework.Domain.Domains.Visibility Visibility
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.IVisibilityContainer Build();
    }
}
#nullable disable
