namespace ClassFramework.Pipelines.Builder.Features;

public class BaseClassComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public BaseClassComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new BaseClassComponent(_formattableStringParser);
}

public class BaseClassComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public BaseClassComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var baseClassResult = GetBuilderBaseClass(context.Context.SourceModel, context);
        if (!baseClassResult.IsSuccessful())
        {
            return Task.FromResult(Result.FromExistingResult<IConcreteTypeBuilder>(baseClassResult));
        }

        context.Model.WithBaseClass(baseClassResult.Value!);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }

    private Result<FormattableStringParserResult> GetBuilderBaseClass(IType instance, PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        var genericTypeArgumentsString = instance.GetGenericTypeArgumentsString();

        var isNotForAbstractBuilder = context.Context.Settings.EnableInheritance
            && context.Context.Settings.EnableBuilderInheritance
            && context.Context.Settings.BaseClass is null
            && !context.Context.Settings.IsForAbstractBuilder;

        var isAbstract = context.Context.Settings.EnableInheritance
            && context.Context.Settings.EnableBuilderInheritance
            && context.Context.Settings.BaseClass is not null
            && !context.Context.Settings.IsForAbstractBuilder
            && context.Context.Settings.IsAbstract;

        var nameResult = _formattableStringParser.Parse(context.Context.Settings.BuilderNameFormatString, context.Context.FormatProvider, context);

        if (!nameResult.IsSuccessful())
        {
            return nameResult;
        }

        if (isNotForAbstractBuilder || isAbstract)
        {
            return Result.Success<FormattableStringParserResult>($"{nameResult.Value}{genericTypeArgumentsString}");
        }

        if (context.Context.Settings.EnableInheritance
            && context.Context.Settings.EnableBuilderInheritance
            && context.Context.Settings.BaseClass is not null
            && !context.Context.Settings.IsForAbstractBuilder) // note that originally, this was only enabled when RemoveDuplicateWithMethods was true. But I don't know why you don't want this... The generics ensure that we don't have to duplicate them, right?
        {
            var inheritanceNameResult = _formattableStringParser.Parse
            (
                context.Context.Settings.BuilderNameFormatString,
                context.Context.FormatProvider,
                new PipelineContext<IConcreteTypeBuilder, BuilderContext>(context.Model, new BuilderContext(context.Context.Settings.BaseClass!, context.Context.Settings, context.Context.FormatProvider))
            );
            if (!inheritanceNameResult.IsSuccessful())
            {
                return inheritanceNameResult;
            }

            return Result.Success<FormattableStringParserResult>($"{context.Context.Settings.BaseClassBuilderNameSpace.AppendWhenNotNullOrEmpty(".")}{inheritanceNameResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
        }

        return instance.GetCustomValueForInheritedClass
        (
            context.Context.Settings.EnableInheritance,
            baseClassContainer =>
            {
                var baseClassResult = GetBaseClassName(context, baseClassContainer);
                if (!baseClassResult.IsSuccessful())
                {
                    return baseClassResult;
                }

                return Result.Success<FormattableStringParserResult>(context.Context.Settings.EnableBuilderInheritance
                    ? $"{baseClassResult.Value}{genericTypeArgumentsString}"
                    : $"{baseClassResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
            }
        );
    }

    private Result<FormattableStringParserResult> GetBaseClassName(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, IBaseClassContainer baseClassContainer)
    {
        var newContext = new PipelineContext<IConcreteTypeBuilder, BuilderContext>
        (
            context.Model,
            new BuilderContext(CreateTypeBase(context.Context.MapTypeName(baseClassContainer.BaseClass!)), context.Context.Settings, context.Context.FormatProvider)
        );

        return _formattableStringParser.Parse(context.Context.Settings.BuilderNameFormatString, context.Context.FormatProvider, newContext);
    }

    private static TypeBase CreateTypeBase(string baseClass)
        => new ClassBuilder()
            .WithNamespace(baseClass.GetNamespaceWithDefault())
            .WithName(baseClass.GetClassName())
            .Build();
}
