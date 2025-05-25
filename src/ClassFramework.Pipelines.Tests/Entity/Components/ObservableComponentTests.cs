namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class ObservableComponentTests : TestBase<Pipelines.Entity.Components.ObservableComponent>
{
    public class ProcessAsync : ObservableComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.ProcessAsync(context: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Does_Not_Add_Interface_And_Event_When_CreateAsObservable_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createAsObservable: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ShouldBeEmpty();
            context.Request.Builder.Fields.ShouldBeEmpty();
        }

        [Fact]
        public async Task Adds_Interface_And_Event_When_CreateAsObservable_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createAsObservable: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.INotifyPropertyChanged" });
            context.Request.Builder.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "PropertyChanged" });
            context.Request.Builder.Fields.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.PropertyChangedEventHandler" });
            context.Request.Builder.Fields.Select(x => x.Event).ToArray().ShouldBeEquivalentTo(new[] { true });
            context.Request.Builder.Fields.Select(x => x.Visibility).ToArray().ShouldBeEquivalentTo(new[] { Visibility.Public });
            context.Request.Builder.Fields.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo(new[] { true });
        }
    }
}
