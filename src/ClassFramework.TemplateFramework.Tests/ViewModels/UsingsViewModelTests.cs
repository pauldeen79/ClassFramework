namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class UsingsViewModelTests : TestBase<UsingsViewModel>
{
    public class Usings : UsingsViewModelTests
    {
        public Usings() : base()
        {
            // For some reason, we have to register this class, because else we get the following exception:
            // AutoFixture was unable to create an instance of type AutoFixture.Kernel.SeededRequest because the traversed object graph contains a circular reference
            // I tried a generic fix in TestBase (omitting Model property), but this makes some tests fail and I don't understand why :-(
            //Fixture.Register(() => new UsingsViewModel());
        }

        [Fact]
        public void Returns_Default_Usigns_When_No_Custom_Usings_Are_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new UsingsModel([new ClassBuilder().WithName("MyClass").Build()]);
            sut.Settings = CreateCsharpClassGeneratorSettings();

            // Act
            var result = sut.Usings.ToArray();

            // Assert
            result.ShouldBeEquivalentTo(new[] { "System", "System.Collections.Generic", "System.Linq", "System.Text" });
        }

        [Fact]
        public void Returns_Distinct_Usigns_When_Custom_Usings_Are_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder()
                .AddCustomUsings("Z")
                .AddCustomUsings("A")
                .AddCustomUsings("Z") // note that we add this two times
                .Build();
            var cls = new ClassBuilder()
                .WithName("MyClass")
                .Build();
            sut.Model = new UsingsModel([cls]);

            // Act
            var result = sut.Usings.ToArray();

            // Assert
            result.ShouldBeEquivalentTo(new[] { "A", "System", "System.Collections.Generic", "System.Linq", "System.Text", "Z" });
        }
    }
}
