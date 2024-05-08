namespace ClassFramework.TemplateFramework.Tests;

public sealed class IntegrationTests : TestBase, IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IServiceScope _scope;

    public IntegrationTests()
    {
        var templateFactory = Fixture.Freeze<ITemplateFactory>();
        var templateProviderPluginFactory = Fixture.Freeze<ITemplateComponentRegistryPluginFactory>();
        var assemblyInfoContextService = Fixture.Freeze<IAssemblyInfoContextService>();
        _serviceProvider = new ServiceCollection()
            .AddTemplateFramework()
            .AddTemplateFrameworkChildTemplateProvider()
            .AddTemplateFrameworkCodeGeneration()
            .AddTemplateFrameworkRuntime()
            .AddScoped(_ => assemblyInfoContextService)
            .AddCsharpExpressionDumper()
            .AddClassFrameworkTemplates()
            .AddParsers()
            .AddPipelines()
            .AddScoped(_ => templateFactory)
            .AddScoped(_ => templateProviderPluginFactory)
            .AddScoped<TestCodeGenerationProvider>()
            .AddScoped<TestPipelineCodeGenerationProvider>()
            .AddScoped<AbstractBuilders>()
            .AddScoped<AbstractEntities>()
            .AddScoped<AbstractionsBuildersExtensions>()
            .AddScoped<AbstractionsBuildersInterfaces>()
            .AddScoped<AbstractionsInterfaces>()
            .AddScoped<AbstractNonGenericBuilders>()
            .AddScoped<ImmutableCoreBuilders>()
            .AddScoped<ImmutableCoreBuilderExtensions>()
            .AddScoped<ImmutableCoreEntities>()
            .AddScoped<ImmutablePrivateSettersCoreBuilders>()
            .AddScoped<ImmutablePrivateSettersCoreEntities>()
            .AddScoped<ImmutableInheritFromInterfacesCoreBuilders>()
            .AddScoped<ImmutableInheritFromInterfacesCoreEntities>()
            .AddScoped<ImmutableInheritFromInterfacesAbstractionsInterfaces>()
            .AddScoped<ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces>()
            .AddScoped<ImmutableNoToBuilderMethodCoreEntities>()
            .AddScoped<ObservableCoreBuilders>()
            .AddScoped<ObservableCoreEntities>()
            .AddScoped<OverrideTypeBuilders>()
            .AddScoped<OverrideTypeEntities>()
            .AddScoped<TemplateFrameworkEntities>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        _scope = _serviceProvider.CreateScope();
        templateFactory.Create(Arg.Any<Type>()).Returns(x => _scope.ServiceProvider.GetRequiredService(x.ArgAt<Type>(0)));
    }

    [Fact]
    public async Task Can_Generate_Code_For_Class()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TestCodeGenerationProvider>();
        var generationEnvironment = new MultipleContentBuilderEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace MyNamespace
{
    [System.ComponentModel.DataAnnotations.RequiredAttribute]
    public class MyClass
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        private readonly string? _myField = @""default value"";

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public new string? MyProperty
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
}
#nullable disable
");
    }

    [Fact]
    public async Task Can_Generate_Code_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TestPipelineCodeGenerationProvider>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

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
    public async Task Can_Generate_Code_For_Abstractions_BuilderInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractionsBuildersInterfaces>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders.Abstractions
{
#nullable enable
    public interface IMyAbstractionBuilder
    {
        string MyProperty
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
    public async Task Can_Generate_Code_For_Abstractions_BuilderExtensions_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractionsBuildersExtensions>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders.Extensions
{
#nullable enable
    public static partial class MyAbstractionBuilderExtensions
    {
        public static T WithMyProperty<T>(this T instance, string myProperty)
            where T : Test.Domain.Builders.Abstractions.IMyAbstractionBuilder
        {
            if (myProperty is null) throw new System.ArgumentNullException(nameof(myProperty));
            instance.MyProperty = myProperty;
            return instance;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Abstractions_Interfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractionsInterfaces>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Abstractions
{
#nullable enable
    public interface IMyAbstraction
    {
        string MyProperty
        {
            get;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

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
    public async Task Can_Generate_Code_For_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreBuilders>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

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

    [Fact]
    public async Task Can_Generate_Code_For_Builder_Extensions_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreBuilderExtensions>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Extensions
{
#nullable enable
    public static partial class LiteralBuilderExtensions
    {
        public static T WithValue<T>(this T instance, string value)
            where T : Test.Domain.Builders.ILiteralBuilder
        {
            if (value is null) throw new System.ArgumentNullException(nameof(value));
            instance.Value = value;
            return instance;
        }

        public static T WithOriginalValue<T>(this T instance, object? originalValue)
            where T : Test.Domain.Builders.ILiteralBuilder
        {
            instance.OriginalValue = originalValue;
            return instance;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Entity_PrivateSetters_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutablePrivateSettersCoreEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

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
            private set;
        }

        public object? OriginalValue
        {
            get;
            private set;
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
    public async Task Can_Generate_Code_For_Builder_PrivateSetters_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutablePrivateSettersCoreBuilders>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

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

    [Fact]
    public async Task Can_Generate_Code_For_Entity_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesCoreEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Literal : Test.Domain.Abstractions.ILiteral
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

        public Test.Abstractions.Builders.ILiteralBuilder ToBuilder()
        {
            return new Test.Domain.Builders.LiteralBuilder(this);
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Builder_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesCoreBuilders>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class LiteralBuilder : Test.Abstractions.Builders.ILiteralBuilder
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

        public LiteralBuilder(Test.Abstractions.ILiteral source)
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

        public Test.Domain.Abstractions.ILiteral Build()
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

    [Fact]
    public async Task Can_Generate_Code_For_Entity_Interface_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesAbstractionsInterfaces>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public interface ILiteral
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string Value
        {
            get;
        }

        object? OriginalValue
        {
            get;
        }

        Test.Abstractions.Builders.ILiteralBuilder ToBuilder();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_BuilderInterface_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public interface ILiteralBuilder
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        string Value
        {
            get;
            set;
        }

        object? OriginalValue
        {
            get;
            set;
        }

        Test.Abstractions.ILiteral Build();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Entity_NoToBuilderMethod_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableNoToBuilderMethodCoreEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

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
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Observable_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ObservableCoreEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Literal : System.ComponentModel.INotifyPropertyChanged
    {
        private string _value;

        private object? _originalValue;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

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
                HandlePropertyChanged(nameof(Value));
            }
        }

        public object? OriginalValue
        {
            get
            {
                return _originalValue;
            }
            set
            {
                _originalValue = value;
                HandlePropertyChanged(nameof(OriginalValue));
            }
        }

        public Literal()
        {
            _value = string.Empty;
            _originalValue = default(System.Object?);
        }

        public Test.Domain.Builders.LiteralBuilder ToBuilder()
        {
            return new Test.Domain.Builders.LiteralBuilder(this);
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Observable_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ObservableCoreBuilders>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class LiteralBuilder : System.ComponentModel.INotifyPropertyChanged
    {
        private string _value;

        private object? _originalValue;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

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
                HandlePropertyChanged(nameof(Value));
            }
        }

        public object? OriginalValue
        {
            get
            {
                return _originalValue;
            }
            set
            {
                _originalValue = value;
                HandlePropertyChanged(nameof(OriginalValue));
            }
        }

        public LiteralBuilder(Test.Domain.Literal source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _value = source.Value;
            _originalValue = source.OriginalValue;
        }

        public LiteralBuilder()
        {
            _value = string.Empty;
            SetDefaultValues();
        }

        public Test.Domain.Literal Build()
        {
            return new Test.Domain.Literal { Value = Value, OriginalValue = OriginalValue };
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

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Non_Core_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TemplateFrameworkEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassFramework.TemplateFramework
{
#nullable enable
    public partial class CsharpClassGeneratorSettings
    {
        public bool RecurseOnDeleteGeneratedFiles
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string LastGeneratedFilesFilename
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Text.Encoding Encoding
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Path
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Globalization.CultureInfo CultureInfo
        {
            get;
        }

        public bool GenerateMultipleFiles
        {
            get;
        }

        public bool SkipWhenFileExists
        {
            get;
        }

        public bool CreateCodeGenerationHeader
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string EnvironmentVersion
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string FilenameSuffix
        {
            get;
        }

        public bool EnableNullableContext
        {
            get;
        }

        public bool EnableGlobalUsings
        {
            get;
        }

        public CsharpClassGeneratorSettings(bool recurseOnDeleteGeneratedFiles, string lastGeneratedFilesFilename, System.Text.Encoding encoding, string path, System.Globalization.CultureInfo cultureInfo, bool generateMultipleFiles, bool skipWhenFileExists, bool createCodeGenerationHeader, string environmentVersion, string filenameSuffix, bool enableNullableContext, bool enableGlobalUsings)
        {
            this.RecurseOnDeleteGeneratedFiles = recurseOnDeleteGeneratedFiles;
            this.LastGeneratedFilesFilename = lastGeneratedFilesFilename;
            this.Encoding = encoding;
            this.Path = path;
            this.CultureInfo = cultureInfo;
            this.GenerateMultipleFiles = generateMultipleFiles;
            this.SkipWhenFileExists = skipWhenFileExists;
            this.CreateCodeGenerationHeader = createCodeGenerationHeader;
            this.EnvironmentVersion = environmentVersion;
            this.FilenameSuffix = filenameSuffix;
            this.EnableNullableContext = enableNullableContext;
            this.EnableGlobalUsings = enableGlobalUsings;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder ToBuilder()
        {
            return new ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder(this);
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Abstract_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractBuilders>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public abstract partial class AbstractBaseBuilder<TBuilder, TEntity> : AbstractBaseBuilder
        where TEntity : Test.Domain.AbstractBase
        where TBuilder : AbstractBaseBuilder<TBuilder, TEntity>
    {
        protected AbstractBaseBuilder(Test.Domain.AbstractBase source) : base(source)
        {
        }

        protected AbstractBaseBuilder() : base()
        {
        }

        public override Test.Domain.AbstractBase Build()
        {
            return BuildTyped();
        }

        public abstract TEntity BuildTyped();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Abstract_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public abstract partial class AbstractBase
    {
        public string MyBaseProperty
        {
            get;
        }

        protected AbstractBase(string myBaseProperty)
        {
            this.MyBaseProperty = myBaseProperty!;
        }

        public abstract Test.Domain.Builders.AbstractBaseBuilder ToBuilder();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Abstract_Non_Generic_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractNonGenericBuilders>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public abstract partial class AbstractBaseBuilder : System.ComponentModel.INotifyPropertyChanged
    {
        private string _myBaseProperty;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public string MyBaseProperty
        {
            get
            {
                return _myBaseProperty;
            }
            set
            {
                _myBaseProperty = value;
                HandlePropertyChanged(nameof(MyBaseProperty));
            }
        }

        protected AbstractBaseBuilder(Test.Domain.AbstractBase source)
        {
            _myBaseProperty = source.MyBaseProperty;
        }

        protected AbstractBaseBuilder()
        {
            _myBaseProperty = string.Empty;
            SetDefaultValues();
        }

        public abstract Test.Domain.AbstractBase Build();

        partial void SetDefaultValues();

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Override_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<OverrideTypeBuilders>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders.Types
{
#nullable enable
    public partial class MyAbstractOverrideBuilder : AbstractBaseBuilder<MyAbstractOverrideBuilder, Test.Domain.Types.MyAbstractOverride>
    {
        private string _myOverrideProperty;

        public string MyOverrideProperty
        {
            get
            {
                return _myOverrideProperty;
            }
            set
            {
                _myOverrideProperty = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(MyOverrideProperty));
            }
        }

        public MyAbstractOverrideBuilder(Test.Domain.Types.MyAbstractOverride source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _myOverrideProperty = source.MyOverrideProperty;
        }

        public MyAbstractOverrideBuilder() : base()
        {
            _myOverrideProperty = string.Empty;
            SetDefaultValues();
        }

        public override Test.Domain.Types.MyAbstractOverride BuildTyped()
        {
            return new Test.Domain.Types.MyAbstractOverride(MyOverrideProperty, MyBaseProperty);
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.Types.MyAbstractOverrideBuilder WithMyOverrideProperty(string myOverrideProperty)
        {
            if (myOverrideProperty is null) throw new System.ArgumentNullException(nameof(myOverrideProperty));
            MyOverrideProperty = myOverrideProperty;
            return this;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Override_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<OverrideTypeEntities>();
        var generationEnvironment = (MultipleContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        await engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().ContainSingle();
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Types
{
#nullable enable
    public partial class MyAbstractOverride : Test.Domain.AbstractBase
    {
        public string MyOverrideProperty
        {
            get;
        }

        public MyAbstractOverride(string myOverrideProperty, string myBaseProperty) : base(myBaseProperty)
        {
            this.MyOverrideProperty = myOverrideProperty;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override Test.Domain.Builders.AbstractBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public Test.Domain.Types.Builders.MyAbstractOverrideBuilder ToTypedBuilder()
        {
            return new Test.Domain.Types.Builders.MyAbstractOverrideBuilder(this);
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
        public override Task<IEnumerable<TypeBase>> GetModel() => Task.FromResult<IEnumerable<TypeBase>>(
        [
            new ClassBuilder()
                .WithNamespace("MyNamespace")
                .WithName("MyClass")
                .AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute)))
                .AddFields(new FieldBuilder().WithName("_myField").WithType(typeof(string)).WithIsNullable().WithReadOnly().WithDefaultValue("default value").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddEnums(new EnumerationBuilder().WithName("MyEnumeration").AddMembers(new EnumerationMemberBuilder().WithName("Value1").WithValue(0), new EnumerationMemberBuilder().WithName("Value2").WithValue(1)).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddConstructors(new ConstructorBuilder().AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))).AddParameters(new ParameterBuilder().WithName("myField").WithType(typeof(string)).WithIsNullable().AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))), new ParameterBuilder().WithName("second").WithType(typeof(bool))).AddStringCodeStatements("// code goes here", "// second line"))
                .AddMethods(new MethodBuilder().WithName("Method1").WithReturnType(typeof(string)).WithReturnTypeIsNullable().AddStringCodeStatements("// code goes here", "// second line").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().WithNew().AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("return _myField;")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("_myField = value;")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddSubClasses(new ClassBuilder().WithName("MySubClass").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))).AddProperties(new PropertyBuilder().WithName("MySubProperty").WithType(typeof(string)).AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute)))).AddSubClasses(new ClassBuilder().WithName("MySubSubClass").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))).AddProperties(new PropertyBuilder().WithName("MySubSubProperty").WithType(typeof(string)).AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))))
                .Build()
        ]);

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
        public TestPipelineCodeGenerationProvider(IPipelineService pipelineService) : base(pipelineService)
        {
        }

        public override string Path => string.Empty;
        public override bool RecurseOnDeleteGeneratedFiles => false;
        public override string LastGeneratedFilesFilename => string.Empty;
        public override Encoding Encoding => Encoding.UTF8;

        public override Task<IEnumerable<TypeBase>> GetModel() => Task.FromResult<IEnumerable<TypeBase>>(
        [
            new InterfaceBuilder()
                .WithName("IMyEntity")
                .WithNamespace("MyNamespace")
                .AddProperties(
                    new PropertyBuilder()
                        .WithName("MySingleProperty")
                        .WithType(typeof(string)),
                    new PropertyBuilder()
                        .WithName("MyCollectionProperty")
                        .WithType(typeof(IEnumerable<string>)))
                .Build()
        ]);

        protected override string ProjectName => "UnitTest";
        protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
        protected override Type EntityConcreteCollectionType => typeof(ReadOnlyCollection<>);
        protected override Type BuilderCollectionType => typeof(List<>);
        protected override bool CreateCodeGenerationHeader => false;
    }
}
