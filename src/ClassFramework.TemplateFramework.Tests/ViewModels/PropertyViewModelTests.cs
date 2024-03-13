namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class PropertyViewModelTests : TestBase<PropertyViewModel>
{
    public class TypeName : PropertyViewModelTests
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
            sut.Model = new PropertyBuilder()
                .WithName("MyField")
                .WithType(typeof(int))
                .Build();

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
            sut.Model = new PropertyBuilder()
                .WithName("MyField")
                .WithType(new ClassBuilder().WithName("MyType"))
                .WithIsNullable()
                .Build();

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
            sut.Model = new PropertyBuilder()
                .WithName("MyField")
                .WithType(new ClassBuilder().WithName("MyType").WithNamespace("MyNamespace"))
                .AddMetadata(MetadataNames.NamespaceToAbbreviate, "MyNamespace")
                .Build();

            // Act
            var result = sut.TypeName;

            // Assert
            result.Should().Be("MyType");
        }
    }

    public class ExplicitInterfaceName : PropertyViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ExplicitInterfaceName)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithExplicitInterfaceName("ISomething").Build();
            sut.Context = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ExplicitInterfaceName)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Context");
        }

        [Fact]
        public void Returns_Empty_String_When_ExplicitInterfaceName_Is_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithExplicitInterfaceName(string.Empty).Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new InterfaceBuilder().WithName("IMyInterface").Build());
            sut.Context = context;

            // Act
            var result = sut.ExplicitInterfaceName;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_ParentModel_Is_Interface()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithExplicitInterfaceName("ISomething").Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new InterfaceBuilder().WithName("IMyInterface").Build());
            sut.Context = context;

            // Act
            var result = sut.ExplicitInterfaceName;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_When_ExplicitInterfaceName_Is_Not_Empty_And_ParentModel_Is_Class()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithExplicitInterfaceName("ISomething").Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            sut.Context = context;

            // Act
            var result = sut.ExplicitInterfaceName;

            // Assert
            result.Should().Be("ISomething.");
        }
    }

    public class ShouldRenderDefaultValue : PropertyViewModelTests
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
            sut.Model = new PropertyBuilder()
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
            sut.Model = new PropertyBuilder()
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

    public class DefaultValueExpression : PropertyViewModelTests
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
            sut.Model = new PropertyBuilder()
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

    public class GetCodeBodyModels : PropertyViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            sut.Context = context;

            // Act & Assert
            sut.Invoking(x => _ = x.CodeBodyItems.ToArray())
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
            sut.Context = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.CodeBodyItems.ToArray())
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Context");
        }

        [Fact]
        public void Returns_Model_For_Getter_When_HasGetter_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithHasGetter(true).WithHasSetter(false).WithHasInitializer(false).Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            sut.Context = context;

            // Act
            var result = sut.CodeBodyItems.ToArray();

            // Assert
            result.Select(x => x.Verb).Should().BeEquivalentTo("get");
        }

        [Fact]
        public void Returns_Model_For_Setter_When_HasSetter_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithHasGetter(false).WithHasSetter(true).WithHasInitializer(false).Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            sut.Context = context;

            // Act
            var result = sut.CodeBodyItems.ToArray();

            // Assert
            result.Select(x => x.Verb).Should().BeEquivalentTo("set");
        }

        [Fact]
        public void Returns_Model_For_Initializer_When_HasInitializer_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithHasGetter(false).WithHasSetter(false).WithHasInitializer(true).Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            sut.Context = context;

            // Act
            var result = sut.CodeBodyItems.ToArray();

            // Assert
            result.Select(x => x.Verb).Should().BeEquivalentTo("init");
        }

        [Fact]
        public void Returns_Model_For_Getter_And_Setter_When_HasGetter_And_HasSetter_Are_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithHasGetter(true).WithHasSetter(true).WithHasInitializer(false).Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            sut.Context = context;

            // Act
            var result = sut.CodeBodyItems.ToArray();

            // Assert
            result.Select(x => x.Verb).Should().BeEquivalentTo("get", "set");
        }
    }
}
