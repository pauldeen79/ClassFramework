namespace ClassFramework.Pipelines.Tests.Interface;

public class PipelineBuilderTests : IntegrationTestBase<IPipelineBuilder<InterfaceContext>>
{
    public class Process : PipelineBuilderTests
    {
        private InterfaceContext CreateContext(bool addProperties = true, bool copyMethods = true, CopyMethodPredicate? copyMethodPredicate = null) => new(
            CreateInterfaceModel(addProperties),
            CreateSettingsForInterface
            (
                allowGenerationWithoutProperties: false,
                copyMethods: copyMethods,
                copyMethodPredicate: copyMethodPredicate
            ).Build(),
            CultureInfo.InvariantCulture
        );

        [Fact]
        public async Task Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Name.Should().Be("IMyClass");
            context.Builder.Namespace.Should().Be("MyNamespace");
        }

        [Fact]
        public async Task Adds_Properties()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(false);
            context.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo("Property1", "Property2");
            context.Builder.Properties.Select(x => x.TypeName).Should().BeEquivalentTo("System.String", "System.Collections.Generic.IReadOnlyCollection<System.String>");
        }

        [Fact]
        public async Task Adds_Methods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Methods.Should().ContainSingle();
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext(copyMethodPredicate: (_, _) => true);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Methods.Should().ContainSingle();
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True_Negative()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext(copyMethodPredicate: (_, _) => false);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Methods.Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Methods_When_CopyMethods_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext(copyMethods: false);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Methods.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext(addProperties: false);

            // Act
            var result = await sut.Process(context);
            var innerResult = result?.InnerResults.FirstOrDefault();

            // Assert
            innerResult.Should().NotBeNull();
            innerResult!.Status.Should().Be(ResultStatus.Invalid);
            innerResult.ErrorMessage.Should().Be("To create an interface, there must be at least one property");
        }
    }

    public class IntegrationTests : PipelineBuilderTests
    {
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
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            context.Builder.Name.Should().Be("IMyClass");
            context.Builder.Namespace.Should().Be("MyNamespace");
            context.Builder.Interfaces.Should().BeEmpty();

            context.Builder.Fields.Should().BeEmpty();

            context.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo
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
            context.Builder.Properties.Select(x => x.TypeName).Should().BeEquivalentTo
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
            context.Builder.Properties.Select(x => x.IsNullable).Should().BeEquivalentTo
            (
                [
                    false,
                    true,
                    false,
                    true,
                    false,
                    true,
                    false,
                    true
                ]
            );
            context.Builder.Properties.Select(x => x.HasGetter).Should().AllBeEquivalentTo(true);
            context.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().BeEmpty();
            context.Builder.Properties.Select(x => x.HasInitializer).Should().AllBeEquivalentTo(false);
            context.Builder.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(false);
            context.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().BeEmpty();
        }

        private static InterfaceContext CreateContext(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings.Build(), CultureInfo.InvariantCulture);
    }
}
