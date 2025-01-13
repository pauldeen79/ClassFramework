namespace ClassFramework.Pipelines.Tests.Extensions;

public class PropertyExtensionsTests : TestBase<PropertyBuilder>
{
    public class GetDefaultValue : PropertyExtensionsTests
    {
        [Fact]
        public void Gets_Value_From_TypeName_When_Metadata_Is_Not_Found()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            var context = new PropertyContext(sut, new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);

            // Act
            var result = sut.GetDefaultValue(csharpExpressionDumper, sut.TypeName, context);

            // Assert
            result.Should().Be("default(System.String)");
        }

        [Fact]
        public void Gets_Value_From_MetadataValue_Literal_When_Found()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            csharpExpressionDumper.Dump(Arg.Any<IStringLiteral>(), Arg.Any<Type?>()).Returns(x => x.ArgAt<IStringLiteral>(0).Value); // note that we mock the behavior of the real csharp expression dumper here :)
            var settings = new PipelineSettingsBuilder().AddTypenameMappings(new TypenameMappingBuilder().WithSourceType(typeof(string)).WithTargetType(typeof(string)).AddMetadata(MetadataNames.CustomBuilderDefaultValue, new Literal("custom value", null))).Build();
            var context = new PropertyContext(sut, settings, CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);

            // Act
            var result = sut.GetDefaultValue(csharpExpressionDumper, sut.TypeName, context);

            // Assert
            result.Should().Be("custom value");
        }

        [Fact]
        public void Gets_Value_From_MetadataValue_Non_Literal_When_Found()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            csharpExpressionDumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns(x => x.ArgAt<object?>(0).ToStringWithNullCheck());
            var settings = new PipelineSettingsBuilder()
                .AddTypenameMappings(new TypenameMappingBuilder()
                    .WithSourceType(typeof(string))
                    .WithTargetType(typeof(string))
                    .AddMetadata(MetadataNames.CustomBuilderDefaultValue, "custom value"))
                .Build();
            var context = new PropertyContext(sut, settings, CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);

            // Act
            var result = sut.GetDefaultValue(csharpExpressionDumper, sut.TypeName, context);

            // Assert
            result.Should().Be("custom value");
        }

        [Fact]
        public void Gets_Value_From_DefaultValue_No_Literal_When_Found()
        {
            // Arrange
            var sut = CreateSut()
                .WithName("MyProperty")
                .WithType(typeof(string))
                .AddAttributes(new AttributeBuilder().WithName(typeof(DefaultValueAttribute)).AddParameters(new AttributeParameterBuilder().WithValue("custom value")))
                .Build();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            csharpExpressionDumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns(x => x.ArgAt<object?>(0).ToStringWithNullCheck());
            var settings = new PipelineSettingsBuilder().Build();
            var context = new PropertyContext(sut, settings, CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);

            // Act
            var result = sut.GetDefaultValue(csharpExpressionDumper, sut.TypeName, context);

            // Assert
            result.Should().Be("custom value");
        }

        [Fact]
        public void Gets_Value_From_DefaultValue_Literal_When_Found()
        {
            // Arrange
            var sut = CreateSut()
                .WithName("MyProperty")
                .WithType(typeof(string))
                .AddAttributes(new AttributeBuilder().WithName(typeof(DefaultValueAttribute)).AddParameters(new AttributeParameterBuilder().WithValue(new Literal("custom value", null))))
                .Build();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            csharpExpressionDumper.Dump(Arg.Any<IStringLiteral>(), Arg.Any<Type?>()).Returns(x => x.ArgAt<IStringLiteral>(0).Value); // note that we mock the behavior of the real csharp expression dumper here :)
            var settings = new PipelineSettingsBuilder().Build();
            var context = new PropertyContext(sut, settings, CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);

            // Act
            var result = sut.GetDefaultValue(csharpExpressionDumper, sut.TypeName, context);

            // Assert
            result.Should().Be("custom value");
        }
    }

    public class GetNullCheckSuffix : PropertyExtensionsTests
    {
        [Fact]
        public void Returns_Empty_String_When_AddNullChecks_Is_False()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).Build();
            var sourceModel = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", false, sourceModel);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_AddNullChecks_Is_True_But_Property_Is_Nullable()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var sourceModel = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", true, sourceModel);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_AddNullChecks_Is_True_But_Property_Is_ValueType()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).Build();
            var sourceModel = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", true, sourceModel);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_NullThrowingExpression_When_AddNullChecks_Is_True_And_Property_Is_Not_ValueType_And_Not_Nullable()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).Build();
            var sourceModel = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", true, sourceModel);

            // Assert
            result.Should().Be(" ?? throw new System.ArgumentNullException(nameof(myProperty))");
        }
    }

    public class GetInitializationName : PropertyExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderMemberName(settings: null!, CultureInfo.InvariantCulture))
               .Should().Throw<ArgumentNullException>().WithParameterName("settings");
        }

        [Fact]
        public void Throws_On_Null_CultureInfo()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var settings = new PipelineSettingsBuilder().Build();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderMemberName(settings, cultureInfo: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("cultureInfo");
        }
    }

    public class GetBuilderClassConstructorInitializer : PropertyExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderConstructorInitializer<string>(context: default!, new object(), string.Empty, string.Empty, string.Empty, formattableStringParser))
               .Should().Throw<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public void Throws_On_Null_ParentChildContext()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var settings = new PipelineSettingsBuilder().Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var context = new TestContext(settings, formatProvider);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderConstructorInitializer(context, parentChildContext: default!, string.Empty, string.Empty, string.Empty, formattableStringParser))
               .Should().Throw<ArgumentNullException>().WithParameterName("parentChildContext");
        }

        [Fact]
        public void Throws_On_Null_MappedTypeName()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var settings = new PipelineSettingsBuilder().Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var context = new TestContext(settings, formatProvider);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderConstructorInitializer(context, new object(), mappedTypeName: default!, string.Empty, string.Empty, formattableStringParser))
               .Should().Throw<ArgumentNullException>().WithParameterName("mappedTypeName");
        }

        [Fact]
        public void Throws_On_Null_NewCollectionTypeName()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var settings = new PipelineSettingsBuilder().Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var context = new TestContext(settings, formatProvider);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderConstructorInitializer(context, new object(), string.Empty, newCollectionTypeName: default!, string.Empty, formattableStringParser))
               .Should().Throw<ArgumentNullException>().WithParameterName("newCollectionTypeName");
        }

        [Fact]
        public void Throws_On_Null_FormatStringParser()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var settings = new PipelineSettingsBuilder().Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var context = new TestContext(settings, formatProvider);

            // Act & Assert
            sut.Invoking(x => x.GetBuilderConstructorInitializer(context, new object(), string.Empty, string.Empty, string.Empty, formattableStringParser: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("formattableStringParser");
        }

        [Fact]
        public void Throws_On_Null_MetadataName()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();
            var settings = new PipelineSettingsBuilder().Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var context = new TestContext(settings, formatProvider);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderConstructorInitializer(context, new object(), string.Empty, string.Empty, metadataName: null!, formattableStringParser))
               .Should().Throw<ArgumentNullException>().WithParameterName("metadataName");
        }

        private sealed class TestContext(PipelineSettings settings, IFormatProvider formatProvider) : ContextBase<string>(string.Empty, settings, formatProvider)
        {
            protected override string NewCollectionTypeName => string.Empty;
        }
    }

    public class GetBuilderArgumentTypeName : PropertyExtensionsTests
    {
        [Fact]
        public void Returns_Correct_Result_On_Collection_With_GenericArgument()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithTypeName("IReadOnlyCollection<ITypedExpression<ValidationError>>").Build();
            var settings = new PipelineSettingsBuilder()
                .AddTypenameMappings(new TypenameMappingBuilder()
                    .WithSourceTypeName("ITypedExpression")
                    .WithTargetTypeName("ITypedExpression")
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderNamespace).WithValue("Builders"))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderName).WithValue("{NoGenerics(ClassName($property.TypeName))}Builder{GenericArguments($property.TypeName, true)}"))
                ).Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var context = new TestContext(settings, formatProvider);
            var parentChildContext = new ParentChildContext<TestContext, PropertyContext>(context, new PropertyContext(sut, settings, formatProvider, sut.TypeName, string.Empty), settings);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
            formattableStringParser.Parse(Arg.Any<string>(), formatProvider, Arg.Any<object?>()).Returns(x => Result.Success<GenericFormattableString>(x.ArgAt<string>(0)));

            // Act
            var result = sut.GetBuilderArgumentTypeName(context, parentChildContext, sut.TypeName, formattableStringParser);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value!.ToString().Should().Be("IReadOnlyCollection<Builders.{NoGenerics(ClassName(GenericArguments($property.TypeName)))}Builder{GenericArguments(CollectionItemType($property.TypeName), true)}>");
        }

        private sealed class TestContext(PipelineSettings settings, IFormatProvider formatProvider) : ContextBase<string>(string.Empty, settings, formatProvider)
        {
            protected override string NewCollectionTypeName => string.Empty;
        }
    }
}
