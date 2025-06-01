namespace ClassFramework.Pipelines.Tests.Reflection.Components;

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
            Action a = () => sut.ProcessAsync(context: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Fields_When_Available()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyFieldTestClass);
            var settings = CreateSettingsForReflection(copyAttributes: true);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Fields.Count.ShouldBe(2);
            context.Request.Builder.Fields.Select(x => x.Visibility).ShouldAllBe(x => x == Visibility.Public);
            context.Request.Builder.Fields.Select(x => x.ReadOnly).ToArray().ShouldBeEquivalentTo(new[] { false, true });
            context.Request.Builder.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "myField", "myReadOnlyField" });
            context.Request.Builder.Fields.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String" });
            context.Request.Builder.Fields.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo(new[] { false, true });
            context.Request.Builder.Fields.Select(x => x.IsValueType).ToArray().ShouldBeEquivalentTo(new[] { true, false });
            context.Request.Builder.Fields[0].Attributes.Count.ShouldBe(1);
            context.Request.Builder.Fields[context.Request.Builder.Fields.Count - 1].Attributes.ShouldBeEmpty();
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
