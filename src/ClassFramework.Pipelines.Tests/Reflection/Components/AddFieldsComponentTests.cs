namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddFieldsComponentTests : TestBase<Pipelines.Reflection.Components.AddFieldsComponent>
{
    public class ExecuteAsync : AddFieldsComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>())
                .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Fields_When_Available()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyFieldTestClass);
            var settings = CreateSettingsForReflection(copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Fields.Count.ShouldBe(2);
            context.Builder.Fields.Select(x => x.Visibility).ShouldAllBe(x => x == Visibility.Public);
            context.Builder.Fields.Select(x => x.ReadOnly).ToArray().ShouldBeEquivalentTo(new[] { false, true });
            context.Builder.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "myField", "myReadOnlyField" });
            context.Builder.Fields.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String" });
            context.Builder.Fields.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo(new[] { false, true });
            context.Builder.Fields.Select(x => x.IsValueType).ToArray().ShouldBeEquivalentTo(new[] { true, false });
            context.Builder.Fields[0].Attributes.Count.ShouldBe(1);
            context.Builder.Fields[context.Builder.Fields.Count - 1].Attributes.ShouldBeEmpty();
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
