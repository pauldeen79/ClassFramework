namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddFluentMethodsForCollectionPropertiesComponentTests : TestBase<Pipelines.Builder.Components.AddFluentMethodsForCollectionPropertiesComponent>
{
    public class ProcessAsync : AddFluentMethodsForCollectionPropertiesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            var t = sut.ProcessAsync(context: null!);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Does_Not_Add_Method_When_AddMethodNameFormatString_Is_Empty()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addMethodNameFormatString: string.Empty);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Adds_Methods_When_AddMethodNameFormatString_Is_Not_Empty()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addMethodNameFormatString: "Add{property.Name}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Methods.Count.ShouldBe(2);
            context.Request.Builder.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "AddProperty3", "AddProperty3" });
            context.Request.Builder.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property3", "property3" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Collections.Generic.IEnumerable<System.Int32>", "System.Int32[]" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return AddProperty3(property3.ToArray());",
                    "foreach (var item in property3) Property3.Add(item);",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Methods_With_CustomBuilderArgumentType_When_Present()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addMethodNameFormatString: "Add{property.Name}", typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue(typeof(IReadOnlyCollection<>).ReplaceGenericTypeName("MyCustomType")))
            ]);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Methods.Count.ShouldBe(2);
            context.Request.Builder.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "AddProperty3", "AddProperty3" });
            context.Request.Builder.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property3", "property3" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Collections.Generic.IEnumerable<MyCustomType>", "MyCustomType[]" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return AddProperty3(property3.ToArray());",
                    "foreach (var item in property3) Property3.Add(item);",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Methods_With_ArgumentNullChecks()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                addMethodNameFormatString: "Add{property.Name}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Methods.Count.ShouldBe(2);
            context.Request.Builder.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "AddProperty3", "AddProperty3" });
            context.Request.Builder.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property3", "property3" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Collections.Generic.IEnumerable<System.Int32>", "System.Int32[]" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "if (property3 is null) throw new System.ArgumentNullException(nameof(property3));",
                    "return AddProperty3(property3.ToArray());",
                    "if (property3 is null) throw new System.ArgumentNullException(nameof(property3));",
                    "foreach (var item in property3) Property3.Add(item);", "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Methods_With_ArgumentNullChecks_CsharpFriendlyName()
        {
            // Arrange
            var sourceModel = CreateClassWithPropertyThatHasAReservedName(typeof(List<int>));
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                addMethodNameFormatString: "Add{property.Name}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Methods.Count.ShouldBe(2);
            context.Request.Builder.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "AddDelegate", "AddDelegate" });
            context.Request.Builder.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "SomeNamespace.Builders.SomeClassBuilder");
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "delegate", "delegate" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Collections.Generic.IEnumerable<System.Int32>", "System.Int32[]" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));",
                    "return AddDelegate(@delegate.ToArray());",
                    "if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));",
                    "foreach (var item in @delegate) Delegate.Add(item);",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Method_For_BuilderForAbstractEntity()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableEntityInheritance: true,
                addMethodNameFormatString: "Add{property.Name}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Methods.Count.ShouldBe(2);
            context.Request.Builder.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "AddProperty3", "AddProperty3" });
            context.Request.Builder.Methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "TBuilder");
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property3", "property3" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Collections.Generic.IEnumerable<System.Int32>", "System.Int32[]" });
            context.Request.Builder.Methods.SelectMany(x => x.Parameters).Select(x => x.DefaultValue).ShouldAllBe(x => x == default(object));
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return AddProperty3(property3.ToArray());",
                    "foreach (var item in property3) Property3.Add(item);",
                    "return (TBuilder)this;"
                }
            );
        }

        [Theory]
        [InlineData(true, ArgumentValidationType.None)]
        [InlineData(false, ArgumentValidationType.None)]
        public async Task Returns_Error_When_Parsing_CustomBuilderArgumentType_Is_Not_Succesful(bool addNullChecks, ArgumentValidationType validateArguments)
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: addNullChecks, validateArguments: validateArguments, typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue("{Error}"))
            ]);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(builderNameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_AddMethodNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addMethodNameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderArgumentNullCheckExpression_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: true, typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(List<int>))
                    .WithTargetType(typeof(List<int>))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentNullCheckExpression).WithValue("{Error}"))
            ]);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderArgumentNullCheckExpression_Is_Not_Succesful_Using_Enumerable_CollectionType()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: true, newCollectionTypeName: typeof(IEnumerable<>).WithoutGenerics(), typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentNullCheckExpression).WithValue("{Error}"))
            ]);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));
    }
}
