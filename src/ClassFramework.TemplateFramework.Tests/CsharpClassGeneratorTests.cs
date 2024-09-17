namespace ClassFramework.TemplateFramework.Tests;

public class CsharpClassGeneratorTests : TestBase
{
    protected static CsharpClassGenerator CreateSut() => new();

    public class Render_StringBuilder : CsharpClassGeneratorTests
    {
        [Fact]
        public async Task Returns_NotSupported_When_GenerateMultipleFiles_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: true),
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
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false),
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
        public async Task Returns_InnerResult_When_RenderNamespaceHierarchy_Result_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is TypeBase ? Result.Error("Kaboom!") : Result.Success());
            var context = Substitute.For<ITemplateContext>();
            context.Engine.Returns(engine);
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false),
                Model = [new ClassBuilder().WithName("MyClass").Build()]
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
        public async Task Returns_Success_When_All_Inner_Results_Are_Successful_Without_Global_Usings()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Success());
            var context = Substitute.For<ITemplateContext>();
            context.Engine.Returns(engine);
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false, enableGlobalUsings: false),
                Model = Array.Empty<TypeBase>()
            };
            sut.Context = context;
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task Returns_Success_When_All_Inner_Results_Are_Successful_With_Global_Usings()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Success());
            var context = Substitute.For<ITemplateContext>();
            context.Engine.Returns(engine);
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false, enableGlobalUsings: true),
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
