namespace ClassFramework.Pipelines.Tests.Entity;

public class PipelineTests : IntegrationTestBase<ICommandService>
{
    public class ExecuteAsync : PipelineTests
    {
        private static GenerateEntityCommand CreateCommand(bool addProperties = true) => new(
            CreateGenericClass(addProperties),
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
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Partial.ShouldBeTrue();
        }

        [Fact]
        public async Task Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand();

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value.Name.ShouldBe("MyClass");
            result.Value.Namespace.ShouldBe("MyNamespace");
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var command = CreateCommand(addProperties: false);

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("There must be at least one property");
        }
    }

    public class IntegrationTests : PipelineTests
    {
        [Fact]
        public async Task Creates_ReadOnly_Entity_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateClassWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var settings = CreateSettingsForEntity(
                namespaceMappings: namespaceMappings,
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                newCollectionTypeName: typeof(IReadOnlyCollection<>).WithoutGenerics(),
                collectionTypeName: typeof(ReadOnlyValueCollection<>).WithoutGenerics());
            var command = CreateCommand(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Name.ShouldBe("MyClass");
            result.Value.Namespace.ShouldBe("MyNamespace");
            result.Value.Interfaces.ShouldBeEmpty();

            result.Value.Constructors.Count.ShouldBe(1);
            var copyConstructor = result.Value.Constructors.Single();
            copyConstructor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            copyConstructor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
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
                }
            );

            result.Value.Fields.ShouldBeEmpty();

            result.Value.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1",
                    "Property2",
                    "Property3",
                    "Property4",
                    "Property5",
                    "Property6",
                    "Property7",
                    "Property8"
                }
            );
            result.Value.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.Int32",
                    "System.Nullable<System.Int32>",
                    "System.String",
                    "System.String",
                    "MyNamespace.MyClass",
                    "MyNamespace.MyClass",
                    "System.Collections.Generic.IReadOnlyCollection<MyNamespace.MyClass>",
                    "System.Collections.Generic.IReadOnlyCollection<MyNamespace.MyClass>"
                }
            );
            result.Value.Properties.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
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
            result.Value.Properties.Select(x => x.HasGetter).ShouldAllBe(x => x == true);
            result.Value.Properties.SelectMany(x => x.GetterCodeStatements).ShouldBeEmpty();
            result.Value.Properties.Select(x => x.HasInitializer).ShouldAllBe(x => x == false);
            result.Value.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == false);
            result.Value.Properties.SelectMany(x => x.SetterCodeStatements).ShouldBeEmpty();
        }

        [Fact]
        public async Task Creates_ReadOnly_Entity_With_NamespaceMapping_And_NullCHecks_Without_PatternMatching()
        {
            // Arrange
            var model = CreateClassWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var settings = CreateSettingsForEntity(
                namespaceMappings: namespaceMappings,
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                newCollectionTypeName: typeof(IReadOnlyCollection<>).WithoutGenerics(),
                collectionTypeName: typeof(ReadOnlyValueCollection<>).WithoutGenerics(),
                usePatternMatchingForNullChecks: false
                );
            var command = CreateCommand(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Name.ShouldBe("MyClass");
            result.Value.Namespace.ShouldBe("MyNamespace");
            result.Value.Interfaces.ShouldBeEmpty();

            result.Value.Constructors.Count.ShouldBe(1);
            var copyConstructor = result.Value.Constructors.Single();
            copyConstructor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            copyConstructor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "if (property3 == null) throw new System.ArgumentNullException(nameof(property3));",
                    "if (property5 == null) throw new System.ArgumentNullException(nameof(property5));",
                    "if (property7 == null) throw new System.ArgumentNullException(nameof(property7));",
                    "this.Property1 = property1;",
                    "this.Property2 = property2;",
                    "this.Property3 = property3;",
                    "this.Property4 = property4;",
                    "this.Property5 = property5;",
                    "this.Property6 = property6;",
                    "this.Property7 = new CrossCutting.Common.ReadOnlyValueCollection<MyNamespace.MyClass>(property7);",
                    "this.Property8 = property8 == null ? null : new CrossCutting.Common.ReadOnlyValueCollection<MyNamespace.MyClass>(property8);"
                }
            );
        }

        [Fact]
        public async Task Creates_Observable_Entity_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateClassWithCustomTypeProperties();
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
            var command = CreateCommand(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Name.ShouldBe("MyClass");
            result.Value.Namespace.ShouldBe("MyNamespace");
            result.Value.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.INotifyPropertyChanged" });

            result.Value.Constructors.Count.ShouldBe(1);
            var publicParameterlessConstructor = result.Value.Constructors.Single();
            publicParameterlessConstructor.Parameters.ShouldBeEmpty();
            publicParameterlessConstructor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            publicParameterlessConstructor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "_property1 = default(System.Int32);",
                    "_property2 = default(System.Int32?);",
                    "_property3 = string.Empty;",
                    "_property4 = default(System.String?);",
                    "_property5 = default(MyNamespace.MyClass)!;",
                    "_property6 = default(MyNamespace.MyClass?);",
                    "_property7 = new CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>();",
                    "_property8 = new CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>();"
                }
            );

            // non collection type properties have a backing field, so we can implement INotifyPropertyChanged
            result.Value.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "_property1",
                    "_property2",
                    "_property3",
                    "_property4",
                    "_property5",
                    "_property6",
                    "_property7",
                    "_property8",
                    "PropertyChanged"
                }
            );
            result.Value.Fields.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.Int32",
                    "System.Nullable<System.Int32>",
                    "System.String",
                    "System.String",
                    "MyNamespace.MyClass",
                    "MyNamespace.MyClass",
                    "CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>",
                    "CrossCutting.Common.ObservableValueCollection<MyNamespace.MyClass>",
                    "System.ComponentModel.PropertyChangedEventHandler"
                }
            );
            result.Value.Fields.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
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
                    true,
                    true
                }
            );

            result.Value.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1",
                    "Property2",
                    "Property3",
                    "Property4",
                    "Property5",
                    "Property6",
                    "Property7",
                    "Property8"
                }
            );
            result.Value.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.Int32",
                    "System.Nullable<System.Int32>",
                    "System.String",
                    "System.String",
                    "MyNamespace.MyClass",
                    "MyNamespace.MyClass",
                    "System.Collections.ObjectModel.ObservableCollection<MyNamespace.MyClass>",
                    "System.Collections.ObjectModel.ObservableCollection<MyNamespace.MyClass>"
                }
            );
            result.Value.Properties.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
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
            result.Value.Properties.Select(x => x.HasGetter).ShouldAllBe(x => x == true);
            result.Value.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return _property1;",
                    "return _property2;",
                    "return _property3;",
                    "return _property4;",
                    "return _property5;",
                    "return _property6;",
                    "return _property7;",
                    "return _property8;"
                }
            );
            result.Value.Properties.Select(x => x.HasInitializer).ShouldAllBe(x => x == false);
            result.Value.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == true);
            result.Value.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Int32>.Default.Equals(_property1, value);",
                    "_property1 = value;",
                    "if (hasChanged) HandlePropertyChanged(nameof(Property1));",
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Nullable<System.Int32>>.Default.Equals(_property2, value);",
                    "_property2 = value;",
                    "if (hasChanged) HandlePropertyChanged(nameof(Property2));",
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_property3!, value!);",
                    "_property3 = value ?? throw new System.ArgumentNullException(nameof(value));",
                    "if (hasChanged) HandlePropertyChanged(nameof(Property3));",
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_property4!, value!);",
                    "_property4 = value;", "if (hasChanged) HandlePropertyChanged(nameof(Property4));",
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<MyNamespace.MyClass>.Default.Equals(_property5!, value!);",
                    "_property5 = value ?? throw new System.ArgumentNullException(nameof(value));",
                    "if (hasChanged) HandlePropertyChanged(nameof(Property5));",
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<MyNamespace.MyClass>.Default.Equals(_property6!, value!);",
                    "_property6 = value;",
                    "if (hasChanged) HandlePropertyChanged(nameof(Property6));",
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<MyNamespace.MyClass>>.Default.Equals(_property7!, value!);",
                    "_property7 = value ?? throw new System.ArgumentNullException(nameof(value));",
                    "if (hasChanged) HandlePropertyChanged(nameof(Property7));",
                    "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<MyNamespace.MyClass>>.Default.Equals(_property8!, value!);",
                    "_property8 = value;", "if (hasChanged) HandlePropertyChanged(nameof(Property8));"
                }
            );
        }

        [Fact]
        public async Task Creates_Entity_With_Custom_Validation()
        {
            // Arrange
            var model = CreateClassWithCustomTypeProperties();
            var settings = CreateSettingsForEntity(
                addNullChecks: true,
                validateArguments: ArgumentValidationType.CustomValidationCode);
            var command = CreateCommand(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Methods.Count(x => x.Name == "Validate").ShouldBe(1);
        }

        [Fact]
        public async Task Creates_Entity_With_IEquatable_Implementation()
        {
            // Arrange
            var model = CreateClassWithCustomTypeProperties();
            var settings = CreateSettingsForEntity(implementIEquatable: true);
            var command = CreateCommand(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<GenerateEntityCommand, ClassBuilder>(command);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value.ShouldNotBeNull();

            result.Value.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(
                new[]
                {
                    "Equals",
                    "Equals",
                    "GetHashCode",
                    "==",
                    "!=",
                    "MySourceNamespace.Builders.MyClassBuilder",
                    "ToBuilder"
                });
        }

        private static GenerateEntityCommand CreateCommand(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings, CultureInfo.InvariantCulture);
    }
}
