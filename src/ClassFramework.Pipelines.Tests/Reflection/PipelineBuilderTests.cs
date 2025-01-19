namespace ClassFramework.Pipelines.Tests.Reflection;

public class PipelineBuilderTests : IntegrationTestBase<IPipeline<ReflectionContext>>
{
    public class IntegrationTests : PipelineBuilderTests
    {
        [Fact]
        public async Task Creates_Class_With_NamespaceMapping()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true, copyInterfaces: true);
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            context.Builder.Attributes.Should().ContainSingle();
            context.Builder.Attributes.Single().Name.Should().Be("System.ComponentModel.DisplayNameAttribute");

            context.Builder.Interfaces.Should().BeEquivalentTo("MyNamespace.IMyInterface");

            context.Builder.Name.Should().Be(nameof(MyClass));
            context.Builder.Namespace.Should().Be("MyNamespace");

            context.Builder.Visibility.Should().Be(Visibility.Public);
        }

        [Fact]
        public async Task Creates_Interface_With_NamespaceMapping()
        {
            // Arrange
            var sourceModel = typeof(IMyInterface);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            context.Builder.Attributes.Should().BeEmpty();
            context.Builder.Interfaces.Should().BeEmpty();

            context.Builder.Name.Should().Be(nameof(IMyInterface));
            context.Builder.Namespace.Should().Be("MyNamespace");

            context.Builder.Visibility.Should().Be(Visibility.Public);
        }

        [Fact]
        public async Task Creates_Internal_Interface()
        {
            // Arrange
            var sourceModel = typeof(IMyInternalInterface);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            context.Builder.Attributes.Should().BeEmpty();
            context.Builder.Interfaces.Should().BeEmpty();

            context.Builder.Name.Should().Be(nameof(IMyInternalInterface));
            context.Builder.Namespace.Should().Be("MyNamespace");

            context.Builder.Visibility.Should().Be(Visibility.Internal);
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sourceModel = GetType(); // this unit test class does not have properties
            var settings = CreateSettingsForReflection();
            var context = new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture);
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);
            var innerResult = result?.InnerResults.FirstOrDefault();

            // Assert
            innerResult.Should().NotBeNull();
            innerResult!.Status.Should().Be(ResultStatus.Invalid);
            innerResult.ErrorMessage.Should().Be("To create a class, there must be at least one property");
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
