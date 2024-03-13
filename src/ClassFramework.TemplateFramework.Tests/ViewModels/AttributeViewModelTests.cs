namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class AttributeViewModelTests : TestBase<AttributeViewModel>
{
    public class Parameters : AttributeViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null;

            // Act
            sut.Invoking(x => _ = x.Parameters)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_String_With_Named_Parameter_Name()
        {
            // Arrange
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            csharpExpressionDumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("true");
            var sut = CreateSut();
            sut.Model = new AttributeBuilder()
                .WithName(typeof(RequiredAttribute))
                .AddParameters(new AttributeParameterBuilder().WithName(nameof(RequiredAttribute.AllowEmptyStrings)).WithValue(true))
                .Build();

            // Act
            var result = sut.Parameters;

            // Assert
            result.Should().Be("(AllowEmptyStrings = true)");
        }

        [Fact]
        public void Returns_String_With_Unnamed_Parameter_Name()
        {
            // Arrange
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            csharpExpressionDumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("true");
            var sut = CreateSut();
            sut.Model = new AttributeBuilder()
                .WithName(typeof(RequiredAttribute))
                .AddParameters(new AttributeParameterBuilder().WithValue(true))
                .Build();

            // Act
            var result = sut.Parameters;

            // Assert
            result.Should().Be("(true)");
        }

        [Fact]
        public void Returns_String_With_Mixed_Parameter_Names()
        {
            // Arrange
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            csharpExpressionDumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns(x => x.Args()[0].ToString()!.ToLowerInvariant());
            var sut = CreateSut();
            sut.Model = new AttributeBuilder()
                .WithName(typeof(RequiredAttribute))
                .AddParameters(
                    new AttributeParameterBuilder().WithName(nameof(RequiredAttribute.AllowEmptyStrings)).WithValue(true),
                    new AttributeParameterBuilder().WithValue(false) // note that the RequiredAttribute does not have an unnamed argument, but I'm just testing the ViewModel here 8-)
                )
                .Build();

            // Act
            var result = sut.Parameters;

            // Assert
            result.Should().Be("(AllowEmptyStrings = true, false)");
        }
    }

    public class AdditionalIndents : AttributeViewModelTests
    {
        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Context = null!;

            // Act
            sut.Invoking(x => _ = x.AdditionalIndents)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Context");
        }

        [Fact]
        public void Returns_Zero_When_ParentModel_Is_Parameter()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ParameterBuilder().WithTypeName(GetType().FullName!).WithName("myParameter").Build());
            sut.Context = context;

            // Act
            var result = sut.AdditionalIndents;

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void Returns_Zero_When_ParentModel_Is_IType()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            sut.Context = context;

            // Act
            var result = sut.AdditionalIndents;

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void Returns_One_When_ParentModel_Is_Not_Parameter_Or_IType()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new PropertyBuilder().WithTypeName(GetType().FullName!).WithName("MyProperty").Build());
            sut.Context = context;

            // Act
            var result = sut.AdditionalIndents;

            // Assert
            result.Should().Be(1);
        }
    }
}
