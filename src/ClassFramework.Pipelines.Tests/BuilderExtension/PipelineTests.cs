using System.Threading;

namespace ClassFramework.Pipelines.Tests.BuilderExtension;

public class PipelineTests : IntegrationTestBase<IPipeline<BuilderExtensionContext>>
{
    public class ProcessAsync : PipelineTests
    {
        private static BuilderExtensionContext CreateContext(bool addProperties = true)
            => new(
                CreateGenericClass(addProperties),
                CreateSettingsForBuilder
                (
                    builderNamespaceFormatString: "{class.Namespace}.Builders",
                    allowGenerationWithoutProperties: false,
                    copyAttributes: true
                ),
                CultureInfo.InvariantCulture,
                CancellationToken.None
            );

        [Fact]
        public async Task Sets_Partial()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Partial.ShouldBeTrue();
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_NonCollection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Builder.Methods.Count(x => x.Name == "WithProperty1").ShouldBe(1);
            var method = context.Builder.Methods.Single(x => x.Name == "WithProperty1");
            method.ReturnTypeName.ShouldBe("T");
            method.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo(new[] { "instance.Property1 = property1;", "return instance;" });

            context.Builder.Methods.Where(x => x.Name == "WithProperty2").ShouldBeEmpty(); //only for the non-collection property
        }

        [Fact]
        public async Task Adds_Fluent_Method_For_Collection_Property()
        {
            // Arrange
            var sut = CreateSut();
            var context = CreateContext();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            var methods = context.Builder.Methods.Where(x => x.Name == "AddProperty2").ToArray();
            methods.Length.ShouldBe(2);
            methods.Select(x => x.ReturnTypeName).ShouldAllBe(x => x == "T");
            methods.SelectMany(x => x.Parameters.Select(y => y.TypeName)).ToArray().ShouldBeEquivalentTo(new[] { "T", "System.Collections.Generic.IEnumerable<System.String>", "T", "System.String[]" });
            methods.SelectMany(x => x.CodeStatements).ShouldAllBe(x => x is StringCodeStatementBuilder);
            methods.SelectMany(x => x.CodeStatements).OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return instance.AddProperty2<T>(property2.ToArray());",
                    "foreach (var item in property2) instance.Property2.Add(item);",
                    "return instance;"
                }
            );
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
            innerResult.ErrorMessage.ShouldBe("To create a builder extensions class, there must be at least one property");
        }
    }
}
