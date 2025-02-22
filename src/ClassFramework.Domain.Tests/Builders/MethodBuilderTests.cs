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
            Action a = () => sut.WithReturnType(type: default!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("type");
        }

        [Fact]
        public void Fills_Properties_Correctly_On_Non_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithReturnType(typeof(MethodBuilderTests));

            // Assert
            result.ReturnTypeName.ShouldBe("ClassFramework.Domain.Tests.Builders.MethodBuilderTests");
            result.ReturnTypeIsValueType.ShouldBeFalse();
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
            Action a = () => sut.WithReturnType(typeBuilder: default!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("typeBuilder");
        }

        [Fact]
        public void Fills_Properties_Correctly_On_Non_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithReturnType(new StructBuilder().WithName("MyClass").WithNamespace("MyNamespace"));

            // Assert
            result.ReturnTypeName.ShouldBe("MyNamespace.MyClass");
            result.ReturnTypeIsValueType.ShouldBeTrue();
        }
    }
}
