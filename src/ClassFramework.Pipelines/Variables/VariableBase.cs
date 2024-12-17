namespace ClassFramework.Pipelines.Variables;

internal static class VariableBase
{
    internal static Result<object?> GetValueFromProperty(IObjectResolver objectResolver, object? context, Func<PipelineSettings, CultureInfo, Property, string, MappedContextBase, object?> valueDelegate)
    {
        var resultSetBuilder = new NamedResultSetBuilder<object?>();
        resultSetBuilder.Add(nameof(Property), () => objectResolver.Resolve<Property>(context).TryCast<object?>());
        resultSetBuilder.Add(nameof(PipelineSettings), () => objectResolver.Resolve<PipelineSettings>(context).TryCast<object?>());
        resultSetBuilder.Add(nameof(CultureInfo), () => objectResolver.Resolve<CultureInfo>(context).TryCast<object?>());
        resultSetBuilder.Add(nameof(ITypeNameMapper), () => objectResolver.Resolve<ITypeNameMapper>(context).TryCast<object?>());
        resultSetBuilder.Add(nameof(MappedContextBase), () => objectResolver.Resolve<MappedContextBase>(context).TryCast<object?>());
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in resolving dependencies
            return Result.FromExistingResult<object?>(error.Result);
        }

        var property = (Property)results.First(x => x.Name == nameof(Property)).Result.Value!;
        var pipelineSettings = (PipelineSettings)results.First(x => x.Name == nameof(PipelineSettings)).Result.Value!;
        var cultureInfo = (CultureInfo)results.First(x => x.Name == nameof(CultureInfo)).Result.Value!;
        var typeNameMapper = (ITypeNameMapper)results.First(x => x.Name == nameof(ITypeNameMapper)).Result.Value!;
        var mappedContextBase = (MappedContextBase)results.First(x => x.Name == nameof(MappedContextBase)).Result.Value!;

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
