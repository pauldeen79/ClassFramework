namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetRecordComponentTests : TestBase<Pipelines.Entity.Components.SetRecordComponent>
{
    public class Process : SetRecordComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.Process(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async Task Sets_Record_Based_On_Setting(bool createRecordSettingValue, bool expectedRecordValue)
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createRecord: createRecordSettingValue);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Record.Should().Be(expectedRecordValue);
        }
    }
}
