namespace ClassFramework.Domain.Tests.Types;

public class ClassTests
{
    public class Constructor
    {
        // note that a derrived class should throw an ValidationException.
        // the override classes should perform validation
        [Fact]
        public void Should_Throw_On_Null_Name()
        {
            // Act & Assert
            this.Invoking(_ => new Class
            (
                string.Empty,
                default,
                [],
                [],
                [],
                [],
                default,
                name: null!,
                [],
                [],
                [],
                [],
                default,
                default,
                default,
                [],
                default,
                string.Empty,
                [],
                []
            ))
            .Should().Throw<ValidationException>();
        }
    }
}
