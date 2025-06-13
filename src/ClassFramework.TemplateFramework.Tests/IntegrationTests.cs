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
            .AddExpressionEvaluator()
            .AddClassFrameworkPipelines()
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
            .AddScoped<ImmutableCoreErrorBuilders>()
            .AddScoped<ImmutableCoreModelErrorBuilders>()
            .AddScoped<ImmutableCoreBaseClassErrorBuilders>()
            .AddScoped<ImmutablePrivateSettersCoreBuilders>()
            .AddScoped<ImmutablePrivateSettersCoreEntities>()
            .AddScoped<ImmutableInheritFromInterfacesCoreBuilders>()
            .AddScoped<ImmutableInheritFromInterfacesCoreEntities>()
            .AddScoped<ImmutableInheritFromInterfacesAbstractionsInterfaces>()
            .AddScoped<ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces>()
            .AddScoped<ImmutableUseBuilderAbstractionsTypeConversionAbstractionsInterfaces>()
            .AddScoped<ImmutableUseBuilderAbstractionsTypeConversionAbstractionsBuilderInterfaces>()
            .AddScoped<ImmutableNoToBuilderMethodCoreEntities>()
            .AddScoped<MappedTypeBuilders>()
            .AddScoped<MappedTypeEntities>()
            .AddScoped<ObservableCoreBuilders>()
            .AddScoped<ObservableCoreEntities>()
            .AddScoped<OverrideTypeBuilders>()
            .AddScoped<OverrideTypeEntities>()
            .AddScoped<TemplateFrameworkBuilders>()
            .AddScoped<TemplateFrameworkEntities>()
            .AddScoped<CrossCuttingAbstractBuilders>()
            .AddScoped<CrossCuttingAbstractEntities>()
            .AddScoped<CrossCuttingAbstractionsBuildersInterfaces>()
            .AddScoped<CrossCuttingAbstractionsInterfaces>()
            .AddScoped<CrossCuttingAbstractNonGenericBuilders>()
            .AddScoped<CrossCuttingOverrideBuilders>()
            .AddScoped<CrossCuttingOverrideEntities>()
            .AddScoped<MultipleInterfacesAbstractBuilders>()
            .AddScoped<MultipleInterfacesAbstractEntities>()
            .AddScoped<MultipleInterfacesAbstractionsBuildersExtensions>()
            .AddScoped<MultipleInterfacesAbstractionsBuildersInterfaces>()
            .AddScoped<MultipleInterfacesAbstractionsInterfaces>()
            .AddScoped<MultipleInterfacesOverrideBuilders>()
            .AddScoped<MultipleInterfacesOverrideEntities>()
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
        var generationEnvironment = new MultipleStringContentBuilderEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TestPipelineCodeGenerationProvider>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Abstractions_BuilderInterfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractionsBuildersInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders.Abstractions
{
#nullable enable
    public partial interface IMyAbstractionBuilder
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
    public async Task Can_Generate_Code_For_Abstractions_BuilderExtensions()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractionsBuildersExtensions>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Abstractions_Interfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractionsInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Abstractions
{
#nullable enable
    public partial interface IMyAbstraction
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
    public async Task Can_Generate_Code_For_Entity()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Generic<T>
    {
        public T MyProperty
        {
            get;
        }

        public Generic(T myProperty)
        {
            this.MyProperty = myProperty;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public Test.Domain.Builders.GenericBuilder<T> ToBuilder()
        {
            return new Test.Domain.Builders.GenericBuilder<T>(this);
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Builder()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class GenericBuilder<T>
    {
        private T _myProperty;

        public T MyProperty
        {
            get
            {
                return _myProperty;
            }
            set
            {
                _myProperty = value;
            }
        }

        public GenericBuilder(Test.Domain.Generic<T> source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _myProperty = source.MyProperty;
        }

        public GenericBuilder()
        {
            _myProperty = default(T)!;
            SetDefaultValues();
        }

        public Test.Domain.Generic<T> Build()
        {
            return new Test.Domain.Generic<T>(MyProperty);
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.GenericBuilder<T> WithMyProperty(T myProperty)
        {
            MyProperty = myProperty;
            return this;
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Get_Useful_ErrorMessage_When_Generating_Code_For_Builder_Goes_Wrong()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreErrorBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.ErrorMessage.ShouldBe("Could not create builders. See the inner results for more details.");
        result.InnerResults.Count.ShouldBe(1);
        result.InnerResults.First().Status.ShouldBe(ResultStatus.Error);
        result.InnerResults.First().ErrorMessage.ShouldBe("An error occured while processing the pipeline. See the inner results for more details.");
        result.InnerResults.First().InnerResults.Count.ShouldBe(1);
        result.InnerResults.First().InnerResults.First().Status.ShouldBe(ResultStatus.Invalid);
        result.InnerResults.First().InnerResults.First().ErrorMessage.ShouldBe("Unknown property on type System.Object: Kaboom");
    }

    [Fact]
    public async Task Can_Get_Useful_ErrorMessage_When_Creating_Model_For_Builder_Goes_Wrong()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreModelErrorBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.ErrorMessage.ShouldBe("Kaboom");
    }

    [Fact]
    public async Task Can_Get_Useful_ErrorMessage_When_Creating_BaseClass_For_Builder_Goes_Wrong()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreBaseClassErrorBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.ErrorMessage.ShouldBe("Could not create builders. See the inner results for more details.");
        result.InnerResults.Count.ShouldBe(1);
        result.InnerResults.First().ErrorMessage.ShouldBe("Could not create settings, see inner results for details");
        result.InnerResults.First().InnerResults.Count.ShouldBe(1);
        result.InnerResults.First().InnerResults.First().ErrorMessage.ShouldBe("Could not get base class, see inner results for details");
        result.InnerResults.First().InnerResults.First().InnerResults.Count.ShouldBe(1);
        result.InnerResults.First().InnerResults.First().InnerResults.First().ErrorMessage.ShouldBe("Kaboom");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Builder_Extensions()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableCoreBuilderExtensions>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Extensions
{
#nullable enable
    public static partial class GenericBuilderExtensions
    {
        public static T WithMyProperty<T>(this T instance, T myProperty)
            where T : Test.Domain.Builders.IGenericBuilder<T>
        {
            instance.MyProperty = myProperty;
            return instance;
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Entity_PrivateSetters()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutablePrivateSettersCoreEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Generic<T>
    {
        public T MyProperty
        {
            get;
            private set;
        }

        public Generic(T myProperty)
        {
            this.MyProperty = myProperty;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public Test.Domain.Builders.GenericBuilder<T> ToBuilder()
        {
            return new Test.Domain.Builders.GenericBuilder<T>(this);
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Builder_PrivateSetters()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutablePrivateSettersCoreBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class GenericBuilder<T>
    {
        private T _myProperty;

        public T MyProperty
        {
            get
            {
                return _myProperty;
            }
            set
            {
                _myProperty = value;
            }
        }

        public GenericBuilder(Test.Domain.Generic<T> source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _myProperty = source.MyProperty;
        }

        public GenericBuilder()
        {
            _myProperty = default(T)!;
            SetDefaultValues();
        }

        public Test.Domain.Generic<T> Build()
        {
            return new Test.Domain.Generic<T>(MyProperty);
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.GenericBuilder<T> WithMyProperty(T myProperty)
        {
            MyProperty = myProperty;
            return this;
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Entity_InheritFromInterfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesCoreEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Generic<T> : Test.Domain.Abstractions.IGeneric
    {
        public T MyProperty
        {
            get;
        }

        public Generic(T myProperty)
        {
            this.MyProperty = myProperty;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public Test.Abstractions.Builders.IGenericBuilder<T> ToBuilder()
        {
            return new Test.Domain.Builders.GenericBuilder<T>(this);
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Builder_InheritFromInterfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesCoreBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class GenericBuilder<T> : Test.Abstractions.Builders.IGenericBuilder
    {
        private T _myProperty;

        public T MyProperty
        {
            get
            {
                return _myProperty;
            }
            set
            {
                _myProperty = value;
            }
        }

        public GenericBuilder(Test.Abstractions.IGeneric<T> source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _myProperty = source.MyProperty;
        }

        public GenericBuilder()
        {
            _myProperty = default(T)!;
            SetDefaultValues();
        }

        public Test.Domain.Abstractions.IGeneric<T> Build()
        {
            return new Test.Domain.Generic<T>(MyProperty);
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.GenericBuilder<T> WithMyProperty(T myProperty)
        {
            MyProperty = myProperty;
            return this;
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Entity_Interface_InheritFromInterfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesAbstractionsInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface IGeneric<T>
    {
        T MyProperty
        {
            get;
        }

        Test.Abstractions.Builders.IGenericBuilder<T> ToBuilder();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface ILiteral
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
    public async Task Can_Generate_Code_For_Builder_Interface_InheritFromInterfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface IGenericBuilder<T>
    {
        T MyProperty
        {
            get;
            set;
        }

        Test.Abstractions.IGeneric<T> Build();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface ILiteralBuilder
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
    public async Task Can_Generate_Code_For_Entity_Interface_UseBuilderAbstractionsTypeConversion()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableUseBuilderAbstractionsTypeConversionAbstractionsInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface IGeneric<T>
    {
        T MyProperty
        {
            get;
        }

        Test.Domain.Abstractions.Builders.IGenericBuilder<T> ToBuilder();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface ILiteral
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

        Test.Domain.Abstractions.Builders.ILiteralBuilder ToBuilder();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Builder_Interface_UseBuilderAbstractionsTypeConversion()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableUseBuilderAbstractionsTypeConversionAbstractionsBuilderInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface IGenericBuilder<T>
    {
        T MyProperty
        {
            get;
            set;
        }

        Test.Domain.Abstractions.IGeneric<T> Build();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Abstractions
{
#nullable enable
    public partial interface ILiteralBuilder
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

        Test.Domain.Abstractions.ILiteral Build();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Entity_No_ToBuilderMethod()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ImmutableNoToBuilderMethodCoreEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Generic<T>
    {
        public T MyProperty
        {
            get;
        }

        public Generic(T myProperty)
        {
            this.MyProperty = myProperty;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
    public async Task Can_Generate_Code_For_Observable_Entity()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ObservableCoreEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class Generic<T> : System.ComponentModel.INotifyPropertyChanged
    {
        private T _myProperty;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public T MyProperty
        {
            get
            {
                return _myProperty;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<T>.Default.Equals(_myProperty!, value!);
                _myProperty = value;
                if (hasChanged) HandlePropertyChanged(nameof(MyProperty));
            }
        }

        public Generic()
        {
            _myProperty = default(T)!;
        }

        public Test.Domain.Builders.GenericBuilder<T> ToBuilder()
        {
            return new Test.Domain.Builders.GenericBuilder<T>(this);
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_value!, value!);
                _value = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Value));
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
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(_originalValue!, value!);
                _originalValue = value;
                if (hasChanged) HandlePropertyChanged(nameof(OriginalValue));
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
    public async Task Can_Generate_Code_For_Observable_Builder()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<ObservableCoreBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class GenericBuilder<T> : System.ComponentModel.INotifyPropertyChanged
    {
        private T _myProperty;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public T MyProperty
        {
            get
            {
                return _myProperty;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<T>.Default.Equals(_myProperty!, value!);
                _myProperty = value;
                if (hasChanged) HandlePropertyChanged(nameof(MyProperty));
            }
        }

        public GenericBuilder(Test.Domain.Generic<T> source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _myProperty = source.MyProperty;
        }

        public GenericBuilder()
        {
            _myProperty = default(T)!;
            SetDefaultValues();
        }

        public Test.Domain.Generic<T> Build()
        {
            return new Test.Domain.Generic<T> { MyProperty = MyProperty };
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.GenericBuilder<T> WithMyProperty(T myProperty)
        {
            MyProperty = myProperty;
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
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
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
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_value!, value!);
                _value = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Value));
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
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(_originalValue!, value!);
                _originalValue = value;
                if (hasChanged) HandlePropertyChanged(nameof(OriginalValue));
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
    public async Task Can_Generate_Code_For_Non_Core_Entity()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TemplateFrameworkEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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

        [System.ComponentModel.DefaultValueAttribute(true)]
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
    public async Task Can_Generate_Code_For_Non_Core_Builders()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<TemplateFrameworkBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassFramework.TemplateFramework.Builders
{
#nullable enable
    public partial class CsharpClassGeneratorSettingsBuilder
    {
        private string _lastGeneratedFilesFilename;

        private System.Text.Encoding _encoding;

        private string _path;

        private System.Globalization.CultureInfo _cultureInfo;

        private string _environmentVersion;

        private string _filenameSuffix;

        public bool RecurseOnDeleteGeneratedFiles
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string LastGeneratedFilesFilename
        {
            get
            {
                return _lastGeneratedFilesFilename;
            }
            set
            {
                _lastGeneratedFilesFilename = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Text.Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                _encoding = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Globalization.CultureInfo CultureInfo
        {
            get
            {
                return _cultureInfo;
            }
            set
            {
                _cultureInfo = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool GenerateMultipleFiles
        {
            get;
            set;
        }

        public bool SkipWhenFileExists
        {
            get;
            set;
        }

        public bool CreateCodeGenerationHeader
        {
            get;
            set;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string EnvironmentVersion
        {
            get
            {
                return _environmentVersion;
            }
            set
            {
                _environmentVersion = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string FilenameSuffix
        {
            get
            {
                return _filenameSuffix;
            }
            set
            {
                _filenameSuffix = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        public bool EnableNullableContext
        {
            get;
            set;
        }

        public bool EnableGlobalUsings
        {
            get;
            set;
        }

        public CsharpClassGeneratorSettingsBuilder(ClassFramework.TemplateFramework.CsharpClassGeneratorSettings source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            RecurseOnDeleteGeneratedFiles = source.RecurseOnDeleteGeneratedFiles;
            _lastGeneratedFilesFilename = source.LastGeneratedFilesFilename;
            _encoding = source.Encoding;
            _path = source.Path;
            _cultureInfo = source.CultureInfo;
            GenerateMultipleFiles = source.GenerateMultipleFiles;
            SkipWhenFileExists = source.SkipWhenFileExists;
            CreateCodeGenerationHeader = source.CreateCodeGenerationHeader;
            _environmentVersion = source.EnvironmentVersion;
            _filenameSuffix = source.FilenameSuffix;
            EnableNullableContext = source.EnableNullableContext;
            EnableGlobalUsings = source.EnableGlobalUsings;
        }

        public CsharpClassGeneratorSettingsBuilder()
        {
            _lastGeneratedFilesFilename = string.Empty;
            _encoding = default(System.Text.Encoding)!;
            _path = string.Empty;
            _cultureInfo = default(System.Globalization.CultureInfo)!;
            GenerateMultipleFiles = true;
            _environmentVersion = string.Empty;
            _filenameSuffix = string.Empty;
            SetDefaultValues();
        }

        public ClassFramework.TemplateFramework.CsharpClassGeneratorSettings Build()
        {
            return new ClassFramework.TemplateFramework.CsharpClassGeneratorSettings(RecurseOnDeleteGeneratedFiles, LastGeneratedFilesFilename, Encoding, Path, CultureInfo, GenerateMultipleFiles, SkipWhenFileExists, CreateCodeGenerationHeader, EnvironmentVersion, FilenameSuffix, EnableNullableContext, EnableGlobalUsings);
        }

        partial void SetDefaultValues();

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithRecurseOnDeleteGeneratedFiles(bool recurseOnDeleteGeneratedFiles = true)
        {
            RecurseOnDeleteGeneratedFiles = recurseOnDeleteGeneratedFiles;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithLastGeneratedFilesFilename(string lastGeneratedFilesFilename)
        {
            if (lastGeneratedFilesFilename is null) throw new System.ArgumentNullException(nameof(lastGeneratedFilesFilename));
            LastGeneratedFilesFilename = lastGeneratedFilesFilename;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEncoding(System.Text.Encoding encoding)
        {
            if (encoding is null) throw new System.ArgumentNullException(nameof(encoding));
            Encoding = encoding;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithPath(string path)
        {
            if (path is null) throw new System.ArgumentNullException(nameof(path));
            Path = path;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithCultureInfo(System.Globalization.CultureInfo cultureInfo)
        {
            if (cultureInfo is null) throw new System.ArgumentNullException(nameof(cultureInfo));
            CultureInfo = cultureInfo;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithGenerateMultipleFiles(bool generateMultipleFiles = true)
        {
            GenerateMultipleFiles = generateMultipleFiles;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithSkipWhenFileExists(bool skipWhenFileExists = true)
        {
            SkipWhenFileExists = skipWhenFileExists;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithCreateCodeGenerationHeader(bool createCodeGenerationHeader = true)
        {
            CreateCodeGenerationHeader = createCodeGenerationHeader;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEnvironmentVersion(string environmentVersion)
        {
            if (environmentVersion is null) throw new System.ArgumentNullException(nameof(environmentVersion));
            EnvironmentVersion = environmentVersion;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithFilenameSuffix(string filenameSuffix)
        {
            if (filenameSuffix is null) throw new System.ArgumentNullException(nameof(filenameSuffix));
            FilenameSuffix = filenameSuffix;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEnableNullableContext(bool enableNullableContext = true)
        {
            EnableNullableContext = enableNullableContext;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEnableGlobalUsings(bool enableGlobalUsings = true)
        {
            EnableGlobalUsings = enableGlobalUsings;
            return this;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Abstract_Builder()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public abstract partial class AbstractBaseBuilder<TBuilder, TEntity, T> : AbstractBaseBuilder<T>
        where TEntity : Test.Domain.AbstractBase<T>
        where TBuilder : AbstractBaseBuilder<TBuilder, TEntity, T>
    {
        protected AbstractBaseBuilder(Test.Domain.AbstractBase<T> source) : base(source)
        {
        }

        protected AbstractBaseBuilder() : base()
        {
        }

        public override Test.Domain.AbstractBase<T> Build()
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
    public async Task Can_Generate_Code_For_Abstract_Entity()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public abstract partial class AbstractBase<T>
    {
        protected AbstractBase()
        {
        }

        public abstract Test.Domain.Builders.AbstractBaseBuilder<T> ToBuilder();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Abstract_Non_Generic_Builder()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<AbstractNonGenericBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_myBaseProperty!, value!);
                _myBaseProperty = value;
                if (hasChanged) HandlePropertyChanged(nameof(MyBaseProperty));
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
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public abstract partial class AbstractBaseBuilder<T> : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected AbstractBaseBuilder(Test.Domain.AbstractBase<T> source)
        {
        }

        protected AbstractBaseBuilder()
        {
            SetDefaultValues();
        }

        public abstract Test.Domain.AbstractBase<T> Build();

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
    public async Task Can_Generate_Code_For_Override_Builder()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<OverrideTypeBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_myOverrideProperty!, value!);
                _myOverrideProperty = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(MyOverrideProperty));
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
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders.Types
{
#nullable enable
    public partial class MyGenericOverrideBuilder<T> : AbstractBaseBuilder<MyGenericOverrideBuilder<T>, Test.Domain.Types.MyGenericOverride<T>>, Test.Domain.Abstractions.Builders.IAbstractBaseBuilder
    {
        private T _myOverrideProperty;

        public T MyOverrideProperty
        {
            get
            {
                return _myOverrideProperty;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<T>.Default.Equals(_myOverrideProperty!, value!);
                _myOverrideProperty = value;
                if (hasChanged) HandlePropertyChanged(nameof(MyOverrideProperty));
            }
        }

        public MyGenericOverrideBuilder(Test.Domain.Types.MyGenericOverride<T> source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _myOverrideProperty = source.MyOverrideProperty;
        }

        public MyGenericOverrideBuilder() : base()
        {
            _myOverrideProperty = default(T)!;
            SetDefaultValues();
        }

        public override Test.Domain.Types.MyGenericOverride<T> BuildTyped()
        {
            return new Test.Domain.Types.MyGenericOverride<T>(MyOverrideProperty);
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.Types.MyGenericOverrideBuilder<T> WithMyOverrideProperty(T myOverrideProperty)
        {
            MyOverrideProperty = myOverrideProperty;
            return this;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Override_Entity()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<OverrideTypeEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
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
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Types
{
#nullable enable
    public partial class MyGenericOverride<T> : Test.Domain.AbstractBase, Test.Domain.AbstractBase<T>
    {
        public T MyOverrideProperty
        {
            get;
        }

        public MyGenericOverride(T myOverrideProperty) : base(myBaseProperty)
        {
            this.MyOverrideProperty = myOverrideProperty;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override Test.Domain.Builders.AbstractBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public Test.Domain.Types.Builders.MyGenericOverrideBuilder<T> ToTypedBuilder()
        {
            return new Test.Domain.Types.Builders.MyGenericOverrideBuilder<T>(this);
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Entity_With_TypenameMapping()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MappedTypeEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain
{
#nullable enable
    public partial class MyClass
    {
        public ClassFramework.TemplateFramework.Tests.SomeNamespace.IMyMappedType MyProperty
        {
            get;
        }

        public MyClass(ClassFramework.TemplateFramework.Tests.SomeNamespace.IMyMappedType myProperty)
        {
            this.MyProperty = myProperty;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public Test.Domain.Builders.MyClassBuilder ToBuilder()
        {
            return new Test.Domain.Builders.MyClassBuilder(this);
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_Builder_With_TypenameMapping()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MappedTypeBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Domain.Builders
{
#nullable enable
    public partial class MyClassBuilder
    {
        private ClassFramework.TemplateFramework.Tests.SomeNamespace.Builders.IMyMappedTypeBuilder _myProperty;

        public ClassFramework.TemplateFramework.Tests.SomeNamespace.Builders.IMyMappedTypeBuilder MyProperty
        {
            get
            {
                return _myProperty;
            }
            set
            {
                _myProperty = value ?? throw new System.ArgumentNullException(nameof(value));
            }
        }

        public MyClassBuilder(Test.Domain.MyClass source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _myProperty = source.MyProperty?.ToBuilder()!;
        }

        public MyClassBuilder()
        {
            _myProperty = default(ClassFramework.TemplateFramework.Tests.SomeNamespace.IMyMappedType)!;
            SetDefaultValues();
        }

        public Test.Domain.MyClass Build()
        {
            return new Test.Domain.MyClass(MyProperty?.Build()!);
        }

        partial void SetDefaultValues();

        public Test.Domain.Builders.MyClassBuilder WithMyProperty(ClassFramework.TemplateFramework.Tests.SomeNamespace.Builders.IMyMappedTypeBuilder myProperty)
        {
            if (myProperty is null) throw new System.ArgumentNullException(nameof(myProperty));
            MyProperty = myProperty;
            return this;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_CrossCutting_Abstract_Builders()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CrossCuttingAbstractBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders
{
#nullable enable
    public abstract partial class FunctionCallArgumentBaseBuilder<TBuilder, TEntity> : FunctionCallArgumentBaseBuilder, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
        where TEntity : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase
        where TBuilder : FunctionCallArgumentBaseBuilder<TBuilder, TEntity>
    {
        protected FunctionCallArgumentBaseBuilder(CrossCutting.Utilities.Parsers.FunctionCallArgumentBase source) : base(source)
        {
        }

        protected FunctionCallArgumentBaseBuilder() : base()
        {
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArgumentBase Build()
        {
            return BuildTyped();
        }

        public abstract TEntity BuildTyped();

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_CrossCutting_Abstract_Entities()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CrossCuttingAbstractEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers
{
#nullable enable
    public abstract partial record FunctionCallArgumentBase : CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        protected FunctionCallArgumentBase()
        {
        }

        public abstract CrossCutting.Utilities.Parsers.Builders.FunctionCallArgumentBaseBuilder ToBuilder();

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToBuilder();
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_CrossCutting_Abstractions_Builders_Interfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CrossCuttingAbstractionsBuildersInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.Abstractions
{
#nullable enable
    public partial interface IFunctionCallArgumentBuilder
    {
        CrossCutting.Utilities.Parsers.Abstractions.FunctionCallArgument Build();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.Abstractions
{
#nullable enable
    public partial interface IFunctionCallArgumentBuilder<T> : CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        new CrossCutting.Utilities.Parsers.Abstractions.FunctionCallArgument<T> Build();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_CrossCutting_Abstractions_Interfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CrossCuttingAbstractionsInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Abstractions
{
#nullable enable
    public partial interface IFunctionCallArgument
    {
        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder ToBuilder();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Abstractions
{
#nullable enable
    public partial interface IFunctionCallArgument<T> : CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        new CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T> ToBuilder();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_CrossCutting_Abstract_Non_Generic_Builders()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CrossCuttingAbstractNonGenericBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(2);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders
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
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_myBaseProperty!, value!);
                _myBaseProperty = value;
                if (hasChanged) HandlePropertyChanged(nameof(MyBaseProperty));
            }
        }

        protected AbstractBaseBuilder(CrossCutting.Utilities.Parsers.AbstractBase source)
        {
            _myBaseProperty = source.MyBaseProperty;
        }

        protected AbstractBaseBuilder()
        {
            _myBaseProperty = string.Empty;
            SetDefaultValues();
        }

        public abstract CrossCutting.Utilities.Parsers.AbstractBase Build();

        partial void SetDefaultValues();

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.Last().Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders
{
#nullable enable
    public abstract partial class AbstractBaseBuilder<T> : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected AbstractBaseBuilder(CrossCutting.Utilities.Parsers.AbstractBase<T> source)
        {
        }

        protected AbstractBaseBuilder()
        {
            SetDefaultValues();
        }

        public abstract CrossCutting.Utilities.Parsers.AbstractBase<T> Build();

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
    public async Task Can_Generate_Code_For_CrossCutting_Override_Builders()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CrossCuttingOverrideBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(12);
        generationEnvironment.Builder.Contents.ElementAt(0).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class ConstantArgumentBuilder : FunctionCallArgumentBaseBuilder<ConstantArgumentBuilder, CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        private object? _value;

        public object? Value
        {
            get
            {
                return _value;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(_value!, value!);
                _value = value;
                if (hasChanged) HandlePropertyChanged(nameof(Value));
            }
        }

        public ConstantArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _value = source.Value;
        }

        public ConstantArgumentBuilder() : base()
        {
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument(Value);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.ConstantArgumentBuilder WithValue(object? value)
        {
            Value = value;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument(ConstantArgumentBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(1).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class ConstantArgumentBuilder<T> : FunctionCallArgumentBaseBuilder<ConstantArgumentBuilder<T>, CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument<T>>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>
    {
        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<T>.Default.Equals(_value!, value!);
                _value = value;
                if (hasChanged) HandlePropertyChanged(nameof(Value));
            }
        }

        public ConstantArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument<T> source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _value = source.Value;
        }

        public ConstantArgumentBuilder() : base()
        {
            _value = default(T)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument<T> BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument<T>(Value);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T> CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.ConstantArgumentBuilder<T> WithValue(T value)
        {
            Value = value;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantArgument<T>(ConstantArgumentBuilder<T> entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(2).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class ConstantResultArgumentBuilder : FunctionCallArgumentBaseBuilder<ConstantResultArgumentBuilder, CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        private CrossCutting.Common.Results.Result<object?> _result;

        public CrossCutting.Common.Results.Result<object?> Result
        {
            get
            {
                return _result;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<CrossCutting.Common.Results.Result<System.Object?>>.Default.Equals(_result!, value!);
                _result = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Result));
            }
        }

        public ConstantResultArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _result = source.Result;
        }

        public ConstantResultArgumentBuilder() : base()
        {
            _result = default(CrossCutting.Common.Results.Result<System.Object?>)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument(Result);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.ConstantResultArgumentBuilder WithResult(CrossCutting.Common.Results.Result<object?> result)
        {
            if (result is null) throw new System.ArgumentNullException(nameof(result));
            Result = result;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument(ConstantResultArgumentBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(3).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class ConstantResultArgumentBuilder<T> : FunctionCallArgumentBaseBuilder<ConstantResultArgumentBuilder<T>, CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument<T>>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>
    {
        private CrossCutting.Common.Results.Result<T> _result;

        public CrossCutting.Common.Results.Result<T> Result
        {
            get
            {
                return _result;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<CrossCutting.Common.Results.Result<T>>.Default.Equals(_result!, value!);
                _result = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Result));
            }
        }

        public ConstantResultArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument<T> source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _result = source.Result;
        }

        public ConstantResultArgumentBuilder() : base()
        {
            _result = default(CrossCutting.Common.Results.Result<T>)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument<T> BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument<T>(Result);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T> CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.ConstantResultArgumentBuilder<T> WithResult(CrossCutting.Common.Results.Result<T> result)
        {
            if (result is null) throw new System.ArgumentNullException(nameof(result));
            Result = result;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.ConstantResultArgument<T>(ConstantResultArgumentBuilder<T> entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(4).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class DelegateArgumentBuilder : FunctionCallArgumentBaseBuilder<DelegateArgumentBuilder, CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        private System.Func<object?> _delegate;

        private System.Func<System.Type>? _validationDelegate;

        public System.Func<object?> Delegate
        {
            get
            {
                return _delegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<System.Object?>>.Default.Equals(_delegate!, value!);
                _delegate = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Delegate));
            }
        }

        public System.Func<System.Type>? ValidationDelegate
        {
            get
            {
                return _validationDelegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<System.Type>?>.Default.Equals(_validationDelegate!, value!);
                _validationDelegate = value;
                if (hasChanged) HandlePropertyChanged(nameof(ValidationDelegate));
            }
        }

        public DelegateArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _delegate = source.Delegate;
            _validationDelegate = source.ValidationDelegate;
        }

        public DelegateArgumentBuilder() : base()
        {
            _delegate = default(System.Func<System.Object?>)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument(Delegate, ValidationDelegate);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateArgumentBuilder WithDelegate(System.Func<object?> @delegate)
        {
            if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));
            Delegate = @delegate;
            return this;
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateArgumentBuilder WithValidationDelegate(System.Func<System.Type>? validationDelegate)
        {
            ValidationDelegate = validationDelegate;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument(DelegateArgumentBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(5).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class DelegateArgumentBuilder<T> : FunctionCallArgumentBaseBuilder<DelegateArgumentBuilder<T>, CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument<T>>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>
    {
        private System.Func<T> _delegate;

        private System.Func<System.Type>? _validationDelegate;

        public System.Func<T> Delegate
        {
            get
            {
                return _delegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<T>>.Default.Equals(_delegate!, value!);
                _delegate = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Delegate));
            }
        }

        public System.Func<System.Type>? ValidationDelegate
        {
            get
            {
                return _validationDelegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<System.Type>?>.Default.Equals(_validationDelegate!, value!);
                _validationDelegate = value;
                if (hasChanged) HandlePropertyChanged(nameof(ValidationDelegate));
            }
        }

        public DelegateArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument<T> source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _delegate = source.Delegate;
            _validationDelegate = source.ValidationDelegate;
        }

        public DelegateArgumentBuilder() : base()
        {
            _delegate = default(System.Func<T>)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument<T> BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument<T>(Delegate, ValidationDelegate);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T> CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateArgumentBuilder<T> WithDelegate(System.Func<T> @delegate)
        {
            if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));
            Delegate = @delegate;
            return this;
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateArgumentBuilder<T> WithValidationDelegate(System.Func<System.Type>? validationDelegate)
        {
            ValidationDelegate = validationDelegate;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateArgument<T>(DelegateArgumentBuilder<T> entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(6).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class DelegateResultArgumentBuilder : FunctionCallArgumentBaseBuilder<DelegateResultArgumentBuilder, CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        private System.Func<CrossCutting.Common.Results.Result<object?>> _delegate;

        private System.Func<CrossCutting.Common.Results.Result<System.Type>>? _validationDelegate;

        public System.Func<CrossCutting.Common.Results.Result<object?>> Delegate
        {
            get
            {
                return _delegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<CrossCutting.Common.Results.Result<System.Object?>>>.Default.Equals(_delegate!, value!);
                _delegate = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Delegate));
            }
        }

        public System.Func<CrossCutting.Common.Results.Result<System.Type>>? ValidationDelegate
        {
            get
            {
                return _validationDelegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<CrossCutting.Common.Results.Result<System.Type>>?>.Default.Equals(_validationDelegate!, value!);
                _validationDelegate = value;
                if (hasChanged) HandlePropertyChanged(nameof(ValidationDelegate));
            }
        }

        public DelegateResultArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _delegate = source.Delegate;
            _validationDelegate = source.ValidationDelegate;
        }

        public DelegateResultArgumentBuilder() : base()
        {
            _delegate = default(System.Func<CrossCutting.Common.Results.Result<System.Object?>>)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument(Delegate, ValidationDelegate);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateResultArgumentBuilder WithDelegate(System.Func<CrossCutting.Common.Results.Result<object?>> @delegate)
        {
            if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));
            Delegate = @delegate;
            return this;
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateResultArgumentBuilder WithValidationDelegate(System.Func<CrossCutting.Common.Results.Result<System.Type>>? validationDelegate)
        {
            ValidationDelegate = validationDelegate;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument(DelegateResultArgumentBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(7).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class DelegateResultArgumentBuilder<T> : FunctionCallArgumentBaseBuilder<DelegateResultArgumentBuilder<T>, CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument<T>>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>
    {
        private System.Func<CrossCutting.Common.Results.Result<T>> _delegate;

        private System.Func<CrossCutting.Common.Results.Result<System.Type>>? _validationDelegate;

        public System.Func<CrossCutting.Common.Results.Result<T>> Delegate
        {
            get
            {
                return _delegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<CrossCutting.Common.Results.Result<T>>>.Default.Equals(_delegate!, value!);
                _delegate = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Delegate));
            }
        }

        public System.Func<CrossCutting.Common.Results.Result<System.Type>>? ValidationDelegate
        {
            get
            {
                return _validationDelegate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Func<CrossCutting.Common.Results.Result<System.Type>>?>.Default.Equals(_validationDelegate!, value!);
                _validationDelegate = value;
                if (hasChanged) HandlePropertyChanged(nameof(ValidationDelegate));
            }
        }

        public DelegateResultArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument<T> source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _delegate = source.Delegate;
            _validationDelegate = source.ValidationDelegate;
        }

        public DelegateResultArgumentBuilder() : base()
        {
            _delegate = default(System.Func<CrossCutting.Common.Results.Result<T>>)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument<T> BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument<T>(Delegate, ValidationDelegate);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T> CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateResultArgumentBuilder<T> WithDelegate(System.Func<CrossCutting.Common.Results.Result<T>> @delegate)
        {
            if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));
            Delegate = @delegate;
            return this;
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.DelegateResultArgumentBuilder<T> WithValidationDelegate(System.Func<CrossCutting.Common.Results.Result<System.Type>>? validationDelegate)
        {
            ValidationDelegate = validationDelegate;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.DelegateResultArgument<T>(DelegateResultArgumentBuilder<T> entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(8).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class EmptyArgumentBuilder : FunctionCallArgumentBaseBuilder<EmptyArgumentBuilder, CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        public EmptyArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
        }

        public EmptyArgumentBuilder() : base()
        {
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument();
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument(EmptyArgumentBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(9).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class EmptyArgumentBuilder<T> : FunctionCallArgumentBaseBuilder<EmptyArgumentBuilder<T>, CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument<T>>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>
    {
        public EmptyArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument<T> source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
        }

        public EmptyArgumentBuilder() : base()
        {
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument<T> BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument<T>();
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T> CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T>.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.EmptyArgument<T>(EmptyArgumentBuilder<T> entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(10).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class ExpressionArgumentBuilder : FunctionCallArgumentBaseBuilder<ExpressionArgumentBuilder, CrossCutting.Utilities.Parsers.FunctionCallArguments.ExpressionArgument>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        private string _value;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_value!, value!);
                _value = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Value));
            }
        }

        public ExpressionArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.ExpressionArgument source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _value = source.Value;
        }

        public ExpressionArgumentBuilder() : base()
        {
            _value = string.Empty;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.ExpressionArgument BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.ExpressionArgument(Value);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.ExpressionArgumentBuilder WithValue(string value)
        {
            if (value is null) throw new System.ArgumentNullException(nameof(value));
            Value = value;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.ExpressionArgument(ExpressionArgumentBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(11).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments
{
#nullable enable
    public partial class FunctionArgumentBuilder : FunctionCallArgumentBaseBuilder<FunctionArgumentBuilder, CrossCutting.Utilities.Parsers.FunctionCallArguments.FunctionArgument>, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder
    {
        private CrossCutting.Utilities.Parsers.IFunctionCall _function;

        public CrossCutting.Utilities.Parsers.IFunctionCall Function
        {
            get
            {
                return _function;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<CrossCutting.Utilities.Parsers.IFunctionCall>.Default.Equals(_function!, value!);
                _function = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Function));
            }
        }

        public FunctionArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArguments.FunctionArgument source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _function = source.Function;
        }

        public FunctionArgumentBuilder() : base()
        {
            _function = default(CrossCutting.Utilities.Parsers.IFunctionCall)!;
            SetDefaultValues();
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.FunctionArgument BuildTyped()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.FunctionArgument(Function);
        }

        CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallArguments.FunctionArgumentBuilder WithFunction(CrossCutting.Utilities.Parsers.IFunctionCall function)
        {
            if (function is null) throw new System.ArgumentNullException(nameof(function));
            Function = function;
            return this;
        }

        public static implicit operator CrossCutting.Utilities.Parsers.FunctionCallArguments.FunctionArgument(FunctionArgumentBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_CrossCutting_Override_Entities()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<CrossCuttingOverrideEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(12);
        generationEnvironment.Builder.Contents.ElementAt(0).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record ConstantArgument : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        public object? Value
        {
            get;
        }

        public ConstantArgument(object? value) : base()
        {
            this.Value = value;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantArgumentBuilder ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantArgumentBuilder(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(1).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record ConstantArgument<T> : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>
    {
        public T Value
        {
            get;
        }

        public ConstantArgument(T value) : base()
        {
            this.Value = value;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantArgumentBuilder<T> ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantArgumentBuilder<T>(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T> CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(2).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record ConstantResultArgument : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        public CrossCutting.Common.Results.Result<object?> Result
        {
            get;
        }

        public ConstantResultArgument(CrossCutting.Common.Results.Result<object?> result) : base()
        {
            this.Result = result;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantResultArgumentBuilder ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantResultArgumentBuilder(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(3).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record ConstantResultArgument<T> : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>
    {
        public CrossCutting.Common.Results.Result<T> Result
        {
            get;
        }

        public ConstantResultArgument(CrossCutting.Common.Results.Result<T> result) : base()
        {
            this.Result = result;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantResultArgumentBuilder<T> ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ConstantResultArgumentBuilder<T>(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T> CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(4).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record DelegateArgument : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        public System.Func<object?> Delegate
        {
            get;
        }

        public System.Func<System.Type>? ValidationDelegate
        {
            get;
        }

        public DelegateArgument(System.Func<object?> @delegate, System.Func<System.Type>? validationDelegate) : base()
        {
            this.Delegate = @delegate;
            this.ValidationDelegate = validationDelegate;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateArgumentBuilder ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateArgumentBuilder(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(5).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record DelegateArgument<T> : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>
    {
        public System.Func<T> Delegate
        {
            get;
        }

        public System.Func<System.Type>? ValidationDelegate
        {
            get;
        }

        public DelegateArgument(System.Func<T> @delegate, System.Func<System.Type>? validationDelegate) : base()
        {
            this.Delegate = @delegate;
            this.ValidationDelegate = validationDelegate;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateArgumentBuilder<T> ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateArgumentBuilder<T>(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T> CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(6).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record DelegateResultArgument : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        public System.Func<CrossCutting.Common.Results.Result<object?>> Delegate
        {
            get;
        }

        public System.Func<CrossCutting.Common.Results.Result<System.Type>>? ValidationDelegate
        {
            get;
        }

        public DelegateResultArgument(System.Func<CrossCutting.Common.Results.Result<object?>> @delegate, System.Func<CrossCutting.Common.Results.Result<System.Type>>? validationDelegate) : base()
        {
            this.Delegate = @delegate;
            this.ValidationDelegate = validationDelegate;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateResultArgumentBuilder ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateResultArgumentBuilder(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(7).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record DelegateResultArgument<T> : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>
    {
        public System.Func<CrossCutting.Common.Results.Result<T>> Delegate
        {
            get;
        }

        public System.Func<CrossCutting.Common.Results.Result<System.Type>>? ValidationDelegate
        {
            get;
        }

        public DelegateResultArgument(System.Func<CrossCutting.Common.Results.Result<T>> @delegate, System.Func<CrossCutting.Common.Results.Result<System.Type>>? validationDelegate) : base()
        {
            this.Delegate = @delegate;
            this.ValidationDelegate = validationDelegate;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateResultArgumentBuilder<T> ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.DelegateResultArgumentBuilder<T>(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T> CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(8).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record EmptyArgument : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        public EmptyArgument() : base()
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.EmptyArgumentBuilder ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.EmptyArgumentBuilder(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(9).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record EmptyArgument<T> : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>
    {
        public EmptyArgument() : base()
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.EmptyArgumentBuilder<T> ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.EmptyArgumentBuilder<T>(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder<T> CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(10).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record ExpressionArgument : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        public string Value
        {
            get;
        }

        public ExpressionArgument(string value) : base()
        {
            this.Value = value;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ExpressionArgumentBuilder ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.ExpressionArgumentBuilder(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(11).Builder.ToString().ShouldBe(@"namespace CrossCutting.Utilities.Parsers.FunctionCallArguments
{
#nullable enable
    public partial record FunctionArgument : CrossCutting.Utilities.Parsers.FunctionCallArgumentBase, CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase, CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument
    {
        public CrossCutting.Utilities.Parsers.IFunctionCall Function
        {
            get;
        }

        public FunctionArgument(CrossCutting.Utilities.Parsers.IFunctionCall function) : base()
        {
            this.Function = function;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionCallArgumentBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionArgumentBuilder ToTypedBuilder()
        {
            return new CrossCutting.Utilities.Parsers.FunctionCallArguments.Builders.FunctionArgumentBuilder(this);
        }

        CrossCutting.Utilities.Parsers.Builders.Abstractions.IFunctionCallArgumentBuilder CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument.ToBuilder()
        {
            return ToTypedBuilder();
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_ClassFramework_Abstract_Builders()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MultipleInterfacesAbstractBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders
{
#nullable enable
    public abstract partial class TypeBaseBuilder<TBuilder, TEntity> : TypeBaseBuilder, ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IDefaultValueContainerBuilder
        where TEntity : ClassFramework.Domain.TypeBase
        where TBuilder : TypeBaseBuilder<TBuilder, TEntity>
    {
        protected TypeBaseBuilder(ClassFramework.Domain.TypeBase source) : base(source)
        {
        }

        protected TypeBaseBuilder() : base()
        {
        }

        public override ClassFramework.Domain.TypeBase Build()
        {
            return BuildTyped();
        }

        public abstract TEntity BuildTyped();

        ClassFramework.Domain.Abstractions.IType ClassFramework.Domain.Builders.Abstractions.ITypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.INameContainer ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IDefaultValueContainer ClassFramework.Domain.Builders.Abstractions.IDefaultValueContainerBuilder.Build()
        {
            return BuildTyped();
        }

        public static implicit operator ClassFramework.Domain.TypeBase(TypeBaseBuilder<TBuilder, TEntity> entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_ClassFramework_Abstract_Entities()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MultipleInterfacesAbstractEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain
{
#nullable enable
    public abstract partial class TypeBase : ClassFramework.Domain.Abstractions.IType, ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IDefaultValueContainer
    {
        protected TypeBase()
        {
        }

        public abstract ClassFramework.Domain.Builders.TypeBaseBuilder ToBuilder();

        ClassFramework.Domain.Builders.Abstractions.ITypeBuilder ClassFramework.Domain.Abstractions.IType.ToBuilder()
        {
            return ToBuilder();
        }

        ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder ClassFramework.Domain.Abstractions.INameContainer.ToBuilder()
        {
            return ToBuilder();
        }

        ClassFramework.Domain.Builders.Abstractions.IDefaultValueContainerBuilder ClassFramework.Domain.Abstractions.IDefaultValueContainer.ToBuilder()
        {
            return ToBuilder();
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_ClassFramework_Abstractions_Builders_Extensions()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MultipleInterfacesAbstractionsBuildersExtensions>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(3);
        generationEnvironment.Builder.Contents.ElementAt(0).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders.Extensions
{
#nullable enable
    public static partial class DefaultValueContainerBuilderExtensions
    {
        public static T WithDefaultValue<T>(this T instance, object? defaultValue)
            where T : ClassFramework.Domain.Builders.Abstractions.IDefaultValueContainerBuilder
        {
            instance.DefaultValue = defaultValue;
            return instance;
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(1).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders.Extensions
{
#nullable enable
    public static partial class NameContainerBuilderExtensions
    {
        public static T WithName<T>(this T instance, string name)
            where T : ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder
        {
            if (name is null) throw new System.ArgumentNullException(nameof(name));
            instance.Name = name;
            return instance;
        }
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(2).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders.Extensions
{
#nullable enable
    public static partial class TypeBuilderExtensions
    {
        public static T WithNamespace<T>(this T instance, string @namespace)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (@namespace is null) throw new System.ArgumentNullException(nameof(@namespace));
            instance.Namespace = @namespace;
            return instance;
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_ClassFramework_Abstractions_Builders_Interfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MultipleInterfacesAbstractionsBuildersInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(3);
        generationEnvironment.Builder.Contents.ElementAt(0).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders.Abstractions
{
#nullable enable
    public partial interface IDefaultValueContainerBuilder
    {
        object? DefaultValue
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.IDefaultValueContainer Build();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(1).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders.Abstractions
{
#nullable enable
    public partial interface INameContainerBuilder
    {
        string Name
        {
            get;
            set;
        }

        ClassFramework.Domain.Abstractions.INameContainer Build();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(2).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders.Abstractions
{
#nullable enable
    public partial interface ITypeBuilder : ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IDefaultValueContainerBuilder
    {
        string Namespace
        {
            get;
            set;
        }

        new ClassFramework.Domain.Abstractions.IType Build();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_ClassFramework_Abstractions_Interfaces()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MultipleInterfacesAbstractionsInterfaces>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(3);
        generationEnvironment.Builder.Contents.ElementAt(0).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Abstractions
{
#nullable enable
    public partial interface IDefaultValueContainer
    {
        object? DefaultValue
        {
            get;
        }

        ClassFramework.Domain.Builders.Abstractions.IDefaultValueContainerBuilder ToBuilder();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(1).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Abstractions
{
#nullable enable
    public partial interface INameContainer
    {
        string Name
        {
            get;
        }

        ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder ToBuilder();
    }
#nullable restore
}
");
        generationEnvironment.Builder.Contents.ElementAt(2).Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Abstractions
{
#nullable enable
    public partial interface IType : ClassFramework.Domain.Abstractions.INameContainer, ClassFramework.Domain.Abstractions.IDefaultValueContainer
    {
        string Namespace
        {
            get;
        }

        new ClassFramework.Domain.Builders.Abstractions.ITypeBuilder ToBuilder();
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_ClassFramework_Override_Builders()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MultipleInterfacesOverrideBuilders>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Builders.Types
{
#nullable enable
    public partial class ClassBuilder : TypeBaseBuilder<ClassBuilder, ClassFramework.Domain.Types.Class>, ClassFramework.Domain.ITypeBase
    {
        public ClassBuilder(ClassFramework.Domain.Types.Class source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
        }

        public ClassBuilder() : base()
        {
            SetDefaultValues();
        }

        public override ClassFramework.Domain.Types.Class BuildTyped()
        {
            return new ClassFramework.Domain.Types.Class();
        }

        partial void SetDefaultValues();

        public static implicit operator ClassFramework.Domain.Types.Class(ClassBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
#nullable restore
}
");
    }

    [Fact]
    public async Task Can_Generate_Code_For_ClassFramework_Override_Entities()
    {
        // Arrange
        var engine = _scope.ServiceProvider.GetRequiredService<ICodeGenerationEngine>();
        var codeGenerationProvider = _scope.ServiceProvider.GetRequiredService<MultipleInterfacesOverrideEntities>();
        var generationEnvironment = (MultipleStringContentBuilderEnvironment)codeGenerationProvider.CreateGenerationEnvironment();
        var codeGenerationSettings = new CodeGenerationSettings(string.Empty, "GeneratedCode.cs", dryRun: true);

        // Act
        var result = await engine.GenerateAsync(codeGenerationProvider, generationEnvironment, codeGenerationSettings, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        generationEnvironment.Builder.Contents.Count().ShouldBe(1);
        generationEnvironment.Builder.Contents.First().Builder.ToString().ShouldBe(@"namespace ClassFramework.Domain.Types
{
#nullable enable
    public partial class Class : ClassFramework.Domain.TypeBase, ClassFramework.Domain.ITypeBase
    {
        public Class() : base()
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override ClassFramework.Domain.Types.Builders.TypeBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public ClassFramework.Domain.Types.Builders.ClassBuilder ToTypedBuilder()
        {
            return new ClassFramework.Domain.Types.Builders.ClassBuilder(this);
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
        public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => Task.FromResult<Result<IEnumerable<TypeBase>>>(
            Result.Success<IEnumerable<TypeBase>>(
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
                ]));

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

    private sealed class TestPipelineCodeGenerationProvider(IPipelineService pipelineService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(pipelineService)
    {
        public override string Path => string.Empty;
        public override bool RecurseOnDeleteGeneratedFiles => false;
        public override string LastGeneratedFilesFilename => string.Empty;
        public override Encoding Encoding => Encoding.UTF8;

        public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => Task.FromResult<Result<IEnumerable<TypeBase>>>(
            Result.Success<IEnumerable<TypeBase>>(
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
                ]));

        protected override string ProjectName => "UnitTest";
        protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
        protected override Type EntityConcreteCollectionType => typeof(ReadOnlyCollection<>);
        protected override Type BuilderCollectionType => typeof(List<>);
        protected override bool CreateCodeGenerationHeader => false;
    }
}
