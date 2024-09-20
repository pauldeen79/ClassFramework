namespace ClassFramework.TemplateFramework.Extensions;

public static class ResultExtensions
{
    public static T OnSuccess<T>(this T result, Action<T> successDelegate) where T : Result
    {
        Guard.IsNotNull(successDelegate);

        if (result.IsSuccessful())
        {
            successDelegate(result);
        }

        return result;
    }

    public static T OnSuccess<T>(this T result, Action successDelegate) where T : Result
    {
        Guard.IsNotNull(successDelegate);

        if (result.IsSuccessful())
        {
            successDelegate();
        }

        return result;
    }

    public static Task<T> OnSuccess<T>(this T result, Func<T, Task<T>> successDelegate) where T : Result
    {
        Guard.IsNotNull(successDelegate);

        if (!result.IsSuccessful())
        {
            return Task.FromResult(result);
        }

        return successDelegate(result);
    }

    public static Task<T> OnSuccess<T>(this T result, Func<Task<T>> successDelegate) where T : Result
    {
        Guard.IsNotNull(successDelegate);

        if (!result.IsSuccessful())
        {
            return Task.FromResult(result);
        }

        return successDelegate();
    }
}
