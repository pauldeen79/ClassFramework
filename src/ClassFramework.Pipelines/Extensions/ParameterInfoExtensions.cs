namespace ClassFramework.Pipelines.Extensions;

public static class ParameterInfoExtensions
{
    public static bool IsNullable(this ParameterInfo parameter)
        => parameter.ParameterType.IsNullable(parameter.Member, parameter.CustomAttributes, 0);
}
