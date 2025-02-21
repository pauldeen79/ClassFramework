namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class CodeStatementViewModelBaseTests : TestBase<CodeStatementViewModelBase<CodeStatementBase>>
{
    public class AdditionalIndents : CodeStatementViewModelBaseTests
    {
        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Context = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.AdditionalIndents)
               .ShouldThrow<ArgumentNullException>();
               .ParamName.ShouldBe("Context");
        }

        [Fact]
        public void Returns_3_When_ParentContext_Model_Is_PropertyCodeBodyModel()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new PropertyCodeBodyModel("some verb", default, default, default, Array.Empty<CodeStatementBase>(), CultureInfo.InvariantCulture));
            sut.Context = context;

            // Act
            var result = sut.AdditionalIndents;

            // Assert
            result.ShouldBe(3);
        }

        [Fact]
        public void Returns_2_When_ParentContext_Model_Is_Method()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new MethodBuilder().WithName("MyMethod").Build());
            sut.Context = context;

            // Act
            var result = sut.AdditionalIndents;

            // Assert
            result.ShouldBe(2);
        }

        [Fact]
        public void Returns_2_When_ParentContext_Model_Is_Constructor()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new ConstructorBuilder().Build());
            sut.Context = context;

            // Act
            var result = sut.AdditionalIndents;

            // Assert
            result.ShouldBe(2);
        }

        [Fact]
        public void Throws_When_ParentContext_Model_Is_Not_PropertyCodeBodyModel_Or_Method_Or_Constructor()
        {
            // Arrange
            var sut = CreateSut();
            var context = Fixture.Freeze<ITemplateContext>();
            context.ParentContext.Returns(context); // note that we're using short-circuit here 8-) but who cares, we're just calling ParentContext.Model so it works.
            context.Model.Returns(new EnumerationBuilder().WithName("MyEnumeration").Build());
            sut.Context = context;

            // Act & Assert
            sut.Invoking(x => _ = x.AdditionalIndents)
               .ShouldThrow<NotSupportedException>();
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
            sut.Invoking(x => _ = x.AdditionalIndents)
               .ShouldThrow<NotSupportedException>();
        }
    }
}
