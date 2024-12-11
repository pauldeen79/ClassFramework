namespace ClassFramework.TemplateFramework.Tests.Models;

public class PropertyCodeBodyModelTests : TestBase
{
    public class Constructor : PropertyCodeBodyModelTests
    {
        [Fact]
        public void Throws_On_Null_Verb()
        {
            // Act & Assert
            this.Invoking(_ => new PropertyCodeBodyModel(verb: null!, default, default, default, Array.Empty<CodeStatementBase>(), CultureInfo.InvariantCulture))
                .Should().Throw<ArgumentNullException>().WithParameterName("verb");
        }

        [Fact]
        public void Throws_On_Null_CodeStatementModels()
        {
            // Act & Assert
            this.Invoking(_ => new PropertyCodeBodyModel("get", default, default, default, codeStatementModels: null!, CultureInfo.InvariantCulture))
                .Should().Throw<ArgumentNullException>().WithParameterName("codeStatementModels");
        }

        [Fact]
        public void Throws_On_Null_CultureInfo()
        {
            // Act & Assert
            this.Invoking(_ => new PropertyCodeBodyModel("get", default, default, default, Array.Empty<CodeStatementBase>(), cultureInfo: null!))
                .Should().Throw<ArgumentNullException>().WithParameterName("cultureInfo");
        }

        [Theory]
        [InlineData(Visibility.Public, SubVisibility.InheritFromParent, "")]
        [InlineData(Visibility.Public, SubVisibility.Public, "")]
        [InlineData(Visibility.Public, SubVisibility.Internal, "internal ")]
        public void Sets_Modifiers_Correctly(Visibility visibility, SubVisibility subVisibility, string expectedResult)
        {
            // Arrange
            var sut = new PropertyCodeBodyModel("get", visibility, subVisibility, null, Array.Empty<CodeStatementBase>(), CultureInfo.InvariantCulture);

            // Act
            var result = sut.Modifiers;

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(false, "class", true)]
        [InlineData(false, "interface", true)]
        [InlineData(false, "wrong_type", true)]
        [InlineData(true, "class", false)]
        [InlineData(true, "interface", true)]
        [InlineData(true, "wrong_type", false)]
        public void Sets_OmitCode_Correctly(bool filledCodeStatements, string parentModelType, bool exptectedResult)
        {
            // Arrange
            var codeStatementsList = new List<CodeStatementBase>();
            if (filledCodeStatements)
            {
                codeStatementsList.Add(new StringCodeStatementBuilder().WithStatement("// statement goes here").Build());
            }
            object? parentModel = parentModelType switch
            {
                "class" => new TypeViewModel() { Model = new ClassBuilder().WithName("MyClass").Build() },
                "interface" => new TypeViewModel() { Model = new InterfaceBuilder().WithName("IMyInterface").Build() },
                "wrong_type" => new object(),
                _ => throw new NotSupportedException("Only 'class', 'interface' and 'wrong_type' are supported as parentModelType")
            };
            var sut = new PropertyCodeBodyModel("get", default, default, parentModel, codeStatementsList, CultureInfo.InvariantCulture);

            // Act
            var result = sut.OmitCode;

            // Assert
            result.Should().Be(exptectedResult);
        }
    }
}
