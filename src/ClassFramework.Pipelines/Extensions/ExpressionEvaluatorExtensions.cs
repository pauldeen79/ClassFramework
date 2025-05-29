namespace ClassFramework.Pipelines.Extensions;

public static class ExpressionEvaluatorExtensions
{
    public static Task<Result<GenericFormattableString>> EvaluateAsync(this IExpressionEvaluator instance, string formatString, IFormatProvider formatProvider, object context, CancellationToken token)
        => instance.EvaluateTypedAsync<GenericFormattableString>
        (
            new ExpressionEvaluatorContext
            (
                $"$\"{formatString}\"",
                new ExpressionEvaluatorSettingsBuilder().WithFormatProvider(formatProvider),
                instance,
                CreateState(context)
            ), token
        );

    private static IReadOnlyDictionary<string, Task<Result<object?>>> CreateState(object context)
    {
        var builder = new AsyncResultDictionaryBuilder<object?>();

        if (context is BuilderContext builderContext)
        {
            builder
                .Add("class", Result.Success<object?>(new ClassModel(builderContext.SourceModel)))
                .Add("collectionTypeName", Result.Success<object?>(builderContext.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(builderContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(builderContext.Settings))
                .Add("typename", Result.Success<object?>(builderContext.MapTypeName(builderContext.SourceModel.GetFullName(), string.Empty)))
                .Add("context", Result.Success<object?>(builderContext));
        }
        else if (context is BuilderExtensionContext builderExtensionContext)
        {
            builder
                .Add("class", Result.Success<object?>(new ClassModel(builderExtensionContext.SourceModel)))
                .Add("collectionTypeName", Result.Success<object?>(builderExtensionContext.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(builderExtensionContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(builderExtensionContext.Settings))
                .Add("typename", Result.Success<object?>(builderExtensionContext.MapTypeName(builderExtensionContext.SourceModel.GetFullName(), string.Empty)))
                .Add("context", Result.Success<object?>(builderExtensionContext));
        }
        else if (context is EntityContext entityContext)
        {
            builder
                .Add("class", Result.Success<object?>(new ClassModel(entityContext.SourceModel)))
                .Add("collectionTypeName", Result.Success<object?>(entityContext.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(entityContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(entityContext.Settings))
                .Add("typename", Result.Success<object?>(entityContext.MapTypeName(entityContext.SourceModel.GetFullName(), string.Empty)))
                .Add("context", Result.Success<object?>(entityContext));
        }
        else if (context is InterfaceContext interfaceContext)
        {
            builder
                .Add("class", Result.Success<object?>(new ClassModel(interfaceContext.SourceModel)))
                .Add("collectionTypeName", Result.Success<object?>(interfaceContext.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(interfaceContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(interfaceContext.Settings))
                .Add("typename", Result.Success<object?>(interfaceContext.MapTypeName(interfaceContext.SourceModel.GetFullName(), string.Empty)))
                .Add("context", Result.Success<object?>(interfaceContext));
        }
        else if (context is Reflection.ReflectionContext reflectionContext)
        {
            builder
                .Add("class", Result.Success<object?>(new ClassModel(reflectionContext.SourceModel)))
                .Add("collectionTypeName", Result.Success<object?>(reflectionContext.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(reflectionContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(reflectionContext.Settings))
                .Add("typename", Result.Success<object?>(reflectionContext.MapTypeName(reflectionContext.SourceModel.FullName, string.Empty)))
                .Add("context", Result.Success<object?>(reflectionContext));
        }
        else if (context is ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder)
        {
            builder
                .Add("class", Result.Success<object?>(parentChildContextBuilder.ParentContext.Request.SourceModel))
                .Add("property", Result.Success<object?>(parentChildContextBuilder.ChildContext))
                .Add("collectionTypeName", Result.Success<object?>(parentChildContextBuilder.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(parentChildContextBuilder.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(parentChildContextBuilder.Settings))
                .Add("typename", Result.Success<object?>(parentChildContextBuilder.ParentContext.Request.MapTypeName(parentChildContextBuilder.ParentContext.Request.SourceModel.GetFullName(), string.Empty)))
                .Add("context", Result.Success<object?>(parentChildContextBuilder.ParentContext.Request));
        }
        else if (context is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension)
        {
            builder
                .Add("class", Result.Success<object?>(parentChildContextBuilderExtension.ParentContext.Request.SourceModel))
                .Add("property", Result.Success<object?>(parentChildContextBuilderExtension.ChildContext))
                .Add("collectionTypeName", Result.Success<object?>(parentChildContextBuilderExtension.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(parentChildContextBuilderExtension.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(parentChildContextBuilderExtension.Settings))
                .Add("typename", Result.Success<object?>(parentChildContextBuilderExtension.ParentContext.Request.MapTypeName(parentChildContextBuilderExtension.ParentContext.Request.SourceModel.GetFullName(), string.Empty)))
                .Add("context", Result.Success<object?>(parentChildContextBuilderExtension.ParentContext.Request));
        }
        else if (context is ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity)
        {
            builder
                .Add("class", Result.Success<object?>(parentChildContextEntity.ParentContext.Request.SourceModel))
                .Add("property", Result.Success<object?>(parentChildContextEntity.ChildContext))
                .Add("collectionTypeName", Result.Success<object?>(parentChildContextEntity.Settings.CollectionTypeName))
                .Add("addMethodNameFormatString", Result.Success<object?>(parentChildContextEntity.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())))
                .Add("settings", Result.Success<object?>(parentChildContextEntity.Settings))
                .Add("typename", Result.Success<object?>(parentChildContextEntity.ParentContext.Request.MapTypeName(parentChildContextEntity.ParentContext.Request.SourceModel.GetFullName(), string.Empty)))
                .Add("context", Result.Success<object?>(parentChildContextEntity.ParentContext.Request));
        }
        else if (context is PropertyContext propertyContext)
        {
            builder
                .Add("property", Result.Success<object?>(propertyContext.SourceModel))
                .Add("settings", Result.Success<object?>(propertyContext.Settings))
                .Add("context", Result.Success<object?>(propertyContext));
        }
        else
        {
            builder
                .Add("class", Result.NotSupported<object?>($"Could not get class from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"))
                .Add("collectionTypeName", Result.NotSupported<object?>($"Could not get collection typename from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"))
                .Add("settings", Result.NotSupported<object?>($"Could not get settings from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"))
                .Add("context", Result.NotSupported<object?>($"Could not get context from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"));
        }

        return builder.BuildDeferred();
    }
}
