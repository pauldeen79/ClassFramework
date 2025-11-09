namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddFluentMethodsForNonCollectionPropertiesComponentTests : TestBase<Pipelines.Builder.Components.AddFluentMethodsForNonCollectionPropertiesComponent>
{
    public class ExecuteAsync : AddFluentMethodsForNonCollectionPropertiesComponentTests
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
        public async Task Does_Not_Add_Method_When_SetMethodNameFormatString_Is_Empty()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setMethodNameFormatString: string.Empty);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Adds_Method_When_SetMethodNameFormatString_Is_Not_Empty()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setMethodNameFormatString: "With{property.Name}");
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1 = property1;",
                    "return this;",
                    "Property2 = property2;",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Method_When_SetMethodNameFormatString_Is_Not_Empty_CsharpFriendlyName()
        {
            // Arrange
            var sourceModel = CreateClassWithPropertyThatHasAReservedName(typeof(int));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setMethodNameFormatString: "With{property.Name}");
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(1);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithDelegate" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "delegate" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Delegate = @delegate;",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Method_With_Default_ArgumentNullChecks_When_SetMethodNameFormatString_Is_Not_Empty()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                setMethodNameFormatString: "With{property.Name}",
                addNullChecks: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1 = property1;",
                    "return this;",
                    "if (property2 is null) throw new System.ArgumentNullException(nameof(property2));",
                    "Property2 = property2;",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Method_Without_ArgumentNullChecks_When_Property_Has_TypeName_T()
        {
            // Arrange
            var sourceModel = CreateClass()
                .ToTypedBuilder()
                .With(x => x.Properties.First(y => y.Name == "Property2").TypeName = "T")
                .AddGenericTypeArguments("T")
                .BuildTyped();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                setMethodNameFormatString: "With{property.Name}",
                addNullChecks: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder<T>");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "T" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1 = property1;",
                    "return this;",
                    "Property2 = property2;",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Method_Without_ArgumentNullChecks_When_Property_Has_TypeName_Func_T()
        {
            // Arrange
            var sourceModel = CreateClass()
                .ToTypedBuilder()
                .With(x => x.Properties.First(y => y.Name == "Property2").TypeName = "System.Func<T>")
                .AddGenericTypeArguments("T")
                .BuildTyped();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                setMethodNameFormatString: "With{property.Name}",
                addNullChecks: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder<T>");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.Func<T>" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1 = property1;",
                    "return this;",
                    "if (property2 is null) throw new System.ArgumentNullException(nameof(property2));",
                    "Property2 = property2;",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Method_With_Custom_ArgumentNullChecks_When_SetMethodNameFormatString_Is_Not_Empty()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                setMethodNameFormatString: "With{property.Name}",
                addNullChecks: true,
                typenameMappings:
                [
                    new TypenameMappingBuilder()
                        .WithSourceType(typeof(int))
                        .WithTargetType(typeof(int))
                        .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentNullCheckExpression).WithValue("/* custom argument null check goes here */")),
                    new TypenameMappingBuilder()
                        .WithSourceType(typeof(string))
                        .WithTargetType(typeof(string))
                        .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentNullCheckExpression).WithValue("/* custom argument null check goes here */"))
                ]);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "/* custom argument null check goes here */",
                    "Property1 = property1;",
                    "return this;",
                    "/* custom argument null check goes here */",
                    "Property2 = property2;",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Method_For_BuilderForAbstractEntity()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableEntityInheritance: true,
                setMethodNameFormatString: "With{property.Name}");
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "TBuilder");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1 = property1;",
                    "return (TBuilder)this;",
                    "Property2 = property2;",
                    "return (TBuilder)this;"
                }
            );
        }

        [Fact]
        public async Task Uses_CustomBuilderArgumentType_When_Present()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setMethodNameFormatString: "With{property.Name}", typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue("Custom{property.Name}")),
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(string))
                    .WithTargetType(typeof(string))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue("Custom{property.Name}"))
            ]);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "CustomProperty1", "CustomProperty2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1 = property1;",
                    "return this;",
                    "Property2 = property2;",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Uses_CustomBuilderWithDefaultPropertyValue_When_Present()
        {
            // Arrange
            // Note that this doesn't seem logical for this unit test, but in code generation the Literal is needed for correct formatting of literal values.
            // If you would use a string without wrapping it in a Literal, then it will get formatted to "customDefaultValue" which may not be what you want.
            // Or, in case you just want a default boolean value, you might also use true and false directly, without wrapping it in a Literal...
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setMethodNameFormatString: "With{property.Name}", typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomBuilderWithDefaultPropertyValue, new Literal("customDefaultValue"))),
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(string))
                    .WithTargetType(typeof(string))
                    .AddMetadata(new MetadataBuilder(MetadataNames.CustomBuilderWithDefaultPropertyValue, new Literal("customDefaultValue")))

            ]);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2" });
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x is Literal);
            response.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).OfType<Literal>().Select(x => x.Value).ShouldAllBe(x => x == "customDefaultValue");
        }

        [Fact]
        public async Task Uses_CustomBuilderWithExpression_When_Present()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setMethodNameFormatString: "With{property.Name}", typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderWithExpression).WithValue("{property.Name} = {property.Name.ToCamelCase()}; // custom")),
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(string))
                    .WithTargetType(typeof(string))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderWithExpression).WithValue("{property.Name} = {property.Name.ToCamelCase()}; // custom"))
            ]);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
            response.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            response.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1 = property1; // custom",
                    "return this;",
                    "Property2 = property2; // custom",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Uses_Correct_BuilderNameFormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(builderNameFormatString: "My{class.Name}Builder");
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(2);
            response.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "WithProperty1", "WithProperty2" });
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue("{Error}"))
            ]);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static GenerateBuilderCommand CreateCommand(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new GenerateBuilderCommand(sourceModel, settings, CultureInfo.InvariantCulture);
    }
}
