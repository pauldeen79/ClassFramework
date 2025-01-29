namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddPropertiesComponentTests : TestBase<Pipelines.Builder.Components.AddPropertiesComponent>
{
    public class ProcessAsync : AddPropertiesComponentTests
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
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Should().BeEmpty();
        }

        [Fact]
        public async Task Adds_Properties_On_Non_Abstract_Builder()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                isAbstract: false,
                baseClass: new ClassBuilder().WithName("MyBaseClass").AddProperties(new PropertyBuilder().WithName("Property4").WithType(typeof(int))).BuildTyped());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo(sourceModel.Properties.Select(x => x.Name));
            context.Request.Builder.Properties.Select(x => x.TypeName).Should().BeEquivalentTo(sourceModel.Properties.Select(x => x.TypeName.FixTypeName()));
            context.Request.Builder.Properties.Select(x => x.IsNullable).Should().BeEquivalentTo(sourceModel.Properties.Select(x => x.IsNullable));
            context.Request.Builder.Properties.Select(x => x.IsValueType).Should().BeEquivalentTo(sourceModel.Properties.Select(x => x.IsValueType));
        }

        [Fact]
        public async Task Uses_CustomBuilderArgumentType_When_Present()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
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
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo(sourceModel.Properties.Select(x => x.Name));
            context.Request.Builder.Properties.Select(x => x.TypeName).Should().AllBe("MyCustomType");
        }

        [Fact]
        public async Task Replaces_CollectionTypeName_Correctly()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
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
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo(sourceModel.Properties.Select(x => x.Name));
            context.Request.Builder.Properties.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String", "System.Collections.ObjectModel.ReadOnlyCollection<System.Int32>");
        }

        [Fact]
        public async Task Adds_Attributes_To_Properties_From_SourceModel_Properties_When_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.Attributes).Select(x => x.Name).Should().NotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.Attributes).Select(x => x.Name).Should().AllBeEquivalentTo("MyAttribute");
        }

        [Fact]
        public async Task Does_Not_Add_Attributes_To_Properties_From_SourceModel_Properties_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyAttributes: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.Attributes).Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_CodeStatements_To_Properties_When_AddNullChecks_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().BeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().BeEmpty();
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
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                validateArguments: validateArguments);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().NotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().AllBeOfType<StringCodeStatementBuilder>();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("return _myRequiredProperty;");
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().NotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().AllBeOfType<StringCodeStatementBuilder>();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("_myRequiredProperty = value ?? throw new System.ArgumentNullException(nameof(value));");
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
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                validateArguments: validateArguments);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().NotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().AllBeOfType<StringCodeStatementBuilder>();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("return _delegate;");
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().NotBeEmpty();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().AllBeOfType<StringCodeStatementBuilder>();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("_delegate = value ?? throw new System.ArgumentNullException(nameof(value));");
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
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                validateArguments: validateArguments);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Fields.Select(x => x.Name).Should().BeEquivalentTo("_myProperty");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderArgumentType_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
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
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}
