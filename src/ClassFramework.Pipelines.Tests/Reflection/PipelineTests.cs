namespace ClassFramework.Pipelines.Tests.Reflection;

public class PipelineTests : IntegrationTestBase<ICommandService>
{
    public class IntegrationTests : PipelineTests
    {
        [Fact]
        public async Task Creates_Class_With_NamespaceMapping()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true, copyInterfaces: true);
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<ReflectionContext, TypeBaseBuilder>(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Attributes.Count.ShouldBe(1);
            result.Value.Attributes.Single().Name.ShouldBe("System.ComponentModel.DisplayNameAttribute");

            result.Value.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "MyNamespace.IMyInterface" });

            result.Value.Name.ShouldBe(nameof(MyClass));
            result.Value.Namespace.ShouldBe("MyNamespace");

            result.Value.Visibility.ShouldBe(Visibility.Public);
        }

        [Fact]
        public async Task Creates_Interface_With_NamespaceMapping()
        {
            // Arrange
            var sourceModel = typeof(IMyInterface);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<ReflectionContext, TypeBaseBuilder>(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Attributes.ShouldBeEmpty();
            result.Value.Interfaces.ShouldBeEmpty();

            result.Value.Name.ShouldBe(nameof(IMyInterface));
            result.Value.Namespace.ShouldBe("MyNamespace");

            result.Value.Visibility.ShouldBe(Visibility.Public);
        }

        [Fact]
        public async Task Creates_Internal_Interface()
        {
            // Arrange
            var sourceModel = typeof(IMyInternalInterface);
            var namespaceMappings = CreateNamespaceMappings("ClassFramework.Pipelines.Tests.Reflection");
            var settings = CreateSettingsForReflection(namespaceMappings: namespaceMappings, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<ReflectionContext, TypeBaseBuilder>(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Attributes.ShouldBeEmpty();
            result.Value.Interfaces.ShouldBeEmpty();

            result.Value.Name.ShouldBe(nameof(IMyInternalInterface));
            result.Value.Namespace.ShouldBe("MyNamespace");

            result.Value.Visibility.ShouldBe(Visibility.Internal);
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sourceModel = GetType(); // this unit test class does not have properties
            var settings = CreateSettingsForReflection();
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<ReflectionContext, TypeBaseBuilder>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("There must be at least one property");
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
