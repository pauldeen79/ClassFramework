namespace ClassFramework.TemplateFramework.Extensions;

public static class VisibilityContainerExtensions
{
    public static string GetModifiers<T>(this T instance, CultureInfo cultureInfo)
        where T : IVisibilityContainer
    {
        var builder = new StringBuilder();
        if (instance is IModifiersContainer modifiersContainer)
        {
            var classMethod = instance as Method;

            if (classMethod is null || !classMethod.Partial)
            {
                builder.AppendWithCondition("protected", modifiersContainer.Protected);
                builder.AppendWithCondition(instance.Visibility.ToString().ToLower(cultureInfo), !(modifiersContainer.Protected && instance.Visibility != Visibility.Internal));
                builder.AppendWithCondition("static", modifiersContainer.Static);
                builder.AppendWithCondition("new", modifiersContainer.New);
                builder.AppendWithCondition("abstract", modifiersContainer.Abstract);
                builder.AppendWithCondition("virtual", modifiersContainer.Virtual);
                builder.AppendWithCondition("override", modifiersContainer.Override);

                var classField = instance as Field;
                builder.AppendWithCondition("readonly", classField?.ReadOnly == true);
                builder.AppendWithCondition("const", classField?.Constant == true);
            }

            builder.AppendWithCondition("partial", classMethod?.Partial == true);
            builder.AppendWithCondition("async", classMethod?.Async == true);
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
