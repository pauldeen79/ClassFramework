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
                Enumerable.Empty<string>(),
                Enumerable.Empty<Field>(),
                Enumerable.Empty<Property>(),
                Enumerable.Empty<Method>(),
                Enumerable.Empty<Metadata>(),
                default,
                name: null!,
                Enumerable.Empty<Attribute>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                default,
                default,
                default,
                Enumerable.Empty<Domain.Constructor>(),
                default,
                string.Empty,
                Enumerable.Empty<Enumeration>(),
                Enumerable.Empty<TypeBase>()
            ))
            .Should().Throw<ValidationException>();
        }
    }
}
