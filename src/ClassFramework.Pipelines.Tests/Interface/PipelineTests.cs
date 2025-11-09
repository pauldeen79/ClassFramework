namespace ClassFramework.Pipelines.Tests.Interface;

public class PipelineTests : IntegrationTestBase<ICommandService>
{
    public class ExecuteAsync : PipelineTests
    {
        private static GenerateInterfaceCommand CreateCommand(bool addProperties = true, bool copyMethods = true, CopyMethodPredicate? copyMethodPredicate = null) => new(
            CreateInterface(addProperties),
            CreateSettingsForInterface
            (
                allowGenerationWithoutProperties: false,
                copyMethods: copyMethods,
                copyMethodPredicate: copyMethodPredicate
            ).Build(),
            CultureInfo.InvariantCulture
        );

        [Fact]
        public async Task Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Name.ShouldBe("IMyClass");
            result.Value.Namespace.ShouldBe("MyNamespace");
        }

        [Fact]
        public async Task Adds_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == false);
            result.Value.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
            result.Value.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.String", "System.Collections.Generic.IReadOnlyCollection<System.String>" });
        }

        [Fact]
        public async Task Adds_Methods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Methods.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(copyMethodPredicate: (_, _) => true);

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Methods.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True_Negative()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(copyMethodPredicate: (_, _) => false);

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Methods_When_CopyMethods_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(copyMethods: false);

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(addProperties: false);

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("There must be at least one property");
        }
    }

    public class IntegrationTests : PipelineTests
    {
        [Fact]
        public async Task Creates_Interface_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateInterfaceWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var settings = CreateSettingsForInterface(
                namespaceMappings: namespaceMappings);
            var command = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<GenerateInterfaceCommand, InterfaceBuilder>(command, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Name.ShouldBe("IMyClass");
            result.Value.Namespace.ShouldBe("MyNamespace");
            result.Value.Interfaces.ShouldBeEmpty();

            result.Value.Fields.ShouldBeEmpty();

            result.Value.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1",
                    "Property2",
                    "Property3",
                    "Property4",
                    "Property5",
                    "Property6",
                    "Property7",
                    "Property8"
                }
            );
            result.Value.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.Int32",
                    "System.Nullable<System.Int32>",
                    "System.String",
                    "System.String",
                    "MyNamespace.IMyClass",
                    "MyNamespace.IMyClass",
                    "System.Collections.Generic.IReadOnlyCollection<MyNamespace.IMyClass>",
                    "System.Collections.Generic.IReadOnlyCollection<MyNamespace.IMyClass>"
                }
            );
            result.Value.Properties.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    false,
                    true,
                    false,
                    true,
                    false,
                    true,
                    false,
                    true
                }
            );
            result.Value.Properties.Select(x => x.HasGetter).ShouldAllBe(x => x == true);
            result.Value.Properties.SelectMany(x => x.GetterCodeStatements).ShouldBeEmpty();
            result.Value.Properties.Select(x => x.HasInitializer).ShouldAllBe(x => x == false);
            result.Value.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == false);
            result.Value.Properties.SelectMany(x => x.SetterCodeStatements).ShouldBeEmpty();
        }

        private static GenerateInterfaceCommand CreateContext(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings, CultureInfo.InvariantCulture);
    }
}
