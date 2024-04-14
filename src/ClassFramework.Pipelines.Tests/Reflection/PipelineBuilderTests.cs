namespace ClassFramework.Pipelines.Tests.Reflection;

public class PipelineBuilderTests : IntegrationTestBase<IPipelineBuilder<TypeBaseBuilder, ReflectionContext>>
{
    public class IntegrationTests : PipelineBuilderTests
    {
        [Fact]
        public async Task Creates_Class_With_NamespaceMapping()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = typeof(MyClass);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true, copyInterfaces: true);
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(model, context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().NotBeNull();

            result.Value!.Attributes.Should().ContainSingle();
            result.Value.Attributes.Single().Name.Should().Be("System.ComponentModel.DisplayNameAttribute");

            result.Value.Interfaces.Should().BeEquivalentTo("MyNamespace.IMyInterface");

            result.Value.Name.Should().Be(nameof(MyClass));
            result.Value.Namespace.Should().Be("MyNamespace");

            result.Value.Visibility.Should().Be(Visibility.Public);
        }

        [Fact]
        public async Task Creates_Interface_With_NamespaceMapping()
        {
            // Arrange
            var model = new InterfaceBuilder();
            var sourceModel = typeof(IMyInterface);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(model, context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().NotBeNull();

            result.Value!.Attributes.Should().BeEmpty();
            result.Value.Interfaces.Should().BeEmpty();

            result.Value.Name.Should().Be(nameof(IMyInterface));
            result.Value.Namespace.Should().Be("MyNamespace");

            result.Value.Visibility.Should().Be(Visibility.Public);
        }

        [Fact]
        public async Task Creates_Internal_Interface()
        {
            // Arrange
            var model = new InterfaceBuilder();
            var sourceModel = typeof(IMyInternalInterface);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(model, context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Attributes.Should().BeEmpty();
            result.Value.Interfaces.Should().BeEmpty();

            result.Value.Name.Should().Be(nameof(IMyInternalInterface));
            result.Value.Namespace.Should().Be("MyNamespace");

            result.Value.Visibility.Should().Be(Visibility.Internal);
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = GetType(); // this unit test class does not have properties
            var settings = CreateSettingsForReflection();
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(model, context);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("To create a class, there must be at least one property");
        }
    }
}

[DisplayName("Test")]
public class MyClass : IMyInterface
{
    public string? MyProperty { get; set; }
    public void DoSomething(int myParameter)
    {
        // Method intentionally left empty.
    }
}

public class MyNullableClass
{
    public Func<object, IEnumerable<object?>> MyDelegateProperty { get; set; } = default!;
}

public interface IMyInterface
{
    string? MyProperty { get; set; }
    void DoSomething(int myParameter);
}

internal interface IMyInternalInterface
{
    int MyProperty { get; set; }
    int MyField { get; }
}
