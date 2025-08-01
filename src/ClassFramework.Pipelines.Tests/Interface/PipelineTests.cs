﻿namespace ClassFramework.Pipelines.Tests.Interface;

public class PipelineTests : IntegrationTestBase<IPipeline<InterfaceContext>>
{
    public class ProcessAsync : PipelineTests
    {
        private static InterfaceContext CreateContext(bool addProperties = true, bool copyMethods = true, CopyMethodPredicate? copyMethodPredicate = null) => new(
            CreateInterface(addProperties),
            CreateSettingsForInterface
            (
                allowGenerationWithoutProperties: false,
                copyMethods: copyMethods,
                copyMethodPredicate: copyMethodPredicate
            ).Build(),
            CultureInfo.InvariantCulture,
            CancellationToken.None
        );

        [Fact]
        public async Task Sets_Namespace_And_Name_According_To_Settings()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Name.ShouldBe("IMyClass");
            context.Builder.Namespace.ShouldBe("MyNamespace");
        }

        [Fact]
        public async Task Adds_Properties()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == false);
            context.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Property1", "Property2" });
            context.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.String", "System.Collections.Generic.IReadOnlyCollection<System.String>" });
        }

        [Fact]
        public async Task Adds_Methods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(copyMethodPredicate: (_, _) => true);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Adds_FilteredMethods_When_CopyMethods_Is_Set_To_True_Negative()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(copyMethodPredicate: (_, _) => false);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Methods_When_CopyMethods_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(copyMethods: false);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Returns_Invalid_When_SourceModel_Does_Not_Have_Properties_And_AllowGenerationWithoutProperties_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext(addProperties: false);

            // Act
            var result = await sut.ProcessAsync(context);
            var innerResult = result?.InnerResults.FirstOrDefault();

            // Assert
            innerResult.ShouldNotBeNull();
            innerResult!.Status.ShouldBe(ResultStatus.Invalid);
            innerResult.ErrorMessage.ShouldBe("To create an interface, there must be at least one property");
        }
    }

    public class IntegrationTests : PipelineTests
    {
        [Fact]
        public async Task Creates_Interface_With_NamespaceMapping()
        {
            // Arrange
            var model = CreateInterfaceWithCustomTypeProperties();
            var namespaceMappings = CreateNamespaceMappings();
            var settings = CreateSettingsForInterface(
                namespaceMappings: namespaceMappings);
            var context = CreateContext(model, settings);

            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();

            context.Builder.Name.ShouldBe("IMyClass");
            context.Builder.Namespace.ShouldBe("MyNamespace");
            context.Builder.Interfaces.ShouldBeEmpty();

            context.Builder.Fields.ShouldBeEmpty();

            context.Builder.Properties.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "Property1",
                    "Property2",
                    "Property3",
                    "Property4",
                    "Property5",
                    "Property6",
                    "Property7",
                    "Property8"
                }
            );
            context.Builder.Properties.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.Int32",
                    "System.Nullable<System.Int32>",
                    "System.String",
                    "System.String",
                    "MyNamespace.IMyClass",
                    "MyNamespace.IMyClass",
                    "System.Collections.Generic.IReadOnlyCollection<MyNamespace.IMyClass>",
                    "System.Collections.Generic.IReadOnlyCollection<MyNamespace.IMyClass>"
                }
            );
            context.Builder.Properties.Select(x => x.IsNullable).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    false,
                    true,
                    false,
                    true,
                    false,
                    true,
                    false,
                    true
                }
            );
            context.Builder.Properties.Select(x => x.HasGetter).ShouldAllBe(x => x == true);
            context.Builder.Properties.SelectMany(x => x.GetterCodeStatements).ShouldBeEmpty();
            context.Builder.Properties.Select(x => x.HasInitializer).ShouldAllBe(x => x == false);
            context.Builder.Properties.Select(x => x.HasSetter).ShouldAllBe(x => x == false);
            context.Builder.Properties.SelectMany(x => x.SetterCodeStatements).ShouldBeEmpty();
        }

        private static InterfaceContext CreateContext(TypeBase model, PipelineSettingsBuilder settings)
            => new(model, settings, CultureInfo.InvariantCulture, CancellationToken.None);
    }
}
