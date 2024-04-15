namespace ClassFramework.Pipelines.Builder.Features;

public class AddDefaultConstructorComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddDefaultConstructorComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddDefaultConstructorComponent(_formattableStringParser);
}

public class AddDefaultConstructorComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddDefaultConstructorComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Context.Settings.EnableBuilderInheritance
            && context.Context.IsAbstractBuilder
            && !context.Context.Settings.IsForAbstractBuilder)
        {
            context.Model.AddConstructors(CreateInheritanceDefaultConstructor(context));
        }
        else
        {
            var defaultConstructorResult = CreateDefaultConstructor(context);
            if (!defaultConstructorResult.IsSuccessful())
            {
                return Task.FromResult(Result.FromExistingResult<IConcreteTypeBuilder>(defaultConstructorResult));
            }

            context.Model.AddConstructors(defaultConstructorResult.Value!);
        }

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }

    private Result<ConstructorBuilder> CreateDefaultConstructor(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        var constructorInitializerResults = context.Context.SourceModel.Properties
            .Where(x => context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings) && x.TypeName.FixTypeName().IsCollectionTypeName())
            .Select(x => new
            {
                Name = x.GetBuilderMemberName(context.Context.Settings, context.Context.FormatProvider.ToCultureInfo()),
                Result = x.GetBuilderConstructorInitializer(context.Context, new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, x, context.Context.Settings), context.Context.MapTypeName(x.TypeName, MetadataNames.CustomEntityInterfaceTypeName), context.Context.Settings.BuilderNewCollectionTypeName, string.Empty, _formattableStringParser)
            })
            .TakeWhileWithFirstNonMatching(x => x.Result.IsSuccessful())
            .ToArray();

        var errorResult = Array.Find(constructorInitializerResults, x => !x.Result.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult.Result);
        }

        var ctor = new ConstructorBuilder()
            .WithChainCall(CreateBuilderClassConstructorChainCall(context.Context.SourceModel, context.Context.Settings))
            .WithProtected(context.Context.IsBuilderForAbstractEntity)
            .AddStringCodeStatements(constructorInitializerResults.Select(x => $"{x.Name} = {x.Result.Value};"));

        if (context.Context.Settings.SetDefaultValuesInEntityConstructor)
        {
            var defaultValueResults = context.Context.SourceModel.Properties
                .Where
                (x =>
                    context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings)
                    && !x.TypeName.FixTypeName().IsCollectionTypeName()
                    && ((!x.IsValueType && !x.IsNullable) || (x.Attributes.Any(y => y.Name == typeof(DefaultValueAttribute).FullName) && context.Context.Settings.UseDefaultValueAttributeValuesForBuilderInitialization))
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
            
            var setDefaultValuesMethodNameResult = _formattableStringParser.Parse(context.Context.Settings.SetDefaultValuesMethodName, context.Context.FormatProvider, context);
            if (!setDefaultValuesMethodNameResult.IsSuccessful())
            {
                return Result.FromExistingResult<ConstructorBuilder>(setDefaultValuesMethodNameResult);
            }
            
            if (!string.IsNullOrEmpty(setDefaultValuesMethodNameResult.Value!.ToString()))
            {
                ctor.AddStringCodeStatements($"{setDefaultValuesMethodNameResult.Value}();");
                context.Model.AddMethods(new MethodBuilder()
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

    private Result<FormattableStringParserResult> GenerateDefaultValueStatement(Property property, PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => _formattableStringParser.Parse
        (
            "{BuilderMemberName} = {DefaultValue};",
            context.Context.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
        );

    private static ConstructorBuilder CreateInheritanceDefaultConstructor(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => new ConstructorBuilder()
            .WithChainCall("base()")
            .WithProtected(context.Context.IsBuilderForAbstractEntity);
}
