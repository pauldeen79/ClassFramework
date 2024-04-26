namespace ClassFramework.Pipelines.Entity.Features;

public class AddPublicParameterlessConstructorComponentBuilder : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPublicParameterlessConstructorComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AddPublicParameterlessConstructorComponent(_formattableStringParser);
}

public class AddPublicParameterlessConstructorComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPublicParameterlessConstructorComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddPublicParameterlessConstructor)
        {
            return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
        }

        var ctorResult = CreateEntityConstructor(context);
        if (!ctorResult.IsSuccessful())
        {
            return Task.FromResult(Result.FromExistingResult<IConcreteTypeBuilder>(ctorResult));
        }

        context.Response.AddConstructors(ctorResult.Value!);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }

    private Result<ConstructorBuilder> CreateEntityConstructor(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        var initializationStatements = context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings))
            .Select(x => GenerateDefaultValueStatement(x, context))
            .ToArray();

        var errorResult = Array.Find(initializationStatements, x => !x.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult);
        }

        return Result.Success(new ConstructorBuilder()
            .AddStringCodeStatements(initializationStatements.Select(x => x.Value!))
            );
    }

    private Result<string> GenerateDefaultValueStatement(Property property, PipelineContext<IConcreteTypeBuilder, EntityContext> context)
        => _formattableStringParser.Parse
        (
            property.TypeName.FixTypeName().IsCollectionTypeName()
                ? "{EntityMemberName} = new {CollectionTypeName}<{TypeName.GenericArguments}>();"
                : "{EntityMemberName} = {DefaultValue};",
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, EntityContext>, Property>(context, property, context.Request.Settings)
        ).TransformValue(x => x.ToString());
}
