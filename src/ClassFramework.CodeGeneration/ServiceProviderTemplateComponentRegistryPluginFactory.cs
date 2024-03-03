namespace ClassFramework.CodeGeneration;

[ExcludeFromCodeCoverage]
public class ServiceProviderTemplateComponentRegistryPluginFactory : ITemplateComponentRegistryPluginFactory
{
    private readonly ITemplateFactory _templateFactory;
    private readonly IAssemblyService? _assemblyService;

    public ServiceProviderTemplateComponentRegistryPluginFactory(ITemplateFactory templateFactory, IAssemblyService assemblyService)
    {
        Guard.IsNotNull(templateFactory);
        Guard.IsNotNull(assemblyService);

        _templateFactory = templateFactory;
        _assemblyService = assemblyService;
    }

    public ITemplateComponentRegistryPlugin Create(string assemblyName, string className, string currentDirectory)
    {
        Guard.IsNotNull(assemblyName);
        Guard.IsNotNull(className);
        Guard.IsNotNull(currentDirectory);
    
        var assembly = _assemblyService?.GetAssembly(assemblyName, currentDirectory);
        var type = assembly?.GetType(className) ?? throw new NotSupportedException($"Could not instanciate class name [{className}] from assembly [{assemblyName}]");

        if (_templateFactory.Create(type) is not ITemplateComponentRegistryPlugin plugin)
        {
            throw new NotSupportedException($"Could not convert class name [{className}] from assembly [{assemblyName}] to an instance of type ${nameof(ITemplateComponentRegistryPlugin)}");
        }

        return plugin;
    }
}
