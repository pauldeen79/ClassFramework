namespace ClassFramework.TemplateFramework.Tests.Templates;

public class MethodTemplateTests : TestBase
{
    protected static MethodTemplate CreateSut() => new();
    protected static ITemplateContext CreateContext(ITemplateEngine engine, object template)
    {
        var context = Substitute.For<ITemplateContext>();
        context.Engine.Returns(engine);
        var parentContext = Substitute.For<ITemplateContext>();
        parentContext.ParentContext.Returns(default(ITemplateContext));
        parentContext.RootContext.Returns(parentContext);
        parentContext.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
        parentContext.Template.Returns(template); // important to fill because otherwise you will get NullReferenceExceptions :)
        context.ParentContext.Returns(parentContext);
        context.RootContext.Returns(parentContext);
        return context;
    }

    public class Render : MethodTemplateTests
    {
        [Fact]
        public async Task Returns_InnerResult_When_Rendering_Attributes_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new MethodBuilder().WithName("MyMethod").WithReturnType(typeof(int)).AddAttributes(new AttributeBuilder().WithName("Test")).Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is Domain.Attribute ? Result.Error("Kaboom!") : Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom!");
        }

        [Fact]
        public async Task Returns_InnerResult_When_Rendering_Parameters_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new MethodBuilder().WithName("MyMethod").WithReturnType(typeof(int)).AddAttributes(new AttributeBuilder().WithName("Test")).AddParameter("MyParameter", typeof(int)).Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is Parameter ? Result.Error("Kaboom!") : Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom!");
        }

        [Fact]
        public async Task Creates_Method_Without_Code_Body_When_OmitCode_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new MethodBuilder()
                    .WithName("MyMethod")
                    .WithReturnType(typeof(int))
                    .AddAttributes(new AttributeBuilder().WithName("Test"))
                    .AddParameter("MyParameter", typeof(int))
                    .AddStringCodeStatements("//code goes here")
                    .WithAbstract() // this sets OmitCode to true
                    .Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            builder.ToString().Should().Be(@"        public abstract int MyMethod();
");
        }

        [Fact]
        public async Task Creates_Method_With_Code_Body_When_OmitCode_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new MethodBuilder().WithName("MyMethod").WithReturnType(typeof(int))
                    .AddAttributes(new AttributeBuilder().WithName("Test"))
                    .AddParameter("MyParameter", typeof(int))
                    .AddStringCodeStatements("//code goes here")
                    .WithAbstract(false) // this sets OmitCode to false
                    .Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            var builder = new StringBuilder();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => Result.Success().Chain(() =>
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
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            builder.ToString().Should().Be(@"        public int MyMethod()
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
            sut.Model = new MethodViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new MethodBuilder().WithName("MyMethod").WithReturnType(typeof(int))
                    .AddAttributes(new AttributeBuilder().WithName("Test"))
                    .AddParameter("MyParameter", typeof(int))
                    .AddStringCodeStatements("//code goes here")
                    .WithAbstract(false) // this sets OmitCode to false
                    .Build()
            };
            var engine = Substitute.For<ITemplateEngine>();
            var builder = new StringBuilder();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
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
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom!");
        }
    }
}
