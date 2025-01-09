namespace ClassFramework.Pipelines.Variables;

internal static class VariableBase
{
    internal static Result<object?> GetValueFromProperty(IObjectResolver objectResolver, object? context, Func<PipelineSettings, CultureInfo, Property, string, MappedContextBase, object?> valueDelegate)
    {
        var results = new ResultDictionaryBuilder<object?>()
            .Add(nameof(Property), () => objectResolver.Resolve<Property>(context).TryCast<object?>())
            .Add(nameof(PipelineSettings), () => objectResolver.Resolve<PipelineSettings>(context).TryCast<object?>())
            .Add(nameof(CultureInfo), () => objectResolver.Resolve<CultureInfo>(context).TryCast<object?>())
            .Add(nameof(ITypeNameMapper), () => objectResolver.Resolve<ITypeNameMapper>(context).TryCast<object?>())
            .Add(nameof(MappedContextBase), () => objectResolver.Resolve<MappedContextBase>(context).TryCast<object?>())
            .Build();

        var error = results.GetError();
        if (error is not null)
        {
            // Error in resolving dependencies
            return Result.FromExistingResult<object?>(error);
        }

        var property = (Property)results[nameof(Property)].Value!;
        var pipelineSettings = (PipelineSettings)results[nameof(PipelineSettings)].Value!;
        var cultureInfo = (CultureInfo)results[nameof(CultureInfo)].Value!;
        var typeNameMapper = (ITypeNameMapper)results[nameof(ITypeNameMapper)].Value!;
        var mappedContextBase = (MappedContextBase)results[nameof(MappedContextBase)].Value!;

        return Result.Success(valueDelegate(pipelineSettings, cultureInfo, property, typeNameMapper.MapTypeName(property.TypeName), mappedContextBase));
    }

    internal static Result<object?> GetValueFromSettings(IObjectResolver objectResolver, object? context, Func<PipelineSettings, object?> valueDelegate)
    {
        var pipelineSettingsResult = objectResolver.Resolve<PipelineSettings>(context);
        if (!pipelineSettingsResult.IsSuccessful())
        {
            return Result.FromExistingResult<object?>(pipelineSettingsResult);
        }

        return Result.Success(valueDelegate(pipelineSettingsResult.Value!));
    }
}
