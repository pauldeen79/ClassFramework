namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class AddConstructorsComponentTests : TestBase<Pipelines.Reflection.Features.AddConstructorsComponent>
{
    public class Process : AddConstructorsComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.Process(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Does_Not_Add_Constructors_When_CreateConstructors_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(createConstructors: false);
            var context = new PipelineContext<ReflectionContext, TypeBaseBuilder>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture), model);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Constructors.Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Constructors_When_SourceModel_Is_Of_Type_Interface()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var model = new InterfaceBuilder();
            var settings = CreateSettingsForReflection(createConstructors: false);
            var context = new PipelineContext<ReflectionContext, TypeBaseBuilder>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture), model);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            // can't even check constructors on model, because an interface does not have constructors
        }

        [Fact]
        public async Task Adds_Constructors_When_CreateConstructors_Is_Set_To_True_And_SourceModel_Has_Constructors()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(createConstructors: true);
            var context = new PipelineContext<ReflectionContext, TypeBaseBuilder>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture), model);

            // Act
            var result = await sut.Process(context);

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
