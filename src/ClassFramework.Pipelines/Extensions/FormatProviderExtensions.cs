namespace ClassFramework.Pipelines.Extensions;

public static class FormatProviderExtensions
{
    public static CultureInfo ToCultureInfo(this IFormatProvider instance)
        => instance as CultureInfo ?? CultureInfo.CurrentCulture;
}
