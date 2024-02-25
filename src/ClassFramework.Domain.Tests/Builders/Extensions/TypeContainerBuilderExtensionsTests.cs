namespace ClassFramework.Domain.Tests.Builders.Extensions;

public class TypeContainerBuilderExtensionsTests : TestBase<PropertyBuilder>
{
    public class WithType_Type : TypeContainerBuilderExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.WithType(type: default!))
               .Should().Throw<ArgumentNullException>().WithParameterName("type");
        }

        [Fact]
        public void Adds_Correct_Information()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithType(typeof(int));

            // Assert
            result.TypeName.Should().Be("System.Int32");
        }
    }

    public class WithType_TypeBase : TypeContainerBuilderExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.WithType(typeBuilder: default!))
               .Should().Throw<ArgumentNullException>().WithParameterName("typeBuilder");
        }

        [Fact]
        public void Adds_Correct_Information_For_Class()
        {
            // Arrange
            var sut = CreateSut();
            ITypeBuilder typeBuilder = new ClassBuilder().WithName("MyClass").WithNamespace("MyNamespace");

            // Act
            var result = sut.WithType(typeBuilder);

            // Assert
            result.TypeName.Should().Be("MyNamespace.MyClass");
            result.IsValueType.Should().BeFalse();
        }

        [Fact]
        public void Adds_Correct_Information_For_Struct()
        {
            // Arrange
            var sut = CreateSut();
            ITypeBuilder typeBuilder = new StructBuilder().WithName("MyStruct").WithNamespace("MyNamespace");

            // Act
            var result = sut.WithType(typeBuilder);

            // Assert
            result.TypeName.Should().Be("MyNamespace.MyStruct");
            result.IsValueType.Should().BeTrue();
        }
    }
}
