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
            var response = new ClassBuilder();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
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
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Properties.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Fills_GenericTypeArguments_Correctly()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyNullableClass);
            var settings = CreateSettingsForReflection();
            var response = new ClassBuilder();
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Properties.Count.ShouldBe(1);
            //response.Properties.Single().TypeName.ShouldBe("System.Func<System.Object,System.Collections.Generic.IEnumerable<System.Object?>>");
            response.Properties.Single().IsNullable.ShouldBeFalse();
            response.Properties.Single().GenericTypeArguments.Count.ShouldBe(2);
            response.Properties.Single().GenericTypeArguments[0].TypeName.ShouldBe("System.Object");
            response.Properties.Single().GenericTypeArguments[0].IsNullable.ShouldBeFalse();
            response.Properties.Single().GenericTypeArguments[0].GenericTypeArguments.ShouldBeEmpty();
            //response.Properties.Single().GenericTypeArguments[1].TypeName.ShouldBe("System.Collections.Generic.IEnumerable<System.Object?>");
            response.Properties.Single().GenericTypeArguments[1].IsNullable.ShouldBeFalse();
            response.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Count.ShouldBe(1);
            response.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().TypeName.ShouldBe("System.Object");
            //response.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().IsNullable.ShouldBeTrue();
        }
    }
}
