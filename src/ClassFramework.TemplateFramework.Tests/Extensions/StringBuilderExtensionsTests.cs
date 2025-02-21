namespace ClassFramework.TemplateFramework.Tests.Extensions;

public class StringBuilderExtensionsTests
{
    public class RenderSuppressions
    {
        [Fact]
        public void Throws_On_Null_SuppressWarningCodes()
        {
            // Arrange
            var sut = new StringBuilder();

            // Act & Assert
            sut.Invoking(x => x.RenderSuppressions(suppressWarningCodes: null!, "disable", "    "))
               .ShouldThrow<ArgumentNullException>();
               .ParamName.ShouldBe("suppressWarningCodes");
        }

        [Fact]
        public void Returns_Empty_Result_When_SuppressWarningCodes_Is_Empty()
        {
            // Arrange
            var sut = new StringBuilder();
            var suppressWarningCodes = Array.Empty<string>().AsReadOnly();

            // Act
            sut.RenderSuppressions(suppressWarningCodes, "disable", "    ");

            // Assert
            sut.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_When_SuppressWarningCodes_Is_Not_Empty()
        {
            // Arrange
            var sut = new StringBuilder();
            var suppressWarningCodes = new string[] { "S123", "S124" }.AsReadOnly();

            // Act
            sut.RenderSuppressions(suppressWarningCodes, "disable", "    ");

            // Assert
            sut.ToString().ShouldBe(@"    #pragma warning disable S123;
    #pragma warning disable S124
");
        }
    }
}
