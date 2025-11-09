namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddEquatableMembersComponentTests : TestBase<Pipelines.Entity.Components.AddEquatableMembersComponent>
{
    public class ExecuteAsync : AddEquatableMembersComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            var t = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Returns_Continue_When_ImplementIEquatable_Is_False()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: false);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "{Error}", implementIEquatable: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Success_When_Parsing_NameFormatString_Is_Succesful()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Equals", "Equals", "GetHashCode", "==", "!=" });
        }

        [Fact]
        public async Task Generates_Correct_GetHashCode_Method_For_Both_Nullable_And_Non_Nullable_Properties()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties(itemType: IEquatableItemType.Properties);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: true, iEquatableItemType: IEquatableItemType.Properties);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count(x => x.Name == nameof(GetHashCode)).ShouldBe(1);
            response.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(
                new[]
                {
                    "unchecked",
                    "{",
                    "    int hash = 17;",
                    "    hash = hash * 23 + Property1.GetHashCode();",
                    "    hash = hash * 23 + Property2 is not null ? Property2.GetHashCode() : 0;",
                    "    hash = hash * 23 + Property3.GetHashCode();",
                    "    hash = hash * 23 + Property4 is not null ? Property4.GetHashCode() : 0;",
                    "    hash = hash * 23 + Property5.GetHashCode();",
                    "    hash = hash * 23 + Property6 is not null ? Property6.GetHashCode() : 0;",
                    "    hash = hash * 23 + Property7.GetHashCode();",
                    "    hash = hash * 23 + Property8 is not null ? Property8.GetHashCode() : 0;",
                    "    return hash;",
                    "}"
                });
        }


        [Fact]
        public async Task Generates_Correct_GetHashCode_Method_For_Both_Nullable_And_Non_Nullable_Fields()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties(itemType: IEquatableItemType.Fields);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(implementIEquatable: true, iEquatableItemType: IEquatableItemType.Fields);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count(x => x.Name == nameof(GetHashCode)).ShouldBe(1);
            response.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.Single(x => x.Name == nameof(GetHashCode)).CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(
                new[]
                {
                    "unchecked",
                    "{",
                    "    int hash = 17;",
                    "    hash = hash * 23 + _field1.GetHashCode();",
                    "    hash = hash * 23 + _field2 is not null ? _field2.GetHashCode() : 0;",
                    "    hash = hash * 23 + _field3.GetHashCode();",
                    "    hash = hash * 23 + _field4 is not null ? _field4.GetHashCode() : 0;",
                    "    hash = hash * 23 + _field5.GetHashCode();",
                    "    hash = hash * 23 + _field6 is not null ? _field6.GetHashCode() : 0;",
                    "    hash = hash * 23 + _field7.GetHashCode();",
                    "    hash = hash * 23 + _field8 is not null ? _field8.GetHashCode() : 0;",
                    "    return hash;",
                    "}"
                });
        }
    }
}
