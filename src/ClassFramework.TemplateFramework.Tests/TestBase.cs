namespace ClassFramework.TemplateFramework.Tests;

public class TestBase
{
    protected IFixture Fixture { get; }

    protected TestBase()
    {
        Fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Fixture.Register(() => CreateCsharpClassGeneratorSettings(true));
    }

    protected static CsharpClassGeneratorSettings CreateCsharpClassGeneratorSettings(
        bool generateMultipleFiles = true,
        bool enableNullableContext = true,
        bool enableNullablePragmas = true,
        bool enableGlobalUsings = false,
        string path = "")
        => new CsharpClassGeneratorSettingsBuilder()
            .WithRecurseOnDeleteGeneratedFiles(false)
            .WithLastGeneratedFilesFilename(string.Empty)
            .WithEncoding(Encoding.UTF8)
            .WithGenerateMultipleFiles(generateMultipleFiles)
            //.WithSkipWhenFileExists(false) // default value
            .WithCreateCodeGenerationHeader(true)
            .WithEnableGlobalUsings(enableGlobalUsings)
            .WithEnableNullableContext(enableNullableContext)
            .WithEnableNullablePragmas(enableNullablePragmas)
            .WithCultureInfo(CultureInfo.InvariantCulture)
            .WithEnvironmentVersion("1.0.0")
            .WithPath(path)
            .Build();

    protected ITemplateContext CreateTemplateContext(object? template = null, object? model = null)
        => new TemplateContext(Fixture.Freeze<ITemplateEngine>(), Fixture.Freeze<ITemplateComponentRegistry>(), "default.cs", Fixture.Freeze<ITemplateIdentifier>(), template ?? new object(), model);
}

/// <summary>
/// Base class for non-Template/Generator (i.e. ViewModel) unit tests. This base class uses AutoFixture acvitation, so with assigning fixtures to properties.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TestBase<T> : TestBase
{
    protected T CreateSut() => Fixture.Create<T>();
}
