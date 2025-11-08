namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class ObservableComponentTests : TestBase<Pipelines.Entity.Components.ObservableComponent>
{
    public class ExecuteAsync : ObservableComponentTests
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
        public async Task Does_Not_Add_Interface_And_Event_When_CreateAsObservable_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createAsObservable: false);
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(context, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.ShouldBeEmpty();
            response.Fields.ShouldBeEmpty();
        }

        [Fact]
        public async Task Adds_Interface_And_Event_When_CreateAsObservable_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createAsObservable: true);
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(context, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.INotifyPropertyChanged" });
            response.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "PropertyChanged" });
            response.Fields.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.PropertyChangedEventHandler" });
            response.Fields.Select(x => x.Event).ToArray().ShouldBeEquivalentTo(new[] { true });
            response.Fields.Select(x => x.Visibility).ToArray().ShouldBeEquivalentTo(new[] { Visibility.Public });
            response.Fields.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo(new[] { true });
        }
    }
}
