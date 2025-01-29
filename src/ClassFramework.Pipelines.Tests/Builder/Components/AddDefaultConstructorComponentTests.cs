namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddDefaultConstructorComponentTests : TestBase<Pipelines.Builder.Components.AddDefaultConstructorComponent>
{
    public class ProcessAsync : AddDefaultConstructorComponentTests
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

        [Theory]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        public async Task Adds_Default_Constructor_For_Abstract_Builder(bool hasBaseClass, bool expectedProtected, bool expectedCodeStatements)
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: hasBaseClass ? new ClassBuilder().WithName("BaseClass").BuildTyped() : null,
                addCopyConstructor: false,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().Be(expectedProtected);
            ctor.ChainCall.Should().Be(!hasBaseClass ? "base()" : string.Empty); // sounds unlogical, but this is the non-abstract base class for the builder, and it needs the base() chaincall to the abstract base class for the builder
            ctor.Parameters.Should().BeEmpty();
            if (expectedCodeStatements)
            {
                ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
                ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
                (
                    "Property3 = new System.Collections.Generic.List<int>();",
                    "Property2 = string.Empty;",
                    "SetDefaultValues();"
                );
            }
            else
            {
                ctor.CodeStatements.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Adds_Default_Constructor_For_Non_Abstract_Builder()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: false,
                enableEntityInheritance: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Should().BeEmpty();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "Property3 = new System.Collections.Generic.List<int>();",
                "Property2 = string.Empty;",
                "SetDefaultValues();"
            );
        }

        [Fact]
        public async Task Adds_Default_Constructor_For_Non_Abstract_Builder_With_Backing_Fields()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableNullableReferenceTypes: true, // important for backing fields
                addNullChecks: true,                // important for backing fields
                enableBuilderInheritance: false,
                addCopyConstructor: false,
                enableEntityInheritance: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Should().BeEmpty();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "_property3 = new System.Collections.Generic.List<int>();",
                "_property2 = string.Empty;",
                "SetDefaultValues();"
            );
        }

        [Fact]
        public async Task Adds_Default_Constructor_For_Non_Abstract_Builder_With_CollectionType_Enumerable()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: false,
                newCollectionTypeName: typeof(IEnumerable<>).WithoutGenerics(),
                enableEntityInheritance: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Should().BeEmpty();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "Property3 = System.Linq.Enumerable.Empty<int>();",
                "Property2 = string.Empty;",
                "SetDefaultValues();"
            );
        }

        [Fact]
        public async Task Adds_Default_Constructor_For_Non_Abstract_Builder_With_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: false,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().BeTrue();
            ctor.ChainCall.Should().Be("base()");
            ctor.Parameters.Should().BeEmpty();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "Property3 = new System.Collections.Generic.List<int>();",
                "Property2 = string.Empty;",
                "SetDefaultValues();"
            );
        }

        [Fact]
        public async Task Adds_Default_Constructor_Without_DefaultValue_Initialization_When_SetDefaultValues_Is_Set_To_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: false,
                setDefaultValues: false,
                enableEntityInheritance: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Should().BeEmpty();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "Property3 = new System.Collections.Generic.List<int>();"
            );
        }

        [Fact]
        public async Task Adds_SetDefaultValues_Partial_Method_When_SetDefaultValues_Is_Set_To_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setDefaultValues: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Should().ContainSingle(x => x.Name == "SetDefaultValues");
            var method = context.Request.Builder.Methods.Single(x => x.Name == "SetDefaultValues");
            method.Partial.Should().BeTrue();
            method.Parameters.Should().BeEmpty();
            method.CodeStatements.Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_SetDefaultValues_Partial_Method_When_SetDefaultValues_Is_Set_To_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(setDefaultValues: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Should().NotContain(x => x.Name == "SetDefaultValues");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderArgumentType_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: true,
                enableEntityInheritance: false,
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

        [Fact]
        public async Task Returns_Error_When_Parsing_DefaultValueStatement_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: true,
                enableEntityInheritance: false,
                typenameMappings:
                [
                    new TypenameMappingBuilder()
                        .WithSourceType(typeof(string))
                        .WithTargetType(typeof(string))
                        .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderDefaultValue).WithValue("{Error}"))
                ]);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_SingleProperty()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("Filter").WithTypeName("ExpressionFramework.Domain.Evaluatables.ComposedEvaluatable"))
                .BuildTyped();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder().AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("Filter = new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder();", "SetDefaultValues();");
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_CollectionProperty()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("GroupByFields").WithTypeName(typeof(IReadOnlyCollection<string>).ReplaceGenericTypeName("ExpressionFramework.Domain.Expression")))
                .BuildTyped();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder().AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("GroupByFields = new System.Collections.Generic.List<ExpressionFramework.Domain.Builders.ExpressionBuilder>();", "SetDefaultValues();");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}
