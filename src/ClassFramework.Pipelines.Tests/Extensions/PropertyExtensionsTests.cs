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
            var result = sut.GetDefaultValue(csharpExpressionDumper, false, sut.TypeName, context);

            // Assert
            result.Should().Be("default(System.String)");
        }

        [Fact]
        public void Gets_Value_From_TypeName_When_Metadata_Is_Found_But_Value_Is_Null()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().AddMetadata(MetadataNames.CustomBuilderDefaultValue, null).Build();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
            var context = new PropertyContext(sut, new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);

            // Act
            var result = sut.GetDefaultValue(csharpExpressionDumper, false, sut.TypeName, context);

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
            var result = sut.GetDefaultValue(csharpExpressionDumper, false, sut.TypeName, context);

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
            var result = sut.GetDefaultValue(csharpExpressionDumper, false, sut.TypeName, context);

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

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", false);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_AddNullChecks_Is_True_But_Property_Is_Nullable()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", true);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_AddNullChecks_Is_True_But_Property_Is_ValueType()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).Build();

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", true);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_NullThrowingExpression_When_AddNullChecks_Is_True_And_Property_Is_Not_ValueType_And_Not_Nullable()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).Build();

            // Act
            var result = sut.GetNullCheckSuffix("myProperty", true);

            // Assert
            result.Should().Be(" ?? throw new System.ArgumentNullException(nameof(myProperty))");
        }
    }

    public class GetInitializationName : PropertyExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_CultureInfo()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().Build();

            // Act & Assert
            sut.Invoking(x => x.GetBuilderMemberName(default, default, default, default, cultureInfo: null!))
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

        private sealed class TestContext : ContextBase<string>
        {
            public TestContext(PipelineSettings settings, IFormatProvider formatProvider) : base(string.Empty, settings, formatProvider)
            {
            }

            protected override string NewCollectionTypeName => string.Empty;
        }
    }
}
