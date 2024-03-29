﻿namespace ClassFramework.Domain.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// Removes generics from a typename. (`1)
    /// </summary>
    /// <param name="typeName">Typename with or without generics</param>
    /// <returns>Typename without generics (`1)</returns>
    public static string WithoutGenerics(this Type instance)
    {
        var name = instance.IsGenericParameter
            ? instance.Name
            : instance.FullName.WhenNullOrEmpty(() => $"{instance.Namespace}.{instance.Name}");
        var index = name.IndexOf('`');

        return index == -1
            ? name.FixTypeName()
            : name.Substring(0, index).FixTypeName();
    }

    public static string ReplaceGenericTypeName(this Type instance, Type genericArguments)
        => instance.WithoutGenerics().MakeGenericTypeName(genericArguments.IsNotNull(nameof(genericArguments)).FullName);

    public static string ReplaceGenericTypeName(this Type instance, string genericArgumentsTypeName)
        => instance.WithoutGenerics().MakeGenericTypeName(genericArgumentsTypeName.IsNotNull(nameof(genericArgumentsTypeName)));

    public static string GetTypeName(this Type type, MemberInfo declaringType)
    {
        declaringType = declaringType.IsNotNull(nameof(declaringType));

        if (!type.IsGenericType)
        {
            return type.FullName.FixTypeName().WhenNullOrEmpty(() => type.Name);
        }

        var typeName = type.FullName.FixTypeName();
        if (typeName.IsCollectionTypeName())
        {
            // for now, we will ignore nullability of the generic argument on generic lists
            return typeName.ReplaceGenericTypeName(typeName.GetGenericArguments());
        }

        var builder = new StringBuilder();
        builder.Append(type.WithoutGenerics());
        builder.Append("<");
        var first = true;
        var index = 0;
        foreach (var arg in type.GetGenericArguments())
        {
            if (first)
            {
                first = false;
            }
            else
            {
                builder.Append(",");
            }

            index++;
            builder.Append(arg.GetTypeName(type));
            if ((!arg.IsGenericParameter && arg.IsNullable(arg, declaringType.CustomAttributes, index))
                || (arg.IsGenericParameter && arg.IsNullable(declaringType, declaringType.CustomAttributes, index)))
            {
                builder.Append("?");
            }
        }
        builder.Append(">");
        return builder.ToString();
    }

    public static string GetFullName(this IType type) => $"{type.Namespace.GetNamespacePrefix()}{type.Name}";

    public static bool IsRecord(this Type type)
        => type.GetMethod("<Clone>$") is not null;

    public static bool IsNullable(this Type memberType, MemberInfo declaringType, IEnumerable<CustomAttributeData> customAttributes, int index)
    {
        memberType = memberType.IsNotNull(nameof(memberType));
        declaringType = declaringType.IsNotNull(nameof(declaringType));
        customAttributes = customAttributes.IsNotNull(nameof(customAttributes));

        if (memberType.IsValueType)
        {
            return Nullable.GetUnderlyingType(memberType) is not null;
        }

        var nullable = customAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
        if (nullable is not null && nullable.ConstructorArguments.Count == 1)
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                if (args.Count > index && args[index].ArgumentType == typeof(byte))
                {
                    return (byte)args[index].Value! == 2;
                }
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte)attributeArgument.Value! == 2;
            }
        }

        for (var type = declaringType; type is not null; type = type.DeclaringType)
        {
            var context = type.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if (context is not null &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
            {
                return (byte)context.ConstructorArguments[0].Value! == 2;
            }
        }

        // Couldn't find a suitable attribute
        return false;
    }
}
