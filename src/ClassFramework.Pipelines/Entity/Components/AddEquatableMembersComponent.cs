namespace ClassFramework.Pipelines.Entity.Components;

public class AddEquatableMembersComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.Settings.ImplementIEquatable)
        {
            return Result.Continue();
        }

        return (await _evaluator.EvaluateInterpolatedStringAsync(context.Settings.EntityNameFormatString, context.FormatProvider, context, token)
            .ConfigureAwait(false))
            .OnSuccess(nameResult =>
            {

                var getHashCodeStatements =
                    context.Settings.IEquatableItemType == IEquatableItemType.Fields
                    ? CreateHashCodeStatements(context.SourceModel.Fields, context.NotNullCheck)
                    : CreateHashCodeStatements(context.SourceModel.Properties, context.NotNullCheck);

                response
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
                                $"if (other {context.NullCheck}) return false;",
                                $"return {CreateEqualsCode(context.Settings.IEquatableItemType == IEquatableItemType.Fields ? context.SourceModel.Fields : context.SourceModel.Properties)};"),

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
                            .AddParameter("left", context.SourceModel.Name)
                            .AddParameter("right", context.SourceModel.Name)
                            .AddCodeStatements($"return {typeof(EqualityComparer<>).WithoutGenerics()}<{context.SourceModel.Name}>.Default.Equals(left, right);"),

                        new MethodBuilder()
                            .WithName("!=")
                            .WithReturnType(typeof(bool))
                            .WithStatic()
                            .WithOperator()
                            .AddParameter("left", context.SourceModel.Name)
                            .AddParameter("right", context.SourceModel.Name)
                            .AddCodeStatements("return !(left == right);"));

            });
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
