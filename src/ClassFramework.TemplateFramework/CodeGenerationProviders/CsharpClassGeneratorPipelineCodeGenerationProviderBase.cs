namespace ClassFramework.TemplateFramework.CodeGenerationProviders;

public abstract class CsharpClassGeneratorPipelineCodeGenerationProviderBase : CsharpClassGeneratorCodeGenerationProviderBase
{
    protected CsharpClassGeneratorPipelineCodeGenerationProviderBase(ICommandService commandService)
    {
        Guard.IsNotNull(commandService);

        CommandService = commandService;
    }

    protected ICommandService CommandService { get; }

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
            .WithSkipWhenFileExists(SkipWhenFileExists);

    protected abstract string ProjectName { get; }
    protected abstract Type EntityCollectionType { get; }
    protected abstract Type EntityConcreteCollectionType { get; }
    protected abstract Type BuilderCollectionType { get; }

    protected virtual string RootNamespace => CoreNamespace;
    protected virtual string CodeGenerationRootNamespace => $"{ProjectName}.CodeGeneration";
    protected virtual string CoreNamespace => $"{ProjectName}.Core";
    protected virtual string BuilderAbstractionsNamespace => $"{RootNamespace}.Builders.Abstractions";
    protected virtual string AbstractionsNamespace => $"{CoreNamespace}.Abstractions";
    protected virtual string ValidationNamespace => $"{RootNamespace}.Validation";
    protected virtual string DomainsNamespace => $"{RootNamespace}.Domains";

    protected virtual string EnvironmentVersion => string.Empty;
    protected virtual bool CopyAttributes => false;
    protected virtual bool CopyInterfaces => false;
    protected virtual bool CopyMethods => false;
    protected virtual bool UseBuilderAbstractionsTypeConversion => true;
    protected virtual bool UseBuilderLazyValues => false;
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
    protected virtual Task<Result<TypeBase>> GetBaseClassAsync() => Task.FromResult(Result.NotFound<TypeBase>());
    protected virtual bool IsAbstract => false;
    protected virtual string BaseClassBuilderNamespace => string.Empty;
    protected virtual bool AllowGenerationWithoutProperties => true;
    protected virtual string SetMethodNameFormatString => "With{property.Name}";
    protected virtual string AddMethodNameFormatString => "Add{property.Name}";
    protected virtual string ToBuilderFormatString => "ToBuilder";
    protected virtual string ToTypedBuilderFormatString => "ToTypedBuilder";
    protected virtual string BuildMethodName => "Build";
    protected virtual string BuildTypedMethodName => "BuildTyped";
    protected virtual bool AddFullConstructor => true;
    protected virtual bool AddPublicParameterlessConstructor => false;
    protected virtual bool AddCopyConstructor => true;
    protected virtual bool AddImplicitOperatorOnBuilder => true;
    protected virtual bool SetDefaultValues => true;
    protected virtual string FilenameSuffix => ".template.generated";
    protected virtual bool CreateCodeGenerationHeader => true;
    protected virtual bool SkipWhenFileExists => false;
    protected virtual bool GenerateMultipleFiles => true;
    protected virtual bool EnableNullableContext => true;
    protected virtual bool EnableNullablePragmas => true;
    protected virtual bool EnableGlobalUsings => false;
    protected virtual bool UseCrossCuttingInterfaces => false;
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
                || (parentNameContainer.ParentTypeFullName.StartsWith($"{ProjectName}.") && typeBase.Namespace.In(CoreNamespace, $"{RootNamespace}.Builders", BuilderAbstractionsNamespace))
            );

    protected virtual string[] GetModelAbstractBaseTyped() => [];

    protected virtual string[] GetExternalCustomBuilderTypes() => [];

    protected virtual IEnumerable<TypenameMappingBuilder> GetAdditionalTypenameMappings() => [];

    protected virtual IEnumerable<NamespaceMappingBuilder> GetAdditionalNamespaceMappings() => [];

    protected virtual string[] GetCustomBuilderTypes()
        => GetPureAbstractModels()
            .Select(x => x.GetEntityClassName())
            .Concat(GetExternalCustomBuilderTypes())
            .ToArray();

    protected virtual string[] GetBuilderAbstractionsTypeConversionNamespaces() => [AbstractionsNamespace];
    protected virtual string[] GetCodeGenerationBuilderAbstractionsTypeConversionNamespaces() => [$"{CodeGenerationRootNamespace}.Models.Abstractions"];
    protected virtual string[] GetSkipNamespacesOnFluentBuilderMethods() => [AbstractionsNamespace];

    protected virtual IEnumerable<Type> GetPureAbstractModels()
        => GetType().Assembly.GetTypes().Where(IsAbstractType);

    protected Task<Result<IEnumerable<TypeBase>>> GetEntitiesAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string entitiesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(entitiesNamespace);

        return ProcessModelsResultAsync(modelsResultTask, x => CreateEntityAsync(x, entitiesNamespace), "entities");
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetEntityInterfacesAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return await ProcessModelsResultAsync(modelsResultTask, async x => await CreateInterfaceAsync(await CreateEntityAsync(x, entitiesNamespace, false).ConfigureAwait(false), interfacesNamespace, string.Empty, true, "I{class.Name}", string.Empty, (t, m) => m.Name == ToBuilderFormatString && (UseBuilderAbstractionsTypeConversion && !UseCrossCuttingInterfaces)).ConfigureAwait(false), "interfaces").ConfigureAwait(false);
    }

    protected Task<Result<IEnumerable<TypeBase>>> GetBuildersAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace)
        => GetBuildersAsync(modelsResultTask, buildersNamespace, entitiesNamespace, null);

    private async Task<Result<IEnumerable<TypeBase>>> GetBuildersAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace, bool? useBuilderAbstractionsTypeConversion)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return await ProcessModelsResultAsync(modelsResultTask, CreateEntityPipelineSettingsAsync(entitiesNamespace, useBuilderAbstractionsTypeConversion: useBuilderAbstractionsTypeConversion, useCrossCuttingInterfaces: false), async (settings, x) =>
        {
            var context = new EntityContext(x, settings, Settings.CultureInfo, CancellationToken.None);
            var entityResult = await CommandService.ExecuteAsync<EntityContext, TypeBase>(context).ConfigureAwait(false);
            return await CreateBuilderClassAsync(entityResult, buildersNamespace, entitiesNamespace, useBuilderAbstractionsTypeConversion).ConfigureAwait(false);
        }, "builders").ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetBuilderExtensionsAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(buildersExtensionsNamespace);

        return await ProcessBaseClassResultAsync(baseClass => ProcessModelsResultAsync(modelsResultTask, CreateInterfacePipelineSettingsAsync(entitiesNamespace, string.Empty, CreateInheritanceComparisonDelegate(baseClass), null, true), async (settings, x) =>
        {
            var context = new InterfaceContext(x, settings, Settings.CultureInfo, CancellationToken.None);
            var interfaceResult = await CommandService.ExecuteAsync<InterfaceContext, TypeBase>(context).ConfigureAwait(false);
            return await CreateBuilderExtensionsClassAsync(interfaceResult, buildersNamespace, entitiesNamespace, buildersExtensionsNamespace).ConfigureAwait(false);
        }, "builder extensions")).ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetNonGenericBuildersAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return await ProcessModelsResultAsync(modelsResultTask, CreateEntityPipelineSettingsAsync(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks(), useCrossCuttingInterfaces: false), async (settings, x) =>
        {
            var context = new EntityContext(x, settings, Settings.CultureInfo, CancellationToken.None);
            var typeBaseResult = await CommandService.ExecuteAsync<EntityContext, TypeBase>(context).ConfigureAwait(false);
            return await CreateNonGenericBuilderClassAsync(typeBaseResult, buildersNamespace, entitiesNamespace).ConfigureAwait(false);
        }, "non generic builders").ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetBuilderInterfacesAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return await ProcessModelsResultAsync
        (
            GetBuildersAsync(modelsResultTask, buildersNamespace, entitiesNamespace, false),
            async x => await CreateInterfaceAsync(Result.Success(x.ToBuilder().Chain(y => { var itemsToDelete = y.GenericTypeArguments.Where(z => z == "TEntity" || z == "TBuilder").ToList(); itemsToDelete.ForEach(z => y.GenericTypeArguments.Remove(z)); y.GenericTypeArgumentConstraints.Clear(); }).Build()), interfacesNamespace, BuilderCollectionType.WithoutGenerics(), true, "I{class.Name}", MetadataNames.CustomBuilderInterfaceTypeName, (t, m) => m.Name == BuildMethodName && (UseBuilderAbstractionsTypeConversion && !UseCrossCuttingInterfaces)).ConfigureAwait(false),
            "builder interfaces"
        ).ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetCoreModelsAsync()
    {
        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectAsync(GetModelAsync)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create core models. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetNonCoreModelsAsync(string @namespace)
    {
        Guard.IsNotNull(@namespace);

        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == @namespace && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectAsync(GetModelAsync)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create non core models. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetAbstractionsInterfacesAsync()
    {
        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions")
            .SelectAsync(GetModelAsync)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract interfaces. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetAbstractModelsAsync()
    {
        var modelsResult = await GetPureAbstractModels()
            .SelectAsync(GetModelAsync)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract models. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetOverrideModelsAsync(Type abstractType)
    {
        Guard.IsNotNull(abstractType);

        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && Array.Exists(x.GetInterfaces(), y => y.WithoutGenerics() == abstractType.WithoutGenerics()))
            .SelectAsync(GetModelAsync)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create override models. See the inner results for more details."));
    }

    protected async Task<Result<TypeBase>> CreateBaseClassAsync(Type type, string @namespace)
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(@namespace);

        var commandResult = await CommandService.ExecuteAsync<ReflectionContext, TypeBase>(new ReflectionContext(type, CreateReflectionPipelineSettings(), Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false);
        return await ProcessBaseClassResultAsync(Task.FromResult(commandResult), GenerateBaseClass(@namespace)).ConfigureAwait(false);
    }

    protected Func<TypeBase?, Task<Result<TypeBase>>> GenerateBaseClass(string @namespace)
        => async typeBaseResult =>
        {
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
                .WithCopyAttributePredicate(CopyAttributePredicate ?? DefaultCopyAttributePredicate)
                .WithCopyInterfacePredicate(CopyInterfacePredicate)
                .WithCopyMethodPredicate(CopyMethodPredicate)
                .WithEntityNameFormatString("{NoInterfacePrefix(class.Name)}")
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
                            || (parentNameContainer.ParentTypeFullName.StartsWith($"{ProjectName}.") && typeBase.Namespace.In(CoreNamespace, $"{RootNamespace}.Builders"))
                        ))
                .WithEntityNewCollectionTypeName(EntityCollectionType.WithoutGenerics())
                .WithEnableNullableReferenceTypes()
                .WithValidateArguments(ValidateArgumentsInConstructor)
                .WithAddNullChecks(AddNullChecks)
                .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull)
                .WithUseCrossCuttingInterfaces(UseCrossCuttingInterfaces)
                .AddTypenameMappings(CreateTypenameMappings())
                .AddNamespaceMappings(CreateNamespaceMappings());

            return await CommandService.ExecuteAsync<EntityContext, TypeBase>(new EntityContext(typeBaseResult!, entitySettings, Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false);
        };

    protected virtual PipelineSettings CreateReflectionPipelineSettings()
        => new PipelineSettingsBuilder()
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods)
            .WithCopyAttributePredicate(CopyAttributePredicate)
            .WithCopyInterfacePredicate(CopyInterfacePredicate)
            .WithCopyMethodPredicate(CopyMethodPredicate)
            .AddNamespaceMappings(CreateNamespaceMappings())
            .AddTypenameMappings(CreateTypenameMappings())
            .AddAttributeInitializers(x => x is CsharpTypeNameAttribute csharpTypeNameAttribute
                ? new AttributeBuilder().WithName(csharpTypeNameAttribute.GetType()).AddParameters(new AttributeParameterBuilder().WithValue(csharpTypeNameAttribute.TypeName)).Build()
                : null);

    protected IEnumerable<NamespaceMappingBuilder> CreateNamespaceMappings()
    {
        // From models to domain entities
        yield return new NamespaceMappingBuilder($"{CodeGenerationRootNamespace}.Models", CoreNamespace);
        yield return new NamespaceMappingBuilder($"{CodeGenerationRootNamespace}.Models.Domains", DomainsNamespace);
        yield return new NamespaceMappingBuilder($"{CodeGenerationRootNamespace}.Models.Abstractions", AbstractionsNamespace);
        yield return new NamespaceMappingBuilder($"{CodeGenerationRootNamespace}.Validation", ValidationNamespace);

        // From domain entities to builders
        yield return new NamespaceMappingBuilder(AbstractionsNamespace)
            .AddMetadata
            (
                new MetadataBuilder(MetadataNames.CustomBuilderInterfaceNamespace, BuilderAbstractionsNamespace),
                new MetadataBuilder(MetadataNames.CustomBuilderInterfaceName, "{NoGenerics(ClassName(property.TypeName))}Builder{GenericArguments(property.TypeName, true)}"),
                new MetadataBuilder(MetadataNames.CustomBuilderInterfaceTypeName, $"{BuilderAbstractionsNamespace}.I{{NoGenerics(ClassName(property.TypeName))}}Builder{{GenericArguments(property.TypeName, true)}}"),
                new MetadataBuilder(MetadataNames.CustomBuilderParentTypeNamespace, BuilderAbstractionsNamespace),
                new MetadataBuilder(MetadataNames.CustomBuilderParentTypeName, "{NoGenerics(ClassName(property.ParentTypeFullName))}Builder{GenericArguments(property.ParentTypeFullName, true)}")
            );

        foreach (var mapping in GetAdditionalNamespaceMappings())
        {
            yield return mapping;
        }
    }

    protected IEnumerable<TypenameMappingBuilder> CreateTypenameMappings(bool? useBuilderAbstractionsTypeConversion = null)
    {
        var skipNamespaceOnTypenameMappings = GetSkippedNamespacesOnTypenameMappings();

        return GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface
                && x.Namespace?.StartsWith($"{CodeGenerationRootNamespace}.Models", StringComparison.Ordinal) == true
                && !skipNamespaceOnTypenameMappings.Contains(x.Namespace)
                && x.FullName is not null)
            .SelectMany(x =>
                (new[]
                {
                    new TypenameMappingBuilder(x, $"{CoreNamespace}.{x.Namespace.ReplaceStartNamespace($"{CodeGenerationRootNamespace}.Models", true)}{x.GetEntityClassName()}"),
                    new TypenameMappingBuilder($"{CoreNamespace}.{x.Namespace.ReplaceStartNamespace($"{CodeGenerationRootNamespace}.Models", true)}{x.GetEntityClassName()}")
                        .AddMetadata
                        (
                            new MetadataBuilder(MetadataNames.CustomBuilderNamespace, $"{CoreNamespace}.Builders{x.Namespace.ReplaceStartNamespace($"{CodeGenerationRootNamespace}.Models", false)}"),
                            new MetadataBuilder(MetadataNames.CustomBuilderName, $"{x.GetEntityClassName()}Builder"),
                            new MetadataBuilder(MetadataNames.CustomBuilderInterfaceNamespace, $"{AbstractionsNamespace}.Builders"),
                            new MetadataBuilder(MetadataNames.CustomBuilderInterfaceName, $"I{x.GetEntityClassName()}Builder"),
                            new MetadataBuilder(MetadataNames.CustomBuilderInterfaceTypeName, $"{AbstractionsNamespace}.Builders.I{x.GetEntityClassName()}Builder"),
                            new MetadataBuilder(MetadataNames.CustomBuilderSourceExpression, x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && Array.Exists(x.GetInterfaces(), IsAbstractType)
                                ? $"new {CoreNamespace}.Builders{x.Namespace.ReplaceStartNamespace($"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder([Name])"
                                : "[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]"),
                            new MetadataBuilder(MetadataNames.CustomBuilderDefaultValue, x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && IsAbstractType(x)
                                ? new Literal($"default({CoreNamespace}.Builders{x.Namespace.ReplaceStartNamespace($"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder)")
                                : new Literal($"new {CoreNamespace}.Builders{x.Namespace.ReplaceStartNamespace($"{CodeGenerationRootNamespace}.Models", false)}.{x.GetEntityClassName()}Builder()")),
                            new MetadataBuilder(MetadataNames.CustomBuilderMethodParameterExpression, x.Namespace != $"{CodeGenerationRootNamespace}.Models.Abstractions" && Array.Exists(x.GetInterfaces(), IsAbstractType)
                                ? "[Name][NullableSuffix].BuildTyped()[ForcedNullableSuffix]"
                                : "[Name][NullableSuffix].Build()[ForcedNullableSuffix]"),
                            new MetadataBuilder(MetadataNames.CustomEntityInterfaceTypeName, $"{AbstractionsNamespace}.I{x.GetEntityClassName()}")
                        )
                }))
            .Concat(
            [
                new TypenameMappingBuilder(typeof(bool))
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomBuilderWithDefaultPropertyValue, true)),
                new TypenameMappingBuilder(typeof(List<>).WithoutGenerics())
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomCollectionInitialization, "[Expression].ToList()")),
                new TypenameMappingBuilder(typeof(Collection<>).WithoutGenerics())
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomCollectionInitialization, "new [Type][Generics]([Expression].ToList())")),
                new TypenameMappingBuilder(typeof(ObservableCollection<>).WithoutGenerics())
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomCollectionInitialization, "new [Type][Generics]([Expression])")),
                new TypenameMappingBuilder(typeof(IReadOnlyCollection<>).WithoutGenerics())
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomCollectionInitialization, "[Expression].ToList().AsReadOnly()")),
                new TypenameMappingBuilder(typeof(IList<>).WithoutGenerics())
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomCollectionInitialization, "[Expression].ToList()")),
                new TypenameMappingBuilder(typeof(ICollection<>).WithoutGenerics())
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomCollectionInitialization, "[Expression].ToList()")),
            ])
            .Concat(CreateBuilderAbstractionsTypeConversionTypenameMapping(useBuilderAbstractionsTypeConversion))
            .Concat(GetAdditionalTypenameMappings());
    }

    private IEnumerable<TypenameMappingBuilder> CreateBuilderAbstractionsTypeConversionTypenameMapping(bool? useBuilderAbstractionsTypeConversion)
        => GetType().Assembly.GetTypes()
            .Where(x => (useBuilderAbstractionsTypeConversion ?? UseBuilderAbstractionsTypeConversion)
                && x.IsInterface
                && !string.IsNullOrEmpty(x.Namespace)
                && GetCodeGenerationBuilderAbstractionsTypeConversionNamespaces().Contains(x.Namespace)
                && x.FullName is not null)
            .SelectMany(x => CreateBuilderAbstractionTypeConversionTypenameMappings(x.GetEntityClassName(), x.GetGenericTypeArgumentsString()));

    protected TypenameMappingBuilder[] CreateBuilderAbstractionTypeConversionTypenameMappings(string entityClassName, string genericTypeArgumentsString)
        => CreateBuilderAbstractionTypeConversionTypenameMappings(entityClassName, genericTypeArgumentsString, AbstractionsNamespace, BuilderAbstractionsNamespace, CoreNamespace);

    protected static TypenameMappingBuilder[] CreateBuilderAbstractionTypeConversionTypenameMappings(string entityClassName, string genericTypeArgumentsString, string abstractionsNamespace, string builderAbstractionsNamespace, string coreNamespace)
        =>
        [
            new TypenameMappingBuilder($"{abstractionsNamespace}.I{entityClassName}{genericTypeArgumentsString}", $"{abstractionsNamespace}.I{entityClassName}")
                .AddMetadata
                (
                    new MetadataBuilder(MetadataNames.CustomBuilderNamespace, builderAbstractionsNamespace),
                    new MetadataBuilder(MetadataNames.CustomBuilderName, $"I{entityClassName.WithoutGenerics()}Builder"),
                    new MetadataBuilder(MetadataNames.CustomBuilderInterfaceNamespace, builderAbstractionsNamespace),
                    new MetadataBuilder(MetadataNames.CustomBuilderInterfaceName, $"I{entityClassName.WithoutGenerics()}Builder{genericTypeArgumentsString}"),
                    new MetadataBuilder(MetadataNames.CustomBuilderInterfaceTypeName, $"{builderAbstractionsNamespace}.I{entityClassName.WithoutGenerics()}Builder{genericTypeArgumentsString}"),
                    new MetadataBuilder(MetadataNames.CustomBuilderSourceExpression, "[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]"),
                    new MetadataBuilder(MetadataNames.CustomBuilderDefaultValue, new Literal($"default({builderAbstractionsNamespace}.I{entityClassName.WithoutGenerics()}Builder{genericTypeArgumentsString})")),
                    new MetadataBuilder(MetadataNames.CustomBuilderMethodParameterExpression, "[Name][NullableSuffix].Build()[ForcedNullableSuffix]"),
                    new MetadataBuilder(MetadataNames.CustomEntityInterfaceTypeName, $"{abstractionsNamespace}.I{entityClassName}")
                ),
            //Temporary fix for flaw in abstractions typename mapping
            new TypenameMappingBuilder($"{coreNamespace}.Builders.I{entityClassName}Builder", $"{builderAbstractionsNamespace}.I{entityClassName}Builder"),
            new TypenameMappingBuilder($"{coreNamespace}.Abstractions.{entityClassName}", $"{abstractionsNamespace}.I{entityClassName}"),
            new TypenameMappingBuilder($"{abstractionsNamespace}.{entityClassName}", $"{abstractionsNamespace}.I{entityClassName}")
        ];

    protected virtual bool IsAbstractType(Type type)
    {
        Guard.IsNotNull(type);

        return type.IsInterface
            && type.Namespace == $"{CodeGenerationRootNamespace}.Models"
            && type.Name.WithoutTypeGenerics().EndsWith("Base");
    }

    protected IEnumerable<string> GetSkippedNamespacesOnTypenameMappings()
    {
        yield return $"{CodeGenerationRootNamespace}.Models.Abstractions";
        yield return $"{CodeGenerationRootNamespace}.Models.Domains";
        yield return $"{CodeGenerationRootNamespace}.Validation";

        foreach (var ns in GetAdditionalSkippedNamespacesOnTypenameMappings())
        {
            yield return ns;
        }
    }

    protected virtual string[] GetAdditionalSkippedNamespacesOnTypenameMappings() => [];

    protected static ArgumentValidationType CombineValidateArguments(ArgumentValidationType validateArgumentsInConstructor, bool secondCondition)
        => secondCondition
            ? validateArgumentsInConstructor
            : ArgumentValidationType.None;

    protected static IEnumerable<MetadataBuilder> CreateTypenameMappingMetadata(Type entityType)
    {
        Guard.IsNotNull(entityType);

        return CreateTypenameMappingMetadata($"{entityType.FullName.GetNamespaceWithDefault()}.Builders");
    }

    protected static IEnumerable<MetadataBuilder> CreateTypenameMappingMetadata(string buildersNamespace)
    {
        Guard.IsNotNull(buildersNamespace);

        return
        [
            new MetadataBuilder(MetadataNames.CustomBuilderNamespace, buildersNamespace),
            new MetadataBuilder(MetadataNames.CustomBuilderName, "{ClassName(property.TypeName)}Builder"),
            new MetadataBuilder(MetadataNames.CustomBuilderSourceExpression, "[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]"),
            new MetadataBuilder(MetadataNames.CustomBuilderMethodParameterExpression, "[Name][NullableSuffix].Build()[ForcedNullableSuffix]")
        ];
    }

    private Task<Result<PipelineSettings>> CreateEntityPipelineSettingsAsync(
        string entitiesNamespace,
        ArgumentValidationType? forceValidateArgumentsInConstructor = null,
        bool? overrideAddNullChecks = null,
        string entityNameFormatString = "{NoInterfacePrefix(class.Name)}",
        bool? useBuilderAbstractionsTypeConversion = null,
        bool? useCrossCuttingInterfaces = null)
        => ProcessBaseClassResultAsync(baseClass => Task.FromResult(Result.Success(new PipelineSettingsBuilder()
            .WithAddSetters(AddSetters)
            .WithAddBackingFields(AddBackingFields)
            .WithSetterVisibility(SetterVisibility)
            .WithCreateAsObservable(CreateAsObservable)
            .WithCreateRecord(CreateRecord)
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods)
            .WithInheritFromInterfaces(useBuilderAbstractionsTypeConversion == false)
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
            .WithValidateArguments(forceValidateArgumentsInConstructor ?? CombineValidateArguments(ValidateArgumentsInConstructor, !(EnableEntityInheritance && baseClass is null)))
            .WithCollectionTypeName(EntityConcreteCollectionType.WithoutGenerics())
            .WithAddFullConstructor(AddFullConstructor)
            .WithAddPublicParameterlessConstructor(AddPublicParameterlessConstructor)
            .WithAddNullChecks(overrideAddNullChecks ?? false)
            .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull)
            .WithUseBuilderAbstractionsTypeConversion(useBuilderAbstractionsTypeConversion ?? UseBuilderAbstractionsTypeConversion)
            .WithUseCrossCuttingInterfaces(useCrossCuttingInterfaces ?? UseCrossCuttingInterfaces)
            .AddTypenameMappings(CreateTypenameMappings(useBuilderAbstractionsTypeConversion))
            .AddNamespaceMappings(CreateNamespaceMappings())
            .AddBuilderAbstractionsTypeConversionNamespaces(GetBuilderAbstractionsTypeConversionNamespaces())
            .Build())));

    private Task<Result<PipelineSettings>> CreateInterfacePipelineSettingsAsync(
        string interfacesNamespace,
        string newCollectionTypeName,
        InheritanceComparisonDelegate? inheritanceComparisonDelegate,
        CopyMethodPredicate? copyMethodPredicate,
        bool addSetters,
        string nameFormatString = "{class.Name}",
        string builderAbstractionsTypeConversionMetadataName = "")
        => ProcessBaseClassResultAsync(baseClass => Task.FromResult(Result.Success(new PipelineSettingsBuilder()
            .WithNamespaceFormatString(interfacesNamespace)
            .WithNameFormatString(nameFormatString)
            .WithEnableInheritance(EnableEntityInheritance)
            .WithIsAbstract(IsAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
            .WithInheritanceComparisonDelegate(inheritanceComparisonDelegate)
            .WithEntityNewCollectionTypeName(newCollectionTypeName)
            .WithCopyAttributes(CopyAttributes)
            .WithCopyInterfaces(CopyInterfaces)
            .WithCopyMethods(CopyMethods || copyMethodPredicate != null)
            .WithInheritFromInterfaces()
            .WithCopyAttributePredicate(CopyAttributePredicate ?? DefaultCopyAttributePredicate)
            .WithCopyInterfacePredicate(CopyInterfacePredicate)
            .WithCopyMethodPredicate(copyMethodPredicate ?? CopyMethodPredicate)
            .WithAddSetters(addSetters)
            .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
            .WithUseBuilderAbstractionsTypeConversion(UseBuilderAbstractionsTypeConversion)
            .WithBuilderAbstractionsTypeConversionMetadataName(builderAbstractionsTypeConversionMetadataName)
            .WithUseCrossCuttingInterfaces(UseCrossCuttingInterfaces)
            .AddTypenameMappings(CreateTypenameMappings())
            .AddNamespaceMappings(CreateNamespaceMappings())
            .AddBuilderAbstractionsTypeConversionNamespaces(GetBuilderAbstractionsTypeConversionNamespaces())
            .Build())));

    private Task<Result<PipelineSettings>> CreateBuilderPipelineSettingsAsync(string buildersNamespace, string entitiesNamespace, bool? useBuilderAbstractionsTypeConversion = null)
        => ProcessSettingsResultAsync(CreateEntityPipelineSettingsAsync(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks(), useBuilderAbstractionsTypeConversion: useBuilderAbstractionsTypeConversion),
            settings => Task.FromResult(Result.Success(new PipelineSettingsBuilder(settings)
                .WithBuilderNewCollectionTypeName(BuilderCollectionType.WithoutGenerics())
                .WithBuilderNamespaceFormatString(buildersNamespace)
                .WithSetMethodNameFormatString(SetMethodNameFormatString)
                .WithAddMethodNameFormatString(AddMethodNameFormatString)
                .WithEnableBuilderInheritance(EnableBuilderInhericance)
                .WithBaseClassBuilderNameSpace(BaseClassBuilderNamespace)
                .WithAddCopyConstructor(AddCopyConstructor)
                .WithAddImplicitOperatorOnBuilder(AddImplicitOperatorOnBuilder)
                .WithSetDefaultValuesInEntityConstructor(SetDefaultValues)
                .WithUseBuilderLazyValues(UseBuilderLazyValues)
                .AddSkipNamespacesOnFluentBuilderMethods(GetSkipNamespacesOnFluentBuilderMethods())
                .Build())
            ));

    private Task<Result<PipelineSettings>> CreateBuilderInterfacePipelineSettingsAsync(string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
        => ProcessSettingsResultAsync(CreateEntityPipelineSettingsAsync(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks()),
            settings => Task.FromResult(Result.Success(new PipelineSettingsBuilder(settings)
                .WithBuilderNewCollectionTypeName(BuilderCollectionType.WithoutGenerics())
                .WithBuilderNamespaceFormatString(buildersNamespace)
                .WithBuilderExtensionsNamespaceFormatString(buildersExtensionsNamespace)
                .WithSetMethodNameFormatString(SetMethodNameFormatString)
                .WithAddMethodNameFormatString(AddMethodNameFormatString)
                .WithEnableBuilderInheritance(EnableBuilderInhericance)
                .WithUseBuilderLazyValues(UseBuilderLazyValues)
                .Build())
            ));

    private Task<Result<TypeBase>> CreateEntityAsync(TypeBase typeBase, string entitiesNamespace, bool? useBuilderAbstractionsTypeConversion = null)
        => ProcessSettingsResultAsync(
            CreateEntityPipelineSettingsAsync(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks(), entityNameFormatString: "{NoInterfacePrefix(class.Name)}", useBuilderAbstractionsTypeConversion: useBuilderAbstractionsTypeConversion),
            async settings => await CommandService.ExecuteAsync<EntityContext, TypeBase>(new EntityContext(typeBase, settings, Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false));

    private Task<Result<TypeBase>> CreateBuilderClassAsync(Result<TypeBase> typeBaseResult, string buildersNamespace, string entitiesNamespace, bool? useBuilderAbstractionsTypeConversion = null)
        => typeBaseResult.OnSuccessAsync(
            () => ProcessSettingsResultAsync(
                CreateBuilderPipelineSettingsAsync(buildersNamespace, entitiesNamespace, useBuilderAbstractionsTypeConversion),
                async settings => await CommandService.ExecuteAsync<BuilderContext, TypeBase>(new BuilderContext(typeBaseResult.Value!, settings, Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false))
            );

    private Task<Result<TypeBase>> CreateBuilderExtensionsClassAsync(Result<TypeBase> typeBaseResult, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
        => typeBaseResult.OnSuccessAsync(
            () => ProcessSettingsResultAsync(
                CreateBuilderInterfacePipelineSettingsAsync(buildersNamespace, entitiesNamespace, buildersExtensionsNamespace),
                async settings => await CommandService.ExecuteAsync<BuilderExtensionContext, TypeBase>(new BuilderExtensionContext(typeBaseResult.Value!, settings, Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false))
            );

    private Task<Result<TypeBase>> CreateNonGenericBuilderClassAsync(Result<TypeBase> typeBaseResult, string buildersNamespace, string entitiesNamespace)
        => typeBaseResult.OnSuccessAsync(
            () => ProcessSettingsResultAsync(
                CreateBuilderPipelineSettingsAsync(buildersNamespace, entitiesNamespace),
                async settings => await CommandService.ExecuteAsync<BuilderContext, TypeBase>(new BuilderContext(typeBaseResult.Value!, settings.ToBuilder().WithIsForAbstractBuilder().Build(), Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false))
            );

    private bool? GetOverrideAddNullChecks()
    {
        if (AddNullChecks || ValidateArgumentsInConstructor == ArgumentValidationType.None)
        {
            return true;
        }

        return null;
    }

    private Task<Result<TypeBase>> GetModelAsync(Type type)
        => CommandService.ExecuteAsync<ReflectionContext, TypeBase>(new ReflectionContext(type, CreateReflectionPipelineSettings(), Settings.CultureInfo, CancellationToken.None));

    private Task<Result<TypeBase>> CreateInterfaceAsync(
        Result<TypeBase> typeBaseResult,
        string interfacesNamespace,
        string newCollectionTypeName,
        bool addSetters,
        string nameFormatString = "{class.Name}",
        string builderAbstractionsTypeConversionMetadataName = "",
        CopyMethodPredicate? copyMethodPredicate = null)
            => typeBaseResult.OnSuccessAsync(
                () => ProcessBaseClassResultAsync(
                    baseClass => ProcessSettingsResultAsync(CreateInterfacePipelineSettingsAsync(interfacesNamespace, newCollectionTypeName, CreateInheritanceComparisonDelegate(baseClass), copyMethodPredicate, addSetters, nameFormatString, builderAbstractionsTypeConversionMetadataName), async settings =>
                        await CommandService.ExecuteAsync<InterfaceContext, TypeBase>(new InterfaceContext(typeBaseResult.Value!, settings, Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false)
                    )
                )
            );

    private Task<Result<T>> ProcessBaseClassResultAsync<T>(Func<TypeBase?, Task<Result<T>>> successTask)
        => ProcessBaseClassResultAsync(GetBaseClassAsync(), successTask);

    protected static async Task<Result<T>> ProcessBaseClassResultAsync<T>(Task<Result<TypeBase>> baseClassResultTask, Func<TypeBase?, Task<Result<T>>> successTask)
    {
        Guard.IsNotNull(baseClassResultTask);
        Guard.IsNotNull(successTask);

        var baseClassResult = await baseClassResultTask.ConfigureAwait(false);
        if (!baseClassResult.IsSuccessful() && baseClassResult.Status != ResultStatus.NotFound)
        {
            return Result.Error<T>([baseClassResult], "Could not get base class, see inner results for details");
        }

        return await successTask(baseClassResult.Value).ConfigureAwait(false);
    }

    private static async Task<Result<IEnumerable<TypeBase>>> ProcessModelsResultAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, Task<Result<PipelineSettings>> settingsTask, Func<PipelineSettings, TypeBase, Task<Result<TypeBase>>> successTask, string resultType)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(settingsTask);
        Guard.IsNotNull(successTask);

        var modelsResult = await modelsResultTask.ConfigureAwait(false);

        if (!modelsResult.EnsureValue().IsSuccessful())
        {
            return modelsResult;
        }

        var results = new List<Result<TypeBase>>();

        foreach (var typeBase in modelsResult.Value!)
        {
            var result = await ProcessSettingsResultAsync(settingsTask, settings => successTask(settings, typeBase)).ConfigureAwait(false);

            results.Add(result);

            if (!result.IsSuccessful())
            {
                return Result.Error<IEnumerable<TypeBase>>(results, $"Could not create {resultType}. See the inner results for more details.");
            }
        }

        return Result.Success(results.Select(x => x.Value!));
    }

    private static async Task<Result<IEnumerable<TypeBase>>> ProcessModelsResultAsync(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, Func<TypeBase, Task<Result<TypeBase>>> successTask, string resultType)
    {
        Guard.IsNotNull(modelsResultTask);

        var modelsResult = await modelsResultTask.ConfigureAwait(false);

        return await ProcessModelsResultAsync(modelsResult, Task.FromResult(Result.Continue<PipelineSettings>()), async _ => await modelsResult.Value!.SelectAsync(x => successTask(x)).ConfigureAwait(false), resultType).ConfigureAwait(false);
    }

    private static Task<Result<IEnumerable<TypeBase>>> ProcessModelsResultAsync(Result<IEnumerable<TypeBase>> modelsResult, Task<Result<PipelineSettings>> settingsTask, Func<PipelineSettings, Task<IEnumerable<Result<TypeBase>>>> successTask, string resultType)
    {
        Guard.IsNotNull(modelsResult);
        Guard.IsNotNull(settingsTask);
        Guard.IsNotNull(successTask);

        return modelsResult.OnSuccessAsync(
            () => ProcessSettingsResultAsync(settingsTask, async settings =>
            {
                var results = await successTask(settings).ConfigureAwait(false);

                return Result.Aggregate(results, Result.Success(results.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, $"Could not create {resultType}. See the inner results for more details."));
            }));
    }

    private static async Task<Result<T>> ProcessSettingsResultAsync<T>(Task<Result<PipelineSettings>> settingsTask, Func<PipelineSettings, Task<Result<T>>> successTask)
    {
        Guard.IsNotNull(settingsTask);
        Guard.IsNotNull(successTask);

        var settingsResult = await settingsTask.ConfigureAwait(false);
        if (!settingsResult.IsSuccessful())
        {
            return Result.Error<T>([settingsResult], "Could not create settings, see inner results for details");
        }

        return await successTask(settingsResult.Value!).ConfigureAwait(false);
    }

    private static bool DefaultCopyAttributePredicate(Domain.Attribute attribute)
        => attribute.Name != typeof(CsharpTypeNameAttribute).FullName;
}
