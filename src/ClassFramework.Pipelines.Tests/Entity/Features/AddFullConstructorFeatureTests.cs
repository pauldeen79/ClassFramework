namespace ClassFramework.Pipelines.Tests.Entity.Features;

public class AddFullConstructorFeatureTests : TestBase<Pipelines.Entity.Features.AddFullConstructorFeature>
{
    public class Process : AddFullConstructorFeatureTests
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
        public void Adds_Constructor_Without_NullChecks()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(addNullChecks: false);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().ContainSingle();
            var ctor = model.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Select(x => x.Name).Should().BeEquivalentTo("property1", "property2", "property3");
            ctor.Parameters.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "this.Property1 = property1;",
                "this.Property2 = property2;",
                "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);"
            );
        }

        [Fact]
        public void Adds_Constructor_With_NullChecks()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(addNullChecks: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().ContainSingle();
            var ctor = model.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Select(x => x.Name).Should().BeEquivalentTo("property1", "property2", "property3");
            ctor.Parameters.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "if (property2 is null) throw new System.ArgumentNullException(nameof(property2));",
                "if (property3 is null) throw new System.ArgumentNullException(nameof(property3));",
                "this.Property1 = property1;",
                "this.Property2 = property2;",
                "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);"
            );
        }

        [Fact]
        public void Adds_Constructor_With_NullChecks_And_ExceptionThrowIfNull()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(addNullChecks: true, useExceptionThrowIfNull: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().ContainSingle();
            var ctor = model.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Select(x => x.Name).Should().BeEquivalentTo("property1", "property2", "property3");
            ctor.Parameters.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "System.ArgumentNullException.ThrowIfNull(property2);",
                "System.ArgumentNullException.ThrowIfNull(property3);",
                "this.Property1 = property1;",
                "this.Property2 = property2;",
                "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);"
            );
        }

        [Fact]
        public void Adds_Constructor_With_NullChecks_And_BackingFields()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(addNullChecks: true, addBackingFields: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().ContainSingle();
            var ctor = model.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Select(x => x.Name).Should().BeEquivalentTo("property1", "property2", "property3");
            ctor.Parameters.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "if (property2 is null) throw new System.ArgumentNullException(nameof(property2));",
                "if (property3 is null) throw new System.ArgumentNullException(nameof(property3));",
                "this._property1 = property1;",
                "this._property2 = property2;",
                "this._property3 = new System.Collections.Generic.List<System.Int32>(property3);"
            );
        }

        [Fact]
        public void Adds_Constructor_With_DomainValidation()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(validateArguments: ArgumentValidationType.IValidatableObject);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().ContainSingle();
            var ctor = model.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Select(x => x.Name).Should().BeEquivalentTo("property1", "property2", "property3");
            ctor.Parameters.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>");
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "this.Property1 = property1;",
                "this.Property2 = property2;",
                "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);",
                "System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);"
            );
        }

        [Fact]
        public void Adds_Constructor_With_TypeMapping()
        {
            // Arrange
            var sourceModel = CreateModelWithCustomTypeProperties();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(namespaceMappings: CreateNamespaceMappings());
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().ContainSingle();
            var ctor = model.Constructors.Single();
            ctor.Protected.Should().BeFalse();
            ctor.ChainCall.Should().BeEmpty();
            ctor.Parameters.Select(x => x.Name).Should().BeEquivalentTo
            (
                "property1",
                "property2",
                "property3",
                "property4",
                "property5",
                "property6",
                "property7",
                "property8"
            );
            ctor.Parameters.Select(x => x.TypeName).Should().BeEquivalentTo
            (
                "System.Int32",
                "System.Int32",
                "System.String",
                "System.String",
                "MyNamespace.MyClass",
                "MyNamespace.MyClass",
                "System.Collections.Generic.IEnumerable<MyNamespace.MyClass>",
                "System.Collections.Generic.IEnumerable<MyNamespace.MyClass>"
            );
            ctor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "this.Property1 = property1;",
                "this.Property2 = property2;",
                "this.Property3 = property3;",
                "this.Property4 = property4;",
                "this.Property5 = property5;",
                "this.Property6 = property6;",
                "this.Property7 = new System.Collections.Generic.List<MyNamespace.MyClass>(property7);",
                "this.Property8 = property8 is null ? null : new System.Collections.Generic.List<MyNamespace.MyClass>(property8);"
            );
        }
    }
}
