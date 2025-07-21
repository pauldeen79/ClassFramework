namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface IPipelineSettings
{
    bool AddBackingFields { get; }
    bool AddCopyConstructor { get; }
    bool AddFullConstructor { get; }
    [DefaultValue(true)] bool AddImplicitOperatorOnBuilder { get; }
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
    [Required(AllowEmptyStrings = true)] string BuilderExtensionsNameFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderExtensionsNamespaceFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderNameFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderNamespaceFormatString { get; }
    [Required(AllowEmptyStrings = true)] string BuilderNewCollectionTypeName { get; }
    [Required(AllowEmptyStrings = true)] string BuildMethodName { get; }
    [Required(AllowEmptyStrings = true)] string BuildTypedMethodName { get; }
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
    bool CreateAsPartial { get; }
    bool CreateConstructors { get; }
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
    bool SetDefaultValuesInEntityConstructor { get; }
    [Required(AllowEmptyStrings = true)] string SetDefaultValuesMethodName { get; }
    [Required(AllowEmptyStrings = true)] string SetMethodNameFormatString { get; }
    SubVisibility SetterVisibility { get; }
    [Required] IReadOnlyCollection<string> SkipNamespacesOnFluentBuilderMethods { get; }
    [Required(AllowEmptyStrings = true)] string ToBuilderFormatString { get; }
    [Required(AllowEmptyStrings = true)] string ToTypedBuilderFormatString { get; }
    [Required] IReadOnlyCollection<ITypenameMapping> TypenameMappings { get; }
    bool UseBaseClassFromSourceModel { get; }
    [DefaultValue(true)] bool UseBuilderAbstractionsTypeConversion { get; }
    bool UseBuilderLazyValues { get; }
    bool UseCrossCuttingInterfaces { get; }
    bool UseDefaultValueAttributeValuesForBuilderInitialization { get; }
    bool UseExceptionThrowIfNull { get; }
    [DefaultValue(true)] bool UsePatternMatchingForNullChecks { get; }
    ArgumentValidationType ValidateArguments { get; }
}
