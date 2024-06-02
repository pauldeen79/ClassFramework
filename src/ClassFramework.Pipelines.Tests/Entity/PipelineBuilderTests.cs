namespace ClassFramework.Pipelines.Tests.Entity;

public class PipelineBuilderTests : IntegrationTestBase<IPipelineBuilder<EntityContext>>
{
    public class Process : PipelineBuilderTests
    {
        private EntityContext CreateContext(bool addProperties = true) => new EntityContext
        (
            CreateGenericModel(addProperties),
            CreateSettingsForEntity
            (
                allowGenerationWithoutProperties: false
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
            context.Builder.Name.Should().Be("MyClass");
            context.Builder.Namespace.Should().Be("MyNamespace");
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
            innerResult.ErrorMessage.Should().Be("To create an entity class, there must be at least one property");
        }
    }

    public class IntegrationTests : PipelineBuilderTests
    {
        [Fact]
        public async Task Creates_ReadOnly_Entity_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateModelWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var settings = CreateSettingsForEntity(
                namespaceMappings: namespaceMappings,
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                newCollectionTypeName: typeof(IReadOnlyCollection<>).WithoutGenerics(),
                collectionTypeName: typeof(ReadOnlyValueCollection<>).WithoutGenerics());
            var context = CreateContext(model, settings);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            
            context.Builder.Name.Should().Be("MyClass");
            context.Builder.Namespace.Should().Be("MyNamespace");
            context.Builder.Interfaces.Should().BeEmpty();

            context.Builder.Constructors.Should().ContainSingle();
            var copyConstructor = context.Builder.Constructors.Single();
            copyConstructor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            copyConstructor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "if (property3 is null) throw new System.ArgumentNullException(nameof(property3));",
                "if (property5 is null) throw new System.ArgumentNullException(nameof(property5));",
                "if (property7 is null) throw new System.ArgumentNullException(nameof(property7));",
                "this.Property1 = property1;",
                "this.Property2 = property2;",
                "this.Property3 = property3;",
                "this.Property4 = property4;",
                "this.Property5 = property5;",
                "this.Property6 = property6;",
                "this.Property7 = new CrossCutting.Common.ReadOnlyValueCollection<MyNamespace.MyClass>(property7);",
                "this.Property8 = property8 is null ? null : new CrossCutting.Common.ReadOnlyValueCollection<MyNamespace.MyClass>(property8);"
            );

            context.Builder.Fields.Should().BeEmpty();

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
                "MyNamespace.MyClass",
                "MyNamespace.MyClass",
                "System.Collections.Generic.IReadOnlyCollection<MyNamespace.MyClass>",
                "System.Collections.Generic.IReadOnlyCollection<MyNamespace.MyClass>"
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
            context.Builder.Properties.Select(x => x.HasGetter).Should().AllBeEquivalentTo(true);
            context.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().BeEmpty();
            context.Builder.Properties.Select(x => x.HasInitializer).Should().AllBeEquivalentTo(false);
            context.Builder.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(false);
            context.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().BeEmpty();
        }

        [Fact]
        public async Task Creates_Observable_Entity_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateModelWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var typenameMappings = CreateTypenameMappings();
            var settings = CreateSettingsForEntity(
                namespaceMappings: namespaceMappings,
                typenameMappings: typenameMappings,
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                addBackingFields: true,
                addSetters: true,
                createAsObservable: true,
                addPublicParameterlessConstructor: true,
                addFullConstructor: false,
                newCollectionTypeName: typeof(ObservableCollection<>).WithoutGenerics(),
                collectionTypeName: typeof(ObservableValueCollection<>).WithoutGenerics());
            var context = CreateContext(model, settings);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            
            context.Builder.Name.Should().Be("MyClass");
            context.Builder.Namespace.Should().Be("MyNamespace");
            context.Builder.Interfaces.Should().BeEquivalentTo("System.ComponentModel.INotifyPropertyChanged");

            context.Builder.Constructors.Should().ContainSingle();
            var publicParameterlessConstructor = context.Builder.Constructors.Single();
            publicParameterlessConstructor.Parameters.Should().BeEmpty();
            publicParameterlessConstructor.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            publicParameterlessConstructor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "_property1 = default(System.Int32);",
                "_property2 = default(System.Int32?);",
                "_property3 = string.Empty;",
                "_property4 = default(System.String?);",
                "_property5 = default(MyNamespace.MyClass)!;",
                "_property6 = default(MyNamespace.MyClass?);",
                "_property7 = new CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>();",
                "_property8 = new CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>();"
            );

            // non collection type properties have a backing field, so we can implement INotifyPropertyChanged
            context.Builder.Fields.Select(x => x.Name).Should().BeEquivalentTo
            (
                "_property1",
                "_property2",
                "_property3",
                "_property4",
                "_property5",
                "_property6",
                "_property7",
                "_property8",
                "PropertyChanged"
            );
            context.Builder.Fields.Select(x => x.TypeName).Should().BeEquivalentTo
            (
                "System.Int32",
                "System.Nullable<System.Int32>",
                "System.String", "System.String",
                "MyNamespace.MyClass",
                "MyNamespace.MyClass",
                "CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>",
                "CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>",
                "System.ComponentModel.PropertyChangedEventHandler"
            );
            context.Builder.Fields.Select(x => x.IsNullable).Should().BeEquivalentTo
            (
                new[]
                {
                    false,
                    true,
                    false,
                    true,
                    false,
                    true,
                    true,
                    false,
                    true
                }
            );

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
                "MyNamespace.MyClass",
                "MyNamespace.MyClass",
                "System.Collections.ObjectModel.ObservableCollection<MyNamespace.MyClass>",
                "System.Collections.ObjectModel.ObservableCollection<MyNamespace.MyClass>"
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
            context.Builder.Properties.Select(x => x.HasGetter).Should().AllBeEquivalentTo(true);
            context.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "return _property1;",
                "return _property2;",
                "return _property3;",
                "return _property4;",
                "return _property5;",
                "return _property6;",
                "return _property7;",
                "return _property8;"
            );
            context.Builder.Properties.Select(x => x.HasInitializer).Should().AllBeEquivalentTo(false);
            context.Builder.Properties.Select(x => x.HasSetter).Should().AllBeEquivalentTo(true);
            context.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "_property1 = value;",
                "HandlePropertyChanged(nameof(Property1));",
                "_property2 = value;",
                "HandlePropertyChanged(nameof(Property2));",
                "_property3 = value ?? throw new System.ArgumentNullException(nameof(value));",
                "HandlePropertyChanged(nameof(Property3));",
                "_property4 = value;",
                "HandlePropertyChanged(nameof(Property4));",
                "_property5 = value ?? throw new System.ArgumentNullException(nameof(value));",
                "HandlePropertyChanged(nameof(Property5));",
                "_property6 = value;",
                "HandlePropertyChanged(nameof(Property6));",
                "_property7 = value ?? throw new System.ArgumentNullException(nameof(value));",
                "HandlePropertyChanged(nameof(Property7));",
                "_property8 = value;",
                "HandlePropertyChanged(nameof(Property8));"
            );
        }

        [Fact]
        public async Task Creates_Entity_With_Custom_Validation()
        {
            // Arrange
            var model = CreateModelWithCustomTypeProperties();
            var settings = CreateSettingsForEntity(
                addNullChecks: true,
                validateArguments: ArgumentValidationType.CustomValidationCode);
            var context = CreateContext(model, settings);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            context.Builder.Methods.Should().ContainSingle(x => x.Name == "Validate");
        }

        [Fact]
        public async Task Creates_Entity_With_IEquatable_Implementation()
        {
            // Arrange
            var model = CreateModelWithCustomTypeProperties();
            var settings = CreateSettingsForEntity(implementIEquatable: true);
            var context = CreateContext(model, settings);

            var sut = CreateSut().Build();

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();

            context.Builder.Methods.Select(x => x.Name).Should().BeEquivalentTo(
                "Equals",
                "Equals",
                "GetHashCode",
                "==",
                "!=",
                "ToBuilder");
        }

        private static EntityContext CreateContext(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings.Build(), CultureInfo.InvariantCulture);
    }
}
