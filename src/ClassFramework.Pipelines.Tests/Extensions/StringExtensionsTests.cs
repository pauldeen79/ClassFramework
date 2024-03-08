﻿namespace ClassFramework.Pipelines.Tests.Extensions;

public class StringExtensionsTests
{
    public class MapTypeName
    {
        private const string TypeName = "MyNamespace.MyClass";

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Act & Assert
            TypeName.Invoking(x => x.MapTypeName(settings: null!))
                    .Should().Throw<ArgumentNullException>().WithParameterName("settings");
        }

        [Fact]
        public void Throws_On_Null_NewCollectionTypeName()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("MappedNamespace"))
                .Build();


            // Act & Assert
            TypeName.Invoking(x => x.MapTypeName(settings, newCollectionTypeName: null!))
                    .Should().Throw<ArgumentNullException>().WithParameterName("newCollectionTypeName");
        }

        [Fact]
        public void Throws_On_Null_AlternateTypeMetadataName()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("MappedNamespace"))
                .Build();


            // Act & Assert
            TypeName.Invoking(x => x.MapTypeName(settings, alternateTypeMetadataName: null!))
                    .Should().Throw<ArgumentNullException>().WithParameterName("alternateTypeMetadataName");
        }

        [Fact]
        public void Maps_CollectionItemType_Correctly_Without_New_Collection_TypeName()
        {
            // Arrange
            var collectionTypeName = typeof(IEnumerable<>).ReplaceGenericTypeName(TypeName);
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("MappedNamespace"))
                .Build();

            // Act
            var result = collectionTypeName.MapTypeName(settings);

            // Assert
            result.Should().Be("System.Collections.Generic.IEnumerable<MappedNamespace.MyClass>");
        }

        [Fact]
        public void Maps_CollectionItemType_Correctly_With_New_Collection_TypeName()
        {
            // Arrange
            var collectionTypeName = typeof(IEnumerable<>).ReplaceGenericTypeName(TypeName);
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("MappedNamespace"))
                .Build();

            // Act
            var result = collectionTypeName.MapTypeName(settings, typeof(List<>).WithoutGenerics());

            // Assert
            result.Should().Be("System.Collections.Generic.List<MappedNamespace.MyClass>");
        }

        [Fact]
        public void Maps_SingleType_Correctly_Using_NamespaceMapping()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("MappedNamespace"))
                .Build();

            // Act
            var result = TypeName.MapTypeName(settings);

            // Assert
            result.Should().Be("MappedNamespace.MyClass");
        }

        [Fact]
        public void Maps_SingleType_Correctly_Using_TypenameMapping()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass"))
                .Build();

            // Act
            var result = TypeName.MapTypeName(settings);

            // Assert
            result.Should().Be("MappedNamespace.MappedClass");
        }

        [Fact]
        public void Maps_SingleType_Correctly_Using_TypenameMapping_With_AlternateMetadataName_When_Not_Found()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithInheritFromInterfaces()
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass"))
                .Build();

            // Act
            var result = TypeName.MapTypeName(settings, alternateTypeMetadataName: "NonExistingName");

            // Assert
            result.Should().Be("MappedNamespace.MappedClass");
        }

        [Fact]
        public void Maps_SingleType_Correctly_Using_TypenameMapping_With_AlternateMetadataName_When_Found_But_Empty()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithInheritFromInterfaces()
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass").AddMetadata(new MetadataBuilder().WithName("MyName")))
                .Build();

            // Act
            var result = TypeName.MapTypeName(settings, alternateTypeMetadataName: "MyName");

            // Assert
            result.Should().Be("MappedNamespace.MappedClass");
        }


        [Fact]
        public void Maps_SingleType_Correctly_Using_TypenameMapping_With_AlternateMetadataName_When_Found_And_Not_Empty()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithInheritFromInterfaces()
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass").AddMetadata(new MetadataBuilder().WithName("MyName").WithValue("MappedNamespace.CustomMappedClass")))
                .Build();

            // Act
            var result = TypeName.MapTypeName(settings, alternateTypeMetadataName: "MyName");

            // Assert
            result.Should().Be("MappedNamespace.CustomMappedClass");
        }

        [Fact]
        public void Maps_GenericType_With_Mappable_Generic_Argument_Correctly_Using_NamespaceMapping()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("MappedNamespace"))
                .Build();

            // Act
            var result = $"System.Func<{TypeName}>".MapTypeName(settings);

            // Assert
            result.Should().Be("System.Func<MappedNamespace.MyClass>");
        }

        [Fact]
        public void Maps_GenericType_With_Mappable_Generic_Argument_Using_TypeNameMapping()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass"))
                .Build();

            // Act
            var result = $"System.Func<{TypeName}>".MapTypeName(settings);

            // Assert
            result.Should().Be("System.Func<MappedNamespace.MappedClass>");
        }

        [Fact]
        public void Maps_GenericType_With_Multiple_Mappable_Generic_Arguments_Using_TypeNameMapping()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass"))
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MySecondClass").WithTargetTypeName("MappedNamespace.MappedSecondClass"))
                .Build();

            // Act
            // note that you have to use <x,y> instead of <x, y> else we think it's a fully qualified typename! e.g. System.String, blablabla
            var result = $"System.Func<{TypeName},MyNamespace.MySecondClass>".MapTypeName(settings);

            // Assert
            result.Should().Be("System.Func<MappedNamespace.MappedClass,MappedNamespace.MappedSecondClass>");
        }

        [Fact]
        public void Returns_Input_Value_When_No_Mappings_Are_Present_Without_New_Collection_TypeName()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("WrongNamespace").WithTargetNamespace("MappedNamespace"))
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("WrongNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass"))
                .Build();

            // Act
            var result = TypeName.MapTypeName(settings);

            // Assert
            result.Should().Be(TypeName);
        }

        [Fact]
        public void Returns_Input_Value_When_No_Mappings_Are_Present_With_New_Collection_TypeName()
        {
            // Arrange
            var collectionTypeName = typeof(IEnumerable<>).ReplaceGenericTypeName(TypeName);
            var settings = new PipelineSettingsBuilder()
                .AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("WrongNamespace").WithTargetNamespace("MappedNamespace"))
                .AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("WrongNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass"))
                .Build();

            // Act
            var result = collectionTypeName.MapTypeName(settings, typeof(List<>).WithoutGenerics());

            // Assert
            result.Should().Be(typeof(List<>).ReplaceGenericTypeName(TypeName));
        }
    }
}
