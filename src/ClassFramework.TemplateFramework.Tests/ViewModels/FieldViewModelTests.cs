namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class FieldViewModelTests : TestBase<FieldViewModel>
{
    public class TypeName : FieldViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            Action a = () => _ = sut.TypeName;
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("Model");
        }

        [Fact]
        public void Gets_Csharp_Friendly_TypeName()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new FieldBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .Build();
            var template = new FieldTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.TypeName;

            // Assert
            result.ShouldBe("int");
        }

        [Fact]
        public void Appends_Nullable_Notation()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new FieldBuilder()
                .WithName("MyField")
                .WithType(new ClassBuilder().WithName("MyType"))
                .WithIsNullable()
                .Build();
            var template = new FieldTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.TypeName;

            // Assert
            result.ShouldBe("MyType?");
        }

        [Fact]
        public void Abbreviates_Namespaces()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new FieldBuilder()
                .WithName("MyField")
                .WithType(new ClassBuilder().WithName("MyType").WithNamespace("MyNamespace"))
                .Build();
            var template = new FieldTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.TypeName;

            // Assert
            result.ShouldBe("MyType");
        }
    }

    public class ShouldRenderDefaultValue : FieldViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            Action a = () => _ = sut.ShouldRenderDefaultValue;
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("Model");
        }

        [Fact]
        public void Returns_True_When_Model_DefaultValue_Is_Filled()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new FieldBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithDefaultValue("some default value")
                .Build();

            // Act
            var result = sut.ShouldRenderDefaultValue;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Returns_False_When_Model_DefaultValue_Is_Not_Filled()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new FieldBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithDefaultValue(null)
                .Build();

            // Act
            var result = sut.ShouldRenderDefaultValue;

            // Assert
            result.ShouldBeFalse();
        }
    }

    public class DefaultValueExpression : FieldViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            Action a = () => _ = sut.DefaultValueExpression;
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("Model");
        }

        [Fact]
        public void Returns_True_When_Model_DefaultValue_Is_Filled()
        {
            // Arrange
            Fixture.Freeze<ICsharpExpressionDumper>().Dump(Arg.Any<object?>()).Returns("formatted value");
            var sut = CreateSut();
            sut.Model = new FieldBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .WithDefaultValue("some default value")
                .Build();

            // Act
            var result = sut.DefaultValueExpression;

            // Assert
            result.ShouldBe("formatted value");
        }
    }
}
