namespace ClassFramework.Domain.Tests.Builders.Extensions;

public class CodeStatementsContainerBuilderExtensionsTests : TestBase<ConstructorBuilder>
{
    public class AddStringCodeStatements : CodeStatementsContainerBuilderExtensionsTests
    {
        [Fact]
        public void Can_Add_String_CodeStatements_Using_Array()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddStringCodeStatements("StatementOne();", "StatementTwo();");

            // Assert
            result.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            result.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("StatementOne();", "StatementTwo();");
        }

        [Fact]
        public void Can_Add_String_CodeStatements_Using_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddStringCodeStatements(new[] { "StatementOne();", "StatementTwo();" }.AsEnumerable());

            // Assert
            result.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            result.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("StatementOne();", "StatementTwo();");
        }

        [Fact]
        public void Throws_On_Null_Statements_Using_Array()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddStringCodeStatements(statements: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("statements");
        }

        [Fact]
        public void Throws_On_Null_Statements_Using_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddStringCodeStatements(statements: (IEnumerable<string>)null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("statements");
        }
    }
}
