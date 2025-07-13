namespace ClassFramework.Pipelines.Entity.Components;

public class AddEquatableMembersComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.ImplementIEquatable)
        {
            return Result.Success();
        }

        var nameResult = await _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context.Request, token).ConfigureAwait(false);
        if (!nameResult.IsSuccessful())
        {
            return nameResult;
        }

        var getHashCodeStatements =
            context.Request.Settings.IEquatableItemType == IEquatableItemType.Fields
            ? CreateHashCodeStatements(context.Request.SourceModel.Fields, context.Request.NotNullCheck)
            : CreateHashCodeStatements(context.Request.SourceModel.Properties, context.Request.NotNullCheck);

        context.Request.Builder
            .AddInterfaces($"IEquatable<{nameResult.Value}>")
            .AddMethods(
                new MethodBuilder()
                    .WithReturnType(typeof(bool))
                    .WithOverride()
                    .WithName(nameof(Equals))
                    .AddParameter("obj", typeof(object))
                    .AddCodeStatements($"return Equals(obj as {nameResult.Value});"),

                new MethodBuilder()
                    .WithReturnType(typeof(bool))
                    .WithName(nameof(IEquatable<object>.Equals))
                    .AddParameter("other", nameResult.Value!)
                    .AddCodeStatements(
                        $"if (other {context.Request.NullCheck}) return false;",
                        $"return {CreateEqualsCode(context.Request.Settings.IEquatableItemType == IEquatableItemType.Fields ? context.Request.SourceModel.Fields : context.Request.SourceModel.Properties)};"),

                new MethodBuilder()
                    .WithReturnType(typeof(int))
                    .WithOverride()
                    .WithName(nameof(GetHashCode))
                    .AddCodeStatements(
                        "unchecked",
                        "{",
                        "    int hash = 17;"
                    )
                    .AddCodeStatements(getHashCodeStatements)
                    .AddCodeStatements(
                        "    return hash;",
                        "}"),

                new MethodBuilder()
                    .WithName("==")
                    .WithReturnType(typeof(bool))
                    .WithStatic()
                    .WithOperator()
                    .AddParameter("left", context.Request.SourceModel.Name)
                    .AddParameter("right", context.Request.SourceModel.Name)
                    .AddCodeStatements($"return {typeof(EqualityComparer<>).WithoutGenerics()}<{context.Request.SourceModel.Name}>.Default.Equals(left, right);"),

                new MethodBuilder()
                    .WithName("!=")
                    .WithReturnType(typeof(bool))
                    .WithStatic()
                    .WithOperator()
                    .AddParameter("left", context.Request.SourceModel.Name)
                    .AddParameter("right", context.Request.SourceModel.Name)
                    .AddCodeStatements("return !(left == right);"));

        return Result.Success();
    }

    private static IEnumerable<string> CreateHashCodeStatements<T>(IEnumerable<T> items, string notNullCheck)
        where T : ITypeContainer, INameContainer
        => items.Select(x => $"    hash = hash * 23 + {CreateHashCodeStatement(x, notNullCheck)};");

    private static string CreateHashCodeStatement<T>(T item, string notNullCheck)
        where T : ITypeContainer, INameContainer
        => item.IsNullable(true) // note that nullable reference type setting does not matter in this context, we just want to know if we can do a null check
            ? $"{item.Name} {notNullCheck} ? {item.Name}.GetHashCode() : 0"
            : $"{item.Name}.GetHashCode()";

    private static string CreateEqualsCode(IEnumerable<INameContainer> items)
        => string.Join($"{Environment.NewLine}            && ", items.Select(x => $"{x.Name} == other.{x.Name}"));
}
