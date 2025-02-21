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
            result.ShouldBe("MyNamespace.MyClass");
        }

        [Fact]
        public void Returns_Name_When_Namespace_Is_Not_Present()
        {
            // Arrange
            var sut = CreateSut().WithNamespace(string.Empty).WithName("MyClass");

            // Act
            var result = sut.GetFullName();

            // Assert
            result.ShouldBe("MyClass");
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
            Action a = () => sut.WithFullName(fullName: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("fullName");
        }

        [Fact]
        public void Sets_Information_Correctly_On_FullName_With_Namespace()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithFullName("MyNamespace.MyType");

            // Assert
            result.Namespace.ShouldBe("MyNamespace");
            result.Name.ShouldBe("MyType");
        }

        [Fact]
        public void Sets_Information_Correctly_On_FullName_Without_Namespace()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithFullName("MyType");

            // Assert
            result.Namespace.ShouldBeEmpty();
            result.Name.ShouldBe("MyType");
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
            result.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.INotifyPropertyChanged" });
        }

        [Fact]
        public void Can_Add_Interfaces_Using_Types_In_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddInterfaces(new[] { typeof(INotifyPropertyChanged) }.AsEnumerable());

            // Assert
            result.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.INotifyPropertyChanged" });
        }

        [Fact]
        public void Throws_On_Null_Interfaces_Using_Array()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddInterfaces(interfaces: (Type[])null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("interfaces");
        }

        [Fact]
        public void Throws_On_Null_Interfaces_Using_Enumerable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddInterfaces(interfaces: (IEnumerable<Type>)null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("interfaces");
        }
    }
}
