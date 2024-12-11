namespace ClassFramework.Pipelines.Shared.Variables;

public class ClassVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            $"class.{nameof(Class.Name)}" => GetValueFromClass(context, x => x.Name),
            $"class.{nameof(Class.Namespace)}" => GetValueFromClass(context, x => x.Namespace),
            "class.FullName" => GetValueFromClass(context, x => x.FullName),
            $"class.{nameof(Class.Name)}NoInterfacePrefix" => GetValueFromClass(context, x => x.NameNoInterfacePrefix),
            _ => Result.Continue<object?>()
        };

    private static Result<object?> GetValueFromClass(object? context, Func<ClassWrapper, object?> valueDelegate)
        => context switch
        {
            PipelineContext<BuilderContext> builderContext => Result.Success(valueDelegate(new ClassWrapper(builderContext.Request.SourceModel))),
            PipelineContext<BuilderExtensionContext> builderExtensionContext => Result.Success(valueDelegate(new ClassWrapper(builderExtensionContext.Request.SourceModel))),
            PipelineContext<EntityContext> entityContext => Result.Success(valueDelegate(new ClassWrapper(entityContext.Request.SourceModel))),
            PipelineContext<InterfaceContext> interfaceContext => Result.Success(valueDelegate(new ClassWrapper(interfaceContext.Request.SourceModel))),
            PipelineContext<Reflection.ReflectionContext> reflectionContext => Result.Success(valueDelegate(new ClassWrapper(reflectionContext.Request.SourceModel))),
            ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success(valueDelegate(new ClassWrapper(parentChildContextBuilder.ParentContext.Request.SourceModel))),
            ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success(valueDelegate(new ClassWrapper(parentChildContextBuilderExtension.ParentContext.Request.SourceModel))),
            _ => Result.Invalid<object?>($"Could not get class from context, because the context type {context?.GetType().FullName ?? "null"} is not supported")
        };

    private sealed class ClassWrapper
    {
        private readonly Type? _type;
        private readonly TypeBase? _typeBase;

        public ClassWrapper(Type type)
        {
            ArgumentGuard.IsNotNull(type, nameof(type));

            _type = type;
        }

        public ClassWrapper(TypeBase typeBase)
        {
            ArgumentGuard.IsNotNull(typeBase, nameof(typeBase));
            _typeBase = typeBase;
        }

        public string Name => _type?.Name ?? _typeBase!.Name;
        public string Namespace => _type?.Namespace ?? _typeBase!.Namespace;
        public string FullName => _type?.FullName ?? _typeBase!.GetFullName();
        public string NameNoInterfacePrefix => _type?.WithoutInterfacePrefix() ?? _typeBase!.WithoutInterfacePrefix();
    }
}
