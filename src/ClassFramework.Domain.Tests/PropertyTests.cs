namespace ClassFramework.Domain.Tests;

public class PropertyTests
{
    public class Constructor
    {
        [Fact]
        public void Throws_On_Custom_Validation_Error()
        {
            // Act & Assert
            this.Invoking(_ => new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).WithHasSetter().WithHasInitializer().Build())
                .Should().Throw<ValidationException>().WithMessage("HasSetter and HasInitializer cannot both be true");
        }
    }
}
