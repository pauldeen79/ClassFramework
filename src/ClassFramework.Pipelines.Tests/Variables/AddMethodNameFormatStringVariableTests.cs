namespace ClassFramework.Pipelines.Tests.Variables;

public class AddMethodNameFormatStringVariableTests : TestBase<AddMethodNameFormatStringVariable>
{
    public class ProcessAsync : AddMethodNameFormatStringVariableTests
    {
        [Fact]
        public void Returns_Continue_On_Wrong_Name()
        {
            // Arrange
            var context = CreateSettingsForEntity().Build();
            var sut = CreateSut();

            // Act
            var result = sut.Evaluate("WrongFunctionName", context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Continue);
        }

        [Fact]
        public void Returns_Ok_With_Correct_Value_On_Correct_Name()
        {
            // Arrange
            var context = CreateSettingsForEntity().Build();
            var resolver = Fixture.Freeze<IObjectResolver>();
            resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context));
            var sut = CreateSut();

            // Act
            var result = sut.Evaluate("addMethodNameFormatString", context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe(context.AddMethodNameFormatString);
        }

        [Fact]
        public void Returns_Failure_WHen_Settings_Could_Not_Be_Resolved_From_Context()
        {
            // Arrange
            var context = CreateSettingsForEntity().Build();
            var resolver = Fixture.Freeze<IObjectResolver>();
            resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Error<PipelineSettings>("Kaboom"));
            var sut = CreateSut();

            // Act
            var result = sut.Evaluate("addMethodNameFormatString", context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }
    }
}
