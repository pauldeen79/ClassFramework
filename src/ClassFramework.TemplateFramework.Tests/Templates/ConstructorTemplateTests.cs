﻿namespace ClassFramework.TemplateFramework.Tests.Templates;

public class ConstructorTemplateTests : TemplateTestBase<ConstructorTemplate>
{
    public class Render : ConstructorTemplateTests
    {
        [Fact]
        public async Task Returns_InnerResult_When_Rendering_Attributes_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ConstructorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new ConstructorBuilder().AddAttributes(new AttributeBuilder().WithName("Test")).Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is Domain.Attribute ? Result.Error("Kaboom!") : Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
        }

        [Fact]
        public async Task Returns_InnerResult_When_Rendering_Parameters_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ConstructorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new ConstructorBuilder().AddAttributes(new AttributeBuilder().WithName("Test")).AddParameter("MyParameter", typeof(int)).Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is Parameter ? Result.Error("Kaboom!") : Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
        }

        [Fact]
        public async Task Creates_Method_Without_Code_Body_When_OmitCode_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ConstructorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new ConstructorBuilder()
                    .AddAttributes(new AttributeBuilder().WithName("Test"))
                    .AddParameter("MyParameter", typeof(int))
                    .AddCodeStatements("//code goes here")
                    .WithAbstract() // this sets OmitCode to true
                    .Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            builder.ToString().ShouldBe(@"        public abstract MyClass();
");
        }

        [Fact]
        public async Task Creates_Method_With_Code_Body_When_OmitCode_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ConstructorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new ConstructorBuilder()
                    .AddAttributes(new AttributeBuilder().WithName("Test"))
                    .AddParameter("MyParameter", typeof(int))
                    .AddCodeStatements("//code goes here")
                    .WithAbstract(false) // this sets OmitCode to false
                    .Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            var builder = new StringBuilder();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => Result.Success().Chain(() =>
            {
                // Simulate child template rendering for code statement :)
                var model = x.ArgAt<IRenderTemplateRequest>(0).Model;
                if (model is StringCodeStatement stringCodeStatement)
                {
                    builder.AppendLine($"            {stringCodeStatement.Statement}");
                }
            }));
            sut.Context = CreateContext(engine, sut);

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            builder.ToString().ShouldBe(@"        public MyClass()
        {
            //code goes here
        }
");
        }

        [Fact]
        public async Task Returns_Error_From_Code_Body_Rendering()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ConstructorViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new ConstructorBuilder()
                    .AddAttributes(new AttributeBuilder().WithName("Test"))
                    .AddParameter("MyParameter", typeof(int))
                    .AddCodeStatements("//code goes here")
                    .WithAbstract(false) // this sets OmitCode to false
                    .Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            var builder = new StringBuilder();
            engine.RenderAsync(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
            {
                // Simulate child template rendering for code statement :)
                var model = x.ArgAt<IRenderTemplateRequest>(0).Model;
                if (model is StringCodeStatement)
                {
                    return Result.Error("Kaboom!");
                }

                return Result.Success();
            });
            sut.Context = CreateContext(engine, sut);

            // Act
            var result = await sut.RenderAsync(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
        }
    }
}
