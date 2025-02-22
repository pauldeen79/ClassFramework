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
            Action a = () => sut.AddMetadata(name: null!, value: null);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("name");
        }

        [Fact]
        public void Adds_Metadata_Correctly()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddMetadata(name: "Name", value: "Value");

            // Assert
            result.Metadata.ToArray().ShouldBeEquivalentTo(new[] { new MetadataBuilder().WithName("Name").WithValue("Value") });
        }
    }
}
