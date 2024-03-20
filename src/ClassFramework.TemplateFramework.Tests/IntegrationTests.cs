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
            .AddCsharpExpressionDumper()
            .AddClassFrameworkTemplates()
            .AddParsers()
            .AddPipelines()
            .AddScoped(_ => templateFactory)
            .AddScoped(_ => templateProviderPluginFactory)
            .AddScoped<TestCodeGenerationProvider>()
            .AddScoped<TestPipelineCodeGenerationProvider>()
            .AddScoped<ImmutableCoreBuilders>()
            .AddScoped<ImmutableCoreEntities>()
            .AddScoped<ImmutablePrivateSettersCoreBuilders>()
            .AddScoped<ImmutablePrivateSettersCoreEntities>()
            .AddScoped<ImmutableSharedValidationCoreBuilders>()
            .AddScoped<ImmutableSharedValidationCoreEntities>()
            .AddScoped<ImmutableInheritFromInterfacesCoreBuilders>()
            .AddScoped<ImmutableInheritFromInterfacesCoreEntities>()
            .AddScoped<ImmutableInheritFromInterfacesAbstractionsInterfaces>()
            .AddScoped<ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces>()
            .AddScoped<ImmutableNoToBuilderMethodCoreEntities>()
            .AddScoped<ObservableCoreBuilders>()
            .AddScoped<ObservableCoreEntities>()
            .AddScoped<TemplateFrameworkEntities>()
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
    public void Can_Generate_Code_For_Immutable_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreEntities>();
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
    public void Can_Generate_Code_For_Immutable_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreBuilders>();
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

    [Fact]
    public void Can_Generate_Code_For_Immutable_Entity_PrivateSetters_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutablePrivateSettersCoreEntities>();
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
    public void Can_Generate_Code_For_Immutable_Builder_PrivateSetters_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutablePrivateSettersCoreBuilders>();
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

    [Fact]
    public void Can_Generate_Code_For_Immutable_Entity_SharedValidation_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableSharedValidationCoreEntities>();
        var generationEnvironment = new MultipleContentBuilderEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        engine.Generate(codeGenerationProvider, generationEnvironment, codeGenerationSettings);

        // Assert
        generationEnvironment.Builder.Contents.Should().HaveCount(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Literal : LiteralBase
    {
        public Literal(string value, object? originalValue) : base(value, originalValue)
        {
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().Should().Be(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class LiteralBase
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

        public LiteralBase(string value, object? originalValue)
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
    public void Can_Generate_Code_For_Immutable_Builder_SharedValidation_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableSharedValidationCoreBuilders>();
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
    public partial class LiteralBuilder : System.ComponentModel.DataAnnotations.IValidatableObject
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Value
        {
            get;
            set;
        }

        public object? OriginalValue
        {
            get;
            set;
        }

        public LiteralBuilder(Test.Domain.LiteralBase source)
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

        public System.Collections.Generic.IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var instance = new Test.Domain.LiteralBase(Value, OriginalValue);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(instance, new System.ComponentModel.DataAnnotations.ValidationContext(instance), results, true);
            return results;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public void Can_Generate_Code_For_Immutable_Entity_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesCoreEntities>();
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
    public void Can_Generate_Code_For_Immutable_Builder_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesCoreBuilders>();
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
    public void Can_Generate_Code_For_Immutable_EntityInterface_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesAbstractionsInterfaces>();
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
    public void Can_Generate_Code_For_Immutable_BuilderInterface_InheritFromInterfaces_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces>();
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
    public void Can_Generate_Code_For_Immutable_Entity_NoToBuilderMethod_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableNoToBuilderMethodCoreEntities>();
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
    }
#nullable restore
}
");
    }

    [Fact]
    public void Can_Generate_Code_For_Observable_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ObservableCoreEntities>();
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
    public void Can_Generate_Code_For_Observable_Builder_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ObservableCoreBuilders>();
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
    public void Can_Generate_Code_For_Non_Core_Entity_With_PipelineCodeGenerationProviderBase()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TemplateFrameworkEntities>();
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

    public void Dispose()
    {
        _scope.Dispose();
        _serviceProvider.Dispose();
    }

    private sealed class TestCodeGenerationProvider : CsharpClassGeneratorCodeGenerationProviderBase
    {
        public TestCodeGenerationProvider(ICsharpExpressionDumper csharpExpressionDumper) : base(csharpExpressionDumper)
        {
        }

        public override IEnumerable<TypeBase> Model =>
        [
            new ClassBuilder()
                .WithNamespace("MyNamespace")
                .WithName("MyClass")
                .AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute)))
                .AddFields(new FieldBuilder().WithName("_myField").WithType(typeof(string)).WithIsNullable().WithReadOnly().WithDefaultValue("default value").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddEnums(new EnumerationBuilder().WithName("MyEnumeration").AddMembers(new EnumerationMemberBuilder().WithName("Value1").WithValue(0), new EnumerationMemberBuilder().WithName("Value2").WithValue(1)).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddConstructors(new ConstructorBuilder().AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))).AddParameters(new ParameterBuilder().WithName("myField").WithType(typeof(string)).WithIsNullable().AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))), new ParameterBuilder().WithName("second").WithType(typeof(bool))).AddStringCodeStatements("// code goes here", "// second line"))
                .AddMethods(new MethodBuilder().WithName("Method1").WithReturnType(typeof(string)).WithReturnTypeIsNullable().AddStringCodeStatements("// code goes here", "// second line").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).WithIsNullable().AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("return _myField;")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("_myField = value;")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))
                .AddSubClasses(new ClassBuilder().WithName("MySubClass").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))).AddProperties(new PropertyBuilder().WithName("MySubProperty").WithType(typeof(string)).AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute)))).AddSubClasses(new ClassBuilder().WithName("MySubSubClass").AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))).AddProperties(new PropertyBuilder().WithName("MySubSubProperty").WithType(typeof(string)).AddGetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddSetterCodeStatements(new StringCodeStatementBuilder().WithStatement("// sub code statement")).AddAttributes(new AttributeBuilder().WithName(typeof(RequiredAttribute))))))
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
        public TestPipelineCodeGenerationProvider(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
        {
        }

        public override string Path => string.Empty;
        public override bool RecurseOnDeleteGeneratedFiles => false;
        public override string LastGeneratedFilesFilename => string.Empty;
        public override Encoding Encoding => Encoding.UTF8;

        public override IEnumerable<TypeBase> Model =>
        [
            new InterfaceBuilder().WithName("IMyEntity").WithNamespace("MyNamespace").AddProperties(new PropertyBuilder().WithName("MySingleProperty").WithType(typeof(string)), new PropertyBuilder().WithName("MyCollectionProperty").WithType(typeof(IEnumerable<string>))).Build()
        ];

        protected override string ProjectName => "UnitTest";
        protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
        protected override Type EntityConcreteCollectionType => typeof(ReadOnlyCollection<>);
        protected override Type BuilderCollectionType => typeof(List<>);
        protected override bool CreateCodeGenerationHeader => false;
    }
}
