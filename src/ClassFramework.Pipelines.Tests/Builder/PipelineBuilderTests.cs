namespace ClassFramework.Pipelines.Tests.Builder;

public class PipelineBuilderTests : IntegrationTestBase<IPipelineBuilder<BuilderContext>>
{
    public class Process : PipelineBuilderTests
    {
        private BuilderContext CreateContext(bool addProperties = true, bool createAsObservable = false)
            => new BuilderContext
            (
                CreateGenericModel(addProperties),
                CreateSettingsForBuilder
                (
                    builderNamespaceFormatString: "{Namespace}.Builders",
                    allowGenerationWithoutProperties: false,
                    copyAttributes: true,
                    createAsObservable: createAsObservable
                ).Build(),
                CultureInfo.InvariantCulture
            );

        [Fact]
        public async Task Sets_Partial()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Partial.Should().BeTrue();
        }

        [Fact]
        public async Task Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Name.Should().Be("MyClassBuilder");
            context.Builder.Namespace.Should().Be("MyNamespace.Builders");
        }

        [Fact]
        public async Task Adds_Properties()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(true);
            context.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo("Property1", "Property2");
            context.Builder.Properties.Select(x => x.TypeName).Should().BeEquivalentTo("System.String", "System.Collections.Generic.List<System.String>");
        }

        [Fact]
        public async Task Adds_Constructors()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Constructors.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task Adds_GenericTypeArguments()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.GenericTypeArguments.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Adds_GenericTypeArgumentConstraints()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.GenericTypeArgumentConstraints.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Adds_Attributes()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Attributes.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Adds_Build_Method()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Methods.Where(x => x.Name == "Build").Should().ContainSingle();
            var method = context.Builder.Methods.Single(x => x.Name == "Build");
            method.ReturnTypeName.Should().Be("MyNamespace.MyClass<T>");
            method.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("return new MyNamespace.MyClass<T> { Property2 = Property2 };");
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_NonCollection_Property()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Methods.Where(x => x.Name == "WithProperty1").Should().ContainSingle();
            var method = context.Builder.Methods.Single(x => x.Name == "WithProperty1");
            method.ReturnTypeName.Should().Be("MyNamespace.Builders.MyClassBuilder<T>");
            method.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("Property1 = property1;", "return this;");

            context.Builder.Methods.Where(x => x.Name == "WithProperty2").Should().BeEmpty(); //only for the non-collection property
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Collection_Property()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            var methods = context.Builder.Methods.Where(x => x.Name == "AddProperty2");
            methods.Where(x => x.Name == "AddProperty2").Should().HaveCount(2);
            methods.Select(x => x.ReturnTypeName).Should().BeEquivalentTo("MyNamespace.Builders.MyClassBuilder<T>", "MyNamespace.Builders.MyClassBuilder<T>");
            methods.SelectMany(x => x.Parameters.Select(y => y.TypeName)).Should().BeEquivalentTo("System.Collections.Generic.IEnumerable<System.String>", "System.String[]");
            methods.SelectMany(x => x.CodeStatements).Should().AllBeOfType<StringCodeStatementBuilder>();
            methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "return AddProperty2(property2.ToArray());",
                "foreach (var item in property2) Property2.Add(item);",
                "return this;"
            );
        }

        [Fact]
        public async Task Adds_PropertyChanged_Event_When_Creating_Observable_Builder()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext(createAsObservable: true);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Builder.Fields.Select(x => x.Name).Should().BeEquivalentTo("_property1", "_property2", "PropertyChanged");
            context.Builder.Methods.Should().ContainSingle(x => x.Name == "HandlePropertyChanged");
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut().Build();
            var context = CreateContext(addProperties: false);

            // Act
            var result = await sut.Process(context);
            var innerResult = result?.InnerResults.FirstOrDefault();

            // Assert
            innerResult.Should().NotBeNull();
            innerResult!.Status.Should().Be(ResultStatus.Invalid);
            innerResult.ErrorMessage.Should().Be("To create a builder class, there must be at least one property");
        }
    }

    public class IntegrationTests : PipelineBuilderTests
    {
        [Fact]
        public async Task Creates_Builder_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateModelWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var typenameMappings = CreateTypenameMappings();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true, typenameMappings: typenameMappings, namespaceMappings: namespaceMappings, addNullChecks: true, enableNullableReferenceTypes: true);
            var context = CreateContext(model, settings);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            context.Builder.Name.Should().Be("MyClassBuilder");
            context.Builder.Namespace.Should().Be("MyNamespace.Builders");

            context.Builder.Methods.Where(x => x.Name == "Build").Should().ContainSingle();
            var buildMethod = context.Builder.Methods.Single(x => x.Name == "Build");
            buildMethod.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            buildMethod.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo("return new MyNamespace.MyClass { Property1 = Property1, Property2 = Property2, Property3 = Property3, Property4 = Property4, Property5 = Property5?.Build()!, Property6 = Property6?.Build()!, Property7 = new System.Collections.Generic.List<MySourceNamespace.MyClass>(Property7.Select(x => x.Build()!)), Property8 = new System.Collections.Generic.List<MySourceNamespace.MyClass>(Property8.Select(x => x.Build()!)) };");

            context.Builder.Constructors.Where(x => x.Parameters.Count == 1).Should().ContainSingle();
            var copyConstructor = context.Builder.Constructors.Single(x => x.Parameters.Count == 1);
            copyConstructor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            copyConstructor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "if (source is null) throw new System.ArgumentNullException(nameof(source));",
                "_property7 = new System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>();",
                "Property8 = new System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>();",
                "Property1 = source.Property1;",
                "Property2 = source.Property2;",
                "_property3 = source.Property3;",
                "Property4 = source.Property4;",
                "_property5 = source.Property5?.ToBuilder()!;",
                "Property6 = source.Property6?.ToBuilder()!;",
                "if (source.Property7 is not null) foreach (var item in source.Property7.Select(x => x.ToBuilder())) _property7.Add(item);",
                "foreach (var item in source.Property8.Select(x => x.ToBuilder())) Property8.Add(item);"
            );

            // non-nullable non-value type properties have a backing field, so we can do null checks
            context.Builder.Fields.Select(x => x.Name).Should().BeEquivalentTo
            (
                "_property3",
                "_property5",
                "_property7"
            );
            context.Builder.Fields.Select(x => x.TypeName).Should().BeEquivalentTo
            (
                "System.String",
                "MyNamespace.Builders.MyClassBuilder",
                "System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>"
            );
            context.Builder.Fields.Select(x => x.IsNullable).Should().AllBeEquivalentTo(false);

            context.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo
            (
                "Property1",
                "Property2",
                "Property3",
                "Property4",
                "Property5",
                "Property6",
                "Property7",
                "Property8"
            );
            context.Builder.Properties.Select(x => x.TypeName).Should().BeEquivalentTo
            (
                "System.Int32",
                "System.Nullable<System.Int32>",
                "System.String",
                "System.String",
                "MyNamespace.Builders.MyClassBuilder",
                "MyNamespace.Builders.MyClassBuilder",
                "System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>",
                "System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>"
            );
            context.Builder.Properties.Select(x => x.IsNullable).Should().BeEquivalentTo
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
        }

        private static BuilderContext CreateContext(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings.Build(), CultureInfo.InvariantCulture);
    }
}
