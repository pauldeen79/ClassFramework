﻿namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class ObservableComponentTests : TestBase<Pipelines.Entity.Components.ObservableComponent>
{
    public class Process : ObservableComponentTests
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

        [Fact]
        public async Task Does_Not_Add_Interface_And_Event_When_CreateAsObservable_Is_False()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createAsObservable: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().BeEmpty();
            context.Request.Builder.Fields.Should().BeEmpty();
        }

        [Fact]
        public async Task Adds_Interface_And_Event_When_CreateAsObservable_Is_True()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(createAsObservable: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().BeEquivalentTo("System.ComponentModel.INotifyPropertyChanged");
            context.Request.Builder.Fields.Select(x => x.Name).Should().BeEquivalentTo("PropertyChanged");
            context.Request.Builder.Fields.Select(x => x.TypeName).Should().BeEquivalentTo("System.ComponentModel.PropertyChangedEventHandler");
            context.Request.Builder.Fields.Select(x => x.Event).Should().BeEquivalentTo(new[] { true });
            context.Request.Builder.Fields.Select(x => x.Visibility).Should().BeEquivalentTo(new[] { Visibility.Public });
            context.Request.Builder.Fields.Select(x => x.IsNullable).Should().BeEquivalentTo(new[] { true });
        }
    }
}
