namespace ClassFramework.Pipelines.Variables;

internal static class VariableBase
{
    internal static Result<object?> GetValueFromProperty(IObjectResolver objectResolver, object? context, Func<PipelineSettings, CultureInfo, Property, string, object?> valueDelegate)
    {
        //TODO: Refactor to use getting the results lazy, and getting the first non-successful result and return that
        var propertyResult = objectResolver.Resolve<Property>(context);
        if (!propertyResult.IsSuccessful())
        {
            return Result.FromExistingResult<object?>(propertyResult);
        }

        var pipelineSettingsResult = objectResolver.Resolve<PipelineSettings>(context);
        if (!pipelineSettingsResult.IsSuccessful())
        {
            return Result.FromExistingResult<object?>(pipelineSettingsResult);
        }

        var cultureInfoResult = objectResolver.Resolve<CultureInfo>(context);
        if (!cultureInfoResult.IsSuccessful())
        {
            return Result.FromExistingResult<object?>(cultureInfoResult);
        }

        var typeNameMapperResult = objectResolver.Resolve<ITypeNameMapper>(context);
        if (!typeNameMapperResult.IsSuccessful())
        {
            return Result.FromExistingResult<object?>(typeNameMapperResult);
        }
        
        return Result.Success(valueDelegate(pipelineSettingsResult.Value!, cultureInfoResult.Value!, propertyResult.Value!, typeNameMapperResult.Value!.MapTypeName(propertyResult.Value!.TypeName)));
    }
}
