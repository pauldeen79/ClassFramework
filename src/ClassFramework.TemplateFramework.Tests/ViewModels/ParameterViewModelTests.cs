namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class ParameterViewModelTests : TestBase<ParameterViewModel>
{
    public class TypeName : ParameterViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.TypeName)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Gets_Csharp_Friendly_TypeName()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .Build();
            var template = new ParameterTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.TypeName;

            // Assert
            result.Should().Be("int");
        }

        [Fact]
        public void Appends_Nullable_Notation()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(new ClassBuilder().WithName("MyType"))
                .WithIsNullable()
                .Build();
            var template = new ParameterTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.TypeName;

            // Assert
            result.Should().Be("MyType?");
        }

        [Fact]
        public void Abbreviates_Namespaces()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(new ClassBuilder().WithName("MyType").WithNamespace("MyNamespace"))
                .Build();
            var template = new ParameterTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.TypeName;

            // Assert
            result.Should().Be("MyType");
        }
    }

    public class ShouldRenderDefaultValue : ParameterViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ShouldRenderDefaultValue)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_True_When_Model_DefaultValue_Is_Filled()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithDefaultValue("some default value")
                .Build();

            // Act
            var result = sut.ShouldRenderDefaultValue;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Returns_False_When_Model_DefaultValue_Is_Not_Filled()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithDefaultValue(null)
                .Build();

            // Act
            var result = sut.ShouldRenderDefaultValue;

            // Assert
            result.Should().BeFalse();
        }
    }

    public class DefaultValueExpression : ParameterViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.DefaultValueExpression)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_True_When_Model_DefaultValue_Is_Filled()
        {
            // Arrange
            Fixture.Freeze<ICsharpExpressionDumper>().Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("formatted value");
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithDefaultValue("some default value")
                .Build();

            // Act
            var result = sut.DefaultValueExpression;

            // Assert
            result.Should().Be("formatted value");
        }
    }

    public class Prefix : ParameterViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.Prefix)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_Params_On_ParamArray_Parameter()
        {
            // Arrange
            Fixture.Freeze<ICsharpExpressionDumper>().Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("formatted value");
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithIsParamArray()
                .Build();

            // Act
            var result = sut.Prefix;

            // Assert
            result.Should().Be("params ");
        }

        [Fact]
        public void Returns_Ref_On_Ref_Parameter()
        {
            // Arrange
            Fixture.Freeze<ICsharpExpressionDumper>().Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("formatted value");
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithIsRef()
                .Build();

            // Act
            var result = sut.Prefix;

            // Assert
            result.Should().Be("ref ");
        }

        [Fact]
        public void Returns_Out_On_Out_Parameter()
        {
            // Arrange
            Fixture.Freeze<ICsharpExpressionDumper>().Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("formatted value");
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithIsOut()
                .Build();

            // Act
            var result = sut.Prefix;

            // Assert
            result.Should().Be("out ");
        }

        [Fact]
        public void Returns_Empty_String_On_Regular_Parameter()
        {
            // Arrange
            Fixture.Freeze<ICsharpExpressionDumper>().Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns("formatted value");
            var sut = CreateSut();
            sut.Model = new ParameterBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .Build();

            // Act
            var result = sut.Prefix;

            // Assert
            result.Should().BeEmpty();
        }
    }
}
