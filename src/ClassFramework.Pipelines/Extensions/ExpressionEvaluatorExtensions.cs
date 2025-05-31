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
                .Add("class", new ClassModel(builderContext.SourceModel))
                .Add("collectionTypeName", builderContext.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => builderContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", builderContext.Settings)
                .Add("typename", () => builderContext.MapTypeName(builderContext.SourceModel.GetFullName()))
                .Add("context", builderContext);
        }
        else if (context is BuilderExtensionContext builderExtensionContext)
        {
            builder
                .Add("class", new ClassModel(builderExtensionContext.SourceModel))
                .Add("collectionTypeName", builderExtensionContext.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => builderExtensionContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", builderExtensionContext.Settings)
                .Add("typename", () => builderExtensionContext.MapTypeName(builderExtensionContext.SourceModel.GetFullName()))
                .Add("context", builderExtensionContext);
        }
        else if (context is EntityContext entityContext)
        {
            builder
                .Add("class", new ClassModel(entityContext.SourceModel))
                .Add("collectionTypeName", entityContext.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => entityContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", entityContext.Settings)
                .Add("typename", () => entityContext.MapTypeName(entityContext.SourceModel.GetFullName()))
                .Add("context", entityContext);
        }
        else if (context is InterfaceContext interfaceContext)
        {
            builder
                .Add("class", new ClassModel(interfaceContext.SourceModel))
                .Add("collectionTypeName", interfaceContext.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => interfaceContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", interfaceContext.Settings)
                .Add("typename", () => interfaceContext.MapTypeName(interfaceContext.SourceModel.GetFullName()))
                .Add("context", interfaceContext);
        }
        else if (context is Reflection.ReflectionContext reflectionContext)
        {
            builder
                .Add("class", new ClassModel(reflectionContext.SourceModel))
                .Add("collectionTypeName", reflectionContext.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => reflectionContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", reflectionContext.Settings)
                .Add("typename", () => reflectionContext.MapTypeName(reflectionContext.SourceModel.FullName))
                .Add("context", reflectionContext);
        }
        else if (context is ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder)
        {
            builder
                .Add("class", new ClassModel(parentChildContextBuilder.ParentContext.Request.SourceModel))
                .Add("property", parentChildContextBuilder.ChildContext)
                .Add("collectionTypeName", parentChildContextBuilder.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => parentChildContextBuilder.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", parentChildContextBuilder.Settings)
                .Add("typename", () => parentChildContextBuilder.ParentContext.Request.MapTypeName(parentChildContextBuilder.ChildContext.TypeName))
                .Add("context", parentChildContextBuilder.ParentContext.Request);
        }
        else if (context is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension)
        {
            builder
                .Add("class", new ClassModel(parentChildContextBuilderExtension.ParentContext.Request.SourceModel))
                .Add("property", parentChildContextBuilderExtension.ChildContext)
                .Add("collectionTypeName", parentChildContextBuilderExtension.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => parentChildContextBuilderExtension.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", parentChildContextBuilderExtension.Settings)
                .Add("typename", () => parentChildContextBuilderExtension.ParentContext.Request.MapTypeName(parentChildContextBuilderExtension.ChildContext.TypeName))
                .Add("context", parentChildContextBuilderExtension.ParentContext.Request);
        }
        else if (context is ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity)
        {
            builder
                .Add("class", new ClassModel(parentChildContextEntity.ParentContext.Request.SourceModel))
                .Add("property", parentChildContextEntity.ChildContext)
                .Add("collectionTypeName", parentChildContextEntity.Settings.CollectionTypeName)
                .Add("addMethodNameFormatString", () => parentChildContextEntity.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add("settings", parentChildContextEntity.Settings)
                .Add("typename", () => parentChildContextEntity.ParentContext.Request.MapTypeName(parentChildContextEntity.ChildContext.TypeName))
                .Add("context", parentChildContextEntity.ParentContext.Request);
        }
        else if (context is PropertyContext propertyContext)
        {
            builder
                .Add("property", propertyContext.SourceModel)
                .Add("settings", propertyContext.Settings)
                .Add("typename", () => propertyContext.MapTypeName(propertyContext.SourceModel.TypeName))
                .Add("context", propertyContext);
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
