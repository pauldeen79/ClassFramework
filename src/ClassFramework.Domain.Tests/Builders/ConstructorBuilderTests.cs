namespace ClassFramework.Domain.Tests.Builders;

public class ConstructorBuilderTests : TestBase<ConstructorBuilder>
{
    public class ChainCallToBaseUsingParameters : ConstructorBuilderTests
    {
        [Fact]
        public void Creates_ChainCall_Correctly()
        {
            // Arrange
            var sut = CreateSut()
                .AddParameter("param1", typeof(int))
                .AddParameter("param2", typeof(string))
                .AddParameter("param3", typeof(bool));

            // Act
            var actual = sut.ChainCallToBaseUsingParameters();

            // Assert
            actual.ChainCall.ShouldBe("base(param1, param2, param3)");
        }
    }
}
