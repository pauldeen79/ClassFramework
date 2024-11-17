namespace ClassFramework.Domain.Tests;

public class AttributeTests
{
    public class Constructor
    {
        [Fact]
        public void Throws_On_Null_Parameters()
        {
            // Act & Assert
            this.Invoking(_ => new Attribute(parameters: null!, "Name"))
                .Should().Throw<ValidationException>();
        }

        [Fact]
        public void Throws_On_Null_Name()
        {
            // Act & Assert
            this.Invoking(_ => new Attribute([], name: null!))
                .Should().Throw<ValidationException>();
        }

        [Fact]
        public void Throws_On_Emtpty_Name()
        {
            // Act & Assert
            this.Invoking(_ => new Attribute([], name: string.Empty))
                .Should().Throw<ValidationException>();
        }

        [Fact]
        public void Can_Convert_Entity_To_Builder()
        {
            // Arrange
            var entity = new AttributeBuilder().WithName("MyClass").Build();

            // Act
            var builder = entity.ToBuilder();

            // Assert
            builder.Should().BeOfType<AttributeBuilder>();
        }

        [Fact]
        public void Can_Convert_Builder_To_Entity()
        {
            // Arrange
            var builder = new AttributeBuilder().WithName("MyClass");

            // Act
            var entity = builder.Build();

            // Assert
            entity.Should().BeOfType<Attribute>();
        }
    }
}
