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
}
