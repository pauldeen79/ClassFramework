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
            .Add("instance", context.GetInstanceValueResult<Property>())
            .Add("typeName", context.GetTypeNameAsync())
            .Add("settings", context.GetSettingsAsync())
            .Add("mappedContextBase", context.GetMappedContextBaseAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results => Result.Success<object?>(GetInitializationExpression(results.GetValue<Property>("instance"), results.GetValue<string>("typeName"), results.GetValue<PipelineSettings>("settings"))));
    }

    private static string GetInitializationExpression(Property property, string typeName, PipelineSettings settings)
        => typeName.FixTypeName().IsCollectionTypeName()
            && (settings.CollectionTypeName.Length == 0 || settings.CollectionTypeName != property.TypeName.WithoutGenerics())
                ? GetCollectionFormatStringForInitialization(property, settings)
                : "{CsharpFriendlyName(ToCamelCase(property.Name))}{property.NullableRequiredSuffix}";

    private static string GetCollectionFormatStringForInitialization(Property property, PipelineSettings settings)
        => property.IsNullable || (settings.AddNullChecks && settings.ValidateArguments != ArgumentValidationType.None)
            ? $"{{ToCamelCase(property.Name)}} {{NullCheck()}} ? null{GetPropertyInitializationNullSuffix(property, settings)} : new {{collectionTypeName}}<{{GenericArguments(property.TypeName)}}>({{CsharpFriendlyName(ToCamelCase(property.Name))}})"
            : $"new {{collectionTypeName}}<{{GenericArguments(property.TypeName)}}>({{CsharpFriendlyName(ToCamelCase(property.Name))}})";

    private static string GetPropertyInitializationNullSuffix(Property property, PipelineSettings settings)
        => settings.EnableNullableReferenceTypes && !property.IsNullable
            ? "!"
            : string.Empty;
}
