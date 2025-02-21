namespace ClassFramework.Domain.Tests.Builders.Extensions;

public class BaseClassContainerBuilderExtensionsTests : TestBase<ClassBuilder>
{
    public class WithBaseClass : BaseClassContainerBuilderExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_BaseClassType()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => _ = sut.WithBaseClass(baseClassType: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("baseClassType");
        }

        [Fact]
        public void Returns_Correct_Result()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithBaseClass(typeof(string));

            // Assert
            result.BaseClass.ShouldBe(typeof(string).FullName);
        }

        [Fact]
        public void Can_Convert_Entity_To_Builder()
        {
            // Arrange
            var entity = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var builder = entity.ToBuilder();

            // Assert
            builder.ShouldBeOfType<ClassBuilder>();
        }

        [Fact]
        public void Can_Convert_Builder_To_Entity()
        {
            // Arrange
            var builder = new ClassBuilder().WithName("MyClass");

            // Act
            var entity = builder.Build();

            // Assert
            entity.ShouldBeOfType<Class>();
        }
    }
}
