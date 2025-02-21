﻿namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddPropertiesComponentTests : TestBase<Pipelines.Reflection.Components.AddPropertiesComponent>
{
    public class ProcessAsync : AddPropertiesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Task t = sut.ProcessAsync(context: null!);
            (await t.ShouldThrowAsync<ArgumentNullException>()).ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Copies_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Fills_GenericTypeArguments_Correctly()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyNullableClass);
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Properties.Count.ShouldBe(1);
            //public Func<object, IEnumerable<object?>> MyDelegateProperty { get; set; } = default!;
            //model.Properties.Single().TypeName.ShouldBe("System.Func<System.Object,System.Collections.Generic.IEnumerable<System.Object?>>");
            context.Request.Builder.Properties.Single().IsNullable.ShouldBeFalse();
            context.Request.Builder.Properties.Single().GenericTypeArguments.Count.ShouldBe(2);
            context.Request.Builder.Properties.Single().GenericTypeArguments[0].TypeName.ShouldBe("System.Object");
            context.Request.Builder.Properties.Single().GenericTypeArguments[0].IsNullable.ShouldBeFalse();
            context.Request.Builder.Properties.Single().GenericTypeArguments[0].GenericTypeArguments.ShouldBeEmpty();
            //model.Properties.Single().GenericTypeArguments[1].TypeName.ShouldBe("System.Collections.Generic.IEnumerable<System.Object?>");
            context.Request.Builder.Properties.Single().GenericTypeArguments[1].IsNullable.ShouldBeFalse();
            context.Request.Builder.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Count.ShouldBe(1);
            context.Request.Builder.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().TypeName.ShouldBe("System.Object");
            //model.Properties.Single().GenericTypeArguments[1].GenericTypeArguments.Single().IsNullable.ShouldBeTrue();
        }
    }
}
