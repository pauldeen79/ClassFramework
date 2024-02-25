namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class EnumerationMemberViewModelTests : TestBase<EnumerationMemberViewModel>
{
    public class ValueExpression : EnumerationMemberViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ValueExpression)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_Empty_String_When_Model_Value_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new EnumerationMemberBuilder().WithName("MyName").Build();

            // Act
            var result = sut.ValueExpression;

            // Assert
            result.Should().BeEmpty();
        }
    }
}
