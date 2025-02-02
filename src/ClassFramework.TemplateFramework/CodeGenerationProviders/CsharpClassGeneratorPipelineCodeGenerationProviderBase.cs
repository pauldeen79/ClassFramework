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
            .WithSkipWhenFileExists(SkipWhenFileExists);

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
    protected virtual bool UseBuilderAbstractionsTypeConversion => false;
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
    protected virtual Task<Result<TypeBase>> GetBaseClass() => Task.FromResult(Result.NotFound<TypeBase>());
    protected virtual bool IsAbstract => false;
    protected virtual string BaseClassBuilderNamespace => string.Empty;
    protected virtual bool AllowGenerationWithoutProperties => true;
    protected virtual string SetMethodNameFormatString => "With{$property.Name}";
    protected virtual string AddMethodNameFormatString => "Add{$property.Name}";
    protected virtual string ToBuilderFormatString => "ToBuilder";
    protected virtual string ToTypedBuilderFormatString => "ToTypedBuilder";
    protected virtual string BuildMethodName => "Build";
    protected virtual string BuildTypedMethodName => "BuildTyped";
    protected virtual bool AddFullConstructor => true;
    protected virtual bool AddPublicParameterlessConstructor => false;
    protected virtual bool AddCopyConstructor => true;
    protected virtual bool AddImplicitOperatorOnBuilder => false;
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
                || (parentNameContainer.ParentTypeFullName.StartsWith($"{RootNamespace}.") && typeBase.Namespace.In(CoreNamespace, $"{RootNamespace}.Builders", $"{RootNamespace}.Builders.Abstractions"))
            );

    protected virtual string[] GetModelAbstractBaseTyped() => [];

    protected virtual string[] GetExternalCustomBuilderTypes() => [];

    protected virtual string[] GetCustomBuilderTypes()
        => GetPureAbstractModels()
            .Select(x => x.GetEntityClassName())
            .Concat(GetExternalCustomBuilderTypes())
            .ToArray();

    protected virtual string[] GetBuilderAbstractionsTypeConversionNamespaces() => [];

    protected virtual IEnumerable<Type> GetPureAbstractModels()
        => GetType().Assembly.GetTypes().Where(IsAbstractType);

    protected Task<Result<IEnumerable<TypeBase>>> GetEntities(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string entitiesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(entitiesNamespace);

        return ProcessModelsResult(modelsResultTask, x => CreateEntity(x, entitiesNamespace), "entities");
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetEntityInterfaces(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return await ProcessModelsResult(modelsResultTask, async x => await CreateInterface(await CreateEntity(x, entitiesNamespace).ConfigureAwait(false), interfacesNamespace, string.Empty, true, "I{$class.Name}", (t, m) => InheritFromInterfaces && m.Name == ToBuilderFormatString && t.Interfaces.Count == 0).ConfigureAwait(false), "interfaces").ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetBuilders(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return await ProcessModelsResult(modelsResultTask, CreateEntityPipelineSettings(entitiesNamespace), async (settings, x) =>
        {
            var context = new EntityContext(x, settings, Settings.CultureInfo);
            var entityResult = await PipelineService.ProcessAsync(context).ConfigureAwait(false);
            return await CreateBuilderClass(entityResult, buildersNamespace, entitiesNamespace).ConfigureAwait(false);
        }, "builders").ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetBuilderExtensions(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(buildersExtensionsNamespace);

        return await ProcessBaseClassResult(baseClass => ProcessModelsResult(modelsResultTask, CreateInterfacePipelineSettings(entitiesNamespace, string.Empty, CreateInheritanceComparisonDelegate(baseClass), null, true), async (settings, x) =>
        {
            var context = new InterfaceContext(x, settings, Settings.CultureInfo);
            var interfaceResult = await PipelineService.ProcessAsync(context).ConfigureAwait(false);
            return await CreateBuilderExtensionsClass(interfaceResult.TryCast<TypeBase>(), buildersNamespace, entitiesNamespace, buildersExtensionsNamespace).ConfigureAwait(false);
        }, "builder extensions")).ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetNonGenericBuilders(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);

        return await ProcessModelsResult(modelsResultTask, CreateEntityPipelineSettings(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks()), async (settings, x) =>
        {
            var context = new EntityContext(x, settings, Settings.CultureInfo);
            var typeBaseResult = await PipelineService.ProcessAsync(context).ConfigureAwait(false);
            return await CreateNonGenericBuilderClass(typeBaseResult, buildersNamespace, entitiesNamespace).ConfigureAwait(false);
        }, "non generic builders").ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetBuilderInterfaces(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, string buildersNamespace, string entitiesNamespace, string interfacesNamespace)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(buildersNamespace);
        Guard.IsNotNull(entitiesNamespace);
        Guard.IsNotNull(interfacesNamespace);

        return await ProcessModelsResult
        (
            GetBuilders(modelsResultTask, buildersNamespace, entitiesNamespace),
            async x => await CreateInterface(Result.Success(x.ToBuilder().Chain(y => { var itemsToDelete = y.GenericTypeArguments.Where(z => z == "TEntity" || z == "TBuilder").ToList(); itemsToDelete.ForEach(z => y.GenericTypeArguments.Remove(z)); y.GenericTypeArgumentConstraints.Clear(); }).Build()), interfacesNamespace, BuilderCollectionType.WithoutGenerics(), true, "I{$class.Name}", (t, m) => InheritFromInterfaces && m.Name == BuildMethodName && t.Interfaces.Count == 0).ConfigureAwait(false),
            "builder interfaces"
        ).ConfigureAwait(false);
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetCoreModels()
    {
        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectAsync(GetModel)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create core models. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetNonCoreModels(string @namespace)
    {
        Guard.IsNotNull(@namespace);

        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == @namespace && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectAsync(GetModel)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create non core models. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetAbstractionsInterfaces()
    {
        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions")
            .SelectAsync(GetModel)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract interfaces. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetAbstractModels()
    {
        var modelsResult = await GetPureAbstractModels()
            .SelectAsync(GetModel)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract models. See the inner results for more details."));
    }

    protected async Task<Result<IEnumerable<TypeBase>>> GetOverrideModels(Type abstractType)
    {
        Guard.IsNotNull(abstractType);

        var modelsResult = await GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && Array.Exists(x.GetInterfaces(), y => y.WithoutGenerics() == abstractType.WithoutGenerics()))
            .SelectAsync(GetModel)
            .ConfigureAwait(false);

        return Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create override models. See the inner results for more details."));
    }

    protected async Task<Result<TypeBase>> CreateBaseClass(Type type, string @namespace)
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(@namespace);

        return await ProcessBaseClassResult(PipelineService.ProcessAsync(new ReflectionContext(type, CreateReflectionPipelineSettings(), Settings.CultureInfo)), async typeBaseResult =>
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
               .WithInheritFromInterfaces(InheritFromInterfaces)
               .WithCopyAttributePredicate(CopyAttributePredicate ?? DefaultCopyAttributePredicate)
               .WithCopyInterfacePredicate(CopyInterfacePredicate)
               .WithCopyMethodPredicate(CopyMethodPredicate)
               .WithEntityNameFormatString("{NoInterfacePrefix($class.Name)}")
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
               .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull);

            return await PipelineService.ProcessAsync(new EntityContext(typeBaseResult!, entitySettings, Settings.CultureInfo)).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

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
                : null);

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
                new MetadataBuilder().WithValue("{NoGenerics(ClassName($property.TypeName))}Builder{GenericArguments($property.TypeName, true)}").WithName(MetadataNames.CustomBuilderInterfaceName),
                new MetadataBuilder().WithValue(InheritFromInterfaces ? $"{RootNamespace}.Builders" : $"{CoreNamespace}.Builders.Abstractions").WithName(MetadataNames.CustomBuilderParentTypeNamespace),
                new MetadataBuilder().WithValue("{NoGenerics(ClassName($property.ParentTypeFullName))}Builder{GenericArguments($property.ParentTypeFullName, true)}").WithName(MetadataNames.CustomBuilderParentTypeName)
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
        => [];

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
                            new MetadataBuilder().WithValue("{ClassName($property.TypeName)}Builder").WithName(MetadataNames.CustomBuilderName),
                            new MetadataBuilder().WithValue($"{ProjectName}.Abstractions.Builders").WithName(MetadataNames.CustomBuilderInterfaceNamespace),
                            new MetadataBuilder().WithValue("I{ClassName($property.TypeName)}Builder").WithName(MetadataNames.CustomBuilderInterfaceName),
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
            .Concat(GetType().Assembly.GetTypes()
            .Where(x => !InheritFromInterfaces && UseBuilderAbstractionsTypeConversion
                && x.IsInterface
                && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Abstractions"
                && !SkipNamespaceOnTypenameMappings(x.Namespace)
                && x.FullName is not null).SelectMany(x =>
                new[]
                {
                    new TypenameMappingBuilder()
                        .WithSourceTypeName($"{CoreNamespace}.Abstractions.I{x.GetEntityClassName()}")
                        .WithTargetTypeName($"{CoreNamespace}.Abstractions.I{x.GetEntityClassName()}")
                        .AddMetadata
                        (
                            new MetadataBuilder().WithValue($"{CoreNamespace}.Builders.Abstractions").WithName(MetadataNames.CustomBuilderNamespace),
                            new MetadataBuilder().WithValue("{NoGenerics(ClassName($property.TypeName))}Builder").WithName(MetadataNames.CustomBuilderName),
                            new MetadataBuilder().WithValue($"{CoreNamespace}.Builders.Abstractions").WithName(MetadataNames.CustomBuilderInterfaceNamespace),
                            new MetadataBuilder().WithValue("{NoGenerics(ClassName($property.TypeName))}Builder{GenericArguments(ClassName($property.TypeName), true)}").WithName(MetadataNames.CustomBuilderInterfaceName),
                            new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderSourceExpression),
                            new MetadataBuilder().WithValue(new Literal($"default({CoreNamespace}.Builders.Abstractions.{{NoGenerics(ClassName($property.TypeName))}}Builder{{GenericArguments(ClassName($property.TypeName), true)}})", null)).WithName(MetadataNames.CustomBuilderDefaultValue),
                            new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderMethodParameterExpression),
                            new MetadataBuilder().WithName(MetadataNames.CustomEntityInterfaceTypeName).WithValue($"{CoreNamespace}.Abstractions.I{x.GetEntityClassName()}")
                        ),
                    //Temporary fix for flaw in abstractions typename mapping
                    new TypenameMappingBuilder()
                        .WithSourceTypeName($"{CoreNamespace}.Abstractions.{x.GetEntityClassName()}")
                        .WithTargetTypeName($"{CoreNamespace}.Abstractions.I{x.GetEntityClassName()}"),
                    /*new TypenameMappingBuilder()
                        .WithSourceTypeName($"{CoreNamespace}.{x.GetEntityClassName()}Base")
                        .WithTargetTypeName($"{CoreNamespace}.Abstractions.I{x.GetEntityClassName()}"),
                    new TypenameMappingBuilder()
                        .WithSourceTypeName($"{CoreNamespace}.Builders.{x.GetEntityClassName()}Builder")
                        .WithTargetTypeName(IsAbstractType(x)
                            ? $"{CoreNamespace}.Builders.{x.GetEntityClassName()}BaseBuilder"
                            : $"{CoreNamespace}.Builders.Abstractions.I{x.GetEntityClassName().ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal)}Builder")*/
                }))
            .Concat(CreateAdditionalTypenameMappings());

    protected virtual IEnumerable<TypenameMappingBuilder> CreateAdditionalTypenameMappings()
         => [];

    protected virtual bool IsAbstractType(Type type)
    {
        Guard.IsNotNull(type);

        return type.IsInterface
            && type.Namespace == $"{CodeGenerationRootNamespace}.Models"
            && type.Name.WithoutTypeGenerics().EndsWith("Base");
    }

    protected virtual bool SkipNamespaceOnTypenameMappings(string @namespace) => false;

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
            new MetadataBuilder().WithValue(buildersNamespace).WithName(MetadataNames.CustomBuilderNamespace),
            new MetadataBuilder().WithValue("{ClassName($property.TypeName)}Builder").WithName(MetadataNames.CustomBuilderName),
            new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderSourceExpression),
            new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderMethodParameterExpression)
        ];
    }

    private Task<Result<PipelineSettings>> CreateEntityPipelineSettings(
        string entitiesNamespace,
        ArgumentValidationType? forceValidateArgumentsInConstructor = null,
        bool? overrideAddNullChecks = null,
        string entityNameFormatString = "{NoInterfacePrefix($class.Name)}")
        => ProcessBaseClassResult(baseClass => Task.FromResult(Result.Success(new PipelineSettingsBuilder()
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
            .WithUseBuilderAbstractionsTypeConversion(UseBuilderAbstractionsTypeConversion)
            .AddBuilderAbstractionsTypeConversionNamespaces(GetBuilderAbstractionsTypeConversionNamespaces())
            .Build())));

    private Task<Result<PipelineSettings>> CreateInterfacePipelineSettings(
        string interfacesNamespace,
        string newCollectionTypeName,
        InheritanceComparisonDelegate? inheritanceComparisonDelegate,
        CopyMethodPredicate? copyMethodPredicate,
        bool addSetters,
        string nameFormatString = "{$class.Name}")
        => ProcessBaseClassResult(baseClass => Task.FromResult(Result.Success(new PipelineSettingsBuilder()
            .WithNamespaceFormatString(interfacesNamespace)
            .WithNameFormatString(nameFormatString)
            .WithEnableInheritance(EnableEntityInheritance)
            .WithIsAbstract(IsAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
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
            .WithUseBuilderAbstractionsTypeConversion(UseBuilderAbstractionsTypeConversion)
            .AddBuilderAbstractionsTypeConversionNamespaces(GetBuilderAbstractionsTypeConversionNamespaces())
            .Build())));

    private Task<Result<PipelineSettings>> CreateBuilderPipelineSettings(string buildersNamespace, string entitiesNamespace)
        => ProcessSettingsResult(CreateEntityPipelineSettings(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks()),
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
                .Build())
            ));

    private Task<Result<PipelineSettings>> CreateBuilderInterfacePipelineSettings(string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
        => ProcessSettingsResult(CreateEntityPipelineSettings(entitiesNamespace, forceValidateArgumentsInConstructor: ArgumentValidationType.None, overrideAddNullChecks: GetOverrideAddNullChecks()),
            settings => Task.FromResult(Result.Success(new PipelineSettingsBuilder(settings)
                .WithBuilderNewCollectionTypeName(BuilderCollectionType.WithoutGenerics())
                .WithBuilderNamespaceFormatString(buildersNamespace)
                .WithBuilderExtensionsNamespaceFormatString(buildersExtensionsNamespace)
                .WithSetMethodNameFormatString(SetMethodNameFormatString)
                .WithAddMethodNameFormatString(AddMethodNameFormatString)
                .WithEnableBuilderInheritance(EnableBuilderInhericance)
                .Build())
            ));

    private Task<Result<TypeBase>> CreateEntity(TypeBase typeBase, string entitiesNamespace)
        => ProcessSettingsResult(
            CreateEntityPipelineSettings(entitiesNamespace, overrideAddNullChecks: GetOverrideAddNullChecks(), entityNameFormatString: "{NoInterfacePrefix($class.Name)}"),
            settings => PipelineService.ProcessAsync(new EntityContext(typeBase, settings, Settings.CultureInfo)));

    private Task<Result<TypeBase>> CreateBuilderClass(Result<TypeBase> typeBaseResult, string buildersNamespace, string entitiesNamespace)
        => typeBaseResult.OnSuccess(
            () => ProcessSettingsResult(
                CreateBuilderPipelineSettings(buildersNamespace, entitiesNamespace),
                settings => PipelineService.ProcessAsync(new BuilderContext(typeBaseResult.Value!, settings, Settings.CultureInfo)))
            );

    private Task<Result<TypeBase>> CreateBuilderExtensionsClass(Result<TypeBase> typeBaseResult, string buildersNamespace, string entitiesNamespace, string buildersExtensionsNamespace)
        => typeBaseResult.OnSuccess(
            () => ProcessSettingsResult(
                CreateBuilderInterfacePipelineSettings(buildersNamespace, entitiesNamespace, buildersExtensionsNamespace),
                settings => PipelineService.ProcessAsync(new BuilderExtensionContext(typeBaseResult.Value!, settings, Settings.CultureInfo)))
            );

    private Task<Result<TypeBase>> CreateNonGenericBuilderClass(Result<TypeBase> typeBaseResult, string buildersNamespace, string entitiesNamespace)
        => typeBaseResult.OnSuccess(
            () => ProcessSettingsResult(
                CreateBuilderPipelineSettings(buildersNamespace, entitiesNamespace),
                settings => PipelineService.ProcessAsync(new BuilderContext(typeBaseResult.Value!, settings.ToBuilder().WithIsForAbstractBuilder().Build(), Settings.CultureInfo)))
            );

    private bool? GetOverrideAddNullChecks()
    {
        if (AddNullChecks || ValidateArgumentsInConstructor == ArgumentValidationType.None)
        {
            return true;
        }

        return null;
    }

    private Task<Result<TypeBase>> GetModel(Type type)
        => PipelineService.ProcessAsync(new ReflectionContext(type, CreateReflectionPipelineSettings(), Settings.CultureInfo));

    private Task<Result<TypeBase>> CreateInterface(
        Result<TypeBase> typeBaseResult,
        string interfacesNamespace,
        string newCollectionTypeName,
        bool addSetters,
        string nameFormatString = "{$class.Name}",
        CopyMethodPredicate? copyMethodPredicate = null)
            => typeBaseResult.OnSuccess(
                () => ProcessBaseClassResult(
                    baseClass => ProcessSettingsResult(CreateInterfacePipelineSettings(interfacesNamespace, newCollectionTypeName, CreateInheritanceComparisonDelegate(baseClass), copyMethodPredicate, addSetters, nameFormatString), async settings =>
                        (await PipelineService.ProcessAsync(new InterfaceContext(typeBaseResult.Value!, settings, Settings.CultureInfo)).ConfigureAwait(false)).TryCast<TypeBase>()
                    )
                )
            );

    private Task<Result<T>> ProcessBaseClassResult<T>(Func<TypeBase?, Task<Result<T>>> successTask)
        => ProcessBaseClassResult(GetBaseClass(), successTask);

    private static async Task<Result<T>> ProcessBaseClassResult<T>(Task<Result<TypeBase>> baseClassResultTask, Func<TypeBase?, Task<Result<T>>> successTask)
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

    private static async Task<Result<IEnumerable<TypeBase>>> ProcessModelsResult(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, Task<Result<PipelineSettings>> settingsTask, Func<PipelineSettings, TypeBase, Task<Result<TypeBase>>> successTask, string resultType)
    {
        Guard.IsNotNull(modelsResultTask);
        Guard.IsNotNull(settingsTask);
        Guard.IsNotNull(successTask);

        var modelsResult = await modelsResultTask.ConfigureAwait(false);

        return await modelsResult.OnSuccess(
            async () =>
            {
                var results = await modelsResult.Value!.SelectAsync(x => ProcessSettingsResult(settingsTask, settings => successTask(settings, x))).ConfigureAwait(false);
                return Result.Aggregate
                (
                    results,
                    Result.Success(results.Select(x => x.Value!)),
                    y => Result.Error<IEnumerable<TypeBase>>(y, $"Could not create {resultType}. See the inner results for more details.")
                );
            }).ConfigureAwait(false);
    }

    private static async Task<Result<IEnumerable<TypeBase>>> ProcessModelsResult(Task<Result<IEnumerable<TypeBase>>> modelsResultTask, Func<TypeBase, Task<Result<TypeBase>>> successTask, string resultType)
    {
        Guard.IsNotNull(modelsResultTask);

        var modelsResult = await modelsResultTask.ConfigureAwait(false);

        return await ProcessModelsResult(modelsResult, Task.FromResult(Result.Continue<PipelineSettings>()), async _ => await modelsResult.Value!.SelectAsync(x => successTask(x)).ConfigureAwait(false), resultType).ConfigureAwait(false);
    }

    private static Task<Result<IEnumerable<TypeBase>>> ProcessModelsResult(Result<IEnumerable<TypeBase>> modelsResult, Task<Result<PipelineSettings>> settingsTask, Func<PipelineSettings, Task<IEnumerable<Result<TypeBase>>>> successTask, string resultType)
    {
        Guard.IsNotNull(modelsResult);
        Guard.IsNotNull(settingsTask);
        Guard.IsNotNull(successTask);

        return modelsResult.OnSuccess(
            () => ProcessSettingsResult(settingsTask, async settings =>
            {
                var results = await successTask(settings).ConfigureAwait(false);

                return Result.Aggregate(results, Result.Success(results.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, $"Could not create {resultType}. See the inner results for more details."));
            }));
    }

    private static async Task<Result<T>> ProcessSettingsResult<T>(Task<Result<PipelineSettings>> settingsTask, Func<PipelineSettings, Task<Result<T>>> successTask)
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

    private static bool DefaultCopyAttributePredicate(Domain.Attribute attribute)
        => attribute.Name != typeof(CsharpTypeNameAttribute).FullName;
}
