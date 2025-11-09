namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetRecordComponentTests : TestBase<Pipelines.Entity.Components.SetRecordComponent>
{
    public class ExecuteAsync : SetRecordComponentTests
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

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async Task Sets_Record_Based_On_Setting(bool createRecordSettingValue, bool expectedRecordValue)
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createRecord: createRecordSettingValue);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Record.ShouldBe(expectedRecordValue);
        }
    }
}
