namespace ClassFramework.CsharpExpressionCreator.Tests;

public class DefaultCsharpExpressionCreatorTests : TestBase<DefaultCsharpExpressionCreator>
{
    public class Constructor
    {
        [Fact]
        public void Throws_On_Null_Dumper()
        {
            // Act & Assert
            this.Invoking(_ => new DefaultCsharpExpressionCreator(dumper: null!))
                .Should().Throw<ArgumentNullException>().WithParameterName("dumper");
        }
    }

    public class Create : DefaultCsharpExpressionCreatorTests
    {
        [Fact]
        public void Returns_Result_From_CsharpExpressionDumper()
        {
            // Arrange
            var dumper = Fixture.Freeze<ICsharpExpressionDumper>();
            dumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("mock result");
            var sut = CreateSut();

            // Act 
            var result = sut.Create(this);

            // Assert
            result.Should().Be("mock result");
        }
    }
}
