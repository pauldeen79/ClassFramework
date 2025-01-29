﻿namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddMethodsComponentTests : TestBase<Pipelines.Reflection.Components.AddMethodsComponent>
{
    public class ProcessAsync : AddMethodsComponentTests
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
        public async Task Copies_Methods()
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
            context.Request.Builder.Methods.Should().ContainSingle();
        }
    }
}
