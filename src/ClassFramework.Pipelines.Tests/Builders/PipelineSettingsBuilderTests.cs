
namespace ClassFramework.Pipelines.Tests.Builders;

public class PipelineSettingsBuilderTests
{
    public class AttributeInitializers
    {
        [Fact]
        public void Supports_StringLengthAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(StringLengthClass).GetProperty(nameof(StringLengthClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.ShouldBe(typeof(StringLengthAttribute).FullName);
            result.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { string.Empty, "MinimumLength" });
            result.Parameters.Select(x => x.Value).ToArray().ShouldBeEquivalentTo(new object[] { 10, 10 });
        }

        [Fact]
        public void Supports_RangeAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(RangeClass).GetProperty(nameof(RangeClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.ShouldBe(typeof(RangeAttribute).FullName);
            result.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { string.Empty, string.Empty });
            result.Parameters.Select(x => x.Value).ToArray().ShouldBeEquivalentTo(new object[] { 1, 10 });
        }

        [Fact]
        public void Supports_MinLengthAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MinLengthClass).GetProperty(nameof(MinLengthClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.ShouldBe(typeof(MinLengthAttribute).FullName);
            result.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { string.Empty });
            result.Parameters.Select(x => x.Value).ToArray().ShouldBeEquivalentTo(new object[] { 5 });
        }

        [Fact]
        public void Supports_MaxLengthAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MaxLengthClass).GetProperty(nameof(MaxLengthClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.ShouldBe(typeof(MaxLengthAttribute).FullName);
            result.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { string.Empty });
            result.Parameters.Select(x => x.Value).ToArray().ShouldBeEquivalentTo(new object[] { 5 });
        }

        [Fact]
        public void Supports_MinCountAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MinCountClass).GetProperty(nameof(MinCountClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.ShouldBe(typeof(MinCountAttribute).FullName);
            result.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { string.Empty });
            result.Parameters.Select(x => x.Value).ToArray().ShouldBeEquivalentTo(new object[] { 5 });
        }

        [Fact]
        public void Supports_MaxCountAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MaxCountClass).GetProperty(nameof(MaxCountClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.ShouldBe(typeof(MaxCountAttribute).FullName);
            result.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { string.Empty });
            result.Parameters.Select(x => x.Value).ToArray().ShouldBeEquivalentTo(new object[] { 5 });
        }

        [Fact]
        public void Supports_CountAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(CountClass).GetProperty(nameof(CountClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.ShouldBe(typeof(CountAttribute).FullName);
            result.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { string.Empty, string.Empty });
            result.Parameters.Select(x => x.Value).ToArray().ShouldBeEquivalentTo(new object[] { 1, 10 });
        }

        [Fact]
        public void Can_Use_Builder_As_Entity_Using_Implicit_Operator()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithNameFormatString("test");

            // Act
            var result = MyMethod(settings); // method expects an entity, but we're giving a builder!

            // Assert
            result.ShouldBe("test");
        }

        private static string MyMethod(PipelineSettings settings)
            => settings.NameFormatString;

        private sealed class StringLengthClass
        {
            [StringLength(10, MinimumLength = 10)]
            public string Property { get; set; } = default!;
        }

        private sealed class RangeClass
        {
            [Range(1, 10)]
            public int Property { get; set; }
        }

        private sealed class MinLengthClass
        {
            [MinLength(5)]
            public string Property { get; set; } = default!;
        }

        private sealed class MaxLengthClass
        {
            [MaxLength(5)]
            public string Property { get; set; } = default!;
        }

        private sealed class MinCountClass
        {
            [MinCount(5)]
            public List<string> Property { get; set; } = default!;
        }

        private sealed class MaxCountClass
        {
            [MaxCount(5)]
            public List<string> Property { get; set; } = default!;
        }

        private sealed class CountClass
        {
            [Count(1, 10)]
            public List<int> Property { get; set; } = default!;
        }

        private sealed class TestContext(PipelineSettings settings) : ContextBase<string>(string.Empty, settings, CultureInfo.CurrentCulture, CancellationToken.None)
        {
            protected override string NewCollectionTypeName => string.Empty;

            public override Task<Result<TypeBaseBuilder>> ExecuteCommandAsync<TContext>(ICommandService commandService, TContext command, CancellationToken token)
            {
                throw new NotImplementedException();
            }

            public override bool SourceModelHasNoProperties()
            {
                throw new NotImplementedException();
            }
        }
    }
}
