namespace ClassFramework.Pipelines.Entity.Components;

public class AddEquatableMembersComponentBuilder : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddEquatableMembersComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<EntityContext> Build()
        => new AddEquatableMembersComponent(_formattableStringParser);
}

public class AddEquatableMembersComponent : IPipelineComponent<EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddEquatableMembersComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.ImplementIEquatable)
        {
            return Task.FromResult(Result.Continue());
        }

        var nameResult = _formattableStringParser.Parse(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context);
        if (!nameResult.IsSuccessful())
        {
            return Task.FromResult<Result>(nameResult);
        }

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
                        "if (other is null) return false;",
                        $"return {CreateEqualsCode(context.Request.SourceModel)};"),

                new MethodBuilder()
                    .WithReturnType(typeof(int))
                    .WithOverride()
                    .WithName(nameof(GetHashCode))
                    .AddStringCodeStatements(
                        "unchecked",
                        "{",
                        "    int hash = 17;"
                    )
                    .AddStringCodeStatements(context.Request.SourceModel.Fields.Select(x => $"    hash = hash * 23 + {CreateHashCodeStatement(x)};"))
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
                    .AddStringCodeStatements($"return EqualityComparer<{context.Request.SourceModel.Name}>.Default.Equals(left, right);"),

                new MethodBuilder()
                    .WithName("!=")
                    .WithReturnType(typeof(bool))
                    .WithStatic()
                    .WithOperator()
                    .AddParameter("left", context.Request.SourceModel.Name)
                    .AddParameter("right", context.Request.SourceModel.Name)
                    .AddStringCodeStatements("return !(left == right);"));

        return Task.FromResult(Result.Continue());
    }

    private string CreateHashCodeStatement(Field field)
        => field.IsNullable(true) // note that nullable reference type does not matter in this context, we just want to know if we can do a null check
            ? $"{field.Name} is not null ? {field.Name}.GetHashCode() : 0"
            : $"{field.Name}.GetHashCode()";

    private string CreateEqualsCode(TypeBase sourceModel)
        => string.Join($"{Environment.NewLine}            && ", sourceModel.Fields.Select(x => $"{x.Name} == other.{x.Name}"));
}
/*
public class MyClass : IEquatable<MyClass>
{
    public int Property1 { get; set; }
    public string Property2 { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as MyClass);
    }

    public bool Equals(MyClass other)
    {
        if (other == null) return false;
        return Property1 == other.Property1 && Property2 == other.Property2;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + Property1.GetHashCode();
            hash = hash * 23 + (Property2 != null ? Property2.GetHashCode() : 0);
            return hash;
        }
    }

    public static bool operator ==(MyClass left, MyClass right)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(MyClass left, MyClass right)
    {
        return !(left == right);
    }
}

 */
