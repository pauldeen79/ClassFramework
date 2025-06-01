namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddPropertiesComponentTests : TestBase<Pipelines.Builder.Components.AddPropertiesComponent>
{
    public class ProcessAsync : AddPropertiesComponentTests
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
        public async Task Does_Not_Add_Properties_On_Abstract_Builder()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableBuilderInheritance: true, isAbstract: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.ShouldBeEmpty();
        }

        [Fact]
        public async Task Adds_Properties_On_Non_Abstract_Builder()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                isAbstract: false,
                baseClass: new ClassBuilder().WithName("MyBaseClass").AddProperties(new PropertyBuilder().WithName("Property4").WithType(typeof(int))).BuildTyped());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(sourceModel.Properties.Select(x => x.Name).ToArray());
            context.Request.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(sourceModel.Properties.Select(x => x.TypeName.FixTypeName()).ToArray());
            context.Request.Builder.Properties.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo(sourceModel.Properties.Select(x => x.IsNullable).ToArray());
            context.Request.Builder.Properties.Select(x => x.IsValueType).ToArray().ShouldBeEquivalentTo(sourceModel.Properties.Select(x => x.IsValueType).ToArray());
        }

        [Fact]
        public async Task Uses_CustomBuilderArgumentType_When_Present()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                isAbstract: false,
                baseClass: new ClassBuilder().WithName("MyBaseClass").AddProperties(new PropertyBuilder().WithName("Property4").WithType(typeof(int))).BuildTyped(),
                typenameMappings:
                [
                    new TypenameMappingBuilder().WithSourceType(typeof(int)).WithTargetType(typeof(int)).AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue("MyCustomType")),
                    new TypenameMappingBuilder().WithSourceType(typeof(string)).WithTargetType(typeof(int)).AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue("MyCustomType")),
                    new TypenameMappingBuilder().WithSourceTypeName(typeof(List<int>).FullName!.FixTypeName()).WithTargetType(typeof(int)).AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderArgumentType).WithValue("MyCustomType")),
                ]);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(sourceModel.Properties.Select(x => x.Name).ToArray());
            context.Request.Builder.Properties.Select(x => x.TypeName).ShouldAllBe(x => x == "MyCustomType");
        }

        [Fact]
        public async Task Replaces_CollectionTypeName_Correctly()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                isAbstract: false,
                baseClass: new ClassBuilder().WithName("MyBaseClass").AddProperties(new PropertyBuilder().WithName("Property4").WithType(typeof(int))).BuildTyped(),
                newCollectionTypeName: typeof(ReadOnlyCollection<>).WithoutGenerics());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(sourceModel.Properties.Select(x => x.Name).ToArray());
            context.Request.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String", "System.Collections.ObjectModel.ReadOnlyCollection<System.Int32>" });
        }

        [Fact]
        public async Task Adds_Attributes_To_Properties_From_SourceModel_Properties_When_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.Attributes).Select(x => x.Name).ShouldNotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.Attributes).Select(x => x.Name).ShouldAllBe(x => x == "MyAttribute");
        }

        [Fact]
        public async Task Does_Not_Add_Attributes_To_Properties_From_SourceModel_Properties_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.Attributes).ShouldBeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_CodeStatements_To_Properties_When_AddNullChecks_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).ShouldBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).ShouldBeEmpty();
        }

        [Theory]
        [InlineData(ArgumentValidationType.None)]
        [InlineData(ArgumentValidationType.IValidatableObject)]
        public async Task Adds_CodeStatements_To_Properties_When_AddNullChecks_Is_True_And_ValidateArguments_Is(ArgumentValidationType validateArguments)
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddProperties(new PropertyBuilder().WithName("MyOptionalProperty").WithType(typeof(string)).WithIsNullable())
                .AddProperties(new PropertyBuilder().WithName("MyRequiredProperty").WithType(typeof(string)))
                .BuildTyped();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                validateArguments: validateArguments);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).ShouldNotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "return _myRequiredProperty;" });
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).ShouldNotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "_myRequiredProperty = value ?? throw new System.ArgumentNullException(nameof(value));" });
        }

        [Theory]
        [InlineData(ArgumentValidationType.None)]
        [InlineData(ArgumentValidationType.IValidatableObject)]
        public async Task Adds_CodeStatements_To_Properties_With_CsharpFriendlyName_When_AddNullChecks_Is_True_And_ValidateArguments_Is(ArgumentValidationType validateArguments)
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddProperties(new PropertyBuilder().WithName("Delegate").WithType(typeof(string)))
                .BuildTyped();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                validateArguments: validateArguments);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).ShouldNotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "return _delegate;" });
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).ShouldNotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "_delegate = value ?? throw new System.ArgumentNullException(nameof(value));" });
        }

        [Theory]
        [InlineData(ArgumentValidationType.None)]
        [InlineData(ArgumentValidationType.IValidatableObject)]
        public async Task Adds_Fields_When_AddNullChecks_Is_True_And_ValidateArguments_Is(ArgumentValidationType validateArguments)
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)))
                .BuildTyped();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                validateArguments: validateArguments);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "_myProperty" });
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderArgumentType_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                typenameMappings:
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

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}
