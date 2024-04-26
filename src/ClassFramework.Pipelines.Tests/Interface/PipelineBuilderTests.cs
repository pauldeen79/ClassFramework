namespace ClassFramework.Pipelines.Tests.Interface;

public class PipelineBuilderTests : IntegrationTestBase<IPipelineBuilder<InterfaceContext, InterfaceBuilder>>
{
    public class Process : PipelineBuilderTests
    {
        private InterfaceContext CreateContext(bool addProperties = true, bool copyMethods = true, CopyMethodPredicate? copyMethodPredicate = null) => new InterfaceContext
        (
            CreateInterfaceModel(addProperties),
            CreateSettingsForInterface
            (
                allowGenerationWithoutProperties: false,
                copyMethods: copyMethods,
                copyMethodPredicate: copyMethodPredicate
            ).Build(),
            CultureInfo.InvariantCulture
        );

        private InterfaceBuilder Model { get; } = new();

        [Fact]
        public async Task Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Name.Should().Be("IMyClass");
            Model.Namespace.Should().Be("MyNamespace");
        }

        [Fact]
        public async Task Adds_Properties()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(false);
            Model.Properties.Select(x => x.Name).Should().BeEquivalentTo("Property1", "Property2");
            Model.Properties.Select(x => x.TypeName).Should().BeEquivalentTo("System.String", "System.Collections.Generic.IReadOnlyCollection<System.String>");
        }

        [Fact]
        public async Task Adds_Methods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Methods.Should().ContainSingle();
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(copyMethodPredicate: (_, _) => true), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Methods.Should().ContainSingle();
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True_Negative()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(copyMethodPredicate: (_, _) => false), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Methods.Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Methods_When_CopyMethods_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(copyMethods: false), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Methods.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(addProperties: false), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("To create an interface, there must be at least one property");
        }
    }

    public class IntegrationTests : PipelineBuilderTests
    {
        private InterfaceBuilder Model { get; } = new();

        [Fact]
        public async Task Creates_Interface_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateInterfaceModelWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var settings = CreateSettingsForInterface(
                namespaceMappings: namespaceMappings);
            var context = CreateContext(model, settings);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(context, Model);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            Model.Name.Should().Be("IMyClass");
            Model.Namespace.Should().Be("MyNamespace");
            Model.Interfaces.Should().BeEmpty();

            Model.Fields.Should().BeEmpty();

            Model.Properties.Select(x => x.Name).Should().BeEquivalentTo
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
            Model.Properties.Select(x => x.TypeName).Should().BeEquivalentTo
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
            Model.Properties.Select(x => x.IsNullable).Should().BeEquivalentTo
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
            Model.Properties.Select(x => x.HasGetter).Should().AllBeEquivalentTo(true);
            Model.Properties.SelectMany(x => x.GetterCodeStatements).Should().BeEmpty();
            Model.Properties.Select(x => x.HasInitializer).Should().AllBeEquivalentTo(false);
            Model.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(false);
            Model.Properties.SelectMany(x => x.SetterCodeStatements).Should().BeEmpty();
        }

        private static InterfaceContext CreateContext(IType model, PipelineSettingsBuilder settings)
            => new(model, settings.Build(), CultureInfo.InvariantCulture);
    }
}
