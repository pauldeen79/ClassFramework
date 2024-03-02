namespace ClassFramework.CodeGeneration;

public class ServiceProviderCompiledTemplateFactory : ITemplateFactory
{
    public IServiceProvider Provider { get; set; } = default!;

    public object Create(Type type)
    {
        Guard.IsNotNull(type);

        var ctors = type.GetConstructors();
        if (ctors.Length == 0)
        {
            throw new NotSupportedException($"Type {type.FullName} does not have a public constructor");
        }
        else if (ctors.Length > 1)
        {
            throw new NotSupportedException($"Type {type.FullName} does not have a single public constructor. Cannot choose the constructor.");
        }

        var values = new List<object?>();
        foreach (var arg in ctors[0].GetParameters())
        {
            var svc = Provider.GetService(arg.ParameterType);
            values.Add(svc);
        }

        return ctors[0].Invoke(values.ToArray());
    }
}
