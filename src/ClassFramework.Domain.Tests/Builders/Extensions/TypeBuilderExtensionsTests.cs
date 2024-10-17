namespace ClassFramework.Domain.Tests.Builders.Extensions;

public class TypeBuilderExtensionsTests : TestBase<ClassBuilder>
{
    public class GetFullName : TypeBuilderExtensionsTests
    {
        [Fact]
        public void Returns_Full_Name_When_Namespace_Is_Present()
        {
            // Arrange
            var sut = CreateSut().WithNamespace("MyNamespace").WithName("MyClass");

            // Act
            var result = sut.GetFullName();

            // Assert
            result.Should().Be("MyNamespace.MyClass");
        }

        [Fact]
        public void Returns_Name_When_Namespace_Is_Not_Present()
        {
            // Arrange
            var sut = CreateSut().WithNamespace(string.Empty).WithName("MyClass");

            // Act
            var result = sut.GetFullName();

            // Assert
            result.Should().Be("MyClass");
        }
    }

    public class WithFullName : TypeBuilderExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_FullName()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.WithFullName(fullName: null!))
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("fullName");
        }

        [Fact]
        public void Sets_Information_Correctly_On_FullName_With_Namespace()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithFullName("MyNamespace.MyType");

            // Assert
            result.Namespace.Should().Be("MyNamespace");
            result.Name.Should().Be("MyType");
        }

        [Fact]
        public void Sets_Information_Correctly_On_FullName_Without_Namespace()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithFullName("MyType");

            // Assert
            result.Namespace.Should().BeEmpty();
            result.Name.Should().Be("MyType");
        }
    }

    public class AddInterfaces : TypeBuilderExtensionsTests
    {
        [Fact]
        public void Can_Add_Interfaces_Using_Types_In_Array()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddInterfaces(typeof(INotifyPropertyChanged));

            // Assert
            result.Interfaces.Should().BeEquivalentTo("System.ComponentModel.INotifyPropertyChanged");
        }

        [Fact]
        public void Can_Add_Interfaces_Using_Types_In_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddInterfaces(new[] { typeof(INotifyPropertyChanged) }.AsEnumerable());

            // Assert
            result.Interfaces.Should().BeEquivalentTo("System.ComponentModel.INotifyPropertyChanged");
        }

        [Fact]
        public void Throws_On_Null_Interfaces_Using_Array()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddInterfaces(interfaces: (Type[])null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("interfaces");
        }

        [Fact]
        public void Throws_On_Null_Interfaces_Using_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddInterfaces(interfaces: (IEnumerable<Type>)null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("interfaces");
        }
    }
}
