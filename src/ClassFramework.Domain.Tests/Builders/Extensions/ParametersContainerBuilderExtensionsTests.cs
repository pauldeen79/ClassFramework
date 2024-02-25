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
            sut.Invoking(x => x.AddParameter(name: null!, typeof(int)))
               .Should().Throw<ArgumentNullException>().WithParameterName("name");
        }

        [Fact]
        public void Throws_On_Null_Name_Using_Type_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddParameter(name: null!, typeof(int), true))
               .Should().Throw<ArgumentNullException>().WithParameterName("name");
        }

        [Fact]
        public void Throws_On_Null_Name_Using_String_No_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddParameter(name: null!, "System.Int32"))
               .Should().Throw<ArgumentNullException>().WithParameterName("name");
        }

        [Fact]
        public void Throws_On_Null_Name_Using_String_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddParameter(name: null!, "System.Int32", true))
               .Should().Throw<ArgumentNullException>().WithParameterName("name");
        }

        [Fact]
        public void Adds_Correctly_Using_Type_No_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", typeof(int));

            // Assert
            result.Parameters.Should().BeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32").WithIsValueType().Build() });
        }

        [Fact]
        public void Adds_Correctly_Using_Type_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", typeof(int), true);

            // Assert
            result.Parameters.Should().BeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32").WithIsNullable().WithIsValueType().Build() });
        }

        [Fact]
        public void Adds_Correctly_Using_String_No_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", "System.Int32");

            // Assert
            result.Parameters.Should().BeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32").Build() });
        }

        [Fact]
        public void Adds_Correctly_Using_String_Nullable()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddParameter("Name", "System.Int32", true);

            // Assert
            result.Parameters.Should().BeEquivalentTo(new[] { new ParameterBuilder().WithName("Name").WithTypeName("System.Int32").WithIsNullable().Build() });
        }
    }
}
