﻿namespace ClassFramework.Pipelines.Tests.Entity;

public class PipelineTests : IntegrationTestBase<IPipeline<EntityContext>>
{
    public class ProcessAsync : PipelineTests
    {
        private static EntityContext CreateContext(bool addProperties = true) => new(
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
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Partial.ShouldBeTrue();
        }

        [Fact]
        public async Task Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Name.ShouldBe("MyClass");
            context.Builder.Namespace.ShouldBe("MyNamespace");
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(addProperties: false);

            // Act
            var result = await sut.ProcessAsync(context);
            var innerResult = result?.InnerResults.FirstOrDefault();

            // Assert
            innerResult.ShouldNotBeNull();
            innerResult!.Status.ShouldBe(ResultStatus.Invalid);
            innerResult.ErrorMessage.ShouldBe("To create an entity class, there must be at least one property");
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
            var context = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();

            context.Builder.Name.ShouldBe("MyClass");
            context.Builder.Namespace.ShouldBe("MyNamespace");
            context.Builder.Interfaces.ShouldBeEmpty();

            context.Builder.Constructors.Count.ShouldBe(1);
            var copyConstructor = context.Builder.Constructors.Single();
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

            context.Builder.Fields.ShouldBeEmpty();

            context.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Properties.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Properties.Select(x => x.HasGetter).ShouldAllBe(x => x == true);
            context.Builder.Properties.SelectMany(x => x.GetterCodeStatements).ShouldBeEmpty();
            context.Builder.Properties.Select(x => x.HasInitializer).ShouldAllBe(x => x == false);
            context.Builder.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == false);
            context.Builder.Properties.SelectMany(x => x.SetterCodeStatements).ShouldBeEmpty();
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
            var context = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();

            context.Builder.Name.ShouldBe("MyClass");
            context.Builder.Namespace.ShouldBe("MyNamespace");
            context.Builder.Interfaces.ShouldBeEmpty();

            context.Builder.Constructors.Count.ShouldBe(1);
            var copyConstructor = context.Builder.Constructors.Single();
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
            var context = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();

            context.Builder.Name.ShouldBe("MyClass");
            context.Builder.Namespace.ShouldBe("MyNamespace");
            context.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "System.ComponentModel.INotifyPropertyChanged" });

            context.Builder.Constructors.Count.ShouldBe(1);
            var publicParameterlessConstructor = context.Builder.Constructors.Single();
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
            context.Builder.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Fields.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Fields.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
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

            context.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Properties.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Properties.Select(x => x.HasGetter).ShouldAllBe(x => x == true);
            context.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
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
            context.Builder.Properties.Select(x => x.HasInitializer).ShouldAllBe(x => x == false);
            context.Builder.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == true);
            context.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
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
            var context = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();

            context.Builder.Methods.Count(x => x.Name == "Validate").ShouldBe(1);
        }

        [Fact]
        public async Task Creates_Entity_With_IEquatable_Implementation()
        {
            // Arrange
            var model = CreateClassWithCustomTypeProperties();
            var settings = CreateSettingsForEntity(implementIEquatable: true);
            var context = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();

            context.Builder.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(
                new[]
                {
                    "Equals",
                    "Equals",
                    "GetHashCode",
                    "==",
                    "!=",
                    "ToBuilder"
                });
        }

        private static EntityContext CreateContext(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings, CultureInfo.InvariantCulture);
    }
}
