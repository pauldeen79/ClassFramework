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
            Fixture.Register(() => new UsingsViewModel(Fixture.Freeze<ICsharpExpressionCreator>()));
        }

        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.Usings.ToArray())
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_Default_Usigns_When_No_Custom_Usings_Are_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new UsingsModel(new TypeBase[] { new ClassBuilder().WithName("MyClass").Build() });

            // Act
            var result = sut.Usings.ToArray();

            // Assert
            result.Should().BeEquivalentTo("System", "System.Collections.Generic", "System.Linq", "System.Text");
        }

        [Fact]
        public void Returns_Distinct_Usigns_When_Custom_Usings_Are_Present()
        {
            // Arrange
            var sut = CreateSut();
            var cls = new ClassBuilder()
                .WithName("MyClass")
                .AddMetadata(MetadataNames.CustomUsing, "Z")
                .AddMetadata(MetadataNames.CustomUsing, "A")
                .AddMetadata(MetadataNames.CustomUsing, "Z") // note that we add this two times
                .Build();
            sut.Model = new UsingsModel(new TypeBase[] { cls });

            // Act
            var result = sut.Usings.ToArray();

            // Assert
            result.Should().BeEquivalentTo([ "A", "System", "System.Collections.Generic", "System.Linq", "System.Text", "Z" ], cfg => cfg.WithStrictOrdering());
        }
    }
}
