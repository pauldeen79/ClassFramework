namespace ClassFramework.Pipelines.Tests.BuilderExtension;

public class PipelineBuilderTests : IntegrationTestBase<IPipelineBuilder<BuilderExtensionContext, IConcreteTypeBuilder>>
{
    public class Process : PipelineBuilderTests
    {
        private BuilderExtensionContext CreateContext(bool addProperties = true)
            => new BuilderExtensionContext
            (
                CreateGenericModel(addProperties),
                CreateSettingsForBuilder
                (
                    builderNamespaceFormatString: "{Namespace}.Builders",
                    allowGenerationWithoutProperties: false,
                    copyAttributes: true
                ).Build(),
                CultureInfo.InvariantCulture
            );

        private ClassBuilder Model { get; } = new();

        [Fact]
        public async Task Sets_Partial()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Partial.Should().BeTrue();
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_NonCollection_Property()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            Model.Methods.Where(x => x.Name == "WithProperty1").Should().ContainSingle();
            var method = Model.Methods.Single(x => x.Name == "WithProperty1");
            method.ReturnTypeName.Should().Be("T");
            method.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("instance.Property1 = property1;", "return instance;");

            Model.Methods.Where(x => x.Name == "WithProperty2").Should().BeEmpty(); //only for the non-collection property
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Collection_Property()
        {
            // Arrange
            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(CreateContext(), Model);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            var methods = Model.Methods.Where(x => x.Name == "AddProperty2");
            methods.Where(x => x.Name == "AddProperty2").Should().HaveCount(2);
            methods.Select(x => x.ReturnTypeName).Should().AllBeEquivalentTo("T");
            methods.SelectMany(x => x.Parameters.Select(y => y.TypeName)).Should().BeEquivalentTo("T", "System.Collections.Generic.IEnumerable<System.String>", "T", "System.String[]");
            methods.SelectMany(x => x.CodeStatements).Should().AllBeOfType<StringCodeStatementBuilder>();
            methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "return instance.AddProperty2<T>(property2.ToArray());",
                "foreach (var item in property2) instance.Property2.Add(item);",
                "return instance;"
            );
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
            result.ErrorMessage.Should().Be("To create a builder extensions class, there must be at least one property");
        }
    }
}
