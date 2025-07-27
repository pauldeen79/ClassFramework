namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class ConstructorViewModelTests : TestBase<ConstructorViewModel>
{
    public class Name : ConstructorViewModelTests
    {
        [Fact]
        public void Throws_When_ParentContext_Model_Is_Not_IType()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new EnumerationBuilder().WithName("MyEnumeration").Build());
            sut.Context = context;

            // Act
            Action a = () => _ = sut.Name;
            a.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void Throws_When_ParentContext_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(null);
            sut.Context = context;

            // Act & Assert
            Action a = () => _ = sut.Name;
            a.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void Returns_Name_From_ParentContext_Model_When_ParentContext_Model_Is_IType()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            sut.Context = context;

            // Act
            var result = sut.Name;

            // Assert
            result.ShouldBe("MyClass");
        }
    }

    public class ChainCall : ConstructorViewModelTests
    {
        [Fact]
        public void Returns_Empty_String_When_ChainCall_On_Model_Is_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ConstructorBuilder().WithChainCall(string.Empty).Build();

            // Act
            var result = sut.ChainCall;

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Formatted_String_When_ChainCall_On_Model_Is_Not_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ConstructorBuilder().WithChainCall("base()").Build();

            // Act
            var result = sut.ChainCall;

            // Assert
            result.ShouldBe(" : base()");
        }
    }

    public class OmitCode : ConstructorViewModelTests
    {
        [Fact]
        public void Returns_True_When_ParentContext_Model_Is_Interface()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new InterfaceBuilder().WithName("IMyInterface").Build());
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Returns_True_When_ParentContext_Model_Is_Abstract()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").WithAbstract(true).Build());
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Returns_False_When_ParentContext_Model_Is_Not_Interface_Or_Abstract()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ClassBuilder().WithName("MyClass").WithAbstract(false).Build());
            sut.Context = context;

            // Act
            var result = sut.OmitCode;

            // Assert
            result.ShouldBeTrue();
        }
    }
}
