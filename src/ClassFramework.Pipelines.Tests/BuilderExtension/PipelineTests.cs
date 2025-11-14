namespace ClassFramework.Pipelines.Tests.BuilderExtension;

public class PipelineTests : IntegrationTestBase<ICommandService>
{
    public class ExecuteAsync : PipelineTests
    {
        private static GenerateBuilderExtensionCommand CreateCommand(bool addProperties = true, bool useBuilderLazyValues = false)
            => new(
                CreateGenericClass(addProperties),
                CreateSettingsForBuilder
                (
                    builderNamespaceFormatString: "{class.Namespace}.Builders",
                    allowGenerationWithoutProperties: false,
                    copyAttributes: true,
                    useBuilderLazyValues: useBuilderLazyValues
                ),
                CultureInfo.InvariantCulture
            );

        [Fact]
        public async Task Sets_Partial()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateBuilderExtensionCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Partial.ShouldBeTrue();
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_NonCollection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateBuilderExtensionCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Methods.Count(x => x.Name == "WithProperty1").ShouldBe(1);
            var method = result.Value.Methods.Single(x => x.Name == "WithProperty1");
            method.ReturnTypeName.ShouldBe("T");
            method.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "instance.Property1 = property1;", "return instance;" });

            result.Value.Methods.Where(x => x.Name == "WithProperty2").ShouldBeEmpty(); //only for the non-collection property
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Lazy_NonCollection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(useBuilderLazyValues: true);

            // Act
            var result = await sut.ExecuteAsync<GenerateBuilderExtensionCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Methods.Count(x => x.Name == "WithProperty1").ShouldBe(2);
            var lazyMethod = result.Value.Methods.First(x => x.Name == "WithProperty1");
            lazyMethod.ReturnTypeName.ShouldBe("T");
            lazyMethod.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "T", "System.Func<System.String>" });
            lazyMethod.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            lazyMethod.CodeStatements
                .OfType<StringCodeStatementBuilder>()
                .Select(x => x.Statement)
                .ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    "instance.Property1 = property1;",
                    "return instance;"
                });
            var nonLazyMethod = result.Value.Methods.Last(x => x.Name == "WithProperty1");
            nonLazyMethod.ReturnTypeName.ShouldBe("T");
            nonLazyMethod.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "T", "System.String" });
            nonLazyMethod.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            nonLazyMethod.CodeStatements
                .OfType<StringCodeStatementBuilder>()
                .Select(x => x.Statement)
                .ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    "instance.Property1 = new System.Func<System.String>(() => property1);",
                    "return instance;"
                });

            result.Value.Methods.Where(x => x.Name == "WithProperty2").ShouldBeEmpty(); //only for the non-collection property
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Collection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateBuilderExtensionCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            var methods = result.Value.Methods.Where(x => x.Name == "AddProperty2").ToArray();
            methods.Length.ShouldBe(2);
            methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "T");
            methods.SelectMany(x => x.Parameters.Select(y => y.TypeName)).ToArray().ShouldBeEquivalentTo(new[] { "T", "System.Collections.Generic.IEnumerable<System.String>", "T", "System.String[]" });
            methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return instance.AddProperty2<T>(property2.ToArray());",
                    "foreach (var item in property2) instance.Property2.Add(item);",
                    "return instance;"
                }
            );
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Lazy_Collection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(useBuilderLazyValues: true);

            // Act
            var result = await sut.ExecuteAsync<GenerateBuilderExtensionCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            var methods = result.Value.Methods.Where(x => x.Name == "AddProperty2").ToArray();
            methods.Length.ShouldBe(4);
            methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "T");
            methods.SelectMany(x => x.Parameters.Select(y => y.TypeName)).ToArray().ShouldBeEquivalentTo(new[] { "T", "System.Collections.Generic.IEnumerable<System.Func<System.String>>", "T", "System.Func<System.String>[]", "T", "System.Collections.Generic.IEnumerable<System.String>", "T", "System.String[]" });
            methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            methods.SelectMany(x => x.CodeStatements)
                .OfType<StringCodeStatementBuilder>()
                .Select(x => x.Statement)
                .ToArray()
                .ShouldBeEquivalentTo
                (
                    new[]
                    {
                        "return instance.AddProperty2<T>(property2.ToArray());",
                        "foreach (var item in property2) instance.Property2.Add(item);",
                        "return instance;",
                        "foreach (var item in property2) instance.Property2.Add(() => item);",
                        "return instance;"
                    }
                );
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(addProperties: false);

            // Act
            var result = await sut.ExecuteAsync<GenerateBuilderExtensionCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("There must be at least one property");
        }
    }
}
