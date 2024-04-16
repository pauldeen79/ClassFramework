namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class MethodViewModelTests : TestBase<MethodViewModel>
{
    public class ShouldRenderModifiers : MethodViewModelTests
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
            sut.Invoking(x => _ = x.ShouldRenderModifiers)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").Build();
            sut.Context = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ShouldRenderModifiers)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Context");
        }

        [Theory]
        [InlineData("", "class", true)]
        [InlineData("", "interface", false)]
        [InlineData("IMyInterface", "class", false)]
        [InlineData("IMyInterface", "interface", false)]
        public void Returns_Correct_Result(string explicitInterfaceName, string parentModelType, bool expectedResult)
        {
            // Arrange
            object? parentModel = parentModelType switch
            {
                "class" => new ClassBuilder().WithName("MyClass").Build(),
                "interface" => new InterfaceBuilder().WithName("IMyInterface").Build(),
                _ => throw new NotSupportedException("Only 'class' and 'interface' are supported as parentModelType")
            };
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").WithExplicitInterfaceName(explicitInterfaceName).Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(parentModel);
            sut.Context = context;

            // Act
            var result = sut.ShouldRenderModifiers;

            // Assert
            result.Should().Be(expectedResult);
        }
    }

    public class ReturnTypeName : MethodViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ReturnTypeName)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Gets_Csharp_Friendly_TypeName()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder()
                .WithName("MyMethod")
                .WithReturnType(typeof(int))
                .Build();
            var template = new MethodTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.ReturnTypeName;

            // Assert
            result.Should().Be("int");
        }

        [Fact]
        public void Appends_Nullable_Notation()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder()
                .WithName("MyMethod")
                .WithReturnType(new ClassBuilder().WithName("MyType"))
                .WithReturnTypeIsNullable()
                .Build();
            var template = new MethodTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.ReturnTypeName;

            // Assert
            result.Should().Be("MyType?");
        }

        [Fact]
        public void Abbreviates_Namespaces()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder()
                .WithName("MyMethod")
                .WithReturnType(new ClassBuilder().WithName("MyType").WithNamespace("MyNamespace"))
                .Build();
            var template = new MethodTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.ReturnTypeName;

            // Assert
            result.Should().Be("MyType");
        }

        [Fact]
        public void Returns_void_When_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder()
                .WithName("MyMethod")
                .WithReturnTypeName(string.Empty)
                .Build();
            var template = new MethodTemplate();
            sut.Context = CreateTemplateContext(template, sut.Model);
            template.Context = sut.Context;
            template.Model = sut;
            sut.Settings = CreateCsharpClassGeneratorSettings().ToBuilder().AddNamespacesToAbbreviate("MyNamespace").Build();

            // Act
            var result = sut.ReturnTypeName;

            // Assert
            result.Should().Be("void");
        }
    }

    public class ExplicitInterfaceName : MethodViewModelTests
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
            sut.Invoking(x => _ = x.ExplicitInterfaceName)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").WithExplicitInterfaceName("IMyInterface").Build();
            sut.Context = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ExplicitInterfaceName)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Context");
        }

        [Theory]
        [InlineData("", "class", "")]
        [InlineData("", "interface", "")]
        [InlineData("IMyInterface", "class", "IMyInterface.")]
        [InlineData("IMyInterface", "interface", "")]
        public void Returns_Correct_Result(string explicitInterfaceName, string parentModelType, string expectedResult)
        {
            // Arrange
            object? parentModel = parentModelType switch
            {
                "class" => new ClassBuilder().WithName("MyClass").Build(),
                "interface" => new InterfaceBuilder().WithName("IMyInterface").Build(),
                _ => throw new NotSupportedException("Only 'class' and 'interface' are supported as parentModelType")
            };
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").WithExplicitInterfaceName(explicitInterfaceName).Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(parentModel);
            sut.Context = context;

            // Act
            var result = sut.ExplicitInterfaceName;

            // Assert
            result.Should().Be(expectedResult);
        }
    }

    public class Name : MethodViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.Name)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_Correct_Value_For_Operator()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("==").WithOperator().Build();

            // Act
            var result = sut.Name;

            // Assert
            result.Should().Be("operator ==");
        }

        [Fact]
        public void Returns_Correct_Value_For_InterfaceMethod()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("IMyInterface.MyMethod").Build();

            // Act
            var result = sut.Name;

            // Assert
            result.Should().Be("MyMethod");
        }

        [Fact]
        public void Returns_Correct_Value_For_Regular_Method()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").Build();

            // Act
            var result = sut.Name;

            // Assert
            result.Should().Be("MyMethod");
        }
    }

    public class OmitCode : MethodViewModelTests
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
            sut.Invoking(x => _ = x.OmitCode)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").Build();
            sut.Context = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.OmitCode)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Context");
        }

        [Fact]
        public void Returns_True_When_ParentModel_Is_Interface()
        {
            // Arrange
            var parentModel = new InterfaceBuilder().WithName("IMyInterface").Build();
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(parentModel);
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Returns_True_When_Method_Is_Abstract()
        {
            // Arrange
            var parentModel = new ClassBuilder().WithName("MyClass").Build();
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").WithAbstract().Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(parentModel);
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Returns_True_When_Method_Is_Partial()
        {
            // Arrange
            var parentModel = new ClassBuilder().WithName("MyClass").Build();
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").WithPartial().Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(parentModel);
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Returns_False_When_Method_Is_Not_Abstract_Or_Partial_And_ParentModel_Is_Class()
        {
            // Arrange
            var parentModel = new ClassBuilder().WithName("MyClass").Build();
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(parentModel);
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_False_When_Method_Is_Not_Abstract_Or_Partial_And_ParentModel_Is_Struct()
        {
            // Arrange
            var parentModel = new StructBuilder().WithName("MyStruct").Build();
            var sut = CreateSut();
            sut.Model = new MethodBuilder().WithName("MyMethod").Build();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(parentModel);
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.Should().BeFalse();
        }
    }
}
