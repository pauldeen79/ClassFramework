namespace ClassFramework.Pipelines.Tests.Extensions;

public class PipelineContextExtensionsTests : TestBase
{
    public class CreateEntityInstanciation : PipelineContextExtensionsTests
    {
        [Fact]
        public void Returns_Invalid_On_Abstract_Class()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = new ClassBuilder().WithNamespace("MyNamespace").WithName("MyClass").WithAbstract().AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            InitializeParser();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder().Build(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, builderContext);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = context.CreateEntityInstanciation(formattableStringParser, string.Empty);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("Cannot create an instance of an abstract class");
        }

        [Fact]
        public void Rerturns_Invalid_On_Interface()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = new InterfaceBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            InitializeParser();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder().Build(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, builderContext);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = context.CreateEntityInstanciation(formattableStringParser, string.Empty);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("Cannot create an instance of a type that does not have constructors");
        }

        [Fact]
        public void Returns_Correct_Result_For_Class_With_Public_Parameterless_Constructor()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = new ClassBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            InitializeParser();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder().Build(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, builderContext);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = context.CreateEntityInstanciation(formattableStringParser, string.Empty);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().Be("new MyNamespace.MyClass { MyProperty = MyProperty }");
        }

        [Fact]
        public void Returns_Correct_Result_For_Class_With_Public_Constructor_With_Parameters()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = new ClassBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).AddConstructors(new ConstructorBuilder().AddParameter("myProperty", typeof(string))).Build();
            InitializeParser();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder().Build(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, builderContext);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = context.CreateEntityInstanciation(formattableStringParser, string.Empty);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().Be("new MyNamespace.MyClass(MyProperty)");
        }

        [Fact]
        public void Returns_Correct_Result_For_Struct_With_Public_Parameterless_Constructor()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = new StructBuilder().WithNamespace("MyNamespace").WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string))).Build();
            InitializeParser();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder().Build(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, builderContext);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = context.CreateEntityInstanciation(formattableStringParser, string.Empty);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().Be("new MyNamespace.MyClass { MyProperty = MyProperty }");
        }

        [Fact]
        public void Returns_Correct_Result_For_Class_With_CustomEntityInstanciation_Metadata()
        {
            // Arrange
            var model = new ClassBuilder();
            var sourceModel = new ClassBuilder()
                .WithNamespace("MyNamespace")
                .WithName("MyClass")
                .AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderEntityInstanciation).WithValue("Factory.DoSomething(this)"))
                .Build();
            InitializeParser();
            var builderContext = new BuilderContext(sourceModel, new PipelineSettingsBuilder().Build(), Fixture.Freeze<IFormatProvider>());
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, builderContext);
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();

            // Act
            var result = context.CreateEntityInstanciation(formattableStringParser, string.Empty);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            result.Value.Should().Be("Factory.DoSomething(this)");
        }
    }
}
