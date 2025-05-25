namespace ClassFramework.Pipelines.Tests.Builder.Components;

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
            Action a = () => sut.ProcessAsync(context: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("context");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Adds_Copy_Constructor_For_Abstract_Builder(bool hasBaseClass)
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeParser();
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
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeTrue();
            ctor.ChainCall.ShouldBe("base(source)");
            ctor.Parameters.Count.ShouldBe(1);
            var parameter = ctor.Parameters.Single();
            parameter.Name.ShouldBe("source");
            parameter.TypeName.ShouldBe("SomeNamespace.SomeClass");
            ctor.CodeStatements.ShouldBeEmpty();
        }

        [Fact]
        public async Task Adds_Copy_Constructor_For_Non_Abstract_Builder()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: true,
                enableEntityInheritance: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Count.ShouldBe(1);
            var parameter = ctor.Parameters.Single();
            parameter.Name.ShouldBe("source");
            parameter.TypeName.ShouldBe("SomeNamespace.SomeClass");
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property3 = new System.Collections.Generic.List<int>();",
                    "Property1 = source.Property1;",
                    "Property2 = source.Property2;",
                    "foreach (var item in source.Property3) Property3.Add(item);"
                }
            );
        }

        [Fact]
        public async Task Adds_Copy_Constructor_For_Non_Abstract_Builder_With_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                addCopyConstructor: true,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeTrue();
            ctor.ChainCall.ShouldBe("base(source)");
            ctor.Parameters.Count.ShouldBe(1);
            var parameter = ctor.Parameters.Single();
            parameter.Name.ShouldBe("source");
            parameter.TypeName.ShouldBe("SomeNamespace.SomeClass");
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property3 = new System.Collections.Generic.List<int>();",
                    "Property1 = source.Property1;",
                    "Property2 = source.Property2;",
                    "foreach (var item in source.Property3) Property3.Add(item);"
                }
            );
        }

        [Fact]
        public async Task Adds_Copy_Constructor_For_Non_Abstract_Builder_With_NullChecks()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeParser();
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
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Count.ShouldBe(1);
            var parameter = ctor.Parameters.Single();
            parameter.Name.ShouldBe("source");
            parameter.TypeName.ShouldBe("SomeNamespace.SomeClass");
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "if (source is null) throw new System.ArgumentNullException(nameof(source));",
                    "_property3 = new System.Collections.Generic.List<int>();",
                    "Property1 = source.Property1;",
                    "_property2 = source.Property2;",
                    "if (source.Property3 is not null) foreach (var item in source.Property3) _property3.Add(item);"
                }
            );
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderConstructorInitializeExpression_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeParser();
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
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_CustomBuilderArgumentType_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeParser();
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
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_SingleProperty()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("Filter").WithTypeName("ExpressionFramework.Domain.Evaluatables.ComposedEvaluatable"))
                .BuildTyped();
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "Filter = new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder(source.Filter);" });
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_SingleProperty_BackingMembers()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("Filter").WithTypeName("ExpressionFramework.Domain.Evaluatables.ComposedEvaluatable"))
                .BuildTyped();
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true, addNullChecks: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "if (source is null) throw new System.ArgumentNullException(nameof(source));", "_filter = new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder(source.Filter);" });
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_CollectionProperty()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("GroupByFields").WithTypeName(typeof(IReadOnlyCollection<string>).ReplaceGenericTypeName("ExpressionFramework.Domain.Expression")))
                .BuildTyped();
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "GroupByFields = new System.Collections.Generic.List<ExpressionFramework.Domain.Builders.ExpressionBuilder>(source.GroupByFields.Select(x => ExpressionFramework.Domain.Builders.ExpressionBuilderFactory.Create(x)));" });
        }

        [Fact]
        public async Task Processes_TypeMapping_Correctly_CollectionProperty_BackingMembers()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("GroupByFields").WithTypeName(typeof(IReadOnlyCollection<string>).ReplaceGenericTypeName("ExpressionFramework.Domain.Expression")))
                .BuildTyped();
            await InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true, addNullChecks: true).AddTypenameMappings(CreateExpressionFrameworkTypenameMappings());
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "if (source is null) throw new System.ArgumentNullException(nameof(source));", "_groupByFields = new System.Collections.Generic.List<ExpressionFramework.Domain.Builders.ExpressionBuilder>(source.GroupByFields.Select(x => ExpressionFramework.Domain.Builders.ExpressionBuilderFactory.Create(x)));" });
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}
