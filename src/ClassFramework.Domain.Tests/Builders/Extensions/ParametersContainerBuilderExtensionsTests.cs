namespace ClassFramework.Domain.Tests.Builders.Extensions;

public class ParametersContainerBuilderExtensionsTests : TestBase<MethodBuilder>
{
    public class AddParameter : ParametersContainerBuilderExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_Name_Using_Type_No_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddParameter(name: null!, typeof(int));
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("name");
        }

        [Fact]
        public void Throws_On_Null_Name_Using_Type_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddParameter(name: null!, typeof(int), true);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("name");
        }

        [Fact]
        public void Throws_On_Null_Name_Using_String_No_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddParameter(name: null!, "System.Int32");
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("name");
        }

        [Fact]
        public void Throws_On_Null_Name_Using_String_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.AddParameter(name: null!, "System.Int32", true);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("name");
        }

        [Fact]
        public void Adds_Correctly_Using_Type_No_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", typeof(int));

            // Assert
            result.Parameters.ToArray().ShouldBeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32").WithIsValueType() });
        }

        [Fact]
        public void Adds_Correctly_Using_Type_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", typeof(int), true);

            // Assert
            result.Parameters.ToArray().ShouldBeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32").WithIsNullable().WithIsValueType() });
        }

        [Fact]
        public void Adds_Correctly_Using_String_No_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", "System.Int32");

            // Assert
            result.Parameters.ToArray().ShouldBeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32") });
        }

        [Fact]
        public void Adds_Correctly_Using_String_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", "System.Int32", true);

            // Assert
            result.Parameters.ToArray().ShouldBeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32").WithIsNullable() });
        }
    }
}
