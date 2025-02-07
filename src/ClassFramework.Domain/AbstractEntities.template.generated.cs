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
namespace ClassFramework.Domain
{
    public abstract partial record CodeStatementBase
    {
        protected CodeStatementBase()
        {
        }

        public abstract ClassFramework.Domain.Builders.CodeStatementBaseBuilder ToBuilder();
    }
    public abstract partial record TypeBase : ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Namespace
        {
            get;
        }

        public bool Partial
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<string> Interfaces
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Field> Fields
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Property> Properties
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Method> Methods
        {
            get;
        }

        public ClassFramework.Domain.Domains.Visibility Visibility
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Attribute> Attributes
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<string> GenericTypeArguments
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<string> GenericTypeArgumentConstraints
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.Generic.IReadOnlyCollection<string> SuppressWarningCodes
        {
            get;
        }

        protected TypeBase(string @namespace, bool partial, System.Collections.Generic.IEnumerable<string> interfaces, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Field> fields, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Property> properties, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Method> methods, ClassFramework.Domain.Domains.Visibility visibility, string name, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, System.Collections.Generic.IEnumerable<string> genericTypeArguments, System.Collections.Generic.IEnumerable<string> genericTypeArgumentConstraints, System.Collections.Generic.IEnumerable<string> suppressWarningCodes)
        {
            this.Namespace = @namespace!;
            this.Partial = partial;
            this.Interfaces = new CrossCutting.Common.ReadOnlyValueCollection<System.String>(interfaces);
            this.Fields = new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Field>(fields);
            this.Properties = new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Property>(properties);
            this.Methods = new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Method>(methods);
            this.Visibility = visibility;
            this.Name = name!;
            this.Attributes = new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.GenericTypeArguments = new CrossCutting.Common.ReadOnlyValueCollection<System.String>(genericTypeArguments);
            this.GenericTypeArgumentConstraints = new CrossCutting.Common.ReadOnlyValueCollection<System.String>(genericTypeArgumentConstraints);
            this.SuppressWarningCodes = new CrossCutting.Common.ReadOnlyValueCollection<System.String>(suppressWarningCodes);
        }

        public abstract ClassFramework.Domain.Builders.TypeBaseBuilder ToBuilder();

        ClassFramework.Domain.Builders.Abstractions.ITypeBuilder ClassFramework.Domain.Abstractions.IType.ToBuilder()
        {
            return ToBuilder();
        }

        ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder ClassFramework.Domain.Abstractions.IVisibilityContainer.ToBuilder()
        {
            return ToBuilder();
        }

        ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder ClassFramework.Domain.Abstractions.INameContainer.ToBuilder()
        {
            return ToBuilder();
        }

        ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder ClassFramework.Domain.Abstractions.IAttributesContainer.ToBuilder()
        {
            return ToBuilder();
        }

        ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer.ToBuilder()
        {
            return ToBuilder();
        }

        ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer.ToBuilder()
        {
            return ToBuilder();
        }
    }
}
#nullable disable
