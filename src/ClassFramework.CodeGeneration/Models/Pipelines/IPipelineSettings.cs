namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface IPipelineSettings
{
    bool AddBackingFields { get; }
    [DefaultValue(true)] bool AddCopyConstructor { get; }
    bool AddFullConstructor { get; }
    [DefaultValue(true)] bool AddImplicitOperatorOnBuilder { get; }
    [DefaultValue(true)] bool AddImplicitOperatorOnEntity { get; }
    [Required(AllowEmptyStrings = true)] string AddMethodNameFormatString { get; }
    bool AddNullChecks { get; }
    bool AddPublicParameterlessConstructor { get; }
    bool AddSetters { get; }
    bool AllowGenerationWithoutProperties { get; }
    [Required] IReadOnlyCollection<AttributeInitializerDelegate> AttributeInitializers { get; }
    [ValidateObject] ITypeBase? BaseClass { get; }
    [Required(AllowEmptyStrings = true)] string BaseClassBuilderNameSpace { get; }
    [Required(AllowEmptyStrings = true)] string BuilderAbstractionsTypeConversionMetadataName { get; }
    [Required] IReadOnlyCollection<string> BuilderAbstractionsTypeConversionNamespaces { get; }
    [Required(AllowEmptyStrings = true)] string BuilderExtensionsCollectionCopyStatementFormatString { get; }
    [Required(AllowEmptyStrings = true)] string NonLazyBuilderExtensionsCollectionCopyStatementFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderExtensionsNameFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderExtensionsNamespaceFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderNameFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderNamespaceFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderNewCollectionTypeName { get; }
    [Required(AllowEmptyStrings = true)] [DefaultValue("Build")] string BuildMethodName { get; }
    [Required(AllowEmptyStrings = true)] [DefaultValue("BuildTyped")] string BuildTypedMethodName { get; }
    [Required(AllowEmptyStrings = true)] string CollectionCopyStatementFormatString { get; }
    [Required(AllowEmptyStrings = true)] string NonLazyCollectionCopyStatementFormatString { get; }
    [Required(AllowEmptyStrings = true)] string CollectionInitializationStatementFormatString { get; }
    [Required(AllowEmptyStrings = true)] string CollectionTypeName { get; }
    Predicate<IAttribute>? CopyAttributePredicate { get; }
    bool CopyAttributes { get; }
    Predicate<string>? CopyInterfacePredicate { get; }
    bool CopyInterfaces { get; }
    CopyMethodPredicate? CopyMethodPredicate { get; }
    bool CopyMethods { get; }
    bool CreateAsObservable { get; }
    [DefaultValue(true)] bool CreateAsPartial { get; }
    [DefaultValue(true)] bool CreateConstructors { get; }
    bool CreateRecord { get; }
    bool EnableBuilderInheritance { get; }
    bool EnableInheritance { get; }
    bool EnableNullableReferenceTypes { get; }
    [Required(AllowEmptyStrings = true)] string EntityNameFormatString { get; }
    [Required(AllowEmptyStrings = true)] string EntityNamespaceFormatString { get; }
    [Required(AllowEmptyStrings = true)] string EntityNewCollectionTypeName { get; }
    IEquatableItemType IEquatableItemType { get; }
    bool ImplementIEquatable { get; }
    InheritanceComparisonDelegate? InheritanceComparisonDelegate { get; }
    ReflectionInheritanceComparisonDelegate? InheritanceComparisonDelegateForReflection { get; }
    bool InheritFromInterfaces { get; }
    bool IsAbstract { get; }
    bool IsForAbstractBuilder { get; }
    [Required(AllowEmptyStrings = true)] string NameFormatString { get; }
    [Required(AllowEmptyStrings = true)] string NamespaceFormatString { get; }
    [Required] IReadOnlyCollection<INamespaceMapping> NamespaceMappings { get; }
    [Required(AllowEmptyStrings = true)] string NonCollectionInitializationStatementFormatString { get; }
    [DefaultValue(true)] bool SetDefaultValuesInEntityConstructor { get; }
    [Required(AllowEmptyStrings = true)] [DefaultValue("SetDefaultValues")] string SetDefaultValuesMethodName { get; }
    [Required(AllowEmptyStrings = true)] string SetMethodNameFormatString { get; }
    SubVisibility SetterVisibility { get; }
    [Required] IReadOnlyCollection<string> SkipNamespacesOnFluentBuilderMethods { get; }
    [Required(AllowEmptyStrings = true)] [DefaultValue("ToBuilder")] string ToBuilderFormatString { get; }
    [Required(AllowEmptyStrings = true)] [DefaultValue("ToTypedBuilder")] string ToTypedBuilderFormatString { get; }
    [Required] IReadOnlyCollection<ITypenameMapping> TypenameMappings { get; }
    [DefaultValue(true)] bool UseBaseClassFromSourceModel { get; }
    [DefaultValue(true)] bool UseBuilderAbstractionsTypeConversion { get; }
    bool UseBuilderLazyValues { get; }
    bool UseCrossCuttingInterfaces { get; }
    [DefaultValue(true)] bool UseDefaultValueAttributeValuesForBuilderInitialization { get; }
    bool UseExceptionThrowIfNull { get; }
    [DefaultValue(true)] bool UsePatternMatchingForNullChecks { get; }
    [DefaultValue(true)] bool AddProperties { get; }
    ArgumentValidationType ValidateArguments { get; }
}
