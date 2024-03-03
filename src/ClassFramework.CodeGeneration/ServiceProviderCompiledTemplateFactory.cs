namespace ClassFramework.CodeGeneration;

[ExcludeFromCodeCoverage]
public class ServiceProviderCompiledTemplateFactory : ITemplateFactory
{
    public IServiceProvider Provider { get; }

    public ServiceProviderCompiledTemplateFactory(IServiceProvider provider)
    {
        Guard.IsNotNull(provider);

        Provider = provider;
    }

    public object Create(Type type)
    {
        Guard.IsNotNull(type);

        return Provider.GetRequiredService(type);
    }
}
