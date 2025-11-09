namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddAttributesComponentTests : TestBase<Pipelines.Builder.Components.AddAttributesComponent>
{
    public class ExecuteAsync : AddAttributesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>())
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributes_Setting_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddAttributes(new AttributeBuilder().WithName("MyAttribute"))
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute") });
        }

        [Fact]
        public async Task Adds_Filtered_Attributes_When_CopyAttributes_Setting_Is_True_And_Predicate_Is_Filled()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddAttributes(
                    new AttributeBuilder().WithName("MyAttribute1"),
                    new AttributeBuilder().WithName("MyAttribute2"))
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: true, copyAttributePredicate: x => x.Name == "MyAttribute2");
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute2") });
        }

        [Fact]
        public async Task Does_Not_Add_Attributes_When_CopyAttributes_Setting_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddAttributes(new AttributeBuilder().WithName("MyAttribute"))
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: false);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ShouldBeEmpty();
        }

        private static GenerateBuilderCommand CreateCommand(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new GenerateBuilderCommand(sourceModel, settings, CultureInfo.InvariantCulture);
    }
}
