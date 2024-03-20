namespace ClassFramework.Pipelines.Tests.Builder.Features;

public class ValidatableObjectFeatureTests : TestBase<Pipelines.Builder.Features.ValidatableObjectFeature>
{
    public class Process : ValidatableObjectFeatureTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.Process(context: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public void Adds_IValidatableObject_Interface_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Shared_Between_Builder_And_Entity()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(validateArguments: ArgumentValidationType.Shared);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEquivalentTo("System.ComponentModel.DataAnnotations.IValidatableObject");
            model.Methods.Select(x => x.Name).Should().BeEquivalentTo("Validate");
            model.Methods.Single(x => x.Name == "Validate").CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "var instance = new SomeNamespace.SomeClassBase { MyProperty = MyProperty };",
                "var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();",
                "System.ComponentModel.DataAnnotations.Validator.TryValidateObject(instance, new System.ComponentModel.DataAnnotations.ValidationContext(instance), results, true);",
                "return results;"
            );
        }

        [Fact]
        public void Adds_IValidatableObject_Interface_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Shared_Between_Builder_And_Entity_No_Properties()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(validateArguments: ArgumentValidationType.Shared);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEquivalentTo("System.ComponentModel.DataAnnotations.IValidatableObject");
            model.Methods.Select(x => x.Name).Should().BeEquivalentTo("Validate");
            model.Methods.Single(x => x.Name == "Validate").CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "var instance = new SomeNamespace.SomeClassBase();",
                "var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();",
                "System.ComponentModel.DataAnnotations.Validator.TryValidateObject(instance, new System.ComponentModel.DataAnnotations.ValidationContext(instance), results, true);",
                "return results;"
            );
        }

        [Fact]
        public void Adds_IValidatableObject_Interface_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Shared_Between_Builder_And_Entity_With_Custom_ValidationCode()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)))
                .Build();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(validateArguments: ArgumentValidationType.Shared, typenameMappings:
                [
                    new TypenameMappingBuilder()
                        .WithSourceType(sourceModel)
                        .WithTargetType(sourceModel)
                        .AddMetadata(MetadataNames.CustomBuilderValidationCode, "// here goes some custom validation code")
                        .AddMetadata(MetadataNames.CustomBuilderValidationCode, "return Enumerable.Empty<ValidationResult>();")
                ]);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEquivalentTo("System.ComponentModel.DataAnnotations.IValidatableObject");
            model.Methods.Select(x => x.Name).Should().BeEquivalentTo("Validate");
            model.Methods.Single(x => x.Name == "Validate").CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "var instance = new SomeNamespace.SomeClassBase { MyProperty = MyProperty };",
                "// here goes some custom validation code",
                "return Enumerable.Empty<ValidationResult>();"
            );
        }

        [Fact]
        public void Does_Not_Add_IValidatableObject_Interface_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Not_Shared_Between_Builder_And_Entity()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEmpty();
            model.Methods.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Error_When_Parsing_CustomBuilderEntityInstanciation_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)))
                .Build();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(validateArguments: ArgumentValidationType.Shared, typenameMappings:
                [
                    new TypenameMappingBuilder()
                        .WithSourceType(sourceModel)
                        .WithTargetType(sourceModel)
                        .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderEntityInstanciation).WithValue("{Error}"))
                ]);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        private static PipelineContext<IConcreteTypeBuilder, BuilderContext> CreateContext(TypeBase sourceModel, ClassBuilder model, PipelineSettingsBuilder settings)
            => new(model, new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
    }
}
