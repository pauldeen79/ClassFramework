namespace ClassFramework.Pipelines.Tests.Extensions;

public class PipelineContextExtensionsTests : TestBase
{
    public class CreateEntityInstanciation : PipelineContextExtensionsTests
    {
        [Fact]
        public async Task Returns_Invalid_On_Abstract_Class()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithNamespace("MyNamespace").WithName("MyClass").WithAbstract().AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            await InitializeExpressionEvaluator();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<BuilderContext>(builderContext);
            var formattableStringParser = Fixture.Freeze<IExpressionEvaluator>();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();

            // Act
            var result = await context.CreateEntityInstanciationAsync(formattableStringParser, csharpExpressionDumper, string.Empty, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("Cannot create an instance of an abstract class");
        }

        [Fact]
        public async Task Returns_Invalid_On_Interface()
        {
            // Arrange
            var sourceModel = new InterfaceBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            await InitializeExpressionEvaluator();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<BuilderContext>(builderContext);
            var formattableStringParser = Fixture.Freeze<IExpressionEvaluator>();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();

            // Act
            var result = await context.CreateEntityInstanciationAsync(formattableStringParser, csharpExpressionDumper, string.Empty, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("Cannot create an instance of a type that does not have constructors");
        }

        [Fact]
        public async Task Returns_Correct_Result_For_Class_With_Public_Parameterless_Constructor()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            await InitializeExpressionEvaluator();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<BuilderContext>(builderContext);
            var formattableStringParser = Fixture.Freeze<IExpressionEvaluator>();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();

            // Act
            var result = await context.CreateEntityInstanciationAsync(formattableStringParser, csharpExpressionDumper, string.Empty, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value!.ToString().ShouldBe("new MyNamespace.MyClass { MyProperty = MyProperty }");
        }

        [Fact]
        public async Task Returns_Correct_Result_For_Class_With_Public_Constructor_With_Parameters()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).AddConstructors(new ConstructorBuilder().AddParameter("myProperty", typeof(string))).Build();
            await InitializeExpressionEvaluator();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<BuilderContext>(builderContext);
            var formattableStringParser = Fixture.Freeze<IExpressionEvaluator>();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();

            // Act
            var result = await context.CreateEntityInstanciationAsync(formattableStringParser, csharpExpressionDumper, string.Empty, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value!.ToString().ShouldBe("new MyNamespace.MyClass(MyProperty)");
        }

        [Fact]
        public async Task Returns_Correct_Result_For_Struct_With_Public_Parameterless_Constructor()
        {
            // Arrange
            var sourceModel = new StructBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            await InitializeExpressionEvaluator();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<BuilderContext>(builderContext);
            var formattableStringParser = Fixture.Freeze<IExpressionEvaluator>();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();

            // Act
            var result = await context.CreateEntityInstanciationAsync(formattableStringParser, csharpExpressionDumper, string.Empty, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value!.ToString().ShouldBe("new MyNamespace.MyClass { MyProperty = MyProperty }");
        }

        [Fact]
        public async Task Returns_Correct_Result_For_Class_With_CustomEntityInstanciation_Metadata()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithNamespace("MyNamespace")
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)))
                .Build();
            await InitializeExpressionEvaluator();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder()
                .AddTypenameMappings(new TypenameMappingBuilder()
                    .WithSourceType(sourceModel)
                    .WithTargetType(sourceModel)
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderEntityInstanciation).WithValue("Factory.DoSomething(this)")))
                .Build(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<BuilderContext>(builderContext);
            var expressionEvaluator = Fixture.Freeze<IExpressionEvaluator>();
            var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();

            // Act
            var result = await context.CreateEntityInstanciationAsync(expressionEvaluator, csharpExpressionDumper, string.Empty, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            result.Value!.ToString().ShouldBe("Factory.DoSomething(this)");
        }
    }
}
