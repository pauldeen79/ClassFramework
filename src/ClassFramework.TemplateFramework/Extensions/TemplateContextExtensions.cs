namespace ClassFramework.TemplateFramework.Extensions;

public static class TemplateContextExtensions
{
    public static int GetIndentCount(this ITemplateContext context)
        => GetCount(context, 0);

    public static CsharpClassGeneratorSettings? GetCsharpClassGeneratorSettings(this ITemplateContext instance)
    {
        var rootTemplate = instance.RootContext.Template;
        var modelProperty = rootTemplate.GetType().GetProperty(nameof(IModelContainer<object>.Model));
        if (modelProperty is not null)
        {
            var modelValue = modelProperty.GetValue(instance.RootContext.Template);
            if (modelValue is ICsharpClassGeneratorSettingsContainer container && container.Settings is not null)
            {
                return container.Settings;
            }
        }

        return null;
    }

    private static int GetCount(ITemplateContext context, int recursionLevel)
    {
        var count = context.Model is IType
            ? 1
            : 0;

        if (context.ParentContext is not null)
        {
            if (recursionLevel == 11)
            {
                throw new NotSupportedException("Only 10 nested levels of sub classes are supported");
            }

            count += GetCount(context.ParentContext, recursionLevel + 1);
        }

        return count;
    }
}
