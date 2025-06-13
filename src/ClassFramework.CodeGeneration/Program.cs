namespace ClassFramework.CodeGeneration;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        // Setup code generation
        var currentDirectory = Directory.GetCurrentDirectory();
        var basePath = currentDirectory.EndsWith("ClassFramework")
            ? Path.Combine(currentDirectory, @"src/")
            : Path.Combine(currentDirectory, @"../../../../");
        var services = new ServiceCollection()
            .AddExpressionEvaluator()
            .AddClassFrameworkPipelines()
            .AddTemplateFramework()
            .AddTemplateFrameworkChildTemplateProvider()
            .AddTemplateFrameworkCodeGeneration()
            .AddTemplateFrameworkRuntime()
            .AddCsharpExpressionDumper()
            .AddClassFrameworkTemplates()
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
        var engine = scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();

        // Generate code
        foreach (var generatorType in generators)
        {
            var generator = (CsharpClassGeneratorCodeGenerationProviderBase)scope.ServiceProvider.GetRequiredService(generatorType);
            var result = await engine.GenerateAsync(generator, new MultipleStringContentBuilderEnvironment(), new CodeGenerationSettings(basePath, Path.Combine(generator.Path, $"{generatorType.Name}.template.generated.cs"))).ConfigureAwait(false);
            if (!result.IsSuccessful())
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                Console.WriteLine("Errors:");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                WriteError(result);
                break;
            }
        }

        // Log output to console
        if (!string.IsNullOrEmpty(basePath))
        {
            Console.WriteLine($"Code generation completed, check the output in {basePath}");
        }
    }

    private static void WriteError(Result error)
    {
        Console.WriteLine($"{error.Status} {error.ErrorMessage}");
        foreach (var validationError in error.ValidationErrors)
        {
            Console.WriteLine($"{string.Join(",",  validationError.MemberNames)}: {validationError.ErrorMessage}");
        }

        foreach (var innerResult in error.InnerResults)
        {
            WriteError(innerResult);
        }
    }
}
