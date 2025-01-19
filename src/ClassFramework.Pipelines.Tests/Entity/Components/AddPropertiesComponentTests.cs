namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddPropertiesComponentTests : TestBase<Pipelines.Entity.Components.AddPropertiesComponent>
{
    public class Process : AddPropertiesComponentTests
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

        [Fact]
        public async Task Adds_Properties_From_SourceModel()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Select(x => x.Name).Should().BeEquivalentTo("Property1", "Property2", "Property3");
        }

        [Fact]
        public async Task Maps_TypeNames_Correctly()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(namespaceMappings: [new NamespaceMappingBuilder().WithSourceNamespace("MySourceNamespace").WithTargetNamespace("MyMappedNamespace")]);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Select(x => x.TypeName).Should().BeEquivalentTo
            (
                "System.Int32",
                "System.Nullable<System.Int32>",
                "System.String",
                "System.String",
                "MyMappedNamespace.MyClass",
                "MyMappedNamespace.MyClass",
                "System.Collections.Generic.IReadOnlyCollection<MyMappedNamespace.MyClass>",
                "System.Collections.Generic.IReadOnlyCollection<MyMappedNamespace.MyClass>"
            );
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Adds_Setters_When_Specified_In_Settings(bool addSetters)
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addSetters: addSetters);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Select(x => x.HasSetter).Should().BeEquivalentTo([addSetters, addSetters, false]); // not on the collection property!
        }

        [Theory]
        [InlineData(SubVisibility.InheritFromParent)]
        [InlineData(SubVisibility.Public)]
        [InlineData(SubVisibility.Internal)]
        [InlineData(SubVisibility.Private)]
        public async Task Sets_SetterVisibility_From_Settings(SubVisibility setterVisibility)
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addSetters: true, setterVisibility: setterVisibility);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.Select(x => x.SetterVisibility).Should().AllBeEquivalentTo(setterVisibility);
        }

        [Fact]
        public async Task Adds_Mapped_And_Filtered_Attributes_According_To_Settings()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddProperties
                (
                    new PropertyBuilder()
                        .WithName("MyProperty")
                        .WithType(typeof(int))
                        .AddAttributes(
                            new AttributeBuilder().WithName("MyAttribute1"),
                            new AttributeBuilder().WithName("MyAttribute2"))
                ).BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyAttributes: true, copyAttributePredicate: a => a.Name.EndsWith('2'));
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.Attributes).Select(x => x.Name).Should().BeEquivalentTo("MyAttribute2");
        }

        [Fact]
        public async Task Does_Not_Add_Fields_When_AddBackingFields_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Fields.Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Property_GetterCodeStatements_When_AddBackingFields_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Property_SetterCodeStatements_When_AddBackingFields_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).Should().BeEmpty();
        }

        [Fact]
        public async Task Adds_Fields_When_AddBackingFields_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Fields.Select(x => x.Name).Should().BeEquivalentTo("_property1", "_property2", "_property3");
        }

        [Fact]
        public async Task Adds_Property_GetterCodeStatements_Without_PropertyChanged_Calls_When_AddBackingFields_Is_True_And_CreateAsObservable_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "return _property1;",
                "return _property2;",
                "return _property3;"
            );
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "_property1 = value;",
                "_property2 = value;",
                "_property3 = value;"
            );
        }

        [Fact]
        public async Task Adds_Property_GetterCodeStatements_With_ProperyChanged_Calls_When_AddBackingFields_Is_True_And_CreateAsObservable_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: true, createAsObservable: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.GetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "return _property1;",
                "return _property2;",
                "return _property3;"
            );
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Int32>.Default.Equals(_property1, value);",
                "_property1 = value;",
                "if (hasChanged) HandlePropertyChanged(nameof(Property1));",
                "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_property2, value);",
                "_property2 = value;",
                "if (hasChanged) HandlePropertyChanged(nameof(Property2));",
                "bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<System.Int32>>.Default.Equals(_property3, value);",
                "_property3 = value;",
                "if (hasChanged) HandlePropertyChanged(nameof(Property3));"
            );
        }

        [Fact]
        public async Task Adds_Property_SetterCodeStatements_When_AddBackingFields_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "_property1 = value;",
                "_property2 = value;",
                "_property3 = value;"
            );
        }

        [Fact]
        public async Task Adds_Property_SetterCodeStatements_With_NullChecks_When_AddBackingFields_Is_True_And_AddNullChecks_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addBackingFields: true, addNullChecks: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Properties.SelectMany(x => x.SetterCodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "_property1 = value;",
                "_property2 = value ?? throw new System.ArgumentNullException(nameof(value));",
                "_property3 = value ?? throw new System.ArgumentNullException(nameof(value));"
            );
        }
    }
}
