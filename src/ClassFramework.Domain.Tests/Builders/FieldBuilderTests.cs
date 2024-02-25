namespace ClassFramework.Domain.Tests.Builders;

public class FieldBuilderTests : TestBase<FieldBuilder>
{
    public class Constructor : FieldBuilderTests
    {
        // all other visibility containers have default value of public.
        // that's why we test the field builder, because this should have a default of private.
        [Fact]
        public void Sets_Visibilty_To_Private()
        {
            // Act
            var sut = CreateSut();

            // Assert
            sut.Visibility.Should().Be(Domains.Visibility.Private);
        }
    }
}
