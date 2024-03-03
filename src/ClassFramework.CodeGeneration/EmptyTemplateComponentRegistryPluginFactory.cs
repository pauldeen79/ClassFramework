namespace ClassFramework.CodeGeneration;

[ExcludeFromCodeCoverage]
public class EmptyTemplateComponentRegistryPluginFactory : ITemplateComponentRegistryPluginFactory
{
    public ITemplateComponentRegistryPlugin Create(string assemblyName, string className, string currentDirectory)
    {
        throw new NotImplementedException();
    }
}
