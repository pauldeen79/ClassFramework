namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddPropertiesComponentTests : TestBase<Pipelines.Reflection.Components.AddPropertiesComponent>
{
    public class ExecuteAsync : AddPropertiesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>())
                .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Copies_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection();
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Properties.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Fills_GenericTypeArguments_Correctly()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyNullableClass);
            var settings = CreateSettingsForReflection();
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Properties.Count.ShouldBe(1);
            //context.Builder.Properties.Single().TypeName.ShouldBe("System.Func<System.Object,System.Collections.Generic.IEnumerable<System.Object?>>");
            context.Builder.Properties.Single().IsNullable.ShouldBeFalse();
            context.Builder.Properties.Single().GenericTypeArguments.Count.ShouldBe(2);
            context.Builder.Properties.Single().GenericTypeArguments[0].TypeName.ShouldBe("System.Object");
            context.Builder.Properties.Single().GenericTypeArguments[0].IsNullable.ShouldBeFalse();
            context.Builder.Properties.Single().GenericTypeArguments[0].GenericTypeArguments.ShouldBeEmpty();
            //context.Builder.Properties.Single().GenericTypeArguments[1].TypeName.ShouldBe("System.Collections.Generic.IEnumerable<System.Object?>");
            context.Builder.Properties.Single().GenericTypeArguments[1].IsNullable.ShouldBeFalse();
            context.Builder.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Count.ShouldBe(1);
            context.Builder.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().TypeName.ShouldBe("System.Object");
            //context.Builder.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().IsNullable.ShouldBeTrue();
        }
    }
}
