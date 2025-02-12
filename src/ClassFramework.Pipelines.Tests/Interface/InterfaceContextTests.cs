﻿namespace ClassFramework.Pipelines.Tests.Interface;

public class InterfaceContextTests : TestBase
{
    public class Constructor : InterfaceContextTests
    {
        [Fact]
        public void Throws_On_Null_SourceModel()
        {
            // Act & Assert
            this.Invoking(_ => new InterfaceContext(sourceModel: null!, new PipelineSettingsBuilder(), CultureInfo.InvariantCulture))
                .Should().Throw<ArgumentNullException>().WithParameterName("sourceModel");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Act & Assert
            this.Invoking(_ => new InterfaceContext(sourceModel: CreateClass(), settings: null!, CultureInfo.InvariantCulture))
                .Should().Throw<ArgumentNullException>().WithParameterName("settings");
        }

        [Fact]
        public void Throws_On_Null_FormatProvider()
        {
            // Act & Assert
            this.Invoking(_ => new InterfaceContext(sourceModel: CreateClass(), new PipelineSettingsBuilder(), formatProvider: null!))
                .Should().Throw<ArgumentNullException>().WithParameterName("formatProvider");
        }
    }

    public class MapTypeName : InterfaceContextTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false).Build();
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            sut.Invoking(x => x.MapTypeName(typeName: null!))
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("typeName");
        }
    }

    public class MapAttribute : InterfaceContextTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false).Build();
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            sut.Invoking(x => x.MapAttribute(attribute: null!))
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("attribute");
        }
    }
}
