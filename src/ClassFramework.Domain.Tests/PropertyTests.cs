namespace ClassFramework.Domain.Tests;

public class PropertyTests
{
    public class Constructor
    {
        [Fact]
        public void Throws_On_Custom_Validation_Error()
        {
            // Act & Assert
            Action a = () => _ = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).WithHasSetter().WithHasInitializer().Build();
            a.ShouldThrow<ValidationException>().Message.ShouldBe("HasSetter and HasInitializer cannot both be true");
        }
    }
}
