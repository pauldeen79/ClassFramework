namespace ClassFramework.Pipelines.Tests.Builders;

public class PipelineSettingsBuilderTests
{
    public class AttributeInitializers
    {
        [Fact]
        public void Supports_StringLengthAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder().Build();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(StringLengthClass).GetProperty(nameof(StringLengthClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.Should().Be(typeof(StringLengthAttribute).FullName);
            result.Parameters.Select(x => x.Name).Should().BeEquivalentTo(string.Empty, "MinimumLength");
            result.Parameters.Select(x => x.Value).Should().BeEquivalentTo([ 10, 10 ]);
        }

        [Fact]
        public void Supports_RangeAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder().Build();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(RangeClass).GetProperty(nameof(RangeClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.Should().Be(typeof(RangeAttribute).FullName);
            result.Parameters.Select(x => x.Name).Should().BeEquivalentTo(string.Empty, string.Empty);
            result.Parameters.Select(x => x.Value).Should().BeEquivalentTo([1, 10]);
        }

        [Fact]
        public void Supports_MinLengthAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder().Build();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MinLengthClass).GetProperty(nameof(MinLengthClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.Should().Be(typeof(MinLengthAttribute).FullName);
            result.Parameters.Select(x => x.Name).Should().BeEquivalentTo(string.Empty);
            result.Parameters.Select(x => x.Value).Should().BeEquivalentTo([5]);
        }

        [Fact]
        public void Supports_MaxLengthAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder().Build();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MaxLengthClass).GetProperty(nameof(MaxLengthClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.Should().Be(typeof(MaxLengthAttribute).FullName);
            result.Parameters.Select(x => x.Name).Should().BeEquivalentTo(string.Empty);
            result.Parameters.Select(x => x.Value).Should().BeEquivalentTo([5]);
        }

        [Fact]
        public void Supports_MinCountAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder().Build();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MinCountClass).GetProperty(nameof(MinCountClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.Should().Be(typeof(MinCountAttribute).FullName);
            result.Parameters.Select(x => x.Name).Should().BeEquivalentTo(string.Empty);
            result.Parameters.Select(x => x.Value).Should().BeEquivalentTo([5]);
        }

        [Fact]
        public void Supports_MaxCountAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder().Build();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(MaxCountClass).GetProperty(nameof(MaxCountClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.Should().Be(typeof(MaxCountAttribute).FullName);
            result.Parameters.Select(x => x.Name).Should().BeEquivalentTo(string.Empty);
            result.Parameters.Select(x => x.Value).Should().BeEquivalentTo([5]);
        }

        [Fact]
        public void Supports_CountAttribute()
        {
            // Arrange
            var sut = new PipelineSettingsBuilder().Build();

            // Act
            var result = new TestContext(sut).InitializeDelegate(typeof(CountClass).GetProperty(nameof(CountClass.Property))!.GetCustomAttributes(false).OfType<System.Attribute>().First());

            // Assert
            result.Name.Should().Be(typeof(CountAttribute).FullName);
            result.Parameters.Select(x => x.Name).Should().BeEquivalentTo(string.Empty, string.Empty);
            result.Parameters.Select(x => x.Value).Should().BeEquivalentTo([1, 10]);
        }

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

        private sealed class TestContext : ContextBase<string>
        {
            public TestContext(PipelineSettings settings) : base(string.Empty, settings, CultureInfo.CurrentCulture)
            {
            }

            public override object CreateModel() => string.Empty;

            protected override string NewCollectionTypeName => string.Empty;
        }
    }
}
