namespace ClassFramework.Pipelines.Builder.Components;

public class AddDefaultConstructorComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddDefaultConstructorComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<BuilderContext> Build()
        => new AddDefaultConstructorComponent(_formattableStringParser);
}

public class AddDefaultConstructorComponent : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddDefaultConstructorComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Settings.EnableBuilderInheritance
            && context.Request.IsAbstractBuilder
            && !context.Request.Settings.IsForAbstractBuilder)
        {
            context.Request.Builder.AddConstructors(CreateInheritanceDefaultConstructor(context));
        }
        else
        {
            var defaultConstructorResult = CreateDefaultConstructor(context);
            if (!defaultConstructorResult.IsSuccessful())
            {
                return Task.FromResult<Result>(defaultConstructorResult);
            }

            context.Request.Builder.AddConstructors(defaultConstructorResult.Value!);
        }

        return Task.FromResult(Result.Continue());
    }

    private Result<ConstructorBuilder> CreateDefaultConstructor(PipelineContext<BuilderContext> context)
    {
        var constructorInitializerResults = context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings) && x.TypeName.FixTypeName().IsCollectionTypeName())
            .Select(x => new
            {
                Name = x.GetBuilderMemberName(context.Request.Settings, context.Request.FormatProvider.ToCultureInfo()),
                Result = x.GetBuilderConstructorInitializer(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, x, context.Request.Settings), context.Request.MapTypeName(x.TypeName, MetadataNames.CustomEntityInterfaceTypeName), context.Request.Settings.BuilderNewCollectionTypeName, string.Empty, _formattableStringParser)
            })
            .TakeWhileWithFirstNonMatching(x => x.Result.IsSuccessful())
            .ToArray();

        var errorResult = Array.Find(constructorInitializerResults, x => !x.Result.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult.Result);
        }

        var ctor = new ConstructorBuilder()
            .WithChainCall(CreateBuilderClassConstructorChainCall(context.Request.SourceModel, context.Request.Settings))
            .WithProtected(context.Request.IsBuilderForAbstractEntity)
            .AddStringCodeStatements(constructorInitializerResults.Select(x => $"{x.Name} = {x.Result.Value};"));

        if (context.Request.Settings.SetDefaultValuesInEntityConstructor)
        {
            var defaultValueResults = context.Request.SourceModel.Properties
                .Where
                (x =>
                    context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings)
                    && !x.TypeName.FixTypeName().IsCollectionTypeName()
                    && ((!x.IsValueType && !x.IsNullable) || (x.Attributes.Any(y => y.Name == typeof(DefaultValueAttribute).FullName) && context.Request.Settings.UseDefaultValueAttributeValuesForBuilderInitialization))
                )
                .Select(x => GenerateDefaultValueStatement(x, context))
                .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
                .ToArray();

            var defaultValueErrorResult = Array.Find(defaultValueResults, x => !x.IsSuccessful());
            if (defaultValueErrorResult is not null)
            {
                return Result.FromExistingResult<ConstructorBuilder>(defaultValueErrorResult);
            }

            ctor.AddStringCodeStatements(defaultValueResults.Select(x => x.Value!.ToString()));

            var setDefaultValuesMethodNameResult = _formattableStringParser.Parse(context.Request.Settings.SetDefaultValuesMethodName, context.Request.FormatProvider, context);
            if (!setDefaultValuesMethodNameResult.IsSuccessful())
            {
                return Result.FromExistingResult<ConstructorBuilder>(setDefaultValuesMethodNameResult);
            }

            if (!string.IsNullOrEmpty(setDefaultValuesMethodNameResult.Value!.ToString()))
            {
                ctor.AddStringCodeStatements($"{setDefaultValuesMethodNameResult.Value}();");
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(setDefaultValuesMethodNameResult.Value)
                    .WithPartial()
                    .WithVisibility(Visibility.Private)
                    );
            }
        }

        return Result.Success(ctor);
    }

    private static string CreateBuilderClassConstructorChainCall(IType instance, PipelineSettings settings)
        => instance.GetCustomValueForInheritedClass(settings.EnableInheritance, _ => Result.Success<FormattableStringParserResult>("base()")).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private Result<FormattableStringParserResult> GenerateDefaultValueStatement(Property property, PipelineContext<BuilderContext> context)
        => _formattableStringParser.Parse
        (
            "{BuilderMemberName} = {DefaultValue};",
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings)
        );

    private static ConstructorBuilder CreateInheritanceDefaultConstructor(PipelineContext<BuilderContext> context)
        => new ConstructorBuilder()
            .WithChainCall("base()")
            .WithProtected(context.Request.IsBuilderForAbstractEntity);
}
