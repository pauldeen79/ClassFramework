namespace ClassFramework.Domain.Tests.Builders;

public class AttributeBuilderTests : TestBase<AttributeBuilder>
{
    public class AddNameAndParameter : AttributeBuilderTests
    {
        [Fact]
        public void Sets_Name_And_Parameter_Correctly()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var actual = sut.AddNameAndParameter("System.ComponentModel.ReadOnly", true);

            // Assert
            actual.Name.ShouldBe("System.ComponentModel.ReadOnly");
            actual.Parameters.Count.ShouldBe(1);
            actual.Parameters[0].Value.ShouldBe(true);
        }
    }

    public class ForCodeGenerator : AttributeBuilderTests
    {
        [Fact]
        public void Adds_Parameters_Correctly()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var actual = sut.ForCodeGenerator("MyGenerator", "1.0.0.0");

            // Assert
            actual.Name.ShouldBe(typeof(GeneratedCodeAttribute).FullName);
            actual.Parameters.ToArray().ShouldBeEquivalentTo(new AttributeParameterBuilder[] {
                new AttributeParameterBuilder().WithValue("MyGenerator"),
                new AttributeParameterBuilder().WithValue("1.0.0.0")
            });
        }
    }

    // Note that this is actually generated code, but I want to prove that this is working correctly
    public class AddParameters : AttributeBuilderTests
    {
        [Fact]
        public void Throws_On_Null_Parameters_As_Array()
        {
            // Arrange
            var sut = CreateSut();
            AttributeParameterBuilder[] parameters = default!;

            // Act & Assert
            Action a = () => sut.AddParameters(parameters: parameters);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("parameters");
        }

        [Fact]
        public void Throws_On_Null_Parameters_As_Enumerable()
        {
            // Arrange
            var sut = CreateSut();
            IEnumerable<AttributeParameterBuilder> parameters = default!;

            // Act & Assert
            Action a = () => sut.AddParameters(parameters: parameters);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("parameters");
        }
    }

    public class WithName : AttributeBuilderTests
    {
        [Fact]
        public void Throws_On_Null_Type()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => _ = sut.WithName(sourceType: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("sourceType");
        }

        [Fact]
        public void Sets_Name_Correctly_When_SourceType_Is_Not_Null()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.WithName(typeof(AttributeBuilderTests));

            // Assert
            result.Name.ShouldBe("ClassFramework.Domain.Tests.Builders.AttributeBuilderTests");
        }
    }
}
