namespace ClassFramework.Pipelines.Extensions;

public static class PropertyInfoExtensions
{
    public static bool IsInitOnly(this PropertyInfo property)
    {
        if (!property.CanWrite)
        {
            return false;
        }

        var setMethod = property.SetMethod;

        // Get the modifiers applied to the return parameter.
        var setMethodReturnParameterModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();

        // Init-only properties are marked with the IsExternalInit type.
        return Array.Exists(setMethodReturnParameterModifiers, t => t.FullName == "System.Runtime.CompilerServices.IsExternalInit");
    }

    public static bool IsNullable(this PropertyInfo property)
        => property.PropertyType.IsNullable(property.DeclaringType, property.CustomAttributes, 0);
}
