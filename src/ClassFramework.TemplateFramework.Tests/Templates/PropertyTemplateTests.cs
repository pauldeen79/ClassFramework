namespace ClassFramework.TemplateFramework.Tests.Templates;

public class PropertyTemplateTests : TemplateTestBase<PropertyTemplate>
{
    [Fact]
    public async Task Returns_InnerResult_When_Rendering_Attributes_Is_Not_Successful()
    {
        // Arrange
        var sut = CreateSut();
        sut.Model = new PropertyViewModel(Fixture.Freeze<ICsharpExpressionDumper>())
        {
            Settings = CreateCsharpClassGeneratorSettings(),
            Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).AddAttributes(new AttributeBuilder().WithName("Test")).Build()
        };
        var engine = Substitute.For<ITemplateEngine>();
        engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is Domain.Attribute ? Result.Error("Kaboom!") : Result.Success());
        sut.Context = CreateContext(engine, sut);
        var builder = new StringBuilder();

        // Act
        var result = await sut.Render(builder, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.ErrorMessage.ShouldBe("Kaboom!");
    }

    [Fact]
    public async Task Returns_InnerResult_When_Rendering_CodeBodyItems_Is_Not_Successful()
    {
        // Arrange
        var sut = CreateSut();
        sut.Model = new PropertyViewModel(Fixture.Freeze<ICsharpExpressionDumper>())
        {
            Settings = CreateCsharpClassGeneratorSettings(),
            Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).AddAttributes(new AttributeBuilder().WithName("Test")).Build()
        };
        var engine = Substitute.For<ITemplateEngine>();
        engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is PropertyCodeBodyModel ? Result.Error("Kaboom!") : Result.Success());
        sut.Context = CreateContext(engine, sut);
        var builder = new StringBuilder();

        // Act
        var result = await sut.Render(builder, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.ErrorMessage.ShouldBe("Kaboom!");
    }

    [Fact]
    public async Task Appends_Default_Value_When_Provided()
    {
        // Arrange
        var dumper = Fixture.Freeze<ICsharpExpressionDumper>();
        dumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns(x => x.ArgAt<object>(0).ToString());
        var sut = CreateSut();
        sut.Model = new PropertyViewModel(dumper)
        {
            Settings = CreateCsharpClassGeneratorSettings(),
            Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).WithDefaultValue(13).Build()
        };
        var engine = Substitute.For<ITemplateEngine>();
        var builder = new StringBuilder();
        engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => Result.Success().Chain(() =>
        {
            // Simulate child template rendering for code statement :)
            var model = x.ArgAt<IRenderTemplateRequest>(0).Model;
            if (model is PropertyCodeBodyModel propertyCodeBodyModel)
            {
                //note that we skip rendering the code statements here, and assume we are dealing with a writable property
                builder.AppendLine($"            {propertyCodeBodyModel.Verb};");
            }
        }));
        sut.Context = CreateContext(engine, sut);

        // Act
        var result = await sut.Render(builder, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        builder.ToString().ShouldBe(@"        public int MyProperty
        {
            get;
            set;
        } = 13;
");
    }

    [Fact]
    public async Task Does_Not_Append_Default_Value_When_Not_Provided()
    {
        // Arrange
        var dumper = Fixture.Freeze<ICsharpExpressionDumper>();
        dumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns(x => x.ArgAt<object>(0).ToString());
        var sut = CreateSut();
        sut.Model = new PropertyViewModel(dumper)
        {
            Settings = CreateCsharpClassGeneratorSettings(),
            Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).WithDefaultValue(default(object?)).Build()
        };
        var engine = Substitute.For<ITemplateEngine>();
        var builder = new StringBuilder();
        engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => Result.Success().Chain(() =>
        {
            // Simulate child template rendering for code statement :)
            var model = x.ArgAt<IRenderTemplateRequest>(0).Model;
            if (model is PropertyCodeBodyModel propertyCodeBodyModel)
            {
                //note that we skip rendering the code statements here, and assume we are dealing with a writable property
                builder.AppendLine($"            {propertyCodeBodyModel.Verb};");
            }
        }));
        sut.Context = CreateContext(engine, sut);

        // Act
        var result = await sut.Render(builder, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        builder.ToString().ShouldBe(@"        public int MyProperty
        {
            get;
            set;
        }
");
    }
}
