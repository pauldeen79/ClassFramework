namespace ClassFramework.Pipelines.Tests.Extensions;

public class TypeBaseExtensionsTests : TestBase<ClassBuilder>
{
    public class IsMemberValidForBuilderClass : TypeBaseExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_ParentTypeContainer()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();

            // Act & Assert
            Action a = () => sut.IsMemberValidForBuilderClass(parentTypeContainer: null!, CreateSettingsForBuilder());
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("parentTypeContainer");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();
            var parentTypeContainer = Fixture.Freeze<IParentTypeContainer>();

            // Act & Assert
            Action a = () => sut.IsMemberValidForBuilderClass(parentTypeContainer, settings: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }

        [Fact]
        public void Returns_True_When_Entity_Inheritance_Is_Disabled()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();
            var parentTypeContainer = Fixture.Freeze<IParentTypeContainer>();
            var settings = CreateSettingsForBuilder(enableBuilderInheritance: false).Build();

            // Act
            var result = sut.IsMemberValidForBuilderClass(parentTypeContainer, settings);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Returns_Correct_Result_When_ParentTypeContainer_Is_Defined_On_TypeBase()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();
            var parentTypeContainer = Fixture.Freeze<IParentTypeContainer>();
            var settings = CreateSettingsForBuilder(
                inheritanceComparisonDelegate: (_, _) => false,
                enableEntityInheritance: true
            ).Build();

            // Act
            var result = sut.IsMemberValidForBuilderClass(parentTypeContainer, settings);

            // Assert
            result.ShouldBeFalse();
        }
    }

    public class GetCustomValueForInheritedClass : TypeBaseExtensionsTests
    {
        [Fact]
        public void Throws_When_CustomValue_Is_Null()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();

            // Act & Assert
            Action a = () => sut.GetCustomValueForInheritedClass(true, customValue: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("customValue");
        }

        [Fact]
        public void Returns_Empty_String_When_Entity_Inheritance_Is_Disabled()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();

            // Act
            var result = sut.GetCustomValueForInheritedClass(false, _ => Result.Success<GenericFormattableString>("CustomValue"));

            // Assert
            result.Value.ShouldNotBeNull();
            result.Value!.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_Instance_Is_Not_A_BaseClassContainer()
        {
            // Arrange
            var sut = new StructBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.GetCustomValueForInheritedClass(true, _ => Result.Success<GenericFormattableString>("CustomValue"));

            // Assert
            result.Value.ShouldNotBeNull();
            result.Value!.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_BaseClass_Is_Empty()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();

            // Act
            var result = sut.GetCustomValueForInheritedClass(true, _ => Result.Success<GenericFormattableString>("CustomValue"));

            // Assert
            result.Value.ShouldNotBeNull();
            result.Value!.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void Returns_CustomValue_When_BaseClass_Is_Not_Empty()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").WithBaseClass("SomeBaseClass").Build();

            // Act
            var result = sut.GetCustomValueForInheritedClass(true, _ => Result.Success<GenericFormattableString>("CustomValue"));

            // Assert
            result.Value!.ToString().ShouldBe("CustomValue");
        }
    }

    public class GetBuilderConstructorProperties : TypeBaseExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();

            // Act & Assert
            Action a = () => sut.GetBuilderConstructorProperties(context: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("context");
        }

        [Fact]
        public void Throws_When_Instance_Is_Not_A_ConstructorsContainer()
        {
            // Arrange
            var sut = new InterfaceBuilder().WithName("MyClass").Build();
            var settings = CreateSettingsForBuilder();
            var context = new BuilderContext(sut, settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.GetBuilderConstructorProperties(context);
            var x = a.ShouldThrow<ArgumentException>();
            x.ParamName.ShouldBe("context");
            x.Message.ShouldBe("Cannot get immutable builder constructor properties for type that does not have constructors (Parameter 'context')");
        }

        [Fact]
        public void Returns_All_Properties_With_Setter_Or_Initializer_When_Type_Has_Public_Parameterless_Constructor()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass")
                .AddProperties
                (
                    new PropertyBuilder().WithName("Property1").WithType(typeof(int)).WithHasSetter(true).WithHasInitializer(false),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(int)).WithHasSetter(false).WithHasInitializer(true),
                    new PropertyBuilder().WithName("Property3").WithType(typeof(int)).WithHasSetter(false).WithHasInitializer(false) // this property should be skipped, because it does not have a setter or initializer
                )
                .Build();
            var settings = CreateSettingsForBuilder();
            var context = new BuilderContext(sut, settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.GetBuilderConstructorProperties(context);

            // Assert
            result.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
        }

        [Fact]
        public void Returns_No_Properties_When_There_Is_No_Public_Constructor_With_Any_Parameters()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass")
                .AddProperties
                (
                    new PropertyBuilder().WithName("Property1").WithType(typeof(int)).WithHasSetter(true).WithHasInitializer(false),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(int)).WithHasSetter(false).WithHasInitializer(true),
                    new PropertyBuilder().WithName("Property3").WithType(typeof(int)).WithHasSetter(false).WithHasInitializer(false) // this property should be skipped, because it does not have a setter or initializer
                )
                .AddConstructors(new ConstructorBuilder().WithVisibility(Visibility.Private)) // only private constructor present :)
                .Build();
            var settings = CreateSettingsForBuilder();
            var context = new BuilderContext(sut, settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.GetBuilderConstructorProperties(context);

            // Assert
            result.Select(x => x.Name).ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Properties_From_Instance_And_BaseClass_When_IsBuilderForOverrideEntity_Is_True_And_BaseClass_Is_Filled()
        {
            var sut = CreateSut().WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("Property1").WithType(typeof(int)))
                .AddConstructors(new ConstructorBuilder()
                    .AddParameter("property1", typeof(int))
                    .AddParameter("property2", typeof(int))
                )
                .Build();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("MyBaseClass").AddProperties(new PropertyBuilder().WithName("Property2").WithType(typeof(int))).BuildTyped(),
                enableEntityInheritance: true
            );
            var context = new BuilderContext(sut, settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.GetBuilderConstructorProperties(context);

            // Assert
            result.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
        }

        [Fact]
        public void Returns_Properties_From_Instance_When_IsBuilderForOverrideEntity_Is_True_But_BaseClass_Is_Not_Filled()
        {
            var sut = CreateSut().WithName("MyClass")
                .AddProperties(
                    new PropertyBuilder().WithName("Property1").WithType(typeof(int)),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(int))
                )
                .AddConstructors(new ConstructorBuilder()
                    .AddParameter("property1", typeof(int))
                    .AddParameter("property2", typeof(int))
                )
                .Build();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: null,
                enableEntityInheritance: true
            );
            var context = new BuilderContext(sut, settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.GetBuilderConstructorProperties(context);

            // Assert
            result.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
        }

        [Fact]
        public void Returns_Properties_From_Instance_When_IsBuilderForOverrideEntity_Is_False()
        {
            var sut = CreateSut().WithName("MyClass")
                .AddProperties(
                    new PropertyBuilder().WithName("Property1").WithType(typeof(int)),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(int))
                )
                .AddConstructors(new ConstructorBuilder()
                    .AddParameter("property1", typeof(int))
                    .AddParameter("property2", typeof(int))
                )
                .Build();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                baseClass: null,
                enableEntityInheritance: true
            );
            var context = new BuilderContext(sut, settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.GetBuilderConstructorProperties(context);

            // Assert
            result.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
        }
    }

    public class GetBuilderClassFields : TypeBaseExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act & Assert
            Action a = () => _ = sut.GetBuilderClassFields(context: null!, formattableStringParser).ToArray();
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("context");
        }

        [Fact]
        public void Throws_On_Null_FormattableStringParser()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass").Build();
            var settings = CreateSettingsForBuilder();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(sut, settings, CultureInfo.InvariantCulture));

            // Act & Assert
            Action a = () => _ = sut.GetBuilderClassFields(context, formattableStringParser: null!).ToArray();
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("formattableStringParser");
        }

        [Fact]
        public void Returns_Empty_Sequence_For_Abstract_Builder()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass")
                .AddProperties
                (
                    new PropertyBuilder().WithName("Property1").WithType(typeof(int)),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(int)),
                    new PropertyBuilder().WithName("Property3").WithType(typeof(int))
                )
                .Build();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableBuilderInheritance: true,
                baseClass: null,
                validateArguments: ArgumentValidationType.IValidatableObject
            );
            var context = new PipelineContext<BuilderContext>(new BuilderContext(sut, settings, CultureInfo.InvariantCulture));
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = sut.GetBuilderClassFields(context, formattableStringParser);

            // Assert
            result.Select(x => x.Value!.Name).ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Empty_Sequence_When_Not_Using_NullChecks()
        {
            // Arrange
            var sut = CreateSut().WithName("MyClass")
                .AddProperties
                (
                    new PropertyBuilder().WithName("Property1").WithType(typeof(int)),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(int)),
                    new PropertyBuilder().WithName("Property3").WithType(typeof(int))
                )
                .Build();
            var settings = CreateSettingsForBuilder(
                addNullChecks: false,
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("MyBaseClass").BuildTyped(),
                validateArguments: ArgumentValidationType.IValidatableObject
            );
            var context = new PipelineContext<BuilderContext>(new BuilderContext(sut, settings, CultureInfo.InvariantCulture));
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = sut.GetBuilderClassFields(context, formattableStringParser);

            // Assert
            result.Select(x => x.Value!.Name).ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_On_NonAbstract_Builder_With_NullChecks_And_NonShared_OriginalValidateArguments()
        {
            // Arrange
            await InitializeParser();
            var sut = CreateSut().WithName("MyClass")
                .AddProperties
                (
                    new PropertyBuilder().WithName("Property1").WithType(typeof(string)),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(string)).WithIsNullable()
                )
                .Build();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("MyBaseClass").BuildTyped(),
                validateArguments: ArgumentValidationType.IValidatableObject
            );
            var context = new PipelineContext<BuilderContext>(new BuilderContext(sut, settings, CultureInfo.InvariantCulture));
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = sut.GetBuilderClassFields(context, formattableStringParser).ToArray();

            // Assert
            result.Select(x => x.Value!.Name).ToArray().ShouldBeEquivalentTo(new[] { "_property1" });
            result.Select(x => x.Value!.TypeName).ShouldAllBe(x => x == typeof(string).FullName);
            result.Select(x => x.Value!.IsValueType).ShouldAllBe(x => x == false);
        }

        [Fact]
        public void Returns_Correct_Result_On_NonAbstract_Builder_With_NullChecks_And_NonShared_OriginalValidateArguments_And_CustomBuilderArgumentType()
        {
            // Arrange
            await InitializeParser();
            var sut = CreateSut().WithName("MyClass")
                .AddProperties
                (
                    new PropertyBuilder().WithName("Property1").WithType(typeof(string)),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(string)),
                    new PropertyBuilder().WithName("Property3").WithType(typeof(string)).WithIsNullable()
                )
                .Build();
            var settings = CreateSettingsForBuilder(
                addNullChecks: true,
                enableNullableReferenceTypes: true,
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("MyBaseClass").BuildTyped(),
                validateArguments: ArgumentValidationType.IValidatableObject,
                typenameMappings:
                [
                    new TypenameMappingBuilder()
                        .WithSourceType(typeof(string))
                        .WithTargetType(typeof(string))
                        .AddMetadata(MetadataNames.CustomBuilderArgumentType, "MyCustomType")
                ]
            );
            var context = new PipelineContext<BuilderContext>(new BuilderContext(sut, settings, CultureInfo.InvariantCulture));
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = sut.GetBuilderClassFields(context, formattableStringParser).ToArray();

            // Assert
            result.Select(x => x.Value!.Name).ToArray().ShouldBeEquivalentTo(new[] { "_property1", "_property2" });
            result.Select(x => x.Value!.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "MyCustomType", "MyCustomType" });
            result.Select(x => x.Value!.IsValueType).ShouldAllBe(x => x == false);
        }
    }
}
