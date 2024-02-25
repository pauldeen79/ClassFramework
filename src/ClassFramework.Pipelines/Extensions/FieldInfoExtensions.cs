namespace ClassFramework.Pipelines.Extensions;

public static class FieldInfoExtensions
{
    public static bool IsNullable(this FieldInfo fieldInfo)
        => fieldInfo.FieldType.IsNullable(fieldInfo.DeclaringType, fieldInfo.CustomAttributes, 0);
}
