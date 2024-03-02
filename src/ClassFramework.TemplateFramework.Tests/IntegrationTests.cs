namespace ClassFramework.TemplateFramework.Tests;

public sealed class IntegrationTests : TestBase, IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IServiceScope _scope;

    public IntegrationTests()
    {
        var templateFactory = Fixture.Freeze<ITemplateFactory>();
        var templateProviderPluginFactory = Fixture.Freeze<ITemplateComponentRegistryPluginFactory>();
        _serviceProvider = new ServiceCollection()
            .AddTemplateFramework()
            .AddTemplateFrameworkChildTemplateProvider()
            .AddTemplateFrameworkCodeGeneration()
            .AddCsharpExpressionCreator()
            .AddClassFrameworkTemplates()
            .AddParsers()
            .AddPipelines()
            .AddScoped(_ => templateFactory)
            .AddScoped(_ => templateProviderPluginFactory)
            .AddScoped<TestCodeGenerationProvider>()
            .AddScoped<TestPipelineCodeGenerationProvider>()
            .AddScoped<CoreBuilders>()
            .AddScoped<CoreEntities>()
            .BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();
        templateFactory.Create(Arg.Any<Type>()).Returns(x => _scope.ServiceProvider.GetRequiredService(x.ArgAt<Type>(0)));
    }

    [Fact]
    public void Can_Generate_Code_For_Class()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TestCodeGenerationProvider>();
        var generationEnvironment = new MultipleContentBuilderEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNamespace
{
#nullable enable
    [System.ComponentModel.DataAnnotations.RequiredAttribute]
    public class MyClass
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        private readonly string? _myField = @""default value"";

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string? MyProperty
        {
            get
            {
                return _myField;
            }
            set
            {
                _myField = value;
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public MyClass([System.ComponentModel.DataAnnotations.RequiredAttribute] string? myField, bool second)
        {
            // code goes here
            // second line
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string? Method1()
        {
            // code goes here
            // second line
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public enum MyEnumeration
        {
            Value1 = 0,
            Value2 = 1,
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public class MySubClass
        {
            [System.ComponentModel.DataAnnotations.RequiredAttribute]
            public string MySubProperty
            {
                get
                {
                    // sub code statement
                }
                set
                {
                    // sub code statement
                }
            }

            [System.ComponentModel.DataAnnotations.RequiredAttribute]
            public class MySubSubClass
            {
                [System.ComponentModel.DataAnnotations.RequiredAttribute]
                public string MySubSubProperty
                {
                    get
                    {
                        // sub code statement
                    }
                    set
                    {
                        // sub code statement
                    }
                }
            }
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public void Can_Generate_Code_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TestPipelineCodeGenerationProvider>();
        var generationEnvironment = new MultipleContentBuilderEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNamespace
{
#nullable enable
    public interface IMyEntity
    {
        string MySingleProperty
        {
            get;
            set;
        }

        System.Collections.Generic.IEnumerable<string> MyCollectionProperty
        {
            get;
            set;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public void Can_Generate_Code_For_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CoreEntities>();
        var generationEnvironment = new MultipleContentBuilderEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Literal
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Value
        {
            get;
        }

        public object? OriginalValue
        {
            get;
        }

        public Literal(string value, object? originalValue)
        {
            this.Value = value;
            this.OriginalValue = originalValue;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public Test.Domain.Builders.LiteralBuilder ToBuilder()
        {
            return new Test.Domain.Builders.LiteralBuilder(this);
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public void Can_Generate_Code_For_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CoreBuilders>();
        var generationEnvironment = new MultipleContentBuilderEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class LiteralBuilder
    {
        private string _value;

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        public object? OriginalValue
        {
            get;
            set;
        }

        public LiteralBuilder(Test.Domain.Literal source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _value = source.Value;
            OriginalValue = source.OriginalValue;
        }

        public LiteralBuilder()
        {
            _value = string.Empty;
            SetDefaultValues();
        }

        public Test.Domain.Literal Build()
        {
            return new Test.Domain.Literal(Value, OriginalValue);
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.LiteralBuilder WithValue(string value)
        {
            if (value is null) throw new System.ArgumentNullException(nameof(value));
            Value = value;
            return this;
        }

        public Test.Domain.Builders.LiteralBuilder WithOriginalValue(object? originalValue)
        {
            OriginalValue = originalValue;
            return this;
        }
    }
#nullable restore
}
");
    }

    public void Dispose()
    {
        _scope.Dispose();
        _serviceProvider.Dispose();
    }

    private sealed class TestCodeGenerationProvider : CsharpClassGeneratorCodeGenerationProviderBase
    {
        public TestCodeGenerationProvider(ICsharpExpressionCreator csharpExpressionCreator) : base(csharpExpressionCreator)
        {
        }

        public override IEnumerable<TypeBase> Model =>
        [
            new ClassBuilder()
                .WithNamespace("MyNamespace")
                .WithName("MyClass")
                .AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!))
                .AddFields(new FieldBuilder().WithName("_myField").WithType(typeof(string)).WithIsNullable().WithReadOnly().WithDefaultValue("default value").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)))
                .AddEnums(new EnumerationBuilder().WithName("MyEnumeration").AddMembers(new EnumerationMemberBuilder().WithName("Value1").WithValue(0), new EnumerationMemberBuilder().WithName("Value2").WithValue(1)).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)))
                .AddConstructors(new ConstructorBuilder().AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)).AddParameters(new ParameterBuilder().WithName("myField").WithType(typeof(string)).WithIsNullable().AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)), new ParameterBuilder().WithName("second").WithType(typeof(bool))).AddStringCodeStatements("// code goes here", "// second line"))
                .AddMethods(new MethodBuilder().WithName("Method1").WithReturnType(typeof(string)).WithReturnTypeIsNullable().AddStringCodeStatements("// code goes here", "// second line").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)))
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("return _myField;")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("_myField = value;")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)))
                .AddSubClasses(new ClassBuilder().WithName("MySubClass").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)).AddProperties(new PropertyBuilder().WithName("MySubProperty").WithType(typeof(string)).AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!))).AddSubClasses(new ClassBuilder().WithName("MySubSubClass").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)).AddProperties(new PropertyBuilder().WithName("MySubSubProperty").WithType(typeof(string)).AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute).FullName!)))))
                .Build()
        ];

        public override string Path => string.Empty;
        public override bool RecurseOnDeleteGeneratedFiles => false;
        public override string LastGeneratedFilesFilename => string.Empty;
        public override Encoding Encoding => Encoding.UTF8;

        public override CsharpClassGeneratorSettings Settings => new CsharpClassGeneratorSettingsBuilder()
            .WithPath(Path)
            .WithRecurseOnDeleteGeneratedFiles(RecurseOnDeleteGeneratedFiles)
            .WithLastGeneratedFilesFilename(LastGeneratedFilesFilename)
            .WithEncoding(Encoding)
            .WithCultureInfo(CultureInfo.InvariantCulture)
            .WithEnableNullableContext()
            .Build();
    }

    private sealed class TestPipelineCodeGenerationProvider : CsharpClassGeneratorPipelineCodeGenerationProviderBase
    {
        public TestPipelineCodeGenerationProvider(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
        {
        }

        public override string Path => string.Empty;
        public override bool RecurseOnDeleteGeneratedFiles => false;
        public override string LastGeneratedFilesFilename => string.Empty;
        public override Encoding Encoding => Encoding.UTF8;

        public override IEnumerable<TypeBase> Model => new[]
        {
            new InterfaceBuilder().WithName("IMyEntity").WithNamespace("MyNamespace").AddProperties(new PropertyBuilder().WithName("MySingleProperty").WithType(typeof(string)), new PropertyBuilder().WithName("MyCollectionProperty").WithType(typeof(IEnumerable<string>))).Build()
        };

        protected override string ProjectName => "UnitTest";
        protected override Type RecordCollectionType => typeof(IReadOnlyCollection<>);
        protected override Type RecordConcreteCollectionType => typeof(ReadOnlyCollection<>);
        protected override Type BuilderCollectionType => typeof(List<>);
        protected override bool CreateCodeGenerationHeader => false;
    }
}
