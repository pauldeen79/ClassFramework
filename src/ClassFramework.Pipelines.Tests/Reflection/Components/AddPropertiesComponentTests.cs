namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class AddPropertiesComponentTests : TestBase<Pipelines.Reflection.Features.AddPropertiesComponent>
{
    public class Process : AddPropertiesComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.Process(context: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public void Copies_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Properties.Should().ContainSingle();
        }

        [Fact]
        public void Fills_GenericTypeArguments_Correctly()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyNullableClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Properties.Should().ContainSingle();
            //public Func<object, IEnumerable<object?>> MyDelegateProperty { get; set; } = default!;
            //model.Properties.Single().TypeName.Should().Be("System.Func<System.Object,System.Collections.Generic.IEnumerable<System.Object?>>");
            model.Properties.Single().IsNullable.Should().BeFalse();
            model.Properties.Single().GenericTypeArguments.Should().HaveCount(2);
            model.Properties.Single().GenericTypeArguments[0].TypeName.Should().Be("System.Object");
            model.Properties.Single().GenericTypeArguments[0].IsNullable.Should().BeFalse();
            model.Properties.Single().GenericTypeArguments[0].GenericTypeArguments.Should().BeEmpty();
            //model.Properties.Single().GenericTypeArguments[1].TypeName.Should().Be("System.Collections.Generic.IEnumerable<System.Object?>");
            model.Properties.Single().GenericTypeArguments[1].IsNullable.Should().BeFalse();
            model.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Should().ContainSingle();
            model.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().TypeName.Should().Be("System.Object");
            //model.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().IsNullable.Should().BeTrue();
        }
    }
}
