namespace ClassFramework.TemplateFramework.Extensions;

public static class VisibilityContainerExtensions
{
    public static string GetModifiers<T>(this T instance, CultureInfo cultureInfo)
        where T : IMetadataContainer, IVisibilityContainer
    {
        var customModifiers = instance.Metadata.GetStringValue(MetadataNames.CustomModifiers);
        if (!string.IsNullOrEmpty(customModifiers))
        {
            return customModifiers + " ";
        }

        var builder = new StringBuilder();
        if (instance is IExtendedVisibilityContainer extendedVisibilityContainer)
        {
            var classMethod = instance as Method;

            if (classMethod is null || !classMethod.Partial)
            {
                builder.AppendWithCondition("protected", extendedVisibilityContainer.Protected);
                builder.AppendWithCondition(instance.Visibility.ToString().ToLower(cultureInfo), !(extendedVisibilityContainer.Protected && instance.Visibility != Visibility.Internal));
                builder.AppendWithCondition("static", extendedVisibilityContainer.Static);
                builder.AppendWithCondition("abstract", extendedVisibilityContainer.Abstract);
                builder.AppendWithCondition("virtual", extendedVisibilityContainer.Virtual);
                builder.AppendWithCondition("override", extendedVisibilityContainer.Override);

                var classField = instance as Field;
                builder.AppendWithCondition("readonly", classField?.ReadOnly == true);
                builder.AppendWithCondition("const", classField?.Constant == true);
            }

            builder.AppendWithCondition("async", classMethod?.Async == true);
            builder.AppendWithCondition("partial", classMethod?.Partial == true);
        }
        else
        {
            builder.Append(instance.Visibility.ToString().ToLower(cultureInfo));

            var cls = instance as IReferenceType;
            builder.AppendWithCondition("sealed", cls?.Sealed == true);
            builder.AppendWithCondition("abstract", cls?.Abstract == true);
            builder.AppendWithCondition("static", cls?.Static == true);

            var typeBase = instance as IType;
            builder.AppendWithCondition("partial", typeBase?.Partial == true);
        }

        // Append trailing space when filled
        builder.AppendWithCondition(string.Empty, builder.Length > 0);

        return builder.ToString();
    }
}
