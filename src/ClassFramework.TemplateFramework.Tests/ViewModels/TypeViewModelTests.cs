namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class TypeViewModelTests : TestBase<TypeViewModel>
{
    public TypeViewModelTests() : base()
    {
        // For some reason, we have to register this class, because else we get the following exception:
        // AutoFixture was unable to create an instance of type AutoFixture.Kernel.SeededRequest because the traversed object graph contains a circular reference
        // I tried a generic fix in TestBase (omitting Model property), but this makes some tests fail and I don't understand why :-(
        Fixture.Register(() => new TypeViewModel(Fixture.Freeze<ICsharpExpressionCreator>()));
    }

    public class ShouldRenderNullablePragmas : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Settings_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = null!;
            sut.Context = CreateTemplateContext();

            // Act & Assert
            sut.Invoking(x => x.ShouldRenderNullablePragmas)
               .Should().Throw<ArgumentNullException>().WithParameterName("Settings");
        }

        [Fact]
        public void Throws_When_Context_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings();
            sut.Context = null!;

            // Act & Assert
            sut.Invoking(x => x.ShouldRenderNullablePragmas)
               .Should().Throw<ArgumentNullException>().WithParameterName("Context");
        }

        [Fact]
        public void Returns_False_When_EnableNullableContext_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut();
            var settings = CreateCsharpClassGeneratorSettings(enableNullableContext: false);
            sut.Settings = settings;
            sut.Context = CreateTemplateContext();

            // Act
            var result = sut.ShouldRenderNullablePragmas;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_False_When_Context_Is_Nested_Type()
        {
            // Arrange
            var sut = CreateSut();
            var settings = CreateCsharpClassGeneratorSettings(enableNullableContext: true);
            sut.Settings = settings;
            var templateContext = Fixture.Create<ITemplateContext>();
            var parentTemplateContext = Fixture.Create<ITemplateContext>();
            templateContext.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            templateContext.ParentContext.Returns(parentTemplateContext);
            parentTemplateContext.Model.Returns(new ClassBuilder().WithName("MyParentClass").Build());
            parentTemplateContext.ParentContext.Returns(default(ITemplateContext));
            sut.Context = templateContext;

            // Act
            var result = sut.ShouldRenderNullablePragmas;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_True_When_EnableNullableContext_Is_Set_To_True_And_Context_Is_Not_Nested_Type()
        {
            // Arrange
            var sut = CreateSut();
            var settings = CreateCsharpClassGeneratorSettings(enableNullableContext: true);
            sut.Settings = settings;
            var templateContext = Fixture.Create<ITemplateContext>();
            templateContext.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
            templateContext.ParentContext.Returns(default(ITemplateContext));
            sut.Context = templateContext;

            // Act
            var result = sut.ShouldRenderNullablePragmas;

            // Assert
            result.Should().BeTrue();
        }
    }

    public class ShouldRenderNamespaceScope : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Settings_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = null!;
            sut.Model = new ClassBuilder().WithName("MyClass").Build();

            // Act & Assert
            sut.Invoking(x => x.ShouldRenderNamespaceScope)
               .Should().Throw<ArgumentNullException>().WithParameterName("Settings");
        }

        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => x.ShouldRenderNamespaceScope)
               .Should().Throw<ArgumentNullException>().WithParameterName("Model");
        }

        [Fact]
        public void Returns_False_When_GenerateMultipleFiles_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: false);
            sut.Model = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.ShouldRenderNamespaceScope;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_False_When_Namespace_Is_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: true);
            sut.Model = new ClassBuilder().WithName("MyClass").WithNamespace(string.Empty).Build();

            // Act
            var result = sut.ShouldRenderNamespaceScope;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_False_When_Namespace_Is_Not_Empty_And_GenerateMultipleFiles_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings(generateMultipleFiles: true);
            sut.Model = new ClassBuilder().WithName("MyClass").WithNamespace("MyNamespace").Build();

            // Act
            var result = sut.ShouldRenderNamespaceScope;

            // Assert
            result.Should().BeTrue();
        }
    }

    public class Name : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.Name)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Gets_Name_With_Prefix_When_Name_Is_A_Reserved_Language_Keyword()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder().WithName("delegate").Build();

            // Act
            var result = sut.Name;

            // Assert
            result.Should().Be("@delegate");
        }

        [Fact]
        public void Gets_Name_Unchanged_When_Name_Is_Not_A_Reserved_Language_Keyword()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder().WithName("MyName").Build();

            // Act
            var result = sut.Name;

            // Assert
            result.Should().Be("MyName");
        }
    }

    public class GetMemberModels : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.GetMemberModels())
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }
        
        [Fact]
        public void Includes_Fields_When_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new StructBuilder()
                .WithName("MyClass")
                .AddFields(new FieldBuilder().WithName("myField").WithType(typeof(int)))
                .Build();

            // Act
            var result = sut.GetMemberModels();

            // Assert
            result.Should().AllBeOfType<Field>().And.ContainSingle();
        }

        [Fact]
        public void Includes_Properties_When_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new InterfaceBuilder()
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)))
                .Build();

            // Act
            var result = sut.GetMemberModels();

            // Assert
            result.Should().AllBeOfType<Property>().And.ContainSingle();
        }

        [Fact]
        public void Includes_Constructors_When_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder()
                .WithName("MyClass")
                .AddConstructors(new ConstructorBuilder())
                .Build();

            // Act
            var result = sut.GetMemberModels();

            // Assert
            result.Should().AllBeOfType<Constructor>().And.ContainSingle();
        }

        [Fact]
        public void Includes_Methods_When_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new InterfaceBuilder()
                .WithName("MyClass")
                .AddMethods(new MethodBuilder().WithName("MyMethod"))
                .Build();

            // Act
            var result = sut.GetMemberModels();

            // Assert
            result.Should().AllBeOfType<Method>().And.ContainSingle();
        }

        [Fact]
        public void Includes_Enumerations_When_Present()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder()
                .WithName("MyClass")
                .AddEnums(new EnumerationBuilder().WithName("MyEnumeration"))
                .Build();

            // Act
            var result = sut.GetMemberModels();

            // Assert
            result.Should().AllBeOfType<Enumeration>().And.ContainSingle();
        }

        [Fact]
        public void Includes_Separators_Between_Items()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder()
                .WithName("MyClass")
                .AddMethods(new MethodBuilder().WithName("MyMethod"))
                .AddEnums(new EnumerationBuilder().WithName("MyEnumeration"))
                .Build();

            // Act
            var result = sut.GetMemberModels();

            // Assert
            result.Select(x => x.GetType()).Should().BeEquivalentTo([typeof(Method), typeof(NewLineModel), typeof(Enumeration)]);
        }
    }

    public class GetSubClassModels : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.GetSubClassModels())
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_Empty_Sequence_When_Model_Is_Not_Class()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new StructBuilder().WithName("MyStruct").Build();

            // Act
            var result = sut.GetSubClassModels().ToArray();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_Sequence_When_Model_Is_Class_And_SubClasses_Is_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.GetSubClassModels().ToArray();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Filled_Sequence_When_Model_Is_Class_And_SubClasses_Is_Filled()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder().WithName("MyClass").AddSubClasses(new ClassBuilder().WithName("MySubClass")).Build();

            // Act
            var result = sut.GetSubClassModels().ToArray();

            // Assert
            result.Select(x => x.GetType()).Should().BeEquivalentTo([typeof(NewLineModel), typeof(Class)]);
        }
    }

    public class ContainerType : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.ContainerType)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Theory]
        [InlineData("class", false, "class")]
        [InlineData("class", true, "record")]
        [InlineData("struct", false, "struct")]
        [InlineData("struct", true, "record struct")]
        [InlineData("interface", false, "interface")]
        //note that the combination 'interface' and 'record' is not possible. the InterfaceBuilder does not have a property named Record.
        public void Returns_Correct_Result_When_Model_Is(string modelType, bool record, string expectedResult)
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = modelType switch
            {
                "class" => new ClassBuilder().WithName("MyClass").WithRecord(record).Build(),
                "interface" => new InterfaceBuilder().WithName("IMyInterface").Build(),
                "struct" => new StructBuilder().WithName("MyStruct").WithRecord(record).Build(),
                _ => throw new NotSupportedException("Only 'class', 'interface' and 'struct' are supported as parentModelType")
            };

            // Act
            var result = sut.ContainerType;

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Throws_On_Unknown_Model_Type()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = Fixture.Create<IType>();

            // Act & Assert
            sut.Invoking(x => _ = x.ContainerType)
               .Should().Throw<NotSupportedException>();
        }
    }

    public class InheritedClasses : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Model_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = null!;

            // Act & Assert
            sut.Invoking(x => _ = x.InheritedClasses)
               .Should().Throw<ArgumentNullException>()
               .WithParameterName("Model");
        }

        [Fact]
        public void Returns_Empty_String_On_Model_Of_Type_Interface_Without_Interfaces()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new InterfaceBuilder().WithName("IMyInterface").Build();

            // Act
            var result = sut.InheritedClasses;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_On_Model_Of_Type_Class_Without_Interfaces()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.InheritedClasses;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Interfaces_Separated_And_Prefixed_On_Model_Of_Type_Interface_With_Interfaces()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new InterfaceBuilder().WithName("IMyInterface").AddInterfaces("IBase1", "IBase2").Build();

            // Act
            var result = sut.InheritedClasses;

            // Assert
            result.Should().Be(" : IBase1, IBase2");
        }

        [Fact]
        public void Returns_BaseClass_And_Interfaces_Separated_And_Prefixed_On_Model_Of_Type_Class_With_BaseClass_And_Interfaces()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder().WithName("MyClass").AddInterfaces("IBase1", "IBase2").WithBaseClass("MyBase").Build();

            // Act
            var result = sut.InheritedClasses;

            // Assert
            result.Should().Be(" : MyBase, IBase1, IBase2");
        }

        [Fact]
        public void Returns_BaseClass_Prefixed_On_Model_Of_Type_Class_With_BaseClass()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new ClassBuilder().WithName("MyClass").AddInterfaces("IBase1", "IBase2").Build();

            // Act
            var result = sut.InheritedClasses;

            // Assert
            result.Should().Be(" : IBase1, IBase2");
        }
    }

    public class FilenamePrefix : TypeViewModelTests
    {
        [Fact]
        public void Throws_When_Settings_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = null!;
            sut.Model = new ClassBuilder().WithName("MyClass").Build();

            // Act & Assert
            sut.Invoking(x => x.FilenamePrefix)
               .Should().Throw<ArgumentNullException>().WithParameterName("Settings");
        }

        [Fact]
        public void Returns_Empty_String_When_Path_Is_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings(path: string.Empty);
            sut.Model = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.FilenamePrefix;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Path_With_DirectorySeparator_When_Path_Is_Not_Empty()
        {
            // Arrange
            var sut = CreateSut();
            sut.Settings = CreateCsharpClassGeneratorSettings(path: "SubDir");
            sut.Model = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.FilenamePrefix;

            // Assert
            result.Should().Be($"SubDir{Path.DirectorySeparatorChar}");
        }
    }
}
