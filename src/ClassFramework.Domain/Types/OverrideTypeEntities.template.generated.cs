﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.5
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
namespace ClassFramework.Domain.Types
{
    public partial record Class : ClassFramework.Domain.TypeBase, ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer, ClassFramework.Domain.Abstractions.IReferenceType, ClassFramework.Domain.Abstractions.IConcreteType, ClassFramework.Domain.Abstractions.IConstructorsContainer, ClassFramework.Domain.Abstractions.IRecordContainer, ClassFramework.Domain.Abstractions.IBaseClassContainer, ClassFramework.Domain.Abstractions.IEnumsContainer, ClassFramework.Domain.Abstractions.ISubClassesContainer
    {
        public bool Static
        {
            get;
        }

        public bool Sealed
        {
            get;
        }

        public bool Abstract
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Constructor> Constructors
        {
            get;
        }

        public bool Record
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BaseClass
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Enumeration> Enums
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.TypeBase> SubClasses
        {
            get;
        }

        public Class(string @namespace, bool partial, System.Collections.Generic.IEnumerable<string> interfaces, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Field> fields, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Property> properties, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Method> methods, ClassFramework.Domain.Domains.Visibility visibility, string name, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, System.Collections.Generic.IEnumerable<string> genericTypeArguments, System.Collections.Generic.IEnumerable<string> genericTypeArgumentConstraints, System.Collections.Generic.IEnumerable<string> suppressWarningCodes, bool @static, bool @sealed, bool @abstract, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Constructor> constructors, bool record, string baseClass, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Enumeration> enums, System.Collections.Generic.IEnumerable<ClassFramework.Domain.TypeBase> subClasses) : base(@namespace, partial, interfaces, fields, properties, methods, visibility, name, attributes, genericTypeArguments, genericTypeArgumentConstraints, suppressWarningCodes)
        {
            this.Static = @static;
            this.Sealed = @sealed;
            this.Abstract = @abstract;
            this.Constructors = constructors is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Constructor>(constructors);
            this.Record = record;
            this.BaseClass = baseClass;
            this.Enums = enums is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Enumeration>(enums);
            this.SubClasses = subClasses is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.TypeBase>(subClasses);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override ClassFramework.Domain.Builders.TypeBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder ToTypedBuilder()
        {
            return new ClassFramework.Domain.Builders.Types.ClassBuilder(this);
        }
    }
    public partial record Interface : ClassFramework.Domain.TypeBase, ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer
    {
        public Interface(string @namespace, bool partial, System.Collections.Generic.IEnumerable<string> interfaces, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Field> fields, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Property> properties, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Method> methods, ClassFramework.Domain.Domains.Visibility visibility, string name, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, System.Collections.Generic.IEnumerable<string> genericTypeArguments, System.Collections.Generic.IEnumerable<string> genericTypeArgumentConstraints, System.Collections.Generic.IEnumerable<string> suppressWarningCodes) : base(@namespace, partial, interfaces, fields, properties, methods, visibility, name, attributes, genericTypeArguments, genericTypeArgumentConstraints, suppressWarningCodes)
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override ClassFramework.Domain.Builders.TypeBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public ClassFramework.Domain.Builders.Types.InterfaceBuilder ToTypedBuilder()
        {
            return new ClassFramework.Domain.Builders.Types.InterfaceBuilder(this);
        }
    }
    public partial record Struct : ClassFramework.Domain.TypeBase, ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer, ClassFramework.Domain.Abstractions.IValueType, ClassFramework.Domain.Abstractions.IConcreteType, ClassFramework.Domain.Abstractions.IConstructorsContainer, ClassFramework.Domain.Abstractions.IRecordContainer, ClassFramework.Domain.Abstractions.IBaseClassContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Constructor> Constructors
        {
            get;
        }

        public bool Record
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BaseClass
        {
            get;
        }

        public Struct(string @namespace, bool partial, System.Collections.Generic.IEnumerable<string> interfaces, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Field> fields, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Property> properties, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Method> methods, ClassFramework.Domain.Domains.Visibility visibility, string name, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, System.Collections.Generic.IEnumerable<string> genericTypeArguments, System.Collections.Generic.IEnumerable<string> genericTypeArgumentConstraints, System.Collections.Generic.IEnumerable<string> suppressWarningCodes, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Constructor> constructors, bool record, string baseClass) : base(@namespace, partial, interfaces, fields, properties, methods, visibility, name, attributes, genericTypeArguments, genericTypeArgumentConstraints, suppressWarningCodes)
        {
            this.Constructors = constructors is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Constructor>(constructors);
            this.Record = record;
            this.BaseClass = baseClass;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override ClassFramework.Domain.Builders.TypeBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public ClassFramework.Domain.Builders.Types.StructBuilder ToTypedBuilder()
        {
            return new ClassFramework.Domain.Builders.Types.StructBuilder(this);
        }
    }
}
#nullable disable
