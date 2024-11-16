namespace ClassFramework.TemplateFramework.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendLineWithCondition(this StringBuilder builder, string value, bool condition)
    {
        if (!condition)
        {
            return builder;
        }

        return builder.AppendLine(value);
    }

    public static StringBuilder AppendWithCondition(this StringBuilder builder, string value, bool condition)
    {
        if (!condition)
        {
            return builder;
        }

        if (builder.Length > 0)
        {
            builder.Append(" ");
        }

        return builder.Append(value);
    }

    public static void RenderSuppressions(this StringBuilder builder, IReadOnlyCollection<string> suppressWarningCodes, string verb, string indentation)
    {
        suppressWarningCodes = suppressWarningCodes.IsNotNull(nameof(suppressWarningCodes));

        foreach (var suppression in suppressWarningCodes)
        {
            builder.Append(indentation);
            builder.AppendLine($"#pragma warning {verb} {suppression}");
        }
    }

    public static async Task<Result> RenderMethodBody(this StringBuilder builder, string indentation, Func<Task<Result>> innerAction)
    {
        Guard.IsNotNull(innerAction);

        builder.AppendLine();
        builder.Append(indentation);
        builder.AppendLine("{");
        var result = await innerAction().ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        builder.Append(indentation);
        builder.AppendLine("}");

        return Result.Success();
    }
}
