namespace ClassFramework.CodeGeneration;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static void Main(string[] args)
    {
        // Setup code generation
        var currentDirectory = Directory.GetCurrentDirectory();
        var basePath = currentDirectory.EndsWith("ClassFramework")
            ? Path.Combine(currentDirectory, @"src/")
            : Path.Combine(currentDirectory, @"../../../../");
        var dryRun = false;
        var codeGenerationSettings = new CodeGenerationSettings(basePath, "GeneratedCode.cs", dryRun);
        var services = new ServiceCollection()
            .AddParsers()
            .AddPipelines()
            .AddTemplateFramework()
            .AddTemplateFrameworkChildTemplateProvider()
            .AddTemplateFrameworkCodeGeneration()
            .AddTemplateFrameworkRuntime()
            .AddCsharpExpressionCreator()
            .AddClassFrameworkTemplates()
            .AddScoped<ITemplateFactory>(p => new ServiceProviderCompiledTemplateFactory(p))
            .AddScoped<ITemplateComponentRegistryPluginFactory, ServiceProviderTemplateComponentRegistryPluginFactory>()
            .AddScoped<IAssemblyInfoContextService, MyAssemblyInfoContextService>();

        var generators = typeof(Program).Assembly.GetExportedTypes()
            .Where(x => !x.IsAbstract && x.BaseType == typeof(ClassFrameworkCSharpClassBase))
            .ToArray();

        foreach (var type in generators)
        {
            services.AddScoped(type);
        }

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var generationEnvironment = new MultipleContentBuilderEnvironment();
        var instances = generators
            .Select(x => (ICodeGenerationProvider)scope.ServiceProvider.GetRequiredService(x))
            .ToArray();
        var engine = scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();

        // Generate code
        foreach (var instance in instances)
        {
            engine.Generate(instance, generationEnvironment, codeGenerationSettings);
            if (!codeGenerationSettings.DryRun)
            {
                generationEnvironment.SaveContents(instance, codeGenerationSettings.BasePath, codeGenerationSettings.DefaultFilename);
            }
        }

        // Log output to console
        if (string.IsNullOrEmpty(basePath))
        {
            Console.WriteLine(generationEnvironment.Builder.ToString());
        }
        else
        {
            Console.WriteLine($"Code generation completed, check the output in {basePath}");
            Console.WriteLine($"Generated files: {generationEnvironment.Builder.Contents.Count()}");
        }
    }
}
