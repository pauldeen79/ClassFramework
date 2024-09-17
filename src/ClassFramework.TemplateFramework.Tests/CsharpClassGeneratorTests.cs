namespace ClassFramework.TemplateFramework.Tests;

public class CsharpClassGeneratorTests
{
    protected static CsharpClassGenerator CreateSut() => new();
    protected static CsharpClassGeneratorSettingsBuilder CreateSettings()
        => new CsharpClassGeneratorSettingsBuilder()
            .WithEncoding(Encoding.UTF8)
            .WithCultureInfo(CultureInfo.InvariantCulture);

    public class Render_StringBuilder : CsharpClassGeneratorTests
    {
        [Fact]
        public async Task Returns_NotSupported_When_GenerateMultipleFiles_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateSettings().WithGenerateMultipleFiles().Build(),
            };
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Can't generate multiple files, because the generation environment has a single StringBuilder instance");
        }

        [Fact]
        public async Task Returns_InnerResult_When_RenderHeader_Result_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Error("Kaboom!"));
            var context = Substitute.For<ITemplateContext>();
            context.Engine.Returns(engine);
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateSettings().Build(),
                Model = Array.Empty<TypeBase>()
            };
            sut.Context = context;
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom!");
        }

        [Fact]
        public async Task Returns_Success_When_All_Inner_Results_Are_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Success());
            var context = Substitute.For<ITemplateContext>();
            context.Engine.Returns(engine);
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateSettings().Build(),
                Model = Array.Empty<TypeBase>()
            };
            sut.Context = context;
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
        }
    }
}
