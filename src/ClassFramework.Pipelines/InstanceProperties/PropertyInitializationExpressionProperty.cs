namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("InitializationExpression")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyInitializationExpressionProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add(Constants.Instance, context.GetInstanceValueResult<Property>())
            .Add(ResultNames.TypeName, () => context.GetTypeNameAsync())
            .Add(ResultNames.Settings, () => context.GetSettingsAsync())
            .Add(ResultNames.Context, () => context.GetMappedContextBaseAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => GetInitializationExpression(results.GetValue<Property>(Constants.Instance), results.GetValue<string>(ResultNames.TypeName), results.GetValue<PipelineSettings>(ResultNames.Settings)));
    }

    private static string GetInitializationExpression(Property property, string typeName, PipelineSettings settings)
        => typeName.FixTypeName().IsCollectionTypeName()
            && (settings.CollectionTypeName.Length == 0 || settings.CollectionTypeName != property.TypeName.WithoutGenerics())
                ? GetCollectionFormatStringForInitialization(property, settings)
                : "{CsharpFriendlyName(property.Name.ToCamelCase())}{property.NullableRequiredSuffix}";

    private static string GetCollectionFormatStringForInitialization(Property property, PipelineSettings settings)
        => property.IsNullable || (settings.AddNullChecks && settings.ValidateArguments != ArgumentValidationType.None)
            ? $"{{property.Name.ToCamelCase()}} {{NullCheck()}} ? null{GetPropertyInitializationNullSuffix(property, settings)} : new {{collectionTypeName}}<{{GenericArguments(property.TypeName)}}>({{CsharpFriendlyName(property.Name.ToCamelCase())}})"
            : $"new {{collectionTypeName}}<{{GenericArguments(property.TypeName)}}>({{CsharpFriendlyName(property.Name.ToCamelCase())}})";

    private static string GetPropertyInitializationNullSuffix(Property property, PipelineSettings settings)
        => settings.EnableNullableReferenceTypes && !property.IsNullable
            ? "!"
            : string.Empty;
}
