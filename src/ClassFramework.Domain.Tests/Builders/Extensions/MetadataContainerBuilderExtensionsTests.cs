
namespace ClassFramework.Domain.Tests.Builders.Extensions;

public class MetadataContainerBuilderExtensionsTests : TestBase<MyMetadataContainerBuilder>
{
    public class AddMetadata : MetadataContainerBuilderExtensionsTests
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
            result.Metadata.Should().BeEquivalentTo(new[] { new Metadata(name: "Name", value: "Value") });
        }
    }
}

public class MyMetadataContainerBuilder : IMetadataContainerBuilder
{
    public ObservableCollection<MetadataBuilder> Metadata { get; set; } = new();
}
