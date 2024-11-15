namespace ClassFramework.Pipelines.Tests.Builders;

public class TypenameMappingBuilderTests : TestBase<TypenameMappingBuilder>
{
    public class AddMetadata : TypenameMappingBuilderTests
    {
        [Fact]
        public void Throws_On_Null_Name()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.AddMetadata(name: null!, value: null))
               .Should().Throw<ArgumentNullException>().WithParameterName("name");
        }

        [Fact]
        public void Adds_Metadata_Correctly()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddMetadata(name: "Name", value: "Value");

            // Assert
            result.Metadata.Should().BeEquivalentTo([new Metadata(name: "Name", value: "Value")]);
        }
    }
}
