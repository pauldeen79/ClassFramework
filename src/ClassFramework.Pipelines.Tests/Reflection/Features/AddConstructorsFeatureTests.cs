namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class AddConstructorsFeatureTests : TestBase<Pipelines.Reflection.Features.AddConstructorsFeature>
{
    public class Process : AddConstructorsFeatureTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.Process(context: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public void Does_Not_Add_Constructors_When_CreateConstructors_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(createConstructors: false);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().BeEmpty();
        }

        [Fact]
        public void Does_Not_Add_Constructors_When_SourceModel_Is_Of_Type_Interface()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var model = new InterfaceBuilder();
            var settings = CreateSettingsForReflection(createConstructors: false);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            // can't even check constructors on model, because an interface does not have constructors
        }

        [Fact]
        public void Adds_Constructors_When_CreateConstructors_Is_Set_To_True_And_SourceModel_Has_Constructors()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(createConstructors: true);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().HaveCount(2);
            model.Constructors[0].Parameters.Should().BeEmpty();
            model.Constructors[model.Constructors.Count - 1].Parameters.Select(x => x.Name).Should().BeEquivalentTo("value");
            model.Constructors[model.Constructors.Count - 1].Parameters.Select(x => x.TypeName).Should().BeEquivalentTo("System.Int32");
        }
    }
}

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class MyConstructorTestClass
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
{
    public MyConstructorTestClass()
    {
    }

    public MyConstructorTestClass(int value)
    {
    }
}

internal interface IMyConstructorTestInterface
{
}
