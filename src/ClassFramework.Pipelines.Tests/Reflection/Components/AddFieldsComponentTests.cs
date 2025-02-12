﻿namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddFieldsComponentTests : TestBase<Pipelines.Reflection.Components.AddFieldsComponent>
{
    public class ProcessAsync : AddFieldsComponentTests
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
        public async Task Adds_Fields_When_Available()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyFieldTestClass);
            var settings = CreateSettingsForReflection(copyAttributes: true);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Fields.Should().HaveCount(2);
            context.Request.Builder.Fields.Select(x => x.Visibility).Should().AllBeEquivalentTo(Visibility.Public);
            context.Request.Builder.Fields.Select(x => x.ReadOnly).Should().BeEquivalentTo([false, true]);
            context.Request.Builder.Fields.Select(x => x.Name).Should().BeEquivalentTo("myField", "myReadOnlyField");
            context.Request.Builder.Fields.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String");
            context.Request.Builder.Fields.Select(x => x.IsNullable).Should().BeEquivalentTo([false, true]);
            context.Request.Builder.Fields.Select(x => x.IsValueType).Should().BeEquivalentTo([true, false]);
            context.Request.Builder.Fields[0].Attributes.Should().ContainSingle();
            context.Request.Builder.Fields[context.Request.Builder.Fields.Count - 1].Attributes.Should().BeEmpty();
        }
    }
}

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class MyFieldTestClass
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
{
    [Required]
#pragma warning disable CS0649 // Field 'MyFieldTestClass.myField' is never assigned to, and will always have its default value 0 / null
    public int myField;
    public readonly string? myReadOnlyField;
#pragma warning restore CS0649 // Field 'MyFieldTestClass.myField' is never assigned to, and will always have its default value 0 / null
}
