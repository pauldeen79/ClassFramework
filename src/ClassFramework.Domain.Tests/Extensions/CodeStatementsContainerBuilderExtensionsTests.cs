namespace ClassFramework.Domain.Tests.Extensions;

public class CodeStatementsContainerBuilderExtensionsTests : TestBase<TestCodeStatementsContainerBuilder>
{
    public class NotImplemented : CodeStatementsContainerBuilderExtensionsTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.NotImplemented();

            // Assert
            result.CodeStatements.ToArray().ShouldBeEquivalentTo(new CodeStatementBaseBuilder[] { new StringCodeStatementBuilder("throw new System.NotImplementedException();") });
        }
    }

    public class AddCodeStatements : CodeStatementsContainerBuilderExtensionsTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddCodeStatements(new[] { "// code goes here" }.AsEnumerable());

            // Assert
            result.CodeStatements.ToArray().ShouldBeEquivalentTo(new CodeStatementBaseBuilder[] { new StringCodeStatementBuilder("// code goes here") });
        }
    }
}

public sealed class TestCodeStatementsContainerBuilder : ICodeStatementsContainerBuilder
{
    public ObservableCollection<CodeStatementBaseBuilder> CodeStatements { get; set; } = new();

    public ICodeStatementsContainer Build()
    {
        throw new NotImplementedException();
    }
}
