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

        if (context is GenerateBuilderCommand builderCommand)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(builderCommand.SourceModel))
                .Add(ResultNames.CollectionTypeName, builderCommand.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => builderCommand.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, builderCommand.Settings)
                .Add(ResultNames.TypeName, () => builderCommand.MapTypeName(builderCommand.SourceModel.GetFullName()))
                .Add(ResultNames.Context, builderCommand);
        }
        else if (context is GenerateBuilderExtensionCommand builderExtensionCommand)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(builderExtensionCommand.SourceModel))
                .Add(ResultNames.CollectionTypeName, builderExtensionCommand.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => builderExtensionCommand.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, builderExtensionCommand.Settings)
                .Add(ResultNames.TypeName, () => builderExtensionCommand.MapTypeName(builderExtensionCommand.SourceModel.GetFullName()))
                .Add(ResultNames.Context, builderExtensionCommand);
        }
        else if (context is GenerateEntityCommand entityCommand)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(entityCommand.SourceModel))
                .Add(ResultNames.CollectionTypeName, entityCommand.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => entityCommand.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, entityCommand.Settings)
                .Add(ResultNames.TypeName, () => entityCommand.MapTypeName(entityCommand.SourceModel.GetFullName()))
                .Add(ResultNames.Context, entityCommand);
        }
        else if (context is GenerateInterfaceCommand interfaceCommand)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(interfaceCommand.SourceModel))
                .Add(ResultNames.CollectionTypeName, interfaceCommand.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => interfaceCommand.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, interfaceCommand.Settings)
                .Add(ResultNames.TypeName, () => interfaceCommand.MapTypeName(interfaceCommand.SourceModel.GetFullName()))
                .Add(ResultNames.Context, interfaceCommand);
        }
        else if (context is GenerateTypeFromReflectionCommand reflectionCommand)
        {
            builder
                .Add(ResultNames.Class, new ClassModel(reflectionCommand.SourceModel))
                .Add(ResultNames.CollectionTypeName, reflectionCommand.CollectionTypeName)
                .Add(ResultNames.AddMethodNameFormatString, () => reflectionCommand.Settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics()))
                .Add(ResultNames.Settings, reflectionCommand.Settings)
                .Add(ResultNames.TypeName, () => reflectionCommand.MapTypeName(reflectionCommand.SourceModel.FullName))
                .Add(ResultNames.Context, reflectionCommand);
        }
        else if (context is ParentChildContext<GenerateBuilderCommand, Property> parentChildContextBuilder)
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
        else if (context is ParentChildContext<GenerateBuilderExtensionCommand, Property> parentChildContextBuilderExtension)
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
        else if (context is ParentChildContext<GenerateEntityCommand, Property> parentChildContextEntity)
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
