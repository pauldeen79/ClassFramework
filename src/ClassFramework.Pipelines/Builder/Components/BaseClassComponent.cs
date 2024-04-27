﻿namespace ClassFramework.Pipelines.Builder.Components;

public class BaseClassComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public BaseClassComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new BaseClassComponent(_formattableStringParser);
}

public class BaseClassComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public BaseClassComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var baseClassResult = GetBuilderBaseClass(context.Request.SourceModel, context);
        if (!baseClassResult.IsSuccessful())
        {
            return Task.FromResult<Result>(baseClassResult);
        }

        context.Response.WithBaseClass(baseClassResult.Value!);

        return Task.FromResult(Result.Continue());
    }

    private Result<FormattableStringParserResult> GetBuilderBaseClass(IType instance, PipelineContext<BuilderContext, IConcreteTypeBuilder> context)
    {
        var genericTypeArgumentsString = instance.GetGenericTypeArgumentsString();

        var isNotForAbstractBuilder = context.Request.Settings.EnableInheritance
            && context.Request.Settings.EnableBuilderInheritance
            && context.Request.Settings.BaseClass is null
            && !context.Request.Settings.IsForAbstractBuilder;

        var isAbstract = context.Request.Settings.EnableInheritance
            && context.Request.Settings.EnableBuilderInheritance
            && context.Request.Settings.BaseClass is not null
            && !context.Request.Settings.IsForAbstractBuilder
            && context.Request.Settings.IsAbstract;

        var nameResult = _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context);

        if (!nameResult.IsSuccessful())
        {
            return nameResult;
        }

        if (isNotForAbstractBuilder || isAbstract)
        {
            return Result.Success<FormattableStringParserResult>($"{nameResult.Value}{genericTypeArgumentsString}");
        }

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.EnableBuilderInheritance
            && context.Request.Settings.BaseClass is not null
            && !context.Request.Settings.IsForAbstractBuilder) // note that originally, this was only enabled when RemoveDuplicateWithMethods was true. But I don't know why you don't want this... The generics ensure that we don't have to duplicate them, right?
        {
            var inheritanceNameResult = _formattableStringParser.Parse
            (
                context.Request.Settings.BuilderNameFormatString,
                context.Request.FormatProvider,
                new PipelineContext<BuilderContext, IConcreteTypeBuilder>(new BuilderContext(context.Request.Settings.BaseClass!, context.Request.Settings, context.Request.FormatProvider), context.Response)
            );
            if (!inheritanceNameResult.IsSuccessful())
            {
                return inheritanceNameResult;
            }

            return Result.Success<FormattableStringParserResult>($"{context.Request.Settings.BaseClassBuilderNameSpace.AppendWhenNotNullOrEmpty(".")}{inheritanceNameResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
        }

        return instance.GetCustomValueForInheritedClass
        (
            context.Request.Settings.EnableInheritance,
            baseClassContainer =>
            {
                var baseClassResult = GetBaseClassName(context, baseClassContainer);
                if (!baseClassResult.IsSuccessful())
                {
                    return baseClassResult;
                }

                return Result.Success<FormattableStringParserResult>(context.Request.Settings.EnableBuilderInheritance
                    ? $"{baseClassResult.Value}{genericTypeArgumentsString}"
                    : $"{baseClassResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
            }
        );
    }

    private Result<FormattableStringParserResult> GetBaseClassName(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, IBaseClassContainer baseClassContainer)
    {
        var newContext = new PipelineContext<BuilderContext, IConcreteTypeBuilder>
        (
            new BuilderContext(CreateTypeBase(context.Request.MapTypeName(baseClassContainer.BaseClass!)), context.Request.Settings, context.Request.FormatProvider),
            context.Response
        );

        return _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, newContext);
    }

    private static TypeBase CreateTypeBase(string baseClass)
        => new ClassBuilder()
            .WithNamespace(baseClass.GetNamespaceWithDefault())
            .WithName(baseClass.GetClassName())
            .Build();
}
