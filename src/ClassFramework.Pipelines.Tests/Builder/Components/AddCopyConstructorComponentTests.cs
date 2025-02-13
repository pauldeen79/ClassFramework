﻿namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddCopyConstructorComponentTests : TestBase<Pipelines.Builder.Components.AddCopyConstructorComponent>
{
    public class ProcessAsync : AddCopyConstructorComponentTests
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
        [InlineData(true)]
        [InlineData(false)]
        public async Task Adds_Copy_Constructor_For_Abstract_Builder(bool hasBaseClass)
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: hasBaseClass ? new ClassBuilder().WithName("BaseClass").BuildTyped() : null,
                isAbstract: hasBaseClass,
                addCopyConstructor: true,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().BeTrue();
            ctor.ChainCall.Should().Be("base(source)");
            ctor.Parameters.Should().ContainSingle();
            var parameter = ctor.Parameters.Single();
            parameter.Name.Should().Be("source");
            parameter.TypeName.Should().Be("SomeNamespace.SomeClass");
            ctor.CodeStatements.Should().BeEmpty();
        }

        [Fact]
        public async Task Adds_Copy_Constructor_For_Non_Abstract_Builder()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: true,
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
            ctor.Parameters.Should().ContainSingle();
            var parameter = ctor.Parameters.Single();
            parameter.Name.Should().Be("source");
            parameter.TypeName.Should().Be("SomeNamespace.SomeClass");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "Property3 = new System.Collections.Generic.List<int>();",
                "Property1 = source.Property1;",
                "Property2 = source.Property2;",
                "foreach (var item in source.Property3) Property3.Add(item);"
            );
        }

        [Fact]
        public async Task Adds_Copy_Constructor_For_Non_Abstract_Builder_With_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: true,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.Should().BeTrue();
            ctor.ChainCall.Should().Be("base(source)");
            ctor.Parameters.Should().ContainSingle();
            var parameter = ctor.Parameters.Single();
            parameter.Name.Should().Be("source");
            parameter.TypeName.Should().Be("SomeNamespace.SomeClass");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "Property3 = new System.Collections.Generic.List<int>();",
                "Property1 = source.Property1;",
                "Property2 = source.Property2;",
                "foreach (var item in source.Property3) Property3.Add(item);"
            );
        }

        [Fact]
        public async Task Adds_Copy_Constructor_For_Non_Abstract_Builder_With_NullChecks()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableBuilderInheritance: false,
                addCopyConstructor: true,
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
            ctor.Parameters.Should().ContainSingle();
            var parameter = ctor.Parameters.Single();
            parameter.Name.Should().Be("source");
            parameter.TypeName.Should().Be("SomeNamespace.SomeClass");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "if (source is null) throw new System.ArgumentNullException(nameof(source));",
                "_property3 = new System.Collections.Generic.List<int>();",
                "Property1 = source.Property1;",
                "_property2 = source.Property2;",
                "if (source.Property3 is not null) foreach (var item in source.Property3) _property3.Add(item);"
            );
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderConstructorInitializeExpression_Is_Not_Successful()
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
                        .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderConstructorInitializeExpression).WithValue("{Error}"))
                ]);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
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
        public async Task Processes_TypeMapping_Correctly_SingleProperty()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("Filter").WithTypeName("ExpressionFramework.Domain.Evaluatables.ComposedEvaluatable"))
                .BuildTyped();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("Filter = new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder(source.Filter);");
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_SingleProperty_BackingMembers()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("Filter").WithTypeName("ExpressionFramework.Domain.Evaluatables.ComposedEvaluatable"))
                .BuildTyped();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true, addNullChecks: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("if (source is null) throw new System.ArgumentNullException(nameof(source));", "_filter = new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder(source.Filter);");
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
            var settings = CreateSettingsForBuilder(addCopyConstructor: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("GroupByFields = new System.Collections.Generic.List<ExpressionFramework.Domain.Builders.ExpressionBuilder>(source.GroupByFields.Select(x => ExpressionFramework.Domain.Builders.ExpressionBuilderFactory.Create(x)));");
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_CollectionProperty_BackingMembers()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("GroupByFields").WithTypeName(typeof(IReadOnlyCollection<string>).ReplaceGenericTypeName("ExpressionFramework.Domain.Expression")))
                .BuildTyped();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true, addNullChecks: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Constructors.Should().ContainSingle();
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("if (source is null) throw new System.ArgumentNullException(nameof(source));", "_groupByFields = new System.Collections.Generic.List<ExpressionFramework.Domain.Builders.ExpressionBuilder>(source.GroupByFields.Select(x => ExpressionFramework.Domain.Builders.ExpressionBuilderFactory.Create(x)));");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}
