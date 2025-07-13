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
            var result = sut.AddCodeStatements("StatementOne();", "StatementTwo();");

            // Assert
            result.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            result.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "StatementOne();", "StatementTwo();" });
        }

        [Fact]
        public void Can_Add_String_CodeStatements_Using_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddCodeStatements(new[] { "StatementOne();", "StatementTwo();" }.AsEnumerable());

            // Assert
            result.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            result.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "StatementOne();", "StatementTwo();" });
        }

        [Fact]
        public void Throws_On_Null_Statements_Using_Array()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddCodeStatements(statements: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("statements");
        }

        [Fact]
        public void Throws_On_Null_Statements_Using_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddCodeStatements(statements: (IEnumerable<string>)null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("statements");
        }
    }
}
