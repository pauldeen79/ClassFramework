namespace ClassFramework.Pipelines.Extensions;

public static class MethodInfoExtensions
{
    public static bool ReturnTypeIsNullable(this MethodInfo methodInfo)
        => methodInfo.ReturnType.IsNullable(methodInfo.ReturnParameter.Member, methodInfo.CustomAttributes, 0);
}
