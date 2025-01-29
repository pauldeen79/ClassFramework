namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddEquatableMembersComponentTests : TestBase<Pipelines.Entity.Components.AddEquatableMembersComponent>
{
    public class Process : AddEquatableMembersComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.ProcessAsync(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Returns_Continue_When_ImplementIEquatable_Is_False()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "{Error}", implementIEquatable: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public async Task Returns_Success_When_Parsing_NameFormatString_Is_Succesful()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Select(x => x.Name).Should().BeEquivalentTo("Equals", "Equals", "GetHashCode", "==", "!=");
        }

        [Fact]
        public async Task Generates_Correct_GetHashCode_Method_For_Both_Nullable_And_Non_Nullable_Properties()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties(itemType: IEquatableItemType.Properties);
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: true, iEquatableItemType: IEquatableItemType.Properties);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Where(x => x.Name == nameof(GetHashCode)).Should().ContainSingle();
            context.Request.Builder.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            context.Request.Builder.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo(
                "unchecked", "{",
                "    int hash = 17;",
                "    hash = hash * 23 + Property1.GetHashCode();",
                "    hash = hash * 23 + Property2 is not null ? Property2.GetHashCode() : 0;",
                "    hash = hash * 23 + Property3.GetHashCode();",
                "    hash = hash * 23 + Property4 is not null ? Property4.GetHashCode() : 0;",
                "    hash = hash * 23 + Property5.GetHashCode();",
                "    hash = hash * 23 + Property6 is not null ? Property6.GetHashCode() : 0;",
                "    hash = hash * 23 + Property7.GetHashCode();",
                "    hash = hash * 23 + Property8 is not null ? Property8.GetHashCode() : 0;",
                "    return hash;", "}");
        }


        [Fact]
        public async Task Generates_Correct_GetHashCode_Method_For_Both_Nullable_And_Non_Nullable_Fields()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties(itemType: IEquatableItemType.Fields);
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: true, iEquatableItemType: IEquatableItemType.Fields);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Where(x => x.Name == nameof(GetHashCode)).Should().ContainSingle();
            context.Request.Builder.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            context.Request.Builder.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo(
                "unchecked", "{",
                "    int hash = 17;",
                "    hash = hash * 23 + _field1.GetHashCode();",
                "    hash = hash * 23 + _field2 is not null ? _field2.GetHashCode() : 0;",
                "    hash = hash * 23 + _field3.GetHashCode();",
                "    hash = hash * 23 + _field4 is not null ? _field4.GetHashCode() : 0;",
                "    hash = hash * 23 + _field5.GetHashCode();",
                "    hash = hash * 23 + _field6 is not null ? _field6.GetHashCode() : 0;",
                "    hash = hash * 23 + _field7.GetHashCode();",
                "    hash = hash * 23 + _field8 is not null ? _field8.GetHashCode() : 0;",
                "    return hash;", "}");
        }
    }
}
