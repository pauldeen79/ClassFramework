namespace ClassFramework.TemplateFramework.Tests.Templates;

public class TypeTemplateTests : TemplateTestBase<TypeTemplate>
{
    public class Render : TypeTemplateTests
    {
        [Fact]
        public async Task Returns_InnerResult_When_CodeGenerationHeaders_Rendering_Result_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is CodeGenerationHeaderModel ? Result.Error("Kaboom!") : Result.Success());
            sut.Model = new TypeViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: true),
                Model = new ClassBuilder().WithName("MyClass").Build()
            };
            sut.Context = CreateContext(engine, sut);
            var builder = new MultipleContentBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom!");
        }

        [Fact]
        public async Task Returns_InnerResult_When_Usings_Rendering_Result_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is UsingsModel ? Result.Error("Kaboom!") : Result.Success());
            sut.Model = new TypeViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: true),
                Model = new ClassBuilder().WithName("MyClass").Build()
            };
            sut.Context = CreateContext(engine, sut);
            var builder = new MultipleContentBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom!");
        }
    }
}
