namespace ClassFramework.TemplateFramework.Tests.Templates;

public class CodeGenerationHeaderTemplateTests : TemplateTestBase<CodeGenerationHeaderTemplate>
{
    public class Render : CodeGenerationHeaderTemplateTests
    {
        [Fact]
        public async Task Appends_Text_To_Builder_When_CreateCodeGenerationHeader_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new CodeGenerationHeaderViewModel { Model = new CodeGenerationHeaderModel(createCodeGenerationHeader: true, "1.0.0") };
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            builder.ToString().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Does_Not_Append_Text_To_Builder_When_CreateCodeGenerationHeader_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new CodeGenerationHeaderViewModel { Model = new CodeGenerationHeaderModel(createCodeGenerationHeader: false, "1.0.0") };
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            builder.ToString().ShouldBeEmpty();
        }
    }
}
