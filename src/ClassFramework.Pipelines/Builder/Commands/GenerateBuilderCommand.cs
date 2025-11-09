namespace ClassFramework.Pipelines.Builder.Commands;

public class GenerateBuilderCommand(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : CommandBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public bool IsBuilderForAbstractEntity => Settings.EnableInheritance && (Settings.BaseClass is null || Settings.IsAbstract);
    public bool IsBuilderForOverrideEntity => Settings.EnableInheritance && Settings.BaseClass is not null;
    public bool IsAbstractBuilder => Settings.EnableBuilderInheritance && (Settings.BaseClass is null || Settings.IsAbstract) && !Settings.IsForAbstractBuilder;

    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public string[] CreatePragmaWarningDisableStatementsForBuildMethod()
        => NeedsPragmasForBuildMethod()
            ?
            [
                "#pragma warning disable CS8604 // Possible null reference argument.",
                "#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.",
            ]
            : [];

    public string[] CreatePragmaWarningRestoreStatementsForBuildMethod()
        => NeedsPragmasForBuildMethod()
            ?
            [
                "#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.",
                "#pragma warning restore CS8604 // Possible null reference argument.",
            ]
            : [];

    public bool HasBackingFields()
        => !(IsAbstractBuilder || !Settings.AddNullChecks)
        || Settings.AddBackingFields
        || Settings.CreateAsObservable;

    public bool IsValidForFluentMethod(Property property)
    {
        property = property.IsNotNull(nameof(property));

        if (string.IsNullOrEmpty(property.ParentTypeFullName))
        {
            return true;
        }

        return Settings.SkipNamespacesOnFluentBuilderMethods.Count == 0
            || !Settings.SkipNamespacesOnFluentBuilderMethods.Contains(property.ParentTypeFullName.GetNamespaceWithDefault());
    }

    public string ReturnValueStatementForFluentMethod => $"return {ReturnValue};";

    public string BuildReturnTypeName
    {
        get
        {
            if (IsBuilderForAbstractEntity || IsBuilderForOverrideEntity)
            {
                return MapTypeName(SourceModel.GetFullName());
            }

            return MapTypeName(SourceModel.Interfaces.FirstOrDefault(x => x.GetClassName() == $"I{SourceModel.Name}") ?? SourceModel.GetFullName());
        }
    }

    public string GetReturnTypeForFluentMethod(string builderNamespace, string builderName)
        => IsBuilderForAbstractEntity
            ? $"TBuilder{SourceModel.GetGenericTypeArgumentsString()}"
            : $"{builderNamespace.AppendWhenNotNullOrEmpty(".")}{builderName}{SourceModel.GetGenericTypeArgumentsString()}";

    public async Task<Result<T>[]> GetInterfaceResultsAsync<T>(
        Func<string, GenericFormattableString, T> namespaceTransformation,
        Func<string, T> noNamespaceTransformation,
        IExpressionEvaluator evaluator,
        bool includeNonAbstractionNamespaces,
        CancellationToken token)
    {
        namespaceTransformation = ArgumentGuard.IsNotNull(namespaceTransformation, nameof(namespaceTransformation));
        noNamespaceTransformation = ArgumentGuard.IsNotNull(noNamespaceTransformation, nameof(noNamespaceTransformation));
        evaluator = ArgumentGuard.IsNotNull(evaluator, nameof(evaluator));

        var results = new List<Result<T>>();

        foreach (var @interface in SourceModel.Interfaces
            .Where(x => (Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                        && (includeNonAbstractionNamespaces || Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()))))
        {
            var metadata = GetMappingMetadata(@interface).ToArray();
            var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

            if (!string.IsNullOrEmpty(ns))
            {
                var property = new PropertyBuilder()
                    .WithName("Dummy")
                    .WithTypeName(@interface)
                    .Build();

                var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "I{NoGenerics(ClassName(property.TypeName))}Builder{GenericArguments(property.TypeName, true)}");
                var newFullName = $"{ns}.{newTypeName}";

                var result = (await evaluator.EvaluateInterpolatedStringAsync
                (
                    newFullName,
                    FormatProvider,
                    new ParentChildContext<GenerateBuilderCommand, Property>(this, property, Settings),
                    token
                ).ConfigureAwait(false)).Transform(y => namespaceTransformation(@interface, y));

                results.Add(result);

                if (!result.IsSuccessful())
                {
                    break;
                }
            }
            else
            {
                results.Add(Result.Success(noNamespaceTransformation(@interface)));
            }
        }

        if (Settings.UseCrossCuttingInterfaces && typeof(T) == typeof(string))
        {
            results.Add(Result.FromExistingResult<T>(Result.Success(GetBuilderInterface())));
        }

        return results.ToArray();
    }

    public IEnumerable<MethodBuilder> GetFluentMethodsForCollectionProperty(Property property, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string returnType, string typeNameKey, string enumerableOverloadKeyPrefix, string arrayOverloadKeyPrefix)
    {
        yield return new MethodBuilder()
            .WithName(results.GetValue(ResultNames.AddMethodName))
            .WithReturnTypeName(returnType)
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey).ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
            .AddCodeStatements(results.Where(x => x.Key.StartsWith(enumerableOverloadKeyPrefix)).Select(x => x.Value.Value!.ToString()));

        yield return new MethodBuilder()
            .WithName(results.GetValue(ResultNames.AddMethodName))
            .WithReturnTypeName(returnType)
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey).ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
            .AddCodeStatements(results.Where(x => x.Key.StartsWith(arrayOverloadKeyPrefix)).Select(x => x.Value.Value!.ToString()));
    }

    public IEnumerable<MethodBuilder> GetFluentMethodsForNonCollectionProperty(Property property, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string returnType, string typeNameKey, string expressionKey)
    {
        yield return new MethodBuilder()
            .WithName(results.GetValue("MethodName"))
            .WithReturnTypeName(returnType)
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey)))
            .Chain(method => AddNullChecks(method, results))
            .AddCodeStatements
            (
                results.GetValue(expressionKey),
                ReturnValueStatementForFluentMethod
            );
    }

    public async Task<Result<GenericFormattableString>> CreateEntityInstanciationAsync(
        IExpressionEvaluator evaluator,
        ICsharpExpressionDumper csharpExpressionDumper,
        string classNameSuffix,
        CancellationToken token)
    {
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        var customEntityInstanciation = GetMappingMetadata(SourceModel.GetFullName())
            .GetStringValue(MetadataNames.CustomBuilderEntityInstanciation);
        if (!string.IsNullOrEmpty(customEntityInstanciation))
        {
            return await evaluator.EvaluateInterpolatedStringAsync(customEntityInstanciation, FormatProvider, this, token).ConfigureAwait(false);
        }

        if (SourceModel is not IConstructorsContainer constructorsContainer)
        {
            return Result.Invalid<GenericFormattableString>("Cannot create an instance of a type that does not have constructors");
        }

        if (SourceModel is Class cls && cls.Abstract)
        {
            return Result.Invalid<GenericFormattableString>("Cannot create an instance of an abstract class");
        }

        var hasPublicParameterlessConstructor = constructorsContainer.HasPublicParameterlessConstructor();
        var openSign = GetBuilderPocoOpenSign(hasPublicParameterlessConstructor && SourceModel.Properties.Count != 0);
        var closeSign = GetBuilderPocoCloseSign(hasPublicParameterlessConstructor && SourceModel.Properties.Count != 0);

        var parametersResult = await GetConstructionMethodParametersAsync(evaluator, csharpExpressionDumper, hasPublicParameterlessConstructor, token).ConfigureAwait(false);
        if (!parametersResult.IsSuccessful())
        {
            return parametersResult;
        }

        var entityNamespace = GetMappingMetadata(SourceModel.GetFullName()).GetStringValue(MetadataNames.CustomEntityNamespace, () => SourceModel.Namespace);
        var ns = MapNamespace(entityNamespace).AppendWhenNotNullOrEmpty(".");

        return Result.Success<GenericFormattableString>($"new {ns}{SourceModel.Name}{classNameSuffix}{SourceModel.GetGenericTypeArgumentsString()}{openSign}{parametersResult.Value}{closeSign}");
    }

    private string ReturnValue
    {
        get
        {
            if (IsBuilderForAbstractEntity)
            {
                return "(TBuilder)this";
            }

            return "this";
        }
    }

    private bool NeedsPragmasForBuildMethod()
        => Settings.EnableNullableReferenceTypes
        && !IsBuilderForAbstractEntity
        && !Settings.AddNullChecks;

    private string GetBuilderInterface()
    {
        if (IsBuilderForAbstractEntity)
        {
            var baseClass = Settings.BaseClass ?? SourceModel;
            return typeof(IBuilder<object>).ReplaceGenericTypeName($"{MapTypeName(baseClass.GetFullName())}{baseClass.GetGenericTypeArgumentsString()}");
        }
        else
        {
            return typeof(IBuilder<object>).ReplaceGenericTypeName(BuildReturnTypeName);
        }
    }

    private async Task<Result<GenericFormattableString>> GetConstructionMethodParametersAsync(IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper, bool hasPublicParameterlessConstructor, CancellationToken token)
    {
        var properties = SourceModel.GetBuilderConstructorProperties(this);

        var results = new List<ConstructionMethodParameterInfo>();
        foreach (var property in properties)
        {
            var useBuilderLazyValues = UseBuilderLazyValues(property.TypeName);
            var info =
                new ConstructionMethodParameterInfo(
                    property.Name,
                    property,
                    await evaluator.EvaluateInterpolatedStringAsync
                    (
                        GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderMethodParameterExpression, PlaceholderNames.NamePlaceholder),
                        FormatProvider,
                        new ParentChildContext<GenerateBuilderCommand, Property>(this, property, this.Settings),
                        token
                    ).ConfigureAwait(false),
                    GetMappingMetadata
                    (
                        property.TypeName.FixTypeName().WithoutGenerics() // i.e. List<> etc.
                    ).GetStringValue(MetadataNames.CustomCollectionInitialization, () => "[Expression]"),
                    property.GetSuffix(Settings.EnableNullableReferenceTypes, csharpExpressionDumper, this),
                    useBuilderLazyValues
                );

            results.Add(info);

            if (!info.Result.IsSuccessful())
            {
                break;
            }
        }

        var error = results.Find(x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            return error.Result;
        }

        return Result.Success<GenericFormattableString>(string.Join(", ", results.Select(x => $"{GetPrefix(hasPublicParameterlessConstructor, x.Name)}{GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix, x.UseBuilderLazyValues)}")));
    }

    private static string GetPrefix(bool hasPublicParameterlessConstructor, string propertyName)
        => hasPublicParameterlessConstructor
            ? $"{propertyName} = "
            : string.Empty;

    private static string? GetBuilderPropertyExpression(string? value, Property sourceProperty, string collectionInitializer, string suffix, bool useBuilderLazyValues)
    {
        if (value is null || !value.Contains(PlaceholderNames.NamePlaceholder))
        {
            return value;
        }

        var lazySuffix = GetLazySuffix(sourceProperty, useBuilderLazyValues);

        if (value == PlaceholderNames.NamePlaceholder)
        {
            return sourceProperty.Name + lazySuffix;
        }

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            var valueExpression = value!
                .Replace(PlaceholderNames.NamePlaceholder, "x")
                .Replace("[NullableSuffix]", string.Empty)
                .Replace("[ForcedNullableSuffix]", sourceProperty.IsValueType
                    ? string.Empty
                    : "!");

            return collectionInitializer
                .Replace("[Type]", sourceProperty.TypeName.FixTypeName().WithoutGenerics())
                .Replace("[Generics]", sourceProperty.TypeName.FixTypeName().GetGenericArguments(addBrackets: true))
                .Replace("[Expression]", $"{sourceProperty.Name}{suffix}.Select(x => {valueExpression}{lazySuffix})");
        }
        else
        {
            var valueExpression = value!
                .Replace(PlaceholderNames.NamePlaceholder, sourceProperty.Name)
                .Replace("[NullableSuffix]", suffix)
                .Replace("[ForcedNullableSuffix]", string.IsNullOrEmpty(suffix)
                    ? string.Empty
                    : "!");

            return $"{valueExpression}{lazySuffix}";
        }
    }

    private static string GetLazySuffix(Property sourceProperty, bool useBuilderLazyValues)
    {
        if (!useBuilderLazyValues)
        {
            return string.Empty;
        }

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            // for a collection property, we need to call Invoke using () on each item
            return ".Select(x => x())";
        }

        // for a non collection property, we can just call the Invoke using ()
        return "()";
    }

    private static string GetBuilderPocoCloseSign(bool poco)
        => poco
            ? " }"
            : ")";

    private static string GetBuilderPocoOpenSign(bool poco)
        => poco
            ? " { "
            : "(";

    public override bool SourceModelHasNoProperties() => SourceModel.Properties.Count == 0;

    public override async Task<Result<TypeBaseBuilder>> ExecuteCommandAsync<TContext>(ICommandService commandService, TContext command, CancellationToken token)
    {
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));
        command = ArgumentGuard.IsNotNull(command, nameof(command));

        return (await commandService.ExecuteAsync<TContext, ClassBuilder>(command, token).ConfigureAwait(false))
            .TryCast<TypeBaseBuilder>();
    }
}
