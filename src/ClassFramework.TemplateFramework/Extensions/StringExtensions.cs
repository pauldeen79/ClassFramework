namespace ClassFramework.TemplateFramework.Extensions;

public static class StringExtensions
{
    public static string AppendGenerics(this string typeName, IReadOnlyCollection<ITypeContainer> genericTypeArguments)
    {
        genericTypeArguments = ArgumentGuard.IsNotNull(genericTypeArguments, nameof(genericTypeArguments));

        if (genericTypeArguments.Count == 0 || typeName.EndsWith('>') || typeName.EndsWith(">?"))
        {
            return typeName;
        }

        var generics = string.Join(", ", genericTypeArguments.Select(x => x.TypeName.AppendGenerics(x.GenericTypeArguments)));
        return $"{typeName}<{generics}>";
    }
}