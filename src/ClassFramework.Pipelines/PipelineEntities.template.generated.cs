﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 9.0.7
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace ClassFramework.Pipelines
{
    public partial record Metadata : ClassFramework.Domain.Abstractions.INameContainer
    {
        public object? Value
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get;
        }

        public Metadata(object? value, string name)
        {
            this.Value = value;
            this.Name = name;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Pipelines.Builders.MetadataBuilder ToBuilder()
        {
            return new ClassFramework.Pipelines.Builders.MetadataBuilder(this);
        }

        ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder ClassFramework.Domain.Abstractions.INameContainer.ToBuilder()
        {
            return ToBuilder();
        }
    }
    public partial record NamespaceMapping
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string SourceNamespace
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string TargetNamespace
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Pipelines.Metadata> Metadata
        {
            get;
        }

        public NamespaceMapping(string sourceNamespace, string targetNamespace, System.Collections.Generic.IEnumerable<ClassFramework.Pipelines.Metadata> metadata)
        {
            this.SourceNamespace = sourceNamespace;
            this.TargetNamespace = targetNamespace;
            this.Metadata = metadata is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Pipelines.Metadata>(metadata);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Pipelines.Builders.NamespaceMappingBuilder ToBuilder()
        {
            return new ClassFramework.Pipelines.Builders.NamespaceMappingBuilder(this);
        }
    }
    public partial record PipelineSettings
    {
        public bool AddBackingFields
        {
            get;
        }

        public bool AddCopyConstructor
        {
            get;
        }

        public bool AddFullConstructor
        {
            get;
        }

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool AddImplicitOperatorOnBuilder
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string AddMethodNameFormatString
        {
            get;
        }

        public bool AddNullChecks
        {
            get;
        }

        public bool AddPublicParameterlessConstructor
        {
            get;
        }

        public bool AddSetters
        {
            get;
        }

        public bool AllowGenerationWithoutProperties
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Pipelines.AttributeInitializerDelegate> AttributeInitializers
        {
            get;
        }

        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public ClassFramework.Domain.TypeBase? BaseClass
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BaseClassBuilderNameSpace
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuilderAbstractionsTypeConversionMetadataName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.Generic.IReadOnlyCollection<string> BuilderAbstractionsTypeConversionNamespaces
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuilderExtensionsCollectionCopyStatementFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string NonLazyBuilderExtensionsCollectionCopyStatementFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuilderExtensionsNameFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuilderExtensionsNamespaceFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuilderNameFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuilderNamespaceFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuilderNewCollectionTypeName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuildMethodName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BuildTypedMethodName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string CollectionCopyStatementFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string NonLazyCollectionCopyStatementFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string CollectionInitializationStatementFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string CollectionTypeName
        {
            get;
        }

        public System.Predicate<ClassFramework.Domain.Attribute>? CopyAttributePredicate
        {
            get;
        }

        public bool CopyAttributes
        {
            get;
        }

        public System.Predicate<string>? CopyInterfacePredicate
        {
            get;
        }

        public bool CopyInterfaces
        {
            get;
        }

        public ClassFramework.Pipelines.CopyMethodPredicate? CopyMethodPredicate
        {
            get;
        }

        public bool CopyMethods
        {
            get;
        }

        public bool CreateAsObservable
        {
            get;
        }

        public bool CreateAsPartial
        {
            get;
        }

        public bool CreateConstructors
        {
            get;
        }

        public bool CreateRecord
        {
            get;
        }

        public bool EnableBuilderInheritance
        {
            get;
        }

        public bool EnableInheritance
        {
            get;
        }

        public bool EnableNullableReferenceTypes
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string EntityNameFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string EntityNamespaceFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string EntityNewCollectionTypeName
        {
            get;
        }

        public ClassFramework.Pipelines.Domains.IEquatableItemType IEquatableItemType
        {
            get;
        }

        public bool ImplementIEquatable
        {
            get;
        }

        public ClassFramework.Pipelines.InheritanceComparisonDelegate? InheritanceComparisonDelegate
        {
            get;
        }

        public ClassFramework.Pipelines.ReflectionInheritanceComparisonDelegate? InheritanceComparisonDelegateForReflection
        {
            get;
        }

        public bool InheritFromInterfaces
        {
            get;
        }

        public bool IsAbstract
        {
            get;
        }

        public bool IsForAbstractBuilder
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string NameFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string NamespaceFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Pipelines.NamespaceMapping> NamespaceMappings
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string NonCollectionInitializationStatementFormatString
        {
            get;
        }

        public bool SetDefaultValuesInEntityConstructor
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string SetDefaultValuesMethodName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string SetMethodNameFormatString
        {
            get;
        }

        public ClassFramework.Domain.Domains.SubVisibility SetterVisibility
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.Generic.IReadOnlyCollection<string> SkipNamespacesOnFluentBuilderMethods
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ToBuilderFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string ToTypedBuilderFormatString
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Pipelines.TypenameMapping> TypenameMappings
        {
            get;
        }

        public bool UseBaseClassFromSourceModel
        {
            get;
        }

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool UseBuilderAbstractionsTypeConversion
        {
            get;
        }

        public bool UseBuilderLazyValues
        {
            get;
        }

        public bool UseCrossCuttingInterfaces
        {
            get;
        }

        public bool UseDefaultValueAttributeValuesForBuilderInitialization
        {
            get;
        }

        public bool UseExceptionThrowIfNull
        {
            get;
        }

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool UsePatternMatchingForNullChecks
        {
            get;
        }

        public ClassFramework.Pipelines.Domains.ArgumentValidationType ValidateArguments
        {
            get;
        }

        public PipelineSettings(bool addBackingFields, bool addCopyConstructor, bool addFullConstructor, bool addImplicitOperatorOnBuilder, string addMethodNameFormatString, bool addNullChecks, bool addPublicParameterlessConstructor, bool addSetters, bool allowGenerationWithoutProperties, System.Collections.Generic.IEnumerable<ClassFramework.Pipelines.AttributeInitializerDelegate> attributeInitializers, ClassFramework.Domain.TypeBase? baseClass, string baseClassBuilderNameSpace, string builderAbstractionsTypeConversionMetadataName, System.Collections.Generic.IEnumerable<string> builderAbstractionsTypeConversionNamespaces, string builderExtensionsCollectionCopyStatementFormatString, string nonLazyBuilderExtensionsCollectionCopyStatementFormatString, string builderExtensionsNameFormatString, string builderExtensionsNamespaceFormatString, string builderNameFormatString, string builderNamespaceFormatString, string builderNewCollectionTypeName, string buildMethodName, string buildTypedMethodName, string collectionCopyStatementFormatString, string nonLazyCollectionCopyStatementFormatString, string collectionInitializationStatementFormatString, string collectionTypeName, System.Predicate<ClassFramework.Domain.Attribute>? copyAttributePredicate, bool copyAttributes, System.Predicate<string>? copyInterfacePredicate, bool copyInterfaces, ClassFramework.Pipelines.CopyMethodPredicate? copyMethodPredicate, bool copyMethods, bool createAsObservable, bool createAsPartial, bool createConstructors, bool createRecord, bool enableBuilderInheritance, bool enableInheritance, bool enableNullableReferenceTypes, string entityNameFormatString, string entityNamespaceFormatString, string entityNewCollectionTypeName, ClassFramework.Pipelines.Domains.IEquatableItemType iEquatableItemType, bool implementIEquatable, ClassFramework.Pipelines.InheritanceComparisonDelegate? inheritanceComparisonDelegate, ClassFramework.Pipelines.ReflectionInheritanceComparisonDelegate? inheritanceComparisonDelegateForReflection, bool inheritFromInterfaces, bool isAbstract, bool isForAbstractBuilder, string nameFormatString, string namespaceFormatString, System.Collections.Generic.IEnumerable<ClassFramework.Pipelines.NamespaceMapping> namespaceMappings, string nonCollectionInitializationStatementFormatString, bool setDefaultValuesInEntityConstructor, string setDefaultValuesMethodName, string setMethodNameFormatString, ClassFramework.Domain.Domains.SubVisibility setterVisibility, System.Collections.Generic.IEnumerable<string> skipNamespacesOnFluentBuilderMethods, string toBuilderFormatString, string toTypedBuilderFormatString, System.Collections.Generic.IEnumerable<ClassFramework.Pipelines.TypenameMapping> typenameMappings, bool useBaseClassFromSourceModel, bool useBuilderAbstractionsTypeConversion, bool useBuilderLazyValues, bool useCrossCuttingInterfaces, bool useDefaultValueAttributeValuesForBuilderInitialization, bool useExceptionThrowIfNull, bool usePatternMatchingForNullChecks, ClassFramework.Pipelines.Domains.ArgumentValidationType validateArguments)
        {
            this.AddBackingFields = addBackingFields;
            this.AddCopyConstructor = addCopyConstructor;
            this.AddFullConstructor = addFullConstructor;
            this.AddImplicitOperatorOnBuilder = addImplicitOperatorOnBuilder;
            this.AddMethodNameFormatString = addMethodNameFormatString;
            this.AddNullChecks = addNullChecks;
            this.AddPublicParameterlessConstructor = addPublicParameterlessConstructor;
            this.AddSetters = addSetters;
            this.AllowGenerationWithoutProperties = allowGenerationWithoutProperties;
            this.AttributeInitializers = attributeInitializers is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Pipelines.AttributeInitializerDelegate>(attributeInitializers);
            this.BaseClass = baseClass;
            this.BaseClassBuilderNameSpace = baseClassBuilderNameSpace;
            this.BuilderAbstractionsTypeConversionMetadataName = builderAbstractionsTypeConversionMetadataName;
            this.BuilderAbstractionsTypeConversionNamespaces = builderAbstractionsTypeConversionNamespaces is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<System.String>(builderAbstractionsTypeConversionNamespaces);
            this.BuilderExtensionsCollectionCopyStatementFormatString = builderExtensionsCollectionCopyStatementFormatString;
            this.NonLazyBuilderExtensionsCollectionCopyStatementFormatString = nonLazyBuilderExtensionsCollectionCopyStatementFormatString;
            this.BuilderExtensionsNameFormatString = builderExtensionsNameFormatString;
            this.BuilderExtensionsNamespaceFormatString = builderExtensionsNamespaceFormatString;
            this.BuilderNameFormatString = builderNameFormatString;
            this.BuilderNamespaceFormatString = builderNamespaceFormatString;
            this.BuilderNewCollectionTypeName = builderNewCollectionTypeName;
            this.BuildMethodName = buildMethodName;
            this.BuildTypedMethodName = buildTypedMethodName;
            this.CollectionCopyStatementFormatString = collectionCopyStatementFormatString;
            this.NonLazyCollectionCopyStatementFormatString = nonLazyCollectionCopyStatementFormatString;
            this.CollectionInitializationStatementFormatString = collectionInitializationStatementFormatString;
            this.CollectionTypeName = collectionTypeName;
            this.CopyAttributePredicate = copyAttributePredicate;
            this.CopyAttributes = copyAttributes;
            this.CopyInterfacePredicate = copyInterfacePredicate;
            this.CopyInterfaces = copyInterfaces;
            this.CopyMethodPredicate = copyMethodPredicate;
            this.CopyMethods = copyMethods;
            this.CreateAsObservable = createAsObservable;
            this.CreateAsPartial = createAsPartial;
            this.CreateConstructors = createConstructors;
            this.CreateRecord = createRecord;
            this.EnableBuilderInheritance = enableBuilderInheritance;
            this.EnableInheritance = enableInheritance;
            this.EnableNullableReferenceTypes = enableNullableReferenceTypes;
            this.EntityNameFormatString = entityNameFormatString;
            this.EntityNamespaceFormatString = entityNamespaceFormatString;
            this.EntityNewCollectionTypeName = entityNewCollectionTypeName;
            this.IEquatableItemType = iEquatableItemType;
            this.ImplementIEquatable = implementIEquatable;
            this.InheritanceComparisonDelegate = inheritanceComparisonDelegate;
            this.InheritanceComparisonDelegateForReflection = inheritanceComparisonDelegateForReflection;
            this.InheritFromInterfaces = inheritFromInterfaces;
            this.IsAbstract = isAbstract;
            this.IsForAbstractBuilder = isForAbstractBuilder;
            this.NameFormatString = nameFormatString;
            this.NamespaceFormatString = namespaceFormatString;
            this.NamespaceMappings = namespaceMappings is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Pipelines.NamespaceMapping>(namespaceMappings);
            this.NonCollectionInitializationStatementFormatString = nonCollectionInitializationStatementFormatString;
            this.SetDefaultValuesInEntityConstructor = setDefaultValuesInEntityConstructor;
            this.SetDefaultValuesMethodName = setDefaultValuesMethodName;
            this.SetMethodNameFormatString = setMethodNameFormatString;
            this.SetterVisibility = setterVisibility;
            this.SkipNamespacesOnFluentBuilderMethods = skipNamespacesOnFluentBuilderMethods is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<System.String>(skipNamespacesOnFluentBuilderMethods);
            this.ToBuilderFormatString = toBuilderFormatString;
            this.ToTypedBuilderFormatString = toTypedBuilderFormatString;
            this.TypenameMappings = typenameMappings is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Pipelines.TypenameMapping>(typenameMappings);
            this.UseBaseClassFromSourceModel = useBaseClassFromSourceModel;
            this.UseBuilderAbstractionsTypeConversion = useBuilderAbstractionsTypeConversion;
            this.UseBuilderLazyValues = useBuilderLazyValues;
            this.UseCrossCuttingInterfaces = useCrossCuttingInterfaces;
            this.UseDefaultValueAttributeValuesForBuilderInitialization = useDefaultValueAttributeValuesForBuilderInitialization;
            this.UseExceptionThrowIfNull = useExceptionThrowIfNull;
            this.UsePatternMatchingForNullChecks = usePatternMatchingForNullChecks;
            this.ValidateArguments = validateArguments;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Pipelines.Builders.PipelineSettingsBuilder ToBuilder()
        {
            return new ClassFramework.Pipelines.Builders.PipelineSettingsBuilder(this);
        }
    }
    public partial record TypenameMapping
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string SourceTypeName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string TargetTypeName
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<ClassFramework.Pipelines.Metadata> Metadata
        {
            get;
        }

        public TypenameMapping(string sourceTypeName, string targetTypeName, System.Collections.Generic.IEnumerable<ClassFramework.Pipelines.Metadata> metadata)
        {
            this.SourceTypeName = sourceTypeName;
            this.TargetTypeName = targetTypeName;
            this.Metadata = metadata is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<ClassFramework.Pipelines.Metadata>(metadata);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.Pipelines.Builders.TypenameMappingBuilder ToBuilder()
        {
            return new ClassFramework.Pipelines.Builders.TypenameMappingBuilder(this);
        }
    }
}
#nullable disable
