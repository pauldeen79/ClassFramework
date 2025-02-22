
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
            result.CodeStatements.ToArray().ShouldBeEquivalentTo(new CodeStatementBaseBuilder[] { new StringCodeStatementBuilder().WithStatement("throw new System.NotImplementedException();") });
        }
    }

    public class AddStringCodeStatements : CodeStatementsContainerBuilderExtensionsTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddStringCodeStatements(new[] { "// code goes here" }.AsEnumerable());

            // Assert
            result.CodeStatements.ToArray().ShouldBeEquivalentTo(new CodeStatementBaseBuilder[] { new StringCodeStatementBuilder().WithStatement("// code goes here") });
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
