namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddPropertiesComponentTests : TestBase<Pipelines.Reflection.Components.AddPropertiesComponent>
{
    public class ProcessAsync : AddPropertiesComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.ProcessAsync(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Copies_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Should().ContainSingle();
        }

        [Fact]
        public async Task Fills_GenericTypeArguments_Correctly()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyNullableClass);
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Should().ContainSingle();
            //public Func<object, IEnumerable<object?>> MyDelegateProperty { get; set; } = default!;
            //model.Properties.Single().TypeName.Should().Be("System.Func<System.Object,System.Collections.Generic.IEnumerable<System.Object?>>");
            context.Request.Builder.Properties.Single().IsNullable.Should().BeFalse();
            context.Request.Builder.Properties.Single().GenericTypeArguments.Should().HaveCount(2);
            context.Request.Builder.Properties.Single().GenericTypeArguments[0].TypeName.Should().Be("System.Object");
            context.Request.Builder.Properties.Single().GenericTypeArguments[0].IsNullable.Should().BeFalse();
            context.Request.Builder.Properties.Single().GenericTypeArguments[0].GenericTypeArguments.Should().BeEmpty();
            //model.Properties.Single().GenericTypeArguments[1].TypeName.Should().Be("System.Collections.Generic.IEnumerable<System.Object?>");
            context.Request.Builder.Properties.Single().GenericTypeArguments[1].IsNullable.Should().BeFalse();
            context.Request.Builder.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Should().ContainSingle();
            context.Request.Builder.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().TypeName.Should().Be("System.Object");
            //model.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().IsNullable.Should().BeTrue();
        }
    }
}
