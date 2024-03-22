namespace ClassFramework.Pipelines.BuilderExtension.Features;

public class AddExtensionMethodsForCollectionPropertiesFeatureBuilder : IBuilderExtensionFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForCollectionPropertiesFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext> Build()
        => new AddExtensionMethodsForCollectionPropertiesFeature(_formattableStringParser);
}

public class AddExtensionMethodsForCollectionPropertiesFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForCollectionPropertiesFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Context.Settings.AddMethodNameFormatString))
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        foreach (var property in context.Context.GetSourceProperties().Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = CreateParentChildContext(context, property);

            var resultSetBuilder = new NamedResultSetBuilder<string>();
            resultSetBuilder.Add("TypeName", () => property.GetBuilderArgumentTypeName(context.Context, CreateParentChildContext(context, property), context.Context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _formattableStringParser));
            resultSetBuilder.Add("Namespace", () => _formattableStringParser.Parse(context.Context.Settings.BuilderNamespaceFormatString, context.Context.FormatProvider, parentChildContext));
            resultSetBuilder.Add("BuilderName", () => _formattableStringParser.Parse(context.Context.Settings.BuilderNameFormatString, context.Context.FormatProvider, parentChildContext));
            resultSetBuilder.Add("AddMethodName", () => _formattableStringParser.Parse(context.Context.Settings.AddMethodNameFormatString, context.Context.FormatProvider, parentChildContext));
            resultSetBuilder.AddRange("EnumerableOverload", () => GetCodeStatementsForEnumerableOverload(context, property));
            resultSetBuilder.AddRange("ArrayOverload", () => GetCodeStatementsForArrayOverload(context, property));
            var results = resultSetBuilder.Build();

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
            }

            var returnType = $"{results.First(x => x.Name == "Namespace").Result.Value.AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Context.SourceModel.GetGenericTypeArgumentsString()}";

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters
                (
                    new ParameterBuilder()
                        .WithName(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()))
                        .WithTypeName(results.First(x => x.Name == "TypeName").Result.Value!.FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics()))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                )
                .AddStringCodeStatements(results.Where(x => x.Name == "EnumerableOverload").Select(x => x.Result.Value!))
            );

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters
                (
                    new ParameterBuilder()
                        .WithName(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()))
                        .WithTypeName(results.First(x => x.Name == "TypeName").Result.Value!.FixTypeName().ConvertTypeNameToArray())
                        .WithIsParamArray()
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                )
                .AddStringCodeStatements(results.Where(x => x.Name == "ArrayOverload").Select(x => x.Result.Value!))
            );
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>> ToBuilder()
        => new AddExtensionMethodsForCollectionPropertiesFeatureBuilder(_formattableStringParser);

    private IEnumerable<Result<string>> GetCodeStatementsForEnumerableOverload(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, Property property)
    {
        yield return Result.Success(context.Context.Settings.AddNullChecks
            ? $"return instance.Add{property.Name}<T>({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()}?.ToArray() ?? throw new {typeof(ArgumentNullException).FullName}(nameof({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()})));"
            : $"return instance.Add{property.Name}<T>({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()}.ToArray());");
    }

    private IEnumerable<Result<string>> GetCodeStatementsForArrayOverload(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, Property property)
    {
        if (context.Context.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = _formattableStringParser.Parse
            (
                context.Context.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"),
                context.Context.FormatProvider,
                new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property>(context, property, context.Context.Settings)
            );
            yield return argumentNullCheckResult;

            if (context.Context.Settings.OriginalValidateArguments == ArgumentValidationType.Shared)
            {
                var constructorInitializerResult = property.GetBuilderConstructorInitializer(context.Context, CreateParentChildContext(context, property), context.Context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), context.Context.Settings.BuilderNewCollectionTypeName, string.Empty, _formattableStringParser); // note that we're not checking the status of this result, because it is using the same expression that we heve already checked before (typeNameResult, see above in this class)
                if (!constructorInitializerResult.IsSuccessful())
                {
                    yield return constructorInitializerResult;
                }
                else
                {
                    yield return Result.Success($"if (instance.{property.GetBuilderMemberName(context.Context.Settings.AddNullChecks, context.Context.Settings.EnableNullableReferenceTypes, context.Context.Settings.OriginalValidateArguments, context.Context.Settings.AddBackingFields, context.Context.FormatProvider.ToCultureInfo())} is null) instance.{property.GetBuilderMemberName(context.Context.Settings.AddNullChecks, context.Context.Settings.EnableNullableReferenceTypes, context.Context.Settings.OriginalValidateArguments, context.Context.Settings.AddBackingFields, context.Context.FormatProvider.ToCultureInfo())} = {constructorInitializerResult.Value};");
                }
            }
        }

        var builderAddExpressionResult = _formattableStringParser.Parse
        (
            context.Context
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, context.Context.Settings.CollectionCopyStatementFormatString),
            context.Context.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property>(context, property, context.Context.Settings)
        );

        yield return builderAddExpressionResult;

        yield return Result.Success("return instance;");
    }

    private static ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property> CreateParentChildContext(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, Property property)
        => new(context, property, context.Context.Settings);
}
