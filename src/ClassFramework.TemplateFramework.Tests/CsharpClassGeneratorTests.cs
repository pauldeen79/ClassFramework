namespace ClassFramework.TemplateFramework.Tests;

public class CsharpClassGeneratorTests : TemplateTestBase<CsharpClassGenerator>
{
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
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.NotSupported);
            result.ErrorMessage.ShouldBe("Can't generate multiple files, because the generation environment has a single StringBuilder instance");
        }

        [Fact]
        public async Task Returns_InnerResult_When_RenderHeader_Result_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Error("Kaboom!"));
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false),
                Model = Array.Empty<TypeBase>()
            };
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
        }

        [Fact]
        public async Task Returns_InnerResult_When_RenderNamespaceHierarchy_Result_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is TypeBase ? Result.Error("Kaboom!") : Result.Success());
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false),
                Model = [new ClassBuilder().WithName("MyClass").Build()]
            };
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
        }

        [Fact]
        public async Task Returns_Success_When_All_Inner_Results_Are_Successful_Without_Global_Usings()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Success());
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false, enableGlobalUsings: false),
                Model = Array.Empty<TypeBase>()
            };
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
        }

        [Fact]
        public async Task Returns_Success_When_All_Inner_Results_Are_Successful_With_Global_Usings()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Success());
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false, enableGlobalUsings: true),
                Model = Array.Empty<TypeBase>()
            };
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
        }
    }

    public class Render_MultipleContentBuilder : CsharpClassGeneratorTests
    {
        [Fact]
        public async Task Returns_InnerResult_When_RenderHeader_Result_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Error("Kaboom!"));
            sut.Model = new CsharpClassGeneratorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false),
                Model = Array.Empty<TypeBase>()
            };
            sut.Context = CreateContext(engine, sut);
            var builder = new MultipleContentBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
        }
    }
}
