namespace ClassFramework.Pipelines.Extensions;

public static class ExpressionEvaluatorExtensions
{
    public static Task<Result<GenericFormattableString>> EvaluateInterpolatedStringAsync(this IExpressionEvaluator instance, string formatString, IFormatProvider formatProvider, object context, CancellationToken token)
        => instance.EvaluateTypedAsync<GenericFormattableString>
        (
            new ExpressionEvaluatorContext
            (
                $"$\"{formatString}\"",
                new ExpressionEvaluatorSettingsBuilder().WithFormatProvider(formatProvider),
                instance,
                CreateState(context)
            ),
            token
        );

    private static IReadOnlyDictionary<string, Func<Task<Result<object?>>>> CreateState(object context)
    {
        var builder = new AsyncResultDictionaryBuilder<object?>();

        if (context is BuilderContext builderContext)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(builderContext.SourceModel))
                .Add(ResultNames.CollectionTypeName, builderContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => builderContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, builderContext.Settings)
                .Add(ResultNames.TypeName, () => builderContext.MapTypeName(builderContext.SourceModel.GetFullName()))
                .Add(ResultNames.Context, builderContext);
        }
        else if (context is BuilderExtensionContext builderExtensionContext)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(builderExtensionContext.SourceModel))
                .Add(ResultNames.CollectionTypeName, builderExtensionContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => builderExtensionContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, builderExtensionContext.Settings)
                .Add(ResultNames.TypeName, () => builderExtensionContext.MapTypeName(builderExtensionContext.SourceModel.GetFullName()))
                .Add(ResultNames.Context, builderExtensionContext);
        }
        else if (context is EntityContext entityContext)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(entityContext.SourceModel))
                .Add(ResultNames.CollectionTypeName, entityContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => entityContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, entityContext.Settings)
                .Add(ResultNames.TypeName, () => entityContext.MapTypeName(entityContext.SourceModel.GetFullName()))
                .Add(ResultNames.Context, entityContext);
        }
        else if (context is InterfaceContext interfaceContext)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(interfaceContext.SourceModel))
                .Add(ResultNames.CollectionTypeName, interfaceContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => interfaceContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, interfaceContext.Settings)
                .Add(ResultNames.TypeName, () => interfaceContext.MapTypeName(interfaceContext.SourceModel.GetFullName()))
                .Add(ResultNames.Context, interfaceContext);
        }
        else if (context is Reflection.ReflectionContext reflectionContext)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(reflectionContext.SourceModel))
                .Add(ResultNames.CollectionTypeName, reflectionContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => reflectionContext.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, reflectionContext.Settings)
                .Add(ResultNames.TypeName, () => reflectionContext.MapTypeName(reflectionContext.SourceModel.FullName))
                .Add(ResultNames.Context, reflectionContext);
        }
        else if (context is ParentChildContext<BuilderContext, Property> parentChildContextBuilder)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(parentChildContextBuilder.ParentContext.SourceModel))
                .Add(ResultNames.Property, parentChildContextBuilder.ChildContext)
                .Add(ResultNames.CollectionTypeName, parentChildContextBuilder.ParentContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => parentChildContextBuilder.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, parentChildContextBuilder.Settings)
                .Add(ResultNames.TypeName, () => parentChildContextBuilder.ParentContext.MapTypeName(parentChildContextBuilder.ChildContext.TypeName))
                .Add(ResultNames.Context, parentChildContextBuilder.ParentContext);
        }
        else if (context is ParentChildContext<BuilderExtensionContext, Property> parentChildContextBuilderExtension)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(parentChildContextBuilderExtension.ParentContext.SourceModel))
                .Add(ResultNames.Property, parentChildContextBuilderExtension.ChildContext)
                .Add(ResultNames.CollectionTypeName, parentChildContextBuilderExtension.ParentContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => parentChildContextBuilderExtension.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, parentChildContextBuilderExtension.Settings)
                .Add(ResultNames.TypeName, () => parentChildContextBuilderExtension.ParentContext.MapTypeName(parentChildContextBuilderExtension.ChildContext.TypeName))
                .Add(ResultNames.Context, parentChildContextBuilderExtension.ParentContext);
        }
        else if (context is ParentChildContext<EntityContext, Property> parentChildContextEntity)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(parentChildContextEntity.ParentContext.SourceModel))
                .Add(ResultNames.Property, parentChildContextEntity.ChildContext)
                .Add(ResultNames.CollectionTypeName, parentChildContextEntity.ParentContext.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => parentChildContextEntity.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, parentChildContextEntity.Settings)
                .Add(ResultNames.TypeName, () => parentChildContextEntity.ParentContext.MapTypeName(parentChildContextEntity.ChildContext.TypeName))
                .Add(ResultNames.Context, parentChildContextEntity.ParentContext);
        }
        else
        {
            builder
                .Add(ResultNames.Class, Result.NotSupported<object?>($"Could not get class from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"))
                .Add(ResultNames.CollectionTypeName, Result.NotSupported<object?>($"Could not get collection typename from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"))
                .Add(ResultNames.Settings, Result.NotSupported<object?>($"Could not get settings from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"))
                .Add(ResultNames.Context, Result.NotSupported<object?>($"Could not get context from state, because the context type {context?.GetType().FullName ?? "null"} is not supported"));
        }

        return builder.BuildDeferred();
    }
}
