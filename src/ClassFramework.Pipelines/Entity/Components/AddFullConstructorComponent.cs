using System.Runtime;

namespace ClassFramework.Pipelines.Entity.Features;

public class AddFullConstructorComponentBuilder : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFullConstructorComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AddFullConstructorComponent(_formattableStringParser);
}

public class AddFullConstructorComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFullConstructorComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AddFullConstructor)
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        var ctorResult = CreateEntityConstructor(context);
        if (!ctorResult.IsSuccessful())
        {
            return Result.FromExistingResult<IConcreteTypeBuilder>(ctorResult);
        }

        context.Model.AddConstructors(ctorResult.Value!);

        if (context.Context.Settings.AddValidationCode() == ArgumentValidationType.CustomValidationCode)
        {
            context.Model.AddMethods(new MethodBuilder().WithName("Validate").WithPartial().WithVisibility(Visibility.Private));
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    private Result<ConstructorBuilder> CreateEntityConstructor(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        var initializationResults = context.Context.SourceModel.Properties
            .Where(property => context.Context.SourceModel.IsMemberValidForBuilderClass(property, context.Context.Settings))
            .Select(property => _formattableStringParser.Parse("this.{EntityMemberName} = {InitializationExpression}{NullableRequiredSuffix};", context.Context.FormatProvider, new ParentChildContext<PipelineContext<IConcreteTypeBuilder, EntityContext>, Property>(context, property, context.Context.Settings)))
            .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
            .ToArray();

        var error = Array.Find(initializationResults, x => !x.IsSuccessful());
        if (error is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(error);
        }

        return Result.Success(new ConstructorBuilder()
            .WithProtected(context.Context.Settings.EnableInheritance && context.Context.Settings.IsAbstract)
            .AddParameters(context.Context.SourceModel.Properties.CreateImmutableClassCtorParameters(context.Context.FormatProvider, n => context.Context.MapTypeName(n, MetadataNames.CustomEntityInterfaceTypeName)))
            .AddStringCodeStatements
            (
                context.Context.SourceModel.Properties
                    .Where(property => context.Context.SourceModel.IsMemberValidForBuilderClass(property, context.Context.Settings))
                    .Where(property => context.Context.Settings.AddNullChecks && context.Context.Settings.AddValidationCode() == ArgumentValidationType.None && context.Context.GetMappingMetadata(property.TypeName).GetValue(MetadataNames.EntityNullCheck, () => !property.IsNullable && !property.IsValueType))
                    .Select(property => context.Context.CreateArgumentNullException(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()))
            )
            .AddStringCodeStatements(initializationResults.Select(x => x.Value!))
            .AddStringCodeStatements(context.Context.CreateEntityValidationCode())
            .WithChainCall(context.CreateEntityChainCall()));
    }
}
