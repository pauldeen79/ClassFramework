namespace ClassFramework.TemplateFramework.Extensions;

public static class ResultExtensions
{
    public static T OnSuccess<T>(this T result, Action<T> successDelegate) where T : Result
    {
        successDelegate = successDelegate.IsNotNull(nameof(successDelegate));

        if (result.IsSuccessful())
        {
            successDelegate(result);
        }

        return result;
    }

    public static async Task<T> OnSuccess<T>(this T result, Func<T, Task<T>> successDelegate) where T : Result
    {
        successDelegate = successDelegate.IsNotNull(nameof(successDelegate));

        if (!result.IsSuccessful())
        {
            return result;
        }

        return await successDelegate(result);
    }

    public static T OnSuccess<T>(this T result, Func<T, T> successDelegate) where T : Result
    {
        successDelegate = successDelegate.IsNotNull(nameof(successDelegate));

        if (!result.IsSuccessful())
        {
            return result;
        }

        return successDelegate(result);
    }
}
