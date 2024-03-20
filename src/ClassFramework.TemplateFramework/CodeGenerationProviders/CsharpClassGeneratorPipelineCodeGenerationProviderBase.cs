namespace ClassFramework.TemplateFramework.CodeGenerationProviders;

public abstract class CsharpClassGeneratorPipelineCodeGenerationProviderBase : CsharpClassGeneratorCodeGenerationProviderBase
{
    protected CsharpClassGeneratorPipelineCodeGenerationProviderBase(
        ICsharpExpressionDumper csharpExpressionDumper,
        IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline,
        IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline,
        IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline,
        IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline,
        IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline,
        IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper)
    {
        Guard.IsNotNull(builderPipeline);
        Guard.IsNotNull(builderExtensionPipeline);
        Guard.IsNotNull(entityPipeline);
        Guard.IsNotNull(overrideEntityPipeline);
        Guard.IsNotNull(reflectionPipeline);
        Guard.IsNotNull(interfacePipeline);

        _builderPipeline = builderPipeline;
        _builderExtensionPipeline = builderExtensionPipeline;
        _entityPipeline = entityPipeline;
        _overrideEntityPipeline = overrideEntityPipeline;
        _reflectionPipeline = reflectionPipeline;
        _interfacePipeline = interfacePipeline;
    }

    private readonly IPipeline<IConcreteTypeBuilder, BuilderContext> _builderPipeline;
    private readonly IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> _builderExtensionPipeline;
    private readonly IPipeline<IConcreteTypeBuilder, EntityContext> _entityPipeline;
    private readonly IPipeline<IConcreteTypeBuilder, OverrideEntityContext> _overrideEntityPipeline;
    private readonly IPipeline<TypeBaseBuilder, ReflectionContext> _reflectionPipeline;
    private readonly IPipeline<InterfaceBuilder, InterfaceContext> _interfacePipeline;

    public override CsharpClassGeneratorSettings Settings
        => new CsharpClassGeneratorSettingsBuilder()
            .WithPath(Path)
            .WithRecurseOnDeleteGeneratedFiles(RecurseOnDeleteGeneratedFiles)
            .WithLastGeneratedFilesFilename(LastGeneratedFilesFilename)
            .WithEncoding(Encoding)
            .WithCultureInfo(CultureInfo.InvariantCulture)
            .WithGenerateMultipleFiles()
            .WithCreateCodeGenerationHeader(CreateCodeGenerationHeader)
            .WithEnableNullableContext()
            .WithFilenameSuffix(FilenameSuffix)
            .WithEnvironmentVersion(EnvironmentVersion)
            .Build();

    protected abstract string ProjectName { get; }
    protected abstract Type EntityCollectionType { get; }
    protected abstract Type EntityConcreteCollectionType { get; }
    protected abstract Type BuilderCollectionType { get; }

    protected virtual string EnvironmentVersion => string.Empty;
    protected virtual string RootNamespace => InheritFromInterfaces
        ? $"{ProjectName}.Abstractions"
        : CoreNamespace;
    protected virtual string CodeGenerationRootNamespace => $"{ProjectName}.CodeGeneration";
    protected virtual string CoreNamespace => $"{ProjectName}.Core";
    protected virtual bool CopyAttributes => false;
    protected virtual bool CopyInterfaces => false;
    protected virtual bool CopyMethods => false;
    protected virtual bool InheritFromInterfaces => false;
    protected virtual bool AddNullChecks => true;
    protected virtual bool UseExceptionThrowIfNull => false;
    protected virtual bool CreateRecord => false;
    protected virtual bool AddBackingFields => false;
    protected virtual SubVisibility SetterVisibility => SubVisibility.InheritFromParent;
    protected virtual bool AddSetters => false;
    protected virtual bool CreateAsObservable => false;
    protected virtual string? CollectionPropertyGetStatement => null;
    protected virtual ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.DomainOnly;
    protected virtual bool EnableEntityInheritance => false;
    protected virtual bool EnableBuilderInhericance => false;
    protected virtual Class? BaseClass => null;
    protected virtual bool IsAbstract => false;
    protected virtual string BaseClassBuilderNamespace => string.Empty;
    protected virtual bool AllowGenerationWithoutProperties => true;
    protected virtual string SetMethodNameFormatString => "With{Name}";
    protected virtual string AddMethodNameFormatString => "Add{Name}";
    protected virtual string ToBuilderFormatString => "ToBuilder";
    protected virtual string ToTypedBuilderFormatString => "ToTypedBuilder";
    protected virtual string BuildMethodName => "Build";
    protected virtual string BuildTypedMethodName => "BuildTyped";
    protected virtual bool AddFullConstructor => true;
    protected virtual bool AddPublicParameterlessConstructor => false;
    protected virtual bool AddCopyConstructor => true;
    protected virtual bool SetDefaultValues => true;
    protected virtual string FilenameSuffix => ".template.generated";
    protected virtual bool CreateCodeGenerationHeader => true;
    protected virtual Predicate<Domain.Attribute>? CopyAttributePredicate => null;
    protected virtual Predicate<string>? CopyInterfacePredicate => null;
    protected virtual Func<IType, Method, bool>? CopyMethodPredicate => null;
    protected virtual Func<IParentTypeContainer, IType, bool>? InheritanceComparisonDelegate => new Func<IParentTypeContainer, IType, bool>((parentNameContainer, typeBase)
        => parentNameContainer is not null
        && typeBase is not null
        && (string.IsNullOrEmpty(parentNameContainer.ParentTypeFullName)
            || (BaseClass is not null && !BaseClass.Properties.Any(x => x.Name == (parentNameContainer as INameContainer)?.Name))
            || parentNameContainer.ParentTypeFullName.GetClassName().In(typeBase.Name, $"I{typeBase.Name}")
            || Array.Exists(GetModelAbstractBaseTyped(), x => x == parentNameContainer.ParentTypeFullName.GetClassName())
            || (parentNameContainer.ParentTypeFullName.StartsWith($"{RootNamespace}.") && typeBase.Namespace.In(CoreNamespace, $"{RootNamespace}.Builders"))
        ));

    protected virtual string[] GetModelAbstractBaseTyped() => Array.Empty<string>();

    protected virtual string[] GetExternalCustomBuilderTypes() => Array.Empty<string>();

    protected virtual string[] GetCustomBuilderTypes()
        => GetPureAbstractModels()
            .Select(x => x.GetEntityClassName())
            .Concat(GetExternalCustomBuilderTypes())
            .ToArray();

    protected virtual IEnumerable<Type> GetPureAbstractModels()
        => GetType().Assembly.GetTypes().Where(IsAbstractType);

    protected virtual ClassBuilder PostProcessClassBuilder(ClassBuilder builder) => builder;

    /// <summary>
    /// Gets the base typename, based on a derived class.
    /// </summary>
    /// <param name="className">The typename to get the base classname from.</param>
    /// <returns>Base classname when found, otherwise string.Empty</returns>
    protected string GetEntityClassName(string className)
        => Array.Find(GetCustomBuilderTypes(), x => className.EndsWith(x, StringComparison.InvariantCulture)) ?? string.Empty;

    protected TypeBase[] GetEntities(TypeBase[] models, string entitiesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(entitiesNamespace);

        if (ValidateArgumentsInConstructor == ArgumentValidationType.Shared && !(EnableEntityInheritance && BaseClass is null))
        {
            return models.SelectMany
            (
                x => new[]
                {
                    CreateImmutableClass(x, entitiesNamespace),
                    CreateOverrideEntities(x, entitiesNamespace)
                }
            ).ToArray();
        }

        return models.Select
        (
            x => CreateImmutableClass(x, entitiesNamespace)
        ).ToArray();
    }

    protected TypeBase[] GetEntityInterfaces(TypeBase[] models, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return models
            .Select(x => CreateImmutableClass(x, entitiesNamespace))
            .Select(x => CreateInterface(x, interfacesNamespace, string.Empty, true, "I{Class.Name}", (t, m) => InheritFromInterfaces && m.Name == ToBuilderFormatString && t.Interfaces.Count == 0))
            .ToArray();
    }

    protected TypeBase[] GetInterfaces(TypeBase[] models, string interfacesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(interfacesNamespace);

        return models.Select(x => CreateInterface(x, interfacesNamespace, EntityCollectionType.WithoutGenerics(), AddSetters)).ToArray();
    }

    protected TypeBase[] GetBuilders(TypeBase[] models, string buildersNamespace, string entitiesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return models.Select(x =>
        {
            var entityBuilder = new ClassBuilder();
            _ = _entityPipeline
                .Process(entityBuilder, new EntityContext(x, CreateEntityPipelineSettings(entitiesNamespace), CultureInfo.InvariantCulture))
                .GetValueOrThrow();

            return CreateBuilderClass(entityBuilder.Build(), buildersNamespace, entitiesNamespace);
        }).ToArray();
    }

    protected TypeBase[] GetBuilderExtensions(TypeBase[] models, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return models.Select(x =>
        {
            var interfaceBuilder = new InterfaceBuilder();
            _ = _interfacePipeline
                .Process(interfaceBuilder, new InterfaceContext(x, CreateInterfacePipelineSettings(entitiesNamespace, string.Empty, InheritanceComparisonDelegate, null, true), CultureInfo.InvariantCulture))
                .GetValueOrThrow();

            return CreateBuilderExtensionsClass(interfaceBuilder.Build(), buildersNamespace, entitiesNamespace, buildersExtensionsNamespace);
        }).ToArray();
    }

    protected TypeBase[] GetNonGenericBuilders(TypeBase[] models, string buildersNamespace, string entitiesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return models.Select(x =>
        {
            var entityBuilder = new ClassBuilder();
            _ = _entityPipeline
                .Process(entityBuilder, new EntityContext(x, CreateEntityPipelineSettings(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks()), CultureInfo.InvariantCulture))
                .GetValueOrThrow();

            return CreateNonGenericBuilderClass(entityBuilder.Build(), buildersNamespace, entitiesNamespace);
        }).ToArray();
    }

    protected TypeBase[] GetBuilderInterfaces(TypeBase[] models, string buildersNamespace, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return GetBuilders(models, buildersNamespace, entitiesNamespace)
            .Select(x => CreateInterface(x, interfacesNamespace, BuilderCollectionType.WithoutGenerics(), true, "I{Class.Name}", (t, m) => InheritFromInterfaces && m.Name == BuildMethodName && t.Interfaces.Count == 0))
            .ToArray();
    }

    protected TypeBase[] GetCoreModels()
        => GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .Select(x => _reflectionPipeline.Process(new InterfaceBuilder(), new ReflectionContext(x, CreateReflectionPipelineSettings(), CultureInfo.InvariantCulture)).GetValueOrThrow().Build())
            .ToArray();

    protected TypeBase[] GetNonCoreModels(string @namespace)
        => GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == @namespace && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .Select(x => _reflectionPipeline.Process(new InterfaceBuilder(), new ReflectionContext(x, CreateReflectionPipelineSettings(), CultureInfo.InvariantCulture)).GetValueOrThrow().Build())
            .ToArray();

    protected TypeBase[] GetAbstractionsInterfaces()
        => GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions")
            .Select(x => _reflectionPipeline.Process(new InterfaceBuilder(), new ReflectionContext(x, CreateReflectionPipelineSettings(), CultureInfo.InvariantCulture)).GetValueOrThrow().Build())
            .ToArray();

    protected TypeBase[] GetAbstractModels()
        => GetPureAbstractModels()
            .Select(x => _reflectionPipeline.Process(new InterfaceBuilder(), new ReflectionContext(x, CreateReflectionPipelineSettings(), CultureInfo.InvariantCulture)).GetValueOrThrow().Build())
            .ToArray();

    protected TypeBase[] GetOverrideModels(Type abstractType)
    {
        Guard.IsNotNull(abstractType);

        return GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && Array.Exists(x.GetInterfaces(), y => y == abstractType))
            .Select(x => _reflectionPipeline.Process(new InterfaceBuilder(), new ReflectionContext(x, CreateReflectionPipelineSettings(), CultureInfo.InvariantCulture)).GetValueOrThrow().Build())
            .ToArray();
    }

    protected Class CreateBaseclass(Type type, string @namespace)
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(@namespace);

        var reflectionSettings = CreateReflectionPipelineSettings();
        var typeBase = _reflectionPipeline.Process(new InterfaceBuilder(), new ReflectionContext(type, reflectionSettings, CultureInfo.InvariantCulture)).GetValueOrThrow().Build();

        var builder = new ClassBuilder();
        var entitySettings = new PipelineSettingsBuilder()
            .WithAddSetters(AddSetters)
            .WithAddBackingFields(AddBackingFields)
            .WithSetterVisibility(SetterVisibility)
            .WithCreateAsObservable(CreateAsObservable)
            .WithCreateRecord(CreateRecord)
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods)
            .WithInheritFromInterfaces(InheritFromInterfaces)
            .WithCopyAttributePredicate(CopyAttributePredicate)
            .WithCopyInterfacePredicate(CopyInterfacePredicate)
            .WithCopyMethodPredicate(CopyMethodPredicate)
            .WithEntityNameFormatString("{Class.NameNoInterfacePrefix}")
            .WithEntityNamespaceFormatString(@namespace)
            .WithEnableInheritance()
            .WithIsAbstract()
            .WithBaseClass(null)
            .WithInheritanceComparisonDelegate(
                (parentNameContainer, typeBase)
                    => parentNameContainer is not null
                    && typeBase is not null
                    && (string.IsNullOrEmpty(parentNameContainer.ParentTypeFullName)
                        || parentNameContainer.ParentTypeFullName.GetClassName().In(typeBase.Name, $"I{typeBase.Name}")
                        || Array.Exists(GetModelAbstractBaseTyped(), x => x == parentNameContainer.ParentTypeFullName.GetClassName())
                        || (parentNameContainer.ParentTypeFullName.StartsWith($"{RootNamespace}.") && typeBase.Namespace.In(CoreNamespace, $"{RootNamespace}.Builders"))
                    ))
            .WithEntityNewCollectionTypeName(EntityCollectionType.WithoutGenerics())
            .WithEnableNullableReferenceTypes()
            .AddTypenameMappings(CreateTypenameMappings())
            .AddNamespaceMappings(CreateNamespaceMappings())
            .WithValidateArguments(ValidateArgumentsInConstructor)
            .WithAddNullChecks(AddNullChecks)
            .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull)
            .Build();

        _ = _entityPipeline.Process(builder, new EntityContext(typeBase, entitySettings, CultureInfo.InvariantCulture)).GetValueOrThrow();
        
        return PostProcessClassBuilder(builder).BuildTyped();
    }

    protected ArgumentValidationType CombineValidateArguments(ArgumentValidationType validateArgumentsInConstructor, bool secondCondition)
        => secondCondition
            ? validateArgumentsInConstructor
            : ArgumentValidationType.None;

    private PipelineSettings CreateReflectionPipelineSettings()
        => new PipelineSettingsBuilder()
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods)
            .WithInheritFromInterfaces(InheritFromInterfaces)
            .WithCopyAttributePredicate(CopyAttributePredicate)
            .WithCopyInterfacePredicate(CopyInterfacePredicate)
            .WithCopyMethodPredicate(CopyMethodPredicate)
            .AddNamespaceMappings(CreateNamespaceMappings())
            .AddTypenameMappings(CreateTypenameMappings())
            .Build();

    private PipelineSettings CreateEntityPipelineSettings(
        string entitiesNamespace,
        ArgumentValidationType? forceValidateArgumentsInConstructor = null,
        bool? overrideAddNullChecks = null,
        string entityNameFormatString = "{Class.NameNoInterfacePrefix}")
        => new PipelineSettingsBuilder()
            .WithAddSetters(AddSetters)
            .WithAddBackingFields(AddBackingFields)
            .WithSetterVisibility(SetterVisibility)
            .WithCreateAsObservable(CreateAsObservable)
            .WithCreateRecord(CreateRecord)
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods)
            .WithInheritFromInterfaces(InheritFromInterfaces)
            .WithCopyAttributePredicate(CopyAttributePredicate)
            .WithCopyInterfacePredicate(CopyInterfacePredicate)
            .WithCopyMethodPredicate(CopyMethodPredicate)
            .WithEntityNameFormatString(entityNameFormatString)
            .WithEntityNamespaceFormatString(entitiesNamespace)
            .WithToBuilderFormatString(ToBuilderFormatString)
            .WithToTypedBuilderFormatString(ToTypedBuilderFormatString)
            .WithBuildMethodName(BuildMethodName)
            .WithBuildTypedMethodName(BuildTypedMethodName)
            .WithEnableInheritance(EnableEntityInheritance)
            .WithIsAbstract(IsAbstract)
            .WithBaseClass(BaseClass?.ToBuilder())
            .WithInheritanceComparisonDelegate(InheritanceComparisonDelegate)
            .WithEntityNewCollectionTypeName(EntityCollectionType.WithoutGenerics())
            .WithEnableNullableReferenceTypes()
            .AddTypenameMappings(CreateTypenameMappings())
            .AddNamespaceMappings(CreateNamespaceMappings())
            .WithValidateArguments(forceValidateArgumentsInConstructor ?? CombineValidateArguments(ValidateArgumentsInConstructor, !(EnableEntityInheritance && BaseClass is null)))
            .WithOriginalValidateArguments(ValidateArgumentsInConstructor)
            .WithCollectionTypeName(EntityConcreteCollectionType.WithoutGenerics())
            .WithAddFullConstructor(AddFullConstructor)
            .WithAddPublicParameterlessConstructor(AddPublicParameterlessConstructor)
            .WithAddNullChecks(forceValidateArgumentsInConstructor != ArgumentValidationType.Shared && (overrideAddNullChecks ?? false))
            .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull)
            .Build();

    private PipelineSettings CreateOverrideEntityPipelineSettings(
        string entitiesNamespace,
        ArgumentValidationType? forceValidateArgumentsInConstructor = null,
        bool? overrideAddNullChecks = null,
        string entityNameFormatString = "{Class.NameNoInterfacePrefix}")
        => new PipelineSettingsBuilder()
            .WithCreateRecord(CreateRecord)
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .WithEntityNameFormatString(entityNameFormatString)
            .WithEntityNamespaceFormatString(entitiesNamespace)
            .WithEnableInheritance(EnableEntityInheritance)
            .WithIsAbstract(IsAbstract)
            .WithBaseClass(BaseClass?.ToBuilder())
            .WithInheritanceComparisonDelegate(InheritanceComparisonDelegate)
            .WithEntityNewCollectionTypeName(EntityCollectionType.WithoutGenerics())
            .WithEnableNullableReferenceTypes()
            .AddTypenameMappings(CreateTypenameMappings())
            .AddNamespaceMappings(CreateNamespaceMappings())
            .WithAddNullChecks(forceValidateArgumentsInConstructor != ArgumentValidationType.Shared && (overrideAddNullChecks ?? false))
            .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull)
            .Build();

    private PipelineSettings CreateInterfacePipelineSettings(
        string interfacesNamespace,
        string newCollectionTypeName,
        Func<IParentTypeContainer, IType, bool>? inheritanceComparisonDelegate,
        Func<IType, Method, bool>? copyMethodPredicate,
        bool addSetters,
        string nameFormatString = "{Class.Name}")
        => new PipelineSettingsBuilder()
            .WithNamespaceFormatString(interfacesNamespace)
            .WithNameFormatString(nameFormatString)
            .WithEnableInheritance(EnableEntityInheritance)
            .WithIsAbstract(IsAbstract)
            .WithBaseClass(BaseClass?.ToBuilder())
            .WithInheritanceComparisonDelegate(inheritanceComparisonDelegate)
            .WithEntityNewCollectionTypeName(newCollectionTypeName)
            .AddTypenameMappings(CreateTypenameMappings())
            .AddNamespaceMappings(CreateNamespaceMappings())
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods || copyMethodPredicate != null)
            .WithInheritFromInterfaces(InheritFromInterfaces)
            .WithCopyAttributePredicate(CopyAttributePredicate)
            .WithCopyInterfacePredicate(CopyInterfacePredicate)
            .WithCopyMethodPredicate(copyMethodPredicate ?? CopyMethodPredicate)
            .WithAddSetters(addSetters)
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .Build();

    protected virtual IEnumerable<NamespaceMappingBuilder> CreateNamespaceMappings()
    {
        // From models to domain entities
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models").WithTargetNamespace(CoreNamespace);
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models.Domains").WithTargetNamespace($"{RootNamespace}.Domains");
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models.Abstractions").WithTargetNamespace($"{CoreNamespace}.Abstractions");

        // From domain entities to builders
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CoreNamespace}.Abstractions").WithTargetNamespace($"{CoreNamespace}.Abstractions")
            .AddMetadata
            (
                new MetadataBuilder().WithValue(InheritFromInterfaces ? $"{RootNamespace}.Builders" : $"{CoreNamespace}.Builders.Abstractions").WithName(Pipelines.MetadataNames.CustomBuilderInterfaceNamespace),
                new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderInterfaceName),
                new MetadataBuilder().WithValue(InheritFromInterfaces ? $"{RootNamespace}.Builders" : $"{CoreNamespace}.Builders.Abstractions").WithName(Pipelines.MetadataNames.CustomBuilderParentTypeNamespace),
                new MetadataBuilder().WithValue("{ParentTypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderParentTypeName)
            );

        foreach (var entityClassName in GetPureAbstractModels().Select(x => x.GetEntityClassName().ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal)))
        {
            yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models.{entityClassName}s").WithTargetNamespace($"{CoreNamespace}.{entityClassName}s");
            yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CoreNamespace}.{entityClassName}s").WithTargetNamespace($"{CoreNamespace}.{entityClassName}s");
        }
    }

    protected virtual IEnumerable<TypenameMappingBuilder> CreateTypenameMappings()
        => GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface
                && x.Namespace?.StartsWith($"{CodeGenerationRootNamespace}.Models", StringComparison.Ordinal) == true
                && x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions"
                && x.Namespace != $"{CodeGenerationRootNamespace}.Models.Domains"
                && !SkipNamespaceOnTypenameMappings(x.Namespace)
                && x.FullName is not null)
            .SelectMany(x =>
                new[]
                {
                    new TypenameMappingBuilder().WithSourceType(x).WithTargetTypeName($"{CoreNamespace}.{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", true)}{x.GetEntityClassName()}"),
                    new TypenameMappingBuilder().WithSourceTypeName($"{CoreNamespace}.{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", true)}{x.GetEntityClassName()}").WithTargetTypeName($"{CoreNamespace}.{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", true)}{x.GetEntityClassName()}")
                        .AddMetadata
                        (
                            new MetadataBuilder().WithValue($"{CoreNamespace}.Builders{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", false)}").WithName(Pipelines.MetadataNames.CustomBuilderNamespace),
                            new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderName),
                            new MetadataBuilder().WithValue($"{ProjectName}.Abstractions.Builders").WithName(Pipelines.MetadataNames.CustomBuilderInterfaceNamespace),
                            new MetadataBuilder().WithValue("I{TypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderInterfaceName),
                            new MetadataBuilder().WithValue(x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && Array.Exists(x.GetInterfaces(), IsAbstractType)
                                ? $"new {CoreNamespace}.Builders{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder([Name])"
                                : "[Name][NullableSuffix].ToBuilder()").WithName(Pipelines.MetadataNames.CustomBuilderSourceExpression),
                                //: $"builderFactory.Create<{CoreNamespace}.Builders.{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", true)}{x.GetEntityClassName()}Builder>([Name])", Pipelines.MetadataNames.CustomBuilderSourceExpression),
                            new MetadataBuilder().WithValue(new Literal($"new {CoreNamespace}.Builders{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder()", null)).WithName(Pipelines.MetadataNames.CustomBuilderDefaultValue),
                            new MetadataBuilder().WithValue(x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && Array.Exists(x.GetInterfaces(), IsAbstractType)
                                ? "[Name][NullableSuffix].BuildTyped()"
                                : "[Name][NullableSuffix].Build()").WithName(Pipelines.MetadataNames.CustomBuilderMethodParameterExpression),
                            new MetadataBuilder().WithName(Pipelines.MetadataNames.CustomEntityInterfaceTypeName).WithValue($"{ProjectName}.Abstractions.I{x.GetEntityClassName()}")
                            //new MetadataBuilder().WithValue(new ParameterBuilder().WithName("builderFactory").WithTypeName($"{CoreNamespace}.Builders.Abstractions.IBuilderFactory").Build()).WithName(Pipelines.MetadataNames.CustomBuilderCopyConstructorParameter),
                        )
                })
            .Concat(
            [
                new TypenameMappingBuilder().WithSourceType(typeof(bool)).WithTargetType(typeof(bool)).AddMetadata(new MetadataBuilder().WithValue(true).WithName(Pipelines.MetadataNames.CustomBuilderWithDefaultPropertyValue)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(List<>).WithoutGenerics()).WithTargetTypeName(typeof(List<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList()").WithName(Pipelines.MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(Collection<>).WithoutGenerics()).WithTargetTypeName(typeof(Collection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("new [Type]<[Generics]>([Expression].ToList())").WithName(Pipelines.MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(ObservableCollection<>).WithoutGenerics()).WithTargetTypeName(typeof(ObservableCollection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("new [Type]<[Generics]>([Expression])").WithName(Pipelines.MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(IReadOnlyCollection<>).WithoutGenerics()).WithTargetTypeName(typeof(IReadOnlyCollection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList().AsReadOnly()").WithName(Pipelines.MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(IList<>).WithoutGenerics()).WithTargetTypeName(typeof(IList<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList()").WithName(Pipelines.MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(ICollection<>).WithoutGenerics()).WithTargetTypeName(typeof(ICollection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList()").WithName(Pipelines.MetadataNames.CustomCollectionInitialization)),
            ]);

    protected virtual bool IsAbstractType(Type type)
    {
        type = type.IsNotNull(nameof(type));
        return type.IsInterface && type.Namespace == $"{CodeGenerationRootNamespace}.Models" && type.Name.EndsWith("Base");
    }

    protected virtual bool SkipNamespaceOnTypenameMappings(string @namespace) => false;

    private string ReplaceStart(string fullNamespace, string baseNamespace, bool appendDot)
    {
        if (fullNamespace.Length == 0)
        {
            return fullNamespace;
        }

        if (fullNamespace.StartsWith($"{baseNamespace}."))
        {
            return appendDot
                ? string.Concat(fullNamespace.AsSpan(baseNamespace.Length + 1), ".")
                : string.Concat(".", fullNamespace.AsSpan(baseNamespace.Length + 1));
        }

        return string.Empty;
    }

    private PipelineSettings CreateBuilderPipelineSettings(string buildersNamespace, string entitiesNamespace)
        => new PipelineSettingsBuilder(CreateEntityPipelineSettings(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks()))
            .WithBuilderNewCollectionTypeName(BuilderCollectionType.WithoutGenerics())
            .WithBuilderNamespaceFormatString(buildersNamespace)
            .WithSetMethodNameFormatString(SetMethodNameFormatString)
            .WithAddMethodNameFormatString(AddMethodNameFormatString)
            .WithEnableBuilderInheritance(EnableBuilderInhericance)
            .WithBaseClassBuilderNameSpace(BaseClassBuilderNamespace)
            .WithAddCopyConstructor(AddCopyConstructor)
            .WithSetDefaultValuesInEntityConstructor(SetDefaultValues)
            .Build();

    private PipelineSettings CreateBuilderInterfacePipelineSettings(string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
        => new PipelineSettingsBuilder(CreateEntityPipelineSettings(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks()))
            .WithBuilderNewCollectionTypeName(BuilderCollectionType.WithoutGenerics())
            .WithBuilderNamespaceFormatString(buildersNamespace)
            .WithBuilderExtensionsNamespaceFormatString(buildersExtensionsNamespace)
            .WithSetMethodNameFormatString(SetMethodNameFormatString)
            .WithAddMethodNameFormatString(AddMethodNameFormatString)
            .WithEnableBuilderInheritance(EnableBuilderInhericance)
            .Build();

    private TypeBase CreateImmutableClass(TypeBase typeBase, string entitiesNamespace)
    {
        var builder = new ClassBuilder();
        _ = _entityPipeline
            .Process(builder, new EntityContext(typeBase, CreateEntityPipelineSettings(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks(), entityNameFormatString: "{Class.NameNoInterfacePrefix}{EntityNameSuffix}"), CultureInfo.InvariantCulture))
            .GetValueOrThrow();

        return PostProcessClassBuilder(builder).Build();
    }

    private TypeBase CreateBuilderClass(TypeBase typeBase, string buildersNamespace, string entitiesNamespace)
    {
        var builder = new ClassBuilder();
        _ = _builderPipeline
            .Process(builder, new BuilderContext(typeBase, CreateBuilderPipelineSettings(buildersNamespace, entitiesNamespace), CultureInfo.InvariantCulture))
            .GetValueOrThrow();

        return PostProcessClassBuilder(builder).Build();
    }

    private TypeBase CreateBuilderExtensionsClass(TypeBase typeBase, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
    {
        var builder = new ClassBuilder();
        _ = _builderExtensionPipeline
            .Process(builder, new BuilderExtensionContext(typeBase, CreateBuilderInterfacePipelineSettings(buildersNamespace, entitiesNamespace, buildersExtensionsNamespace), CultureInfo.InvariantCulture))
            .GetValueOrThrow();

        return builder.Build();
    }

    private TypeBase CreateNonGenericBuilderClass(TypeBase typeBase, string buildersNamespace, string entitiesNamespace)
    {
        var builder = new ClassBuilder();
        _ = _builderPipeline
            .Process(builder, new BuilderContext(typeBase, CreateBuilderPipelineSettings(buildersNamespace, entitiesNamespace).ToBuilder().WithIsForAbstractBuilder().Build(), CultureInfo.InvariantCulture))
            .GetValueOrThrow();

        return PostProcessClassBuilder(builder).Build();
    }

    private bool? GetOverrideAddNullChecks()
    {
        if (AddNullChecks || ValidateArgumentsInConstructor == ArgumentValidationType.None)
        {
            return true;
        }

        return null;
    }

    private TypeBase CreateInterface(
        TypeBase typeBase,
        string interfacesNamespace,
        string newCollectionTypeName,
        bool addSetters,
        string nameFormatString = "{Class.Name}",
        Func<IType, Method, bool>? copyMethodPredicate = null)
    {
        var builder = new InterfaceBuilder();
        _ = _interfacePipeline
            .Process(builder, new InterfaceContext(typeBase, CreateInterfacePipelineSettings(interfacesNamespace, newCollectionTypeName, InheritanceComparisonDelegate, copyMethodPredicate, addSetters, nameFormatString), CultureInfo.InvariantCulture))
            .GetValueOrThrow();

        return builder.Build();
    }

    private TypeBase CreateOverrideEntities(TypeBase typeBase, string entitiesNamespace)
    {
        var builder = new ClassBuilder();
        _ = _overrideEntityPipeline
            .Process(builder, new OverrideEntityContext(typeBase, CreateOverrideEntityPipelineSettings(entitiesNamespace, overrideAddNullChecks: true), CultureInfo.InvariantCulture))
            .GetValueOrThrow();

        return PostProcessClassBuilder(builder).Build();
    }
}
