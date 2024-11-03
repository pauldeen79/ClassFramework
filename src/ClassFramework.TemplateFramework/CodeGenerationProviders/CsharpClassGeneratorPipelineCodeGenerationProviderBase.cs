namespace ClassFramework.TemplateFramework.CodeGenerationProviders;

public abstract class CsharpClassGeneratorPipelineCodeGenerationProviderBase : CsharpClassGeneratorCodeGenerationProviderBase
{
    protected CsharpClassGeneratorPipelineCodeGenerationProviderBase(IPipelineService pipelineService)
    {
        Guard.IsNotNull(pipelineService);

        PipelineService = pipelineService;
    }

    protected IPipelineService PipelineService { get; }

    public override CsharpClassGeneratorSettings Settings
        => new CsharpClassGeneratorSettingsBuilder()
            .WithPath(Path)
            .WithRecurseOnDeleteGeneratedFiles(RecurseOnDeleteGeneratedFiles)
            .WithLastGeneratedFilesFilename(LastGeneratedFilesFilename)
            .WithEncoding(Encoding)
            .WithCultureInfo(CultureInfo)
            .WithGenerateMultipleFiles(GenerateMultipleFiles)
            .WithCreateCodeGenerationHeader(CreateCodeGenerationHeader)
            .WithEnableNullableContext(EnableNullableContext)
            .WithEnableNullablePragmas(EnableNullablePragmas)
            .WithEnableGlobalUsings(EnableGlobalUsings)
            .WithFilenameSuffix(FilenameSuffix)
            .WithEnvironmentVersion(EnvironmentVersion)
            .WithSkipWhenFileExists(SkipWhenFileExists)
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
    protected virtual ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.IValidatableObject;
    protected virtual bool EnableEntityInheritance => false;
    protected virtual bool EnableBuilderInhericance => false;
    protected virtual Task<TypeBase?> GetBaseClass() => Task.FromResult(default(TypeBase?));
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
    protected virtual bool SkipWhenFileExists => false;
    protected virtual bool GenerateMultipleFiles => true;
    protected virtual bool EnableNullableContext => true;
    protected virtual bool EnableNullablePragmas => true;
    protected virtual bool EnableGlobalUsings => false;
    protected virtual CultureInfo CultureInfo => CultureInfo.InvariantCulture;
    protected virtual Predicate<Domain.Attribute>? CopyAttributePredicate => null;
    protected virtual Predicate<string>? CopyInterfacePredicate => null;
    protected virtual CopyMethodPredicate? CopyMethodPredicate => null;
    protected virtual InheritanceComparisonDelegate? CreateInheritanceComparisonDelegate(TypeBase? baseClass) => (parentNameContainer, typeBase)
        => parentNameContainer is not null
            && typeBase is not null
            && (string.IsNullOrEmpty(parentNameContainer.ParentTypeFullName)
                || (baseClass is not null && !baseClass.Properties.Any(x => x.Name == (parentNameContainer as INameContainer)?.Name))
                || parentNameContainer.ParentTypeFullName.GetClassName().In(typeBase.Name, $"I{typeBase.Name}")
                || Array.Exists(GetModelAbstractBaseTyped(), x => x == parentNameContainer.ParentTypeFullName.GetClassName())
                || (parentNameContainer.ParentTypeFullName.StartsWith($"{RootNamespace}.") && typeBase.Namespace.In(CoreNamespace, $"{RootNamespace}.Builders"))
            );

    protected virtual string[] GetModelAbstractBaseTyped() => Array.Empty<string>();

    protected virtual string[] GetExternalCustomBuilderTypes() => Array.Empty<string>();

    protected virtual string[] GetCustomBuilderTypes()
        => GetPureAbstractModels()
            .Select(x => x.GetEntityClassName())
            .Concat(GetExternalCustomBuilderTypes())
            .ToArray();

    protected virtual IEnumerable<Type> GetPureAbstractModels()
        => GetType().Assembly.GetTypes().Where(IsAbstractType);

    /// <summary>
    /// Gets the base typename, based on a derived class.
    /// </summary>
    /// <param name="className">The typename to get the base classname from.</param>
    /// <returns>Base classname when found, otherwise string.Empty</returns>
    protected string GetEntityClassName(string className)
        => Array.Find(GetCustomBuilderTypes(), x => className.EndsWith(x, StringComparison.InvariantCulture)) ?? string.Empty;

    protected async Task<TypeBase[]> GetEntities(TypeBase[] models, string entitiesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(entitiesNamespace);

        return (await models.SelectAsync(x => CreateEntity(x, entitiesNamespace))).ToArray();
    }

    protected async Task<TypeBase[]> GetEntityInterfaces(TypeBase[] models, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return (await models.SelectAsync(async x => await CreateInterface(await CreateEntity(x, entitiesNamespace), interfacesNamespace, string.Empty, true, "I{Class.Name}", (t, m) => InheritFromInterfaces && m.Name == ToBuilderFormatString && t.Interfaces.Count == 0))).ToArray();
    }

    protected async Task<TypeBase[]> GetBuilders(TypeBase[] models, string buildersNamespace, string entitiesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return (await models.SelectAsync(async x =>
        {
            var context = new EntityContext(x, await CreateEntityPipelineSettings(entitiesNamespace).ConfigureAwait(false), Settings.CultureInfo);
            var entity = (await PipelineService.Process(context))
                .GetValueOrThrow();

            return await CreateBuilderClass(entity, buildersNamespace, entitiesNamespace).ConfigureAwait(false);
        })).ToArray();
    }

    protected async Task<TypeBase[]> GetBuilderExtensions(TypeBase[] models, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(buildersExtensionsNamespace);

        return (await models.SelectAsync(async x =>
        {
            var context = new InterfaceContext(x, await CreateInterfacePipelineSettings(entitiesNamespace, string.Empty, CreateInheritanceComparisonDelegate(await GetBaseClass()), null, true), Settings.CultureInfo);

            var @interface = (await PipelineService.Process(context))
                .GetValueOrThrow();

            return await CreateBuilderExtensionsClass(@interface, buildersNamespace, entitiesNamespace, buildersExtensionsNamespace).ConfigureAwait(false);
        })).ToArray();
    }

    protected async Task<TypeBase[]> GetNonGenericBuilders(TypeBase[] models, string buildersNamespace, string entitiesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return (await models.SelectAsync(async x =>
        {
            var context = new EntityContext(x, await CreateEntityPipelineSettings(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks()), Settings.CultureInfo);

            var typeBase = (await PipelineService.Process(context))
                .GetValueOrThrow();

            return await CreateNonGenericBuilderClass(typeBase, buildersNamespace, entitiesNamespace).ConfigureAwait(false);
        })).ToArray();
    }

    protected async Task<TypeBase[]> GetBuilderInterfaces(TypeBase[] models, string buildersNamespace, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(models);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return (await (await GetBuilders(models, buildersNamespace, entitiesNamespace))
                .SelectAsync(async x => await CreateInterface(x, interfacesNamespace, BuilderCollectionType.WithoutGenerics(), true, "I{Class.Name}", (t, m) => InheritFromInterfaces && m.Name == BuildMethodName && t.Interfaces.Count == 0))
               ).ToArray();
    }

    protected async Task<TypeBase[]> GetCoreModels()
        => (await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectAsync(GetModel)
           ).ToArray();

    protected async Task<TypeBase[]> GetNonCoreModels(string @namespace)
        => (await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == @namespace && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectAsync(GetModel)
           ).ToArray();

    protected async Task<TypeBase[]> GetAbstractionsInterfaces()
        => (await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions")
            .SelectAsync(GetModel)
           ).ToArray();

    protected async Task<TypeBase[]> GetAbstractModels()
        => (await GetPureAbstractModels()
            .SelectAsync(GetModel)
           ).ToArray();

    protected async Task<TypeBase[]> GetOverrideModels(Type abstractType)
    {
        Guard.IsNotNull(abstractType);

        return (await GetType().Assembly.GetTypes()
                .Where(x => x.IsInterface && Array.Exists(x.GetInterfaces(), y => y == abstractType))
                .SelectAsync(GetModel)
               ).ToArray();
    }

    protected async Task<TypeBase> CreateBaseClass(Type type, string @namespace)
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(@namespace);

        var reflectionSettings = CreateReflectionPipelineSettings();
        var typeBase = (await PipelineService.Process(new ReflectionContext(type, reflectionSettings, Settings.CultureInfo)))
            .GetValueOrThrow();

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
            .WithCopyAttributePredicate(CopyAttributePredicate ?? DefaultCopyAttributePredicate)
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

        return (await PipelineService.Process(new EntityContext(typeBase, entitySettings, Settings.CultureInfo)))
            .GetValueOrThrow();
    }

    private static bool DefaultCopyAttributePredicate(Domain.Attribute attribute)
        => attribute.Name != typeof(CsharpTypeNameAttribute).FullName;

    protected static ArgumentValidationType CombineValidateArguments(ArgumentValidationType validateArgumentsInConstructor, bool secondCondition)
        => secondCondition
            ? validateArgumentsInConstructor
            : ArgumentValidationType.None;

    protected virtual PipelineSettings CreateReflectionPipelineSettings()
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
            .AddAttributeInitializers(x => x is CsharpTypeNameAttribute csharpTypeNameAttribute
                ? new AttributeBuilder().WithName(csharpTypeNameAttribute.GetType()).AddParameters(new AttributeParameterBuilder().WithValue(csharpTypeNameAttribute.TypeName)).Build()
                : null)
            .Build();

    private async Task<PipelineSettings> CreateEntityPipelineSettings(
        string entitiesNamespace,
        ArgumentValidationType? forceValidateArgumentsInConstructor = null,
        bool? overrideAddNullChecks = null,
        string entityNameFormatString = "{Class.NameNoInterfacePrefix}")
    {
        var baseClass = await GetBaseClass().ConfigureAwait(false);

        return new PipelineSettingsBuilder()
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
            .WithCopyAttributePredicate(CopyAttributePredicate ?? DefaultCopyAttributePredicate)
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
            .WithBaseClass(baseClass?.ToBuilder())
            .WithInheritanceComparisonDelegate(CreateInheritanceComparisonDelegate(baseClass))
            .WithEntityNewCollectionTypeName(EntityCollectionType.WithoutGenerics())
            .WithEnableNullableReferenceTypes()
            .AddTypenameMappings(CreateTypenameMappings())
            .AddNamespaceMappings(CreateNamespaceMappings())
            .WithValidateArguments(forceValidateArgumentsInConstructor ?? CombineValidateArguments(ValidateArgumentsInConstructor, !(EnableEntityInheritance && baseClass is null)))
            .WithCollectionTypeName(EntityConcreteCollectionType.WithoutGenerics())
            .WithAddFullConstructor(AddFullConstructor)
            .WithAddPublicParameterlessConstructor(AddPublicParameterlessConstructor)
            .WithAddNullChecks(overrideAddNullChecks ?? false)
            .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull)
            .Build();
    }

    private async Task<PipelineSettings> CreateInterfacePipelineSettings(
        string interfacesNamespace,
        string newCollectionTypeName,
        InheritanceComparisonDelegate? inheritanceComparisonDelegate,
        CopyMethodPredicate? copyMethodPredicate,
        bool addSetters,
        string nameFormatString = "{Class.Name}")
        => new PipelineSettingsBuilder()
            .WithNamespaceFormatString(interfacesNamespace)
            .WithNameFormatString(nameFormatString)
            .WithEnableInheritance(EnableEntityInheritance)
            .WithIsAbstract(IsAbstract)
            .WithBaseClass((await GetBaseClass())?.ToBuilder())
            .WithInheritanceComparisonDelegate(inheritanceComparisonDelegate)
            .WithEntityNewCollectionTypeName(newCollectionTypeName)
            .AddTypenameMappings(CreateTypenameMappings())
            .AddNamespaceMappings(CreateNamespaceMappings())
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods || copyMethodPredicate != null)
            .WithInheritFromInterfaces(InheritFromInterfaces)
            .WithCopyAttributePredicate(CopyAttributePredicate ?? DefaultCopyAttributePredicate)
            .WithCopyInterfacePredicate(CopyInterfacePredicate)
            .WithCopyMethodPredicate(copyMethodPredicate ?? CopyMethodPredicate)
            .WithAddSetters(addSetters)
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .Build();

    protected IEnumerable<NamespaceMappingBuilder> CreateNamespaceMappings()
    {
        // From models to domain entities
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models").WithTargetNamespace(CoreNamespace);
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models.Domains").WithTargetNamespace($"{RootNamespace}.Domains");
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models.Abstractions").WithTargetNamespace($"{CoreNamespace}.Abstractions");

        // From domain entities to builders
        yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CoreNamespace}.Abstractions").WithTargetNamespace($"{CoreNamespace}.Abstractions")
            .AddMetadata
            (
                new MetadataBuilder().WithValue(InheritFromInterfaces ? $"{RootNamespace}.Builders" : $"{CoreNamespace}.Builders.Abstractions").WithName(MetadataNames.CustomBuilderInterfaceNamespace),
                new MetadataBuilder().WithValue("{TypeName.ClassName.NoGenerics}Builder{TypeName.GenericArgumentsWithBrackets}").WithName(MetadataNames.CustomBuilderInterfaceName),
                new MetadataBuilder().WithValue(InheritFromInterfaces ? $"{RootNamespace}.Builders" : $"{CoreNamespace}.Builders.Abstractions").WithName(MetadataNames.CustomBuilderParentTypeNamespace),
                new MetadataBuilder().WithValue("{ParentTypeName.ClassName.NoGenerics}Builder{ParentTypeName.GenericArgumentsWithBrackets}").WithName(MetadataNames.CustomBuilderParentTypeName)
            );

        foreach (var entityClassName in GetPureAbstractModels().Select(x => x.GetEntityClassName().ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal)))
        {
            yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CodeGenerationRootNamespace}.Models.{entityClassName}s").WithTargetNamespace($"{CoreNamespace}.{entityClassName}s");
            yield return new NamespaceMappingBuilder().WithSourceNamespace($"{CoreNamespace}.{entityClassName}s").WithTargetNamespace($"{CoreNamespace}.{entityClassName}s");
        }

        foreach (var mapping in CreateAdditionalNamespaceMappings())
        {
            yield return mapping;
        }
    }

    protected virtual IEnumerable<NamespaceMappingBuilder> CreateAdditionalNamespaceMappings()
        => Enumerable.Empty<NamespaceMappingBuilder>();
    
    protected IEnumerable<TypenameMappingBuilder> CreateTypenameMappings()
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
                            new MetadataBuilder().WithValue($"{CoreNamespace}.Builders{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", false)}").WithName(MetadataNames.CustomBuilderNamespace),
                            new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(MetadataNames.CustomBuilderName),
                            new MetadataBuilder().WithValue($"{ProjectName}.Abstractions.Builders").WithName(MetadataNames.CustomBuilderInterfaceNamespace),
                            new MetadataBuilder().WithValue("I{TypeName.ClassName}Builder").WithName(MetadataNames.CustomBuilderInterfaceName),
                            new MetadataBuilder().WithValue(x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && Array.Exists(x.GetInterfaces(), IsAbstractType)
                                ? $"new {CoreNamespace}.Builders{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder([Name])"
                                : "[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderSourceExpression),
                            new MetadataBuilder().WithValue(x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && IsAbstractType(x)
                                ? new Literal($"default({CoreNamespace}.Builders{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder)", null)
                                : new Literal($"new {CoreNamespace}.Builders{ReplaceStart(x.Namespace ?? string.Empty, $"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder()", null)).WithName(MetadataNames.CustomBuilderDefaultValue),
                            new MetadataBuilder().WithValue(x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && Array.Exists(x.GetInterfaces(), IsAbstractType)
                                ? "[Name][NullableSuffix].BuildTyped()[ForcedNullableSuffix]"
                                : "[Name][NullableSuffix].Build()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderMethodParameterExpression),
                            new MetadataBuilder().WithName(MetadataNames.CustomEntityInterfaceTypeName).WithValue($"{ProjectName}.Abstractions.I{x.GetEntityClassName()}")
                        )
                })
            .Concat(
            [
                new TypenameMappingBuilder().WithSourceType(typeof(bool)).WithTargetType(typeof(bool)).AddMetadata(new MetadataBuilder().WithValue(true).WithName(MetadataNames.CustomBuilderWithDefaultPropertyValue)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(List<>).WithoutGenerics()).WithTargetTypeName(typeof(List<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList()").WithName(MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(Collection<>).WithoutGenerics()).WithTargetTypeName(typeof(Collection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("new [Type][Generics]([Expression].ToList())").WithName(MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(ObservableCollection<>).WithoutGenerics()).WithTargetTypeName(typeof(ObservableCollection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("new [Type][Generics]([Expression])").WithName(MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(IReadOnlyCollection<>).WithoutGenerics()).WithTargetTypeName(typeof(IReadOnlyCollection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList().AsReadOnly()").WithName(MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(IList<>).WithoutGenerics()).WithTargetTypeName(typeof(IList<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList()").WithName(MetadataNames.CustomCollectionInitialization)),
                new TypenameMappingBuilder().WithSourceTypeName(typeof(ICollection<>).WithoutGenerics()).WithTargetTypeName(typeof(ICollection<>).WithoutGenerics()).AddMetadata(new MetadataBuilder().WithValue("[Expression].ToList()").WithName(MetadataNames.CustomCollectionInitialization)),
            ])
        .Concat(CreateAdditionalTypenameMappings());

    protected virtual IEnumerable<TypenameMappingBuilder> CreateAdditionalTypenameMappings()
         => Enumerable.Empty<TypenameMappingBuilder>();

    protected virtual bool IsAbstractType(Type type)
    {
        Guard.IsNotNull(type);
        return type.IsInterface && type.Namespace == $"{CodeGenerationRootNamespace}.Models" && type.Name.EndsWith("Base");
    }

    protected virtual bool SkipNamespaceOnTypenameMappings(string @namespace) => false;

    protected static IEnumerable<MetadataBuilder> CreateTypenameMappingMetadata(Type entityType)
        => CreateTypenameMappingMetadata($"{entityType.IsNotNull(nameof(entityType)).FullName.GetNamespaceWithDefault()}.Builders");

    protected static IEnumerable<MetadataBuilder> CreateTypenameMappingMetadata(string buildersNamespace)
        =>
        [
            new MetadataBuilder().WithValue(buildersNamespace).WithName(MetadataNames.CustomBuilderNamespace),
            new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(MetadataNames.CustomBuilderName),
            new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderSourceExpression),
            new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderMethodParameterExpression)
        ];

    private static string ReplaceStart(string fullNamespace, string baseNamespace, bool appendDot)
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

    private async Task<PipelineSettings> CreateBuilderPipelineSettings(string buildersNamespace, string entitiesNamespace)
        => new PipelineSettingsBuilder(await CreateEntityPipelineSettings(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks()))
            .WithBuilderNewCollectionTypeName(BuilderCollectionType.WithoutGenerics())
            .WithBuilderNamespaceFormatString(buildersNamespace)
            .WithSetMethodNameFormatString(SetMethodNameFormatString)
            .WithAddMethodNameFormatString(AddMethodNameFormatString)
            .WithEnableBuilderInheritance(EnableBuilderInhericance)
            .WithBaseClassBuilderNameSpace(BaseClassBuilderNamespace)
            .WithAddCopyConstructor(AddCopyConstructor)
            .WithSetDefaultValuesInEntityConstructor(SetDefaultValues)
            .Build();

    private async Task<PipelineSettings> CreateBuilderInterfacePipelineSettings(string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
        => new PipelineSettingsBuilder(await CreateEntityPipelineSettings(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks()))
            .WithBuilderNewCollectionTypeName(BuilderCollectionType.WithoutGenerics())
            .WithBuilderNamespaceFormatString(buildersNamespace)
            .WithBuilderExtensionsNamespaceFormatString(buildersExtensionsNamespace)
            .WithSetMethodNameFormatString(SetMethodNameFormatString)
            .WithAddMethodNameFormatString(AddMethodNameFormatString)
            .WithEnableBuilderInheritance(EnableBuilderInhericance)
            .Build();

    private async Task<TypeBase> CreateEntity(TypeBase typeBase, string entitiesNamespace)
        => (await PipelineService.Process(new EntityContext(typeBase, await CreateEntityPipelineSettings(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks(), entityNameFormatString: "{Class.NameNoInterfacePrefix}"), Settings.CultureInfo)))
            .GetValueOrThrow();

    private async Task<TypeBase> CreateBuilderClass(TypeBase typeBase, string buildersNamespace, string entitiesNamespace)
        => (await PipelineService.Process(new BuilderContext(typeBase, await CreateBuilderPipelineSettings(buildersNamespace, entitiesNamespace), Settings.CultureInfo)))
            .GetValueOrThrow();

    private async Task<TypeBase> CreateBuilderExtensionsClass(TypeBase typeBase, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
        => (await PipelineService.Process(new BuilderExtensionContext(typeBase, await CreateBuilderInterfacePipelineSettings(buildersNamespace, entitiesNamespace, buildersExtensionsNamespace), Settings.CultureInfo)))
            .GetValueOrThrow();

    private async Task<TypeBase> CreateNonGenericBuilderClass(TypeBase typeBase, string buildersNamespace, string entitiesNamespace)
        => (await PipelineService.Process(new BuilderContext(typeBase, (await CreateBuilderPipelineSettings(buildersNamespace, entitiesNamespace)).ToBuilder().WithIsForAbstractBuilder().Build(), Settings.CultureInfo)))
            .GetValueOrThrow();

    private bool? GetOverrideAddNullChecks()
    {
        if (AddNullChecks || ValidateArgumentsInConstructor == ArgumentValidationType.None)
        {
            return true;
        }

        return null;
    }

    private async Task<TypeBase> GetModel(Type type)
        => (await PipelineService.Process(new ReflectionContext(type, CreateReflectionPipelineSettings(), Settings.CultureInfo)))
            .GetValueOrThrow();

    private async Task<TypeBase> CreateInterface(
        TypeBase typeBase,
        string interfacesNamespace,
        string newCollectionTypeName,
        bool addSetters,
        string nameFormatString = "{Class.Name}",
        CopyMethodPredicate? copyMethodPredicate = null)
    {
        return (await PipelineService.Process(new InterfaceContext(typeBase, await CreateInterfacePipelineSettings(interfacesNamespace, newCollectionTypeName, CreateInheritanceComparisonDelegate(await GetBaseClass()), copyMethodPredicate, addSetters, nameFormatString), Settings.CultureInfo)).ConfigureAwait(false))
            .GetValueOrThrow();
    }
}
