namespace ClassFramework.Pipelines.Tests.Builder;

public class PipelineTests : IntegrationTestBase<ICommandService>
{
    public class ExecuteAsync : PipelineTests
    {
        private static BuilderContext CreateContext(bool addProperties = true, bool createAsObservable = false, bool useBuilderLazyValues = false)
            => new(
                CreateGenericClass(addProperties),
                CreateSettingsForBuilder
                (
                    builderNamespaceFormatString: "{class.Namespace}.Builders",
                    allowGenerationWithoutProperties: false,
                    copyAttributes: true,
                    createAsObservable: createAsObservable,
                    useBuilderLazyValues: useBuilderLazyValues
                ),
                CultureInfo.InvariantCulture,
                CancellationToken.None
            );

        [Fact]
        public async Task Sets_Partial()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

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
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Name.ShouldBe("MyClassBuilder");
            context.Builder.Namespace.ShouldBe("MyNamespace.Builders");
        }

        [Fact]
        public async Task Adds_Non_Lazy_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == true);
            context.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
            context.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.String", "System.Collections.Generic.List<System.String>" });
        }

        [Fact]
        public async Task Adds_Lazy_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(useBuilderLazyValues: true);

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == true);
            context.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
            context.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Func<System.String>", "System.Collections.Generic.List<System.Func<System.String>>" });
        }

        [Fact]
        public async Task Adds_Constructors()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Constructors.ShouldHaveSingleItem();
            context.Builder.Constructors.Single().CodeStatements
                .OfType<StringCodeStatementBuilder>()
                .Select(x => x.Statement)
                .ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    "Property2 = new System.Collections.Generic.List<string>();",
                    "Property1 = string.Empty;",
                    "SetDefaultValues();"
                });
        }

        [Fact]
        public async Task Adds_Constructors_Lazy()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(useBuilderLazyValues: true);

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Constructors.ShouldHaveSingleItem();
            context.Builder.Constructors.Single().CodeStatements
                .OfType<StringCodeStatementBuilder>()
                .Select(x => x.Statement)
                .ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    "Property2 = new System.Collections.Generic.List<System.Func<string>>();",
                    "Property1 = new System.Func<System.String>(() => string.Empty);",
                    "SetDefaultValues();"
                });
        }

        [Fact]
        public async Task Adds_GenericTypeArguments()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.GenericTypeArguments.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Adds_GenericTypeArgumentConstraints()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.GenericTypeArgumentConstraints.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Adds_Attributes()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Attributes.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Adds_Build_Method()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.Count(x => x.Name == "Build").ShouldBe(1);
            var method = context.Builder.Methods.Single(x => x.Name == "Build");
            method.ReturnTypeName.ShouldBe("MyNamespace.MyClass");
            method.ReturnTypeGenericTypeArguments.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "T" });
            method.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "return new MyNamespace.MyClass<T> { Property2 = Property2 };" });
        }

        [Fact]
        public async Task Adds_Build_Method_With_Lazy_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(useBuilderLazyValues: true);

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.Count(x => x.Name == "Build").ShouldBe(1);
            var method = context.Builder.Methods.Single(x => x.Name == "Build");
            method.ReturnTypeName.ShouldBe("MyNamespace.MyClass");
            method.ReturnTypeGenericTypeArguments.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "T" });
            method.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "return new MyNamespace.MyClass<T> { Property2 = Property2.Select(x => x()) };" });
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_NonCollection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.Count(x => x.Name == "WithProperty1").ShouldBe(1);
            var method = context.Builder.Methods.Single(x => x.Name == "WithProperty1");
            method.ReturnTypeName.ShouldBe("MyNamespace.Builders.MyClassBuilder<T>");
            method.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "Property1 = property1;", "return this;" });

            context.Builder.Methods.Where(x => x.Name == "WithProperty2").ShouldBeEmpty(); //only for the non-collection property
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Lazy_NonCollection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(useBuilderLazyValues: true);

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.Count(x => x.Name == "WithProperty1").ShouldBe(2);
            var lazyMethod = context.Builder.Methods.First(x => x.Name == "WithProperty1");
            lazyMethod.ReturnTypeName.ShouldBe("MyNamespace.Builders.MyClassBuilder<T>");
            lazyMethod.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Func<System.String>" });
            lazyMethod.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            lazyMethod.CodeStatements
                .OfType<StringCodeStatementBuilder>()
                .Select(x => x.Statement)
                .ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    "Property1 = property1;",
                    "return this;"
                });
            var nonLazyMethod = context.Builder.Methods.Last(x => x.Name == "WithProperty1");
            nonLazyMethod.ReturnTypeName.ShouldBe("MyNamespace.Builders.MyClassBuilder<T>");
            nonLazyMethod.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.String" });
            nonLazyMethod.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            nonLazyMethod.CodeStatements
                .OfType<StringCodeStatementBuilder>()
                .Select(x => x.Statement)
                .ToArray()
                .ShouldBeEquivalentTo(new[]
                {
                    "Property1 = new System.Func<System.String>(() => property1);",
                    "return this;"
                });

            context.Builder.Methods.Where(x => x.Name == "WithProperty2").ShouldBeEmpty(); //only for the non-collection property
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Collection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            var methods = context.Builder.Methods.Where(x => x.Name == "AddProperty2").ToArray();
            methods.Count(x => x.Name == "AddProperty2").ShouldBe(2);
            methods.Select(x => x.ReturnTypeName).ToArray().ShouldBeEquivalentTo(new[] { "MyNamespace.Builders.MyClassBuilder<T>", "MyNamespace.Builders.MyClassBuilder<T>" });
            methods.SelectMany(x => x.Parameters.Select(y => y.TypeName)).ToArray().ShouldBeEquivalentTo(new[] { "System.Collections.Generic.IEnumerable<System.String>", "System.String[]" });
            methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return AddProperty2(property2.ToArray());",
                    "foreach (var item in property2) Property2.Add(item);",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Lazy_Collection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(useBuilderLazyValues: true);

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            var methods = context.Builder.Methods.Where(x => x.Name == "AddProperty2").ToArray();
            methods.Count(x => x.Name == "AddProperty2").ShouldBe(4);
            methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "MyNamespace.Builders.MyClassBuilder<T>");
            methods.SelectMany(x => x.Parameters.Select(y => y.TypeName)).ToArray().ShouldBeEquivalentTo(new[] { "System.Collections.Generic.IEnumerable<System.Func<System.String>>", "System.Func<System.String>[]", "System.Collections.Generic.IEnumerable<System.String>", "System.String[]" });
            methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return AddProperty2(property2.ToArray());",
                    "foreach (var item in property2) Property2.Add(item);",
                    "return this;",
                    "return AddProperty2(property2.ToArray());",
                    "foreach (var item in property2) Property2.Add(() => item);",
                    "return this;"
                }
            );
        }

        [Fact]
        public async Task Adds_PropertyChanged_Event_When_Creating_Observable_Builder()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(createAsObservable: true);

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "_property1", "_property2", "PropertyChanged" });
            context.Builder.Methods.Count(x => x.Name == "HandlePropertyChanged").ShouldBe(1);
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(addProperties: false);

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("There must be at least one property");
        }
    }

    public class IntegrationTests : PipelineTests
    {
        [Fact]
        public async Task Creates_Builder_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateClassWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var typenameMappings = CreateTypenameMappings();
            var settings = CreateSettingsForBuilder(addCopyConstructor: true, typenameMappings: typenameMappings, namespaceMappings: namespaceMappings, addNullChecks: true, enableNullableReferenceTypes: true);
            var context = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ExecuteAsync<BuilderContext, Class>(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();

            context.Builder.Name.ShouldBe("MyClassBuilder");
            context.Builder.Namespace.ShouldBe("MyNamespace.Builders");

            context.Builder.Methods.Count(x => x.Name == "Build").ShouldBe(1);
            var buildMethod = context.Builder.Methods.Single(x => x.Name == "Build");
            buildMethod.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            buildMethod.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "return new MyNamespace.MyClass { Property1 = Property1, Property2 = Property2, Property3 = Property3, Property4 = Property4, Property5 = Property5?.Build()!, Property6 = Property6?.Build()!, Property7 = new System.Collections.Generic.List<MySourceNamespace.MyClass>(Property7.Select(x => x.Build()!)), Property8 = new System.Collections.Generic.List<MySourceNamespace.MyClass>(Property8.Select(x => x.Build()!)) };" });

            context.Builder.Constructors.Count(x => x.Parameters.Count == 1).ShouldBe(1);
            var copyConstructor = context.Builder.Constructors.Single(x => x.Parameters.Count == 1);
            copyConstructor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            copyConstructor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
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
                }
            );

            // non-nullable non-value type properties have a backing field, so we can do null checks
            context.Builder.Fields.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "_property3",
                    "_property5",
                    "_property7"
                }
            );
            context.Builder.Fields.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.String",
                    "MyNamespace.Builders.MyClassBuilder",
                    "System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>"
                }
            );
            context.Builder.Fields.Select(x => x.IsNullable).ShouldAllBe(x => x == false);

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
                    "MyNamespace.Builders.MyClassBuilder",
                    "MyNamespace.Builders.MyClassBuilder",
                    "System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>",
                    "System.Collections.Generic.List<MyNamespace.Builders.MyClassBuilder>"
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
        }

        private static BuilderContext CreateContext(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings, CultureInfo.InvariantCulture, CancellationToken.None);
    }
}
