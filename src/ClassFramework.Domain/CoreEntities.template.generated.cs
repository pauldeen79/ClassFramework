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
namespace ClassFramework.Domain
{
    public partial record Attribute : ClassFramework.Domain.Abstractions.INameContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.AttributeParameter> Parameters
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get;
        }

        public Attribute(System.Collections.Generic.IEnumerable<ClassFramework.Domain.AttributeParameter> parameters, string name)
        {
            this.Parameters = parameters is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.AttributeParameter>(parameters);
            this.Name = name;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.AttributeBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.AttributeBuilder(this);
        }
    }
    public partial record AttributeParameter
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Name
        {
            get;
        }

        public object Value
        {
            get;
        }

        public AttributeParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.AttributeParameterBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.AttributeParameterBuilder(this);
        }
    }
    public partial record Constructor : ClassFramework.Domain.Abstractions.IModifiersContainer, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.ICodeStatementsContainer, ClassFramework.Domain.Abstractions.IParametersContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ChainCall
        {
            get;
        }

        public bool Static
        {
            get;
        }

        public bool Virtual
        {
            get;
        }

        public bool Abstract
        {
            get;
        }

        public bool Protected
        {
            get;
        }

        public bool Override
        {
            get;
        }

        public bool New
        {
            get;
        }

        public ClassFramework.Domain.Domains.Visibility Visibility
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
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.CodeStatementBase> CodeStatements
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Parameter> Parameters
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.Generic.IReadOnlyCollection<string> SuppressWarningCodes
        {
            get;
        }

        public Constructor(string chainCall, bool @static, bool @virtual, bool @abstract, bool @protected, bool @override, bool @new, ClassFramework.Domain.Domains.Visibility visibility, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, System.Collections.Generic.IEnumerable<ClassFramework.Domain.CodeStatementBase> codeStatements, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Parameter> parameters, System.Collections.Generic.IEnumerable<string> suppressWarningCodes)
        {
            this.ChainCall = chainCall;
            this.Static = @static;
            this.Virtual = @virtual;
            this.Abstract = @abstract;
            this.Protected = @protected;
            this.Override = @override;
            this.New = @new;
            this.Visibility = visibility;
            this.Attributes = attributes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.CodeStatements = codeStatements is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.CodeStatementBase>(codeStatements);
            this.Parameters = parameters is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Parameter>(parameters);
            this.SuppressWarningCodes = suppressWarningCodes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<System.String>(suppressWarningCodes);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.ConstructorBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.ConstructorBuilder(this);
        }
    }
    public partial record Enumeration : ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IVisibilityContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.EnumerationMember> Members
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
        public string Name
        {
            get;
        }

        public ClassFramework.Domain.Domains.Visibility Visibility
        {
            get;
        }

        public Enumeration(System.Collections.Generic.IEnumerable<ClassFramework.Domain.EnumerationMember> members, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, string name, ClassFramework.Domain.Domains.Visibility visibility)
        {
            this.Members = members is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.EnumerationMember>(members);
            this.Attributes = attributes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.Name = name;
            this.Visibility = visibility;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.EnumerationBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.EnumerationBuilder(this);
        }
    }
    public partial record EnumerationMember : ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.INameContainer
    {
        public object? Value
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
        public string Name
        {
            get;
        }

        public EnumerationMember(object? value, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, string name)
        {
            this.Value = value;
            this.Attributes = attributes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.Name = name;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.EnumerationMemberBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.EnumerationMemberBuilder(this);
        }
    }
    public partial record Field : ClassFramework.Domain.Abstractions.IModifiersContainer, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.ITypeContainer, ClassFramework.Domain.Abstractions.IDefaultValueContainer, ClassFramework.Domain.Abstractions.IParentTypeContainer
    {
        public bool ReadOnly
        {
            get;
        }

        public bool Constant
        {
            get;
        }

        public bool Event
        {
            get;
        }

        public bool Static
        {
            get;
        }

        public bool Virtual
        {
            get;
        }

        public bool Abstract
        {
            get;
        }

        public bool Protected
        {
            get;
        }

        public bool Override
        {
            get;
        }

        public bool New
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
        public string TypeName
        {
            get;
        }

        public bool IsNullable
        {
            get;
        }

        public bool IsValueType
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Abstractions.ITypeContainer> GenericTypeArguments
        {
            get;
        }

        public object? DefaultValue
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ParentTypeFullName
        {
            get;
        }

        public Field(bool readOnly, bool constant, bool @event, bool @static, bool @virtual, bool @abstract, bool @protected, bool @override, bool @new, ClassFramework.Domain.Domains.Visibility visibility, string name, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, string typeName, bool isNullable, bool isValueType, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Abstractions.ITypeContainer> genericTypeArguments, object? defaultValue, string parentTypeFullName)
        {
            this.ReadOnly = readOnly;
            this.Constant = constant;
            this.Event = @event;
            this.Static = @static;
            this.Virtual = @virtual;
            this.Abstract = @abstract;
            this.Protected = @protected;
            this.Override = @override;
            this.New = @new;
            this.Visibility = visibility;
            this.Name = name;
            this.Attributes = attributes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.TypeName = typeName;
            this.IsNullable = isNullable;
            this.IsValueType = isValueType;
            this.GenericTypeArguments = genericTypeArguments is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Abstractions.ITypeContainer>(genericTypeArguments);
            this.DefaultValue = defaultValue;
            this.ParentTypeFullName = parentTypeFullName;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.FieldBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.FieldBuilder(this);
        }
    }
    public partial record Literal
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Value
        {
            get;
        }

        public object? OriginalValue
        {
            get;
        }

        public Literal(string value, object? originalValue)
        {
            this.Value = value;
            this.OriginalValue = originalValue;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.LiteralBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.LiteralBuilder(this);
        }
    }
    public partial record Method : ClassFramework.Domain.Abstractions.IModifiersContainer, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.ICodeStatementsContainer, ClassFramework.Domain.Abstractions.IParametersContainer, ClassFramework.Domain.Abstractions.IExplicitInterfaceNameContainer, ClassFramework.Domain.Abstractions.IParentTypeContainer, ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer, ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ReturnTypeName
        {
            get;
        }

        public bool ReturnTypeIsNullable
        {
            get;
        }

        public bool ReturnTypeIsValueType
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Abstractions.ITypeContainer> ReturnTypeGenericTypeArguments
        {
            get;
        }

        public bool Partial
        {
            get;
        }

        public bool ExtensionMethod
        {
            get;
        }

        public bool Operator
        {
            get;
        }

        public bool Async
        {
            get;
        }

        public bool Static
        {
            get;
        }

        public bool Virtual
        {
            get;
        }

        public bool Abstract
        {
            get;
        }

        public bool Protected
        {
            get;
        }

        public bool Override
        {
            get;
        }

        public bool New
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
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.CodeStatementBase> CodeStatements
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Parameter> Parameters
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ExplicitInterfaceName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ParentTypeFullName
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

        public Method(string returnTypeName, bool returnTypeIsNullable, bool returnTypeIsValueType, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Abstractions.ITypeContainer> returnTypeGenericTypeArguments, bool partial, bool extensionMethod, bool @operator, bool async, bool @static, bool @virtual, bool @abstract, bool @protected, bool @override, bool @new, ClassFramework.Domain.Domains.Visibility visibility, string name, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, System.Collections.Generic.IEnumerable<ClassFramework.Domain.CodeStatementBase> codeStatements, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Parameter> parameters, string explicitInterfaceName, string parentTypeFullName, System.Collections.Generic.IEnumerable<string> genericTypeArguments, System.Collections.Generic.IEnumerable<string> genericTypeArgumentConstraints, System.Collections.Generic.IEnumerable<string> suppressWarningCodes)
        {
            this.ReturnTypeName = returnTypeName;
            this.ReturnTypeIsNullable = returnTypeIsNullable;
            this.ReturnTypeIsValueType = returnTypeIsValueType;
            this.ReturnTypeGenericTypeArguments = returnTypeGenericTypeArguments is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Abstractions.ITypeContainer>(returnTypeGenericTypeArguments);
            this.Partial = partial;
            this.ExtensionMethod = extensionMethod;
            this.Operator = @operator;
            this.Async = async;
            this.Static = @static;
            this.Virtual = @virtual;
            this.Abstract = @abstract;
            this.Protected = @protected;
            this.Override = @override;
            this.New = @new;
            this.Visibility = visibility;
            this.Name = name;
            this.Attributes = attributes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.CodeStatements = codeStatements is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.CodeStatementBase>(codeStatements);
            this.Parameters = parameters is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Parameter>(parameters);
            this.ExplicitInterfaceName = explicitInterfaceName;
            this.ParentTypeFullName = parentTypeFullName;
            this.GenericTypeArguments = genericTypeArguments is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<System.String>(genericTypeArguments);
            this.GenericTypeArgumentConstraints = genericTypeArgumentConstraints is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<System.String>(genericTypeArgumentConstraints);
            this.SuppressWarningCodes = suppressWarningCodes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<System.String>(suppressWarningCodes);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.MethodBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.MethodBuilder(this);
        }
    }
    public partial record Parameter : ClassFramework.Domain.Abstractions.ITypeContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IDefaultValueContainer
    {
        public bool IsParamArray
        {
            get;
        }

        public bool IsOut
        {
            get;
        }

        public bool IsRef
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string TypeName
        {
            get;
        }

        public bool IsNullable
        {
            get;
        }

        public bool IsValueType
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Abstractions.ITypeContainer> GenericTypeArguments
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
        public string Name
        {
            get;
        }

        public object? DefaultValue
        {
            get;
        }

        public Parameter(bool isParamArray, bool isOut, bool isRef, string typeName, bool isNullable, bool isValueType, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Abstractions.ITypeContainer> genericTypeArguments, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, string name, object? defaultValue)
        {
            this.IsParamArray = isParamArray;
            this.IsOut = isOut;
            this.IsRef = isRef;
            this.TypeName = typeName;
            this.IsNullable = isNullable;
            this.IsValueType = isValueType;
            this.GenericTypeArguments = genericTypeArguments is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Abstractions.ITypeContainer>(genericTypeArguments);
            this.Attributes = attributes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.Name = name;
            this.DefaultValue = defaultValue;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.ParameterBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.ParameterBuilder(this);
        }
    }
    public partial record Property : ClassFramework.Domain.Abstractions.IModifiersContainer, ClassFramework.Domain.Abstractions.IVisibilityContainer, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IAttributesContainer, ClassFramework.Domain.Abstractions.ITypeContainer, ClassFramework.Domain.Abstractions.IDefaultValueContainer, ClassFramework.Domain.Abstractions.IExplicitInterfaceNameContainer, ClassFramework.Domain.Abstractions.IParentTypeContainer
    {
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool HasGetter
        {
            get;
        }

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool HasSetter
        {
            get;
        }

        public bool HasInitializer
        {
            get;
        }

        public ClassFramework.Domain.Domains.SubVisibility GetterVisibility
        {
            get;
        }

        public ClassFramework.Domain.Domains.SubVisibility SetterVisibility
        {
            get;
        }

        public ClassFramework.Domain.Domains.SubVisibility InitializerVisibility
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.CodeStatementBase> GetterCodeStatements
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.CodeStatementBase> SetterCodeStatements
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.CodeStatementBase> InitializerCodeStatements
        {
            get;
        }

        public bool Static
        {
            get;
        }

        public bool Virtual
        {
            get;
        }

        public bool Abstract
        {
            get;
        }

        public bool Protected
        {
            get;
        }

        public bool Override
        {
            get;
        }

        public bool New
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
        public string TypeName
        {
            get;
        }

        public bool IsNullable
        {
            get;
        }

        public bool IsValueType
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Abstractions.ITypeContainer> GenericTypeArguments
        {
            get;
        }

        public object? DefaultValue
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ExplicitInterfaceName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ParentTypeFullName
        {
            get;
        }

        public Property(bool hasGetter, bool hasSetter, bool hasInitializer, ClassFramework.Domain.Domains.SubVisibility getterVisibility, ClassFramework.Domain.Domains.SubVisibility setterVisibility, ClassFramework.Domain.Domains.SubVisibility initializerVisibility, System.Collections.Generic.IEnumerable<ClassFramework.Domain.CodeStatementBase> getterCodeStatements, System.Collections.Generic.IEnumerable<ClassFramework.Domain.CodeStatementBase> setterCodeStatements, System.Collections.Generic.IEnumerable<ClassFramework.Domain.CodeStatementBase> initializerCodeStatements, bool @static, bool @virtual, bool @abstract, bool @protected, bool @override, bool @new, ClassFramework.Domain.Domains.Visibility visibility, string name, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Attribute> attributes, string typeName, bool isNullable, bool isValueType, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Abstractions.ITypeContainer> genericTypeArguments, object? defaultValue, string explicitInterfaceName, string parentTypeFullName)
        {
            this.HasGetter = hasGetter;
            this.HasSetter = hasSetter;
            this.HasInitializer = hasInitializer;
            this.GetterVisibility = getterVisibility;
            this.SetterVisibility = setterVisibility;
            this.InitializerVisibility = initializerVisibility;
            this.GetterCodeStatements = getterCodeStatements is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.CodeStatementBase>(getterCodeStatements);
            this.SetterCodeStatements = setterCodeStatements is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.CodeStatementBase>(setterCodeStatements);
            this.InitializerCodeStatements = initializerCodeStatements is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.CodeStatementBase>(initializerCodeStatements);
            this.Static = @static;
            this.Virtual = @virtual;
            this.Abstract = @abstract;
            this.Protected = @protected;
            this.Override = @override;
            this.New = @new;
            this.Visibility = visibility;
            this.Name = name;
            this.Attributes = attributes is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Attribute>(attributes);
            this.TypeName = typeName;
            this.IsNullable = isNullable;
            this.IsValueType = isValueType;
            this.GenericTypeArguments = genericTypeArguments is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Domain.Abstractions.ITypeContainer>(genericTypeArguments);
            this.DefaultValue = defaultValue;
            this.ExplicitInterfaceName = explicitInterfaceName;
            this.ParentTypeFullName = parentTypeFullName;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Domain.Builders.PropertyBuilder ToBuilder()
        {
            return new ClassFramework.Domain.Builders.PropertyBuilder(this);
        }
    }
}
#nullable disable
