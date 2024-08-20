namespace ClassFramework.Domain.Extensions;

public static class StringExtensions
{
    public static string ToPascalCase(this string value, CultureInfo cultureInfo)
        => string.IsNullOrEmpty(value)
            ? value
            : string.Concat(value.Substring(0, 1).ToLower(cultureInfo), value.Substring(1));

    public static string SqlEncode(this string value)
        => "'" + value.Replace("'", "''") + "'";

    public static string FixTypeName(this string? instance)
    {
        if (instance is null)
        {
            return string.Empty;
        }

        int startIndex;
        while (true)
        {
            startIndex = instance.IndexOf(", ");
            if (startIndex == -1)
            {
                break;
            }

            int secondIndex = instance.IndexOf("]", startIndex + 1);
            if (secondIndex == -1)
            {
                break;
            }

            instance = instance.Substring(0, startIndex) + instance.Substring(secondIndex + 1);
        }

        while (true)
        {
            startIndex = instance.IndexOf("`");
            if (startIndex == -1)
            {
                break;
            }

            instance = instance.Substring(0, startIndex) + instance.Substring(startIndex + 2);
        }

        //remove assebmly name from type, e.g. System.String, mscorlib bla bla bla -> System.String
        var index = instance.IndexOf(", ");
        if (index > -1)
        {
            instance = instance.Substring(0, index);
        }

        return FixAnonymousTypeName(instance
            .Replace("[[", "<")
            .Replace(",[", ",")
            .Replace(",]", ">")
            .Replace(']', '>')
            .Replace("[>", "[]") //hacking here! caused by the previous statements...
            .Replace("System.Void", "void")
            .Replace('+', '.')
            .Replace("&", ""));
    }

    public static string GetCsharpFriendlyName(this string instance)
        => _keywords.Contains(instance)
            ? "@" + instance
            : instance;

    public static string GetCsharpFriendlyTypeName(this string instance)
        => instance switch
        {
            "System.Char" => WellKnownTypes.Char,
            "System.String" => WellKnownTypes.String,
            "System.Boolean" => WellKnownTypes.Boolean,
            "System.Object" => WellKnownTypes.Object,
            "System.Decimal" => WellKnownTypes.Decimal,
            "System.Double" => WellKnownTypes.Double,
            "System.Single" => WellKnownTypes.Float,
            "System.Byte" => WellKnownTypes.Byte,
            "System.SByte" => WellKnownTypes.SignedByte,
            "System.Int16" => WellKnownTypes.Short,
            "System.UInt16" => WellKnownTypes.UnsignedShort,
            "System.Int32" => WellKnownTypes.Int,
            "System.UInt32" => WellKnownTypes.UnsignedInt,
            "System.Int64" => WellKnownTypes.Long,
            "System.UInt64" => WellKnownTypes.UnsignedLong,
            _ => instance
                .ReplaceGenericArgument("System.Char", WellKnownTypes.Char)
                .ReplaceGenericArgument("System.String", WellKnownTypes.String)
                .ReplaceGenericArgument("System.Boolean", WellKnownTypes.Boolean)
                .ReplaceGenericArgument("System.Object", WellKnownTypes.Object)
                .ReplaceGenericArgument("System.Decimal", WellKnownTypes.Decimal)
                .ReplaceGenericArgument("System.Double", WellKnownTypes.Double)
                .ReplaceGenericArgument("System.Single", WellKnownTypes.Float)
                .ReplaceGenericArgument("System.Byte", WellKnownTypes.Byte)
                .ReplaceGenericArgument("System.SByte", WellKnownTypes.SignedByte)
                .ReplaceGenericArgument("System.Int16", WellKnownTypes.Short)
                .ReplaceGenericArgument("System.UInt16", WellKnownTypes.UnsignedShort)
                .ReplaceGenericArgument("System.Int32", WellKnownTypes.Int)
                .ReplaceGenericArgument("System.UInt32", WellKnownTypes.UnsignedInt)
                .ReplaceGenericArgument("System.Int64", WellKnownTypes.Long)
                .ReplaceGenericArgument("System.UInt64", WellKnownTypes.UnsignedLong)
        };

    public static string GetGenericArguments(this string value, bool addBrackets = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }
        //Bla<GenericArg1,...>

        var open = value.IndexOf("<");
        if (open == -1)
        {
            return string.Empty;
        }

        var comma = value.LastIndexOf(",");
        if (comma == -1)
        {
            comma = value.LastIndexOf(">");
        }

        if (comma == -1)
        {
            return string.Empty;
        }

        var generics = value.Substring(open + 1, comma - open - 1);

        return addBrackets
            ? $"<{generics}>"
            : generics;
    }

    public static string GetProcessedGenericArguments(this string? value, bool addBrackets = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var open = value!.IndexOf("<");
        if (open == -1)
        {
            return string.Empty;
        }

        var close = value.LastIndexOf(">");
        if (close == -1)
        {
            return string.Empty;
        }

        var generics = value.Substring(open + 1, close - open - 1);

        return addBrackets
            ? $"<{generics}>"
            : generics;
    }

    public static string Sanitize(this string? token)
    {
        if (token is null)
        {
            return string.Empty;
        }

        // Replace all invalid chars by underscores 
        token = Regex.Replace(token, @"[\W\b]", "_", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200));

        // If it starts with a digit, prefix it with an underscore 
        token = Regex.Replace(token, @"^\d", @"_$0", RegexOptions.None, TimeSpan.FromMilliseconds(200));

        return token;
    }

    public static bool IsRequiredEnum(this string? instance)
        => !string.IsNullOrEmpty(instance) && Type.GetType(instance)?.IsEnum == true;

    public static bool IsOptionalEnum(this string? instance)
    {
        if (string.IsNullOrEmpty(instance))
        {
            return false;
        }

        var t = Type.GetType(instance);
        if (t is null)
        {
            return false;
        }

        var u = Nullable.GetUnderlyingType(t);
        return u?.IsEnum == true;
    }

    public static string GetClassName(this string fullyQualifiedClassName)
    {
        var bracket = fullyQualifiedClassName.IndexOf("<");
        var idx = bracket == -1
            ? fullyQualifiedClassName.LastIndexOf(".")
            : fullyQualifiedClassName.LastIndexOf(".", bracket);

        return idx == -1
            ? fullyQualifiedClassName
            : fullyQualifiedClassName.Substring(idx + 1);
    }

    public static string GetNamespaceWithDefault(this string? fullyQualifiedClassName, string defaultValue = "")
    {
        if (fullyQualifiedClassName is null || string.IsNullOrEmpty(fullyQualifiedClassName))
        {
            return defaultValue;
        }

        var bracket = fullyQualifiedClassName.IndexOf("<");
        var idx = bracket == -1
            ? fullyQualifiedClassName.LastIndexOf(".")
            : fullyQualifiedClassName.LastIndexOf(".", bracket);

        return idx == -1
            ? defaultValue
            : fullyQualifiedClassName.Substring(0, idx);
    }

    public static string MakeGenericTypeName(this string instance, string genericTypeParameter)
        => string.IsNullOrEmpty(genericTypeParameter)
            ? instance
            : $"{instance}<{genericTypeParameter}>";

    public static string MakeGenericTypeName(this string instance, params string[] genericTypeParameters)
        => genericTypeParameters == null || genericTypeParameters.Length == 0
            ? instance
            : $"{instance}<{string.Join(",", genericTypeParameters)}>";

    public static string ReplaceSuffix(this string instance, string find, string replace, StringComparison stringComparison)
    {
        find = find.IsNotNull(nameof(find));

        var index = instance.LastIndexOf(find, stringComparison);
        if (index == -1 || index < instance.Length - find.Length)
        {
            return instance;
        }

        return instance.Substring(0, instance.Length - find.Length) + replace;
    }

    public static bool IsStringTypeName(this string? instance)
        => instance.FixTypeName() == typeof(string).FullName;

    public static bool IsBooleanTypeName(this string? instance)
        => instance.FixTypeName() == typeof(bool).FullName;

    public static bool IsNullableBooleanTypeName(this string? instance)
        => instance.FixTypeName() == typeof(bool?).FullName.FixTypeName();

    public static bool IsObjectTypeName(this string? instance)
        => instance.FixTypeName() == typeof(object).FullName;

    public static string ConvertTypeNameToArray(this string typeName)
        => $"{typeName.GetGenericArguments()}[]";

    public static string FixCollectionTypeName(this string typeName, string newCollectionTypeName)
    {
        var fixedTypeName = typeName.FixTypeName();
        return !string.IsNullOrEmpty(newCollectionTypeName)
            && !string.IsNullOrEmpty(fixedTypeName.GetCollectionItemType())
                ? $"{newCollectionTypeName}<{fixedTypeName.GetCollectionItemType()}>"
                : fixedTypeName;
    }

    private static readonly string[] collectionTypeNames =
    [
        "Enumerable<",
        "List<",
        "Collection<",
        "Array<"
    ];

    public static bool IsCollectionTypeName(this string typeName)
        => Array.Exists(collectionTypeNames, x => typeName.IndexOf(x) > -1 && typeName.IndexOf("<") > typeName.IndexOf(x))
        || typeName.EndsWith("[]");

    public static string GetCollectionItemType(this string? instance)
    {
        if (string.IsNullOrEmpty(instance) || !instance!.IsCollectionTypeName())
        {
            return string.Empty;
        }

        if (instance!.EndsWith("[]"))
        {
            return instance.Substring(0, instance.Length - 2);
        }

        return instance.GetGenericArguments();
    }

    public static string RemoveInterfacePrefix(this string name)
    {
        var index = name.IndexOf(".");
        if (index == -1)
        {
            return name;
        }
        return name.Substring(index + 1);
    }

    /// <summary>
    /// Removes generics from a typename. (`1)
    /// </summary>
    /// <param name="typeName">Typename with or without generics</param>
    /// <returns>Typename without generics (`1)</returns>
    public static string WithoutGenerics(this string instance)
    {
        var index = instance.IndexOf('`');
        return index == -1
            ? instance.FixTypeName()
            : instance.Substring(0, index).FixTypeName();
    }

    /// <summary>
    /// Removes generics from a processed (fixed) typename. (<)
    /// </summary>
    /// <param name="typeName">Typename with or without generics</param>
    /// <returns>Typename without generics (<)</returns>
    public static string WithoutProcessedGenerics(this string instance)
    {
        var index = instance.IndexOf('<');
        return index == -1
            ? instance
            : instance.Substring(0, index);
    }

    public static string GetDefaultValue(this string typeName, bool isNullable, bool isValueType, bool enableNullableReferenceTypes)
    {
        if ((typeName.IsStringTypeName() || typeName == WellKnownTypes.String) && !isNullable)
        {
            return "string.Empty";
        }

        if ((typeName.IsObjectTypeName() || typeName == WellKnownTypes.Object) && !isNullable)
        {
            return $"new {typeof(object).FullName}()";
        }

        if (typeName == typeof(IEnumerable).FullName && !isNullable)
        {
            return $"{typeof(Enumerable).FullName}.{nameof(Enumerable.Empty)}<{typeof(object).FullName}>()";
        }

        if (typeName.WithoutProcessedGenerics() == typeof(IEnumerable<>).WithoutGenerics() && !isNullable)
        {
            return $"{typeof(Enumerable).FullName}.{nameof(Enumerable.Empty)}{typeName.GetProcessedGenericArguments(addBrackets: true)}()";
        }

        var preNullableSuffix = isNullable && (enableNullableReferenceTypes || isValueType) && !typeName.EndsWith("?") && !typeName.StartsWith(typeof(Nullable<>).WithoutGenerics())
            ? "?"
            : string.Empty;

        var postNullableSuffix = preNullableSuffix.Length == 0 && !isNullable && enableNullableReferenceTypes && !isValueType
            ? "!"
            : string.Empty;

        return $"default({typeName}{preNullableSuffix}){postNullableSuffix}";
    }

    public static string AppendNullableAnnotation(this string instance,
                                                  bool isNullable,
                                                  bool enableNullableReferenceTypes)
        => !string.IsNullOrEmpty(instance)
            && !instance.StartsWith(typeof(Nullable<>).WithoutGenerics())
            && !instance.EndsWith("?")
            && isNullable
            && enableNullableReferenceTypes
                ? $"{instance}?"
                : instance;

    public static string AbbreviateNamespaces(this string instance, IEnumerable<string> namespacesToAbbreviate)
    {
        foreach (var ns in namespacesToAbbreviate.IsNotNull(nameof(namespacesToAbbreviate)))
        {
            if (instance.GetNamespaceWithDefault() == ns)
            {
                return instance.GetClassName();
            }
        }

        return instance;
    }

    public static string ReplaceGenericTypeName(this string instance, string genericArguments)
        => instance == instance.WithoutProcessedGenerics()
            ? instance
            : instance.WithoutProcessedGenerics().MakeGenericTypeName(genericArguments);

    public static string ReplaceGenericTypeName(this string instance, params string[] genericArguments)
        => instance == instance.WithoutProcessedGenerics()
            ? instance
            : instance.WithoutProcessedGenerics().MakeGenericTypeName(genericArguments);

    public static string GetNamespacePrefix(this string instance)
        => string.IsNullOrEmpty(instance)
            ? string.Empty
            : $"{instance}.";

    public static string GetCollectionInitializeStatement(this string instance, string customBuilderConstructorInitializeExpression)
    {
        if (instance.StartsWith(typeof(IEnumerable<>).WithoutGenerics()))
        {
            return $"{typeof(Enumerable).FullName}.{nameof(Enumerable.Empty)}{instance.GetProcessedGenericArguments(addBrackets: true)}()";
        }

        if (!string.IsNullOrEmpty(customBuilderConstructorInitializeExpression))
        {
            var expression = $"source.[Name].Select(x => " + customBuilderConstructorInitializeExpression + ")";
            return $"new {instance}({expression})";
        }

        return $"new {instance}()";
    }

    private static string FixAnonymousTypeName(string instance)
    {
        var isAnonymousType = instance.Contains("AnonymousType")
            && (instance.Contains("<>") || instance.Contains("VB$"));

        var arraySuffix = instance.EndsWith("[]")
            ? "[]"
            : string.Empty;

        return isAnonymousType
            ? $"AnonymousType{arraySuffix}"
            : instance;
    }

    private static string ReplaceGenericArgument(this string instance, string find, string replace)
        => instance
            .Replace($"<{find}", $"<{replace}")
            .Replace($"{find}>", $"{replace}>")
            .Replace($",{find}", $",{replace}")
            .Replace($", {find}", $", {replace}")
            .Replace($"{find}[]", $"{replace}[]");

    private static readonly string[] _keywords =
    [
        "abstract",
        "as",
        "base",
        WellKnownTypes.Boolean,
        "break",
        WellKnownTypes.Byte,
        "case",
        "catch",
        WellKnownTypes.Char,
        "checked",
        "class",
        "const",
        "continue",
        WellKnownTypes.Decimal,
        "default",
        "delegate",
        "do",
        WellKnownTypes.Double,
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        WellKnownTypes.Float,
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        WellKnownTypes.Int,
        "interface",
        "internal",
        "is",
        "lock",
        WellKnownTypes.Long,
        "namespace",
        "new",
        "null",
        WellKnownTypes.Object,
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        WellKnownTypes.SignedByte,
        "sealed",
        WellKnownTypes.Short,
        "sizeof",
        "stackalloc",
        "static",
        WellKnownTypes.String,
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        WellKnownTypes.UnsignedInt,
        WellKnownTypes.UnsignedLong,
        "unchecked",
        "unsafe",
        WellKnownTypes.UnsignedShort,
        "using",
        "virtual",
        "void",
        "volatile",
        "while"
    ];
}
