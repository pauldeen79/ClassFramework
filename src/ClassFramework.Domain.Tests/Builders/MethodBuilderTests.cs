namespace ClassFramework.Domain.Tests.Builders;

public class MethodBuilderTests : TestBase<MethodBuilder>
{
    public class WithReturnType_Type : MethodBuilderTests
    {
        [Fact]
        public void Throws_On_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.WithReturnType(type: default!))
               .Should().Throw<ArgumentNullException>().WithParameterName("type");
        }

        [Fact]
        public void Fills_Properties_Correctly_On_Non_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithReturnType(typeof(MethodBuilderTests));

            // Assert
            result.ReturnTypeName.Should().Be("ClassFramework.Domain.Tests.Builders.MethodBuilderTests");
            result.ReturnTypeIsValueType.Should().BeFalse();
        }
    }

    public class WithReturnType_IType : MethodBuilderTests
    {
        [Fact]
        public void Throws_On_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.WithReturnType(typeBuilder: default!))
               .Should().Throw<ArgumentNullException>().WithParameterName("typeBuilder");
        }

        [Fact]
        public void Fills_Properties_Correctly_On_Non_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithReturnType(new StructBuilder().WithName("MyClass").WithNamespace("MyNamespace"));

            // Assert
            result.ReturnTypeName.Should().Be("MyNamespace.MyClass");
            result.ReturnTypeIsValueType.Should().BeTrue();
        }
    }
}
