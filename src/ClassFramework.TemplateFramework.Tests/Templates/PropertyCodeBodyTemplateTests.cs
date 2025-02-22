namespace ClassFramework.TemplateFramework.Tests.Templates;

public class PropertyCodeBodyTemplateTests : TemplateTestBase<PropertyCodeBodyTemplate>
{
    public class Render : PropertyCodeBodyTemplateTests
    {
        [Fact]
        public async Task Returns_Result_When_Rendering_CodeStatements_Is_Not_Successful()
        {
            // Arrange
            var sut = CreateSut();
            var parentModel = new TypeViewModel { Model = new ClassBuilder().WithName("MyClass").Build() };
            sut.Model = new PropertyCodeBodyViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new PropertyCodeBodyModel("get", Visibility.Public, SubVisibility.InheritFromParent, parentModel, new[] { new StringCodeStatementBuilder().WithStatement("//code goes here").Build() }.AsReadOnly(), CultureInfo.InvariantCulture)
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is CodeStatementBase ? Result.Error("Kaboom!") : Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
        }

        [Fact]
        public async Task Renders_Abstract_Method_Correctly()
        {
            // Arrange
            var sut = CreateSut();
            var parentModel = new TypeViewModel { Model = new InterfaceBuilder().WithName("IMyInterfact").Build() };
            sut.Model = new PropertyCodeBodyViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new PropertyCodeBodyModel("get", Visibility.Public, SubVisibility.InheritFromParent, parentModel, new[] { new StringCodeStatementBuilder().WithStatement("//code goes here").Build() }.AsReadOnly(), CultureInfo.InvariantCulture)
            };
            var engine = Substitute.For<ITemplateEngine>();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<IRenderTemplateRequest>(0).Model is CodeStatementBase ? Result.Error("Kaboom!") : Result.Success());
            sut.Context = CreateContext(engine, sut);
            var builder = new StringBuilder();

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            builder.ToString().ShouldBe(@"            get;
");
        }

        [Fact]
        public async Task Renders_Non_Abstract_Method_Correctly()
        {
            // Arrange
            var sut = CreateSut();
            var parentModel = new TypeViewModel { Model = new ClassBuilder().WithName("MyClass").Build() };
            sut.Model = new PropertyCodeBodyViewModel
            {
                Settings = CreateCsharpClassGeneratorSettings(),
                Model = new PropertyCodeBodyModel("get", Visibility.Public, SubVisibility.InheritFromParent, parentModel, new[] { new StringCodeStatementBuilder().WithStatement("//code goes here").Build() }.AsReadOnly(), CultureInfo.InvariantCulture)
            };
            var engine = Substitute.For<ITemplateEngine>();
            var builder = new StringBuilder();
            engine.Render(Arg.Any<IRenderTemplateRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
            {
                // Simulate child template rendering for code statement :)
                var model = x.ArgAt<IRenderTemplateRequest>(0).Model;
                if (model is StringCodeStatement stringCodeStatement)
                {
                    builder.AppendLine($"                {stringCodeStatement.Statement}");
                }

                return Result.Success();
            });
            sut.Context = CreateContext(engine, sut);

            // Act
            var result = await sut.Render(builder, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            builder.ToString().ShouldBe(@"            get
            {
                //code goes here
            }
");

        }
    }
}
