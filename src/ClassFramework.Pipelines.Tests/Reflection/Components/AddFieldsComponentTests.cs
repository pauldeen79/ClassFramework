namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class AddFieldsComponentTests : TestBase<Pipelines.Reflection.Features.AddFieldsComponent>
{
    public class Process : AddFieldsComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.Process(context: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public void Adds_Fields_When_Available()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyFieldTestClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(copyAttributes: true);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Fields.Should().HaveCount(2);
            model.Fields.Select(x => x.Visibility).Should().AllBeEquivalentTo(Visibility.Public);
            model.Fields.Select(x => x.ReadOnly).Should().BeEquivalentTo([false, true]);
            model.Fields.Select(x => x.Name).Should().BeEquivalentTo("myField", "myReadOnlyField");
            model.Fields.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String");
            model.Fields.Select(x => x.IsNullable).Should().BeEquivalentTo([false, true]);
            model.Fields.Select(x => x.IsValueType).Should().BeEquivalentTo([true, false]);
            model.Fields[0].Attributes.Should().ContainSingle();
            model.Fields[model.Fields.Count - 1].Attributes.Should().BeEmpty();
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
