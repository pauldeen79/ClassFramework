namespace ClassFramework.Pipelines.Entity.Components;

public class AddEquatableMembersComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.ImplementIEquatable)
        {
            return Task.FromResult(Result.Success());
        }

        var nameResult = _evaluator.Parse(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context.Request);
        if (!nameResult.IsSuccessful())
        {
            return Task.FromResult<Result>(nameResult);
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
                    .AddStringCodeStatements($"return Equals(obj as {nameResult.Value});"),

                new MethodBuilder()
                    .WithReturnType(typeof(bool))
                    .WithName(nameof(IEquatable<object>.Equals))
                    .AddParameter("other", nameResult.Value!)
                    .AddStringCodeStatements(
                        $"if (other {context.Request.NullCheck}) return false;",
                        $"return {CreateEqualsCode(context.Request.Settings.IEquatableItemType == IEquatableItemType.Fields ? context.Request.SourceModel.Fields : context.Request.SourceModel.Properties)};"),

                new MethodBuilder()
                    .WithReturnType(typeof(int))
                    .WithOverride()
                    .WithName(nameof(GetHashCode))
                    .AddStringCodeStatements(
                        "unchecked",
                        "{",
                        "    int hash = 17;"
                    )
                    .AddStringCodeStatements(getHashCodeStatements)
                    .AddStringCodeStatements(
                        "    return hash;",
                        "}"),

                new MethodBuilder()
                    .WithName("==")
                    .WithReturnType(typeof(bool))
                    .WithStatic()
                    .WithOperator()
                    .AddParameter("left", context.Request.SourceModel.Name)
                    .AddParameter("right", context.Request.SourceModel.Name)
                    .AddStringCodeStatements($"return {typeof(EqualityComparer<>).WithoutGenerics()}<{context.Request.SourceModel.Name}>.Default.Equals(left, right);"),

                new MethodBuilder()
                    .WithName("!=")
                    .WithReturnType(typeof(bool))
                    .WithStatic()
                    .WithOperator()
                    .AddParameter("left", context.Request.SourceModel.Name)
                    .AddParameter("right", context.Request.SourceModel.Name)
                    .AddStringCodeStatements("return !(left == right);"));

        return Task.FromResult(Result.Success());
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
