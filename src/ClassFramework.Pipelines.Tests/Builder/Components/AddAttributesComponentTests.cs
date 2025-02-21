namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddAttributesComponentTests : TestBase<Pipelines.Builder.Components.AddAttributesComponent>
{
    public class ProcessAsync : AddAttributesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Task t = sut.ProcessAsync(context: null!);
            (await t.ShouldThrowAsync<ArgumentNullException>()).ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributes_Setting_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute") });
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
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute2") });
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
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Attributes.ShouldBeEmpty();
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}
