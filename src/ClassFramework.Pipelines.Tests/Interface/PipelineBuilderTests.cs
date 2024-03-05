namespace ClassFramework.Pipelines.Tests.Interface;

public class PipelineBuilderTests : IntegrationTestBase<IPipelineBuilder<InterfaceBuilder, InterfaceContext>>
{
    public class Process : PipelineBuilderTests
    {
        private InterfaceContext CreateContext(bool addProperties = true) => new InterfaceContext
        (
            CreateInterfaceModel(addProperties),
            CreateSettingsForInterface
            (
                allowGenerationWithoutProperties: false
            ).Build(),
            CultureInfo.InvariantCulture
        );

        private InterfaceBuilder Model { get; } = new();

        [Fact]
        public void Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = sut.Process(Model, CreateContext());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("IMyClass");
            result.Value.Namespace.Should().Be("MyNamespace");
        }

        [Fact]
        public void Adds_Properties()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = sut.Process(Model, CreateContext());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
            result.Value!.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(false);
            result.Value.Properties.Select(x => x.Name).Should().BeEquivalentTo("Property1", "Property2");
            result.Value.Properties.Select(x => x.TypeName).Should().BeEquivalentTo("System.String", "System.Collections.Generic.IReadOnlyCollection<System.String>");
        }

        [Fact]
        public void Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = sut.Process(Model, CreateContext(addProperties: false));

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("To create an interface, there must be at least one property");
        }
    }

    public class IntegrationTests : PipelineBuilderTests
    {
        private InterfaceBuilder Model { get; } = new();

        [Fact]
        public void Creates_Interface_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateInterfaceModelWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var settings = CreateSettingsForInterface(
                namespaceMappings: namespaceMappings);
            var context = CreateContext(model, settings);

            var sut = CreateSut().Build();

            // Act
            var result = sut.Process(Model, context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().NotBeNull();

            result.Value!.Name.Should().Be("IMyClass");
            result.Value.Namespace.Should().Be("MyNamespace");
            result.Value.Interfaces.Should().BeEmpty();

            result.Value.Fields.Should().BeEmpty();

            result.Value.Properties.Select(x => x.Name).Should().BeEquivalentTo
            (
                "Property1",
                "Property2",
                "Property3",
                "Property4",
                "Property5",
                "Property6",
                "Property7",
                "Property8"
            );
            result.Value.Properties.Select(x => x.TypeName).Should().BeEquivalentTo
            (
                "System.Int32",
                "System.Nullable<System.Int32>",
                "System.String",
                "System.String",
                "MyNamespace.IMyClass",
                "MyNamespace.IMyClass",
                "System.Collections.Generic.IReadOnlyCollection<MyNamespace.IMyClass>",
                "System.Collections.Generic.IReadOnlyCollection<MyNamespace.IMyClass>"
            );
            result.Value.Properties.Select(x => x.IsNullable).Should().BeEquivalentTo
            (
                new[]
                {
                    false,
                    true,
                    false,
                    true,
                    false,
                    true,
                    false,
                    true
                }
            );
            result.Value.Properties.Select(x => x.HasGetter).Should().AllBeEquivalentTo(true);
            result.Value.Properties.SelectMany(x => x.GetterCodeStatements).Should().BeEmpty();
            result.Value.Properties.Select(x => x.HasInitializer).Should().AllBeEquivalentTo(false);
            result.Value.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(false);
            result.Value.Properties.SelectMany(x => x.SetterCodeStatements).Should().BeEmpty();
        }

        private static InterfaceContext CreateContext(IType model, PipelineSettingsBuilder settings)
            => new(model, settings.Build(), CultureInfo.InvariantCulture);
    }
}
