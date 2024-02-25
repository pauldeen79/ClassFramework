namespace ClassFramework.CodeGeneration;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static void Main(string[] args)
    {
        // Setup code generation
        var currentDirectory = Directory.GetCurrentDirectory();
        var basePath = currentDirectory.EndsWith("ModelFramework")
            ? Path.Combine(currentDirectory, @"src/")
            : Path.Combine(currentDirectory, @"../../../../");
        var generateMultipleFiles = true;
        var dryRun = false;
        var multipleContentBuilder = new MultipleContentBuilder { BasePath = basePath };

        // Generate code
        var generationTypeNames = new[] { "Entities", "Builders", "Models", "BuilderFactory", "ModelFactory", "Interfaces", "Extensions" };
        var generators = typeof(ClassFrameworkCSharpClassBase).Assembly.GetExportedTypes().Where(x => !x.IsAbstract && x.BaseType == typeof(ClassFrameworkCSharpClassBase)).ToArray();
        var generationTypes = generators.Where(x => x.Name.EndsWithAny(generationTypeNames));
        _ = generationTypes.Select(x => (ClassFrameworkCSharpClassBase)Activator.CreateInstance(x)!).Select(x => GenerateCode.For(new(basePath, generateMultipleFiles, false, dryRun), multipleContentBuilder, x)).ToArray();
        ///var scaffoldingTypes = generators.Where(x => !x.Name.EndsWithAny(generationTypeNames));
        ///_ = scaffoldingTypes.Select(x => (ClassFrameworkCSharpClassBase)Activator.CreateInstance(x)!).Select(x => GenerateCode.For(new(basePath, generateMultipleFiles, true, dryRun), multipleContentBuilder, x)).ToArray();

        // Log output to console
        if (string.IsNullOrEmpty(basePath))
        {
            Console.WriteLine(multipleContentBuilder.ToString());
        }
        else
        {
            Console.WriteLine($"Code generation completed, check the output in {basePath}");
            Console.WriteLine($"Generated files: {multipleContentBuilder.Contents.Count()}");
        }
    }
}
