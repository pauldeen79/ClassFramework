namespace ClassFramework.Domain.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("something", "something")]
    [InlineData("Something", "something")]
    [InlineData("a", "a")]
    [InlineData("A", "a")]
    public void ToPascalCase_Returns_Correct_Result(string input, string expectedResult)
    {
        // Act
        var actual = input.ToPascalCase(CultureInfo.InvariantCulture);

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "''")]
    [InlineData(" ", "' '")]
    [InlineData("something", "'something'")]
    [InlineData("something 'quoted'", "'something ''quoted'''")]
    [InlineData("something 'quoted' bla", "'something ''quoted'' bla'")]
    public void SqlEncode_Returns_Correct_Result(string input, string expectedResult)
    {
        // Act
        var actual = input.SqlEncode();

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("System.Nullable`1[[System.Boolean, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]", "System.Nullable<System.Boolean>")]
    [InlineData("MyNamespace.MyClass+MySubClass", "MyNamespace.MyClass.MySubClass")]
    [InlineData("System.Boolean, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "System.Boolean")]
    [InlineData("System.Nullable`1[[System.Boolean, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "System.Nullable<System.Boolean")]
    public void FixTypeName_Returns_Correct_Result(string input, string expectedResult)
    {
        // Act
        var actual = input.FixTypeName();

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void FixTypeName_Returns_AnonymousType_When_Type_Is_Anonymous()
    {
        // Arrange
        var typeName = new { Name = "Test" }.GetType().FullName;

        // Act
        var actual = typeName.FixTypeName();

        // Assert
        actual.Should().Be("AnonymousType");
    }

    [Fact]
    public void FixTypeName_Returns_AnonymousType_When_Type_Is_Anonymous_Array()
    {
        // Arrange
        var typeName = new[] { new { Name = "Test" } }.GetType().FullName;

        // Act
        var actual = typeName.FixTypeName();

        // Assert
        actual.Should().Be("AnonymousType[]");
    }

    [Fact]
    public void FixTypeName_Returns_Correct_Result_For_NonGeneric_Type()
    {
        // Arrange
        var input = typeof(int).FullName;

        // Act
        var actual = input.FixTypeName();

        // Assert
        actual.Should().Be("System.Int32");
    }

    [Fact]
    public void FixTypeName_Returns_Correct_Result_For_Nullable_Type()
    {
        // Arrange
        var input = typeof(int?).FullName;

        // Act
        var actual = input.FixTypeName();

        // Assert
        actual.Should().Be("System.Nullable<System.Int32>");
    }

    [Fact]
    public void FixTypeName_Returns_Correct_Result_For_Generic_Func()
    {
        // Arrange
        var input = typeof(Func<int>).FullName;

        // Act
        var actual = input.FixTypeName();

        // Assert
        actual.Should().Be("System.Func<System.Int32>");
    }

    [Fact]
    public void FixTypeName_Returns_Correct_Result_For_Nullable_Generic_Func()
    {
        // Arrange
        var input = typeof(Func<int?>).FullName;

        // Act
        var actual = input.FixTypeName();

        // Assert
        actual.Should().Be("System.Func<System.Nullable<System.Int32>>");
    }

    [Fact]
    public void FixTypeName_Returns_Correct_Result_For_Generic_Enumerable()
    {
        // Arrange
        var input = typeof(IEnumerable<int>).FullName;

        // Act
        var actual = input.FixTypeName();

        // Assert
        actual.Should().Be("System.Collections.Generic.IEnumerable<System.Int32>");
    }

    [Fact]
    public void FixTypeName_Returns_Correct_Result_For_Nullable_Generic_Enumerable()
    {
        // Arrange
        var input = typeof(IEnumerable<int?>).FullName;

        // Act
        var actual = input.FixTypeName();

        // Assert
        actual.Should().Be("System.Collections.Generic.IEnumerable<System.Nullable<System.Int32>>");
    }

    [Fact]
    public void FixTypeName_Returns_Correct_Result_For_Generics_With_Multiple_Generic_Parameters()
    {
        // Arrange
        var input = typeof(Func<object?, IAsyncDisposable, object?>).FullName;

        // Act
        var actual = input.FixTypeName();

        // Assert
        //Note that nullable generic argument types are not recognized. I'm not sure how to fix this...
        actual.Should().Be("System.Func<System.Object,System.IAsyncDisposable,System.Object>");
    }

    [Theory]
    [InlineData("ClassFramework.Domain.Tests.TestFixtures.MyEnumThing, ClassFramework.Domain.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", true)]
    [InlineData("System.Nullable`1[[ClassFramework.Domain.Tests.TestFixtures.MyEnumThing, ClassFramework.Domain.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", false)]
    [InlineData("", false)]
    [InlineData("System.String", false)]
    [InlineData("System.Int32", false)]
    [InlineData("SomeUnknownType", false)]
    public void IsRequiredEnum_Returns_Correct_Result(string input, bool expectedResult)
    {
        // Act
        var actual = input.IsRequiredEnum();

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("ClassFramework.Domain.Tests.TestFixtures.MyEnumThing, ClassFramework.Domain.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", false)]
    [InlineData("System.Nullable`1[[ClassFramework.Domain.Tests.TestFixtures.MyEnumThing, ClassFramework.Domain.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", true)]
    [InlineData("", false)]
    [InlineData("System.String", false)]
    [InlineData("System.Int32", false)]
    [InlineData("SomeUnknownType", false)]
    public void IsOptionalEnum_Returns_Correct_Result(string input, bool expectedResult)
    {
        // Act
        var actual = input.IsOptionalEnum();

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("MyClass", "MyClass")]
    [InlineData("MyNamespace.MyClass", "MyClass")]
    [InlineData("MyNamespace.MyClass<My.GenericType>", "MyClass<My.GenericType>")]
    [InlineData("A.B.C.D", "D")]
    public void GetClassName_Returns_Correct_Result(string input, string expectedResult)
    {
        // Act
        var actual = input.GetClassName();

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("MyClass", "")]
    [InlineData("MyNamespace.MyClass", "MyNamespace")]
    [InlineData("MyNamespace.MyClass<My.GenericType>", "MyNamespace")]
    [InlineData("A.B.C.D", "A.B.C")]
    public void GetNamespaceWithDefault_Returns_Correct_Result(string? input, string expectedResult)
    {
        // Act
        var actual = input.GetNamespaceWithDefault();

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "MyClass")]
    [InlineData("Generic", "MyClass<Generic>")]
    [InlineData("MyNamespace.Generic", "MyClass<MyNamespace.Generic>")]
    public void MakeGenericTypeName_Returns_Correct_Result(string input, string expectedResult)
    {
        // Act
        var actual = "MyClass".MakeGenericTypeName(input);

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "A", "B", "")]
    [InlineData("ASomething", "A", "B", "ASomething")]
    [InlineData("ASomeAthing", "A", "B", "ASomeAthing")]
    [InlineData("SomethingA", "A", "B", "SomethingB")]
    public void ReplaceSuffix_Returns_Correct_Result(string input, string find, string replace, string expectedResult)
    {
        // Act
        var actual = input.ReplaceSuffix(find, replace, StringComparison.InvariantCulture);

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("SomeUnknownType", "SomeUnknownType")]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("    ", "    ")]
    [InlineData("System.Char", "char")]
    [InlineData("System.Char with some prefix", "System.Char with some prefix")]
    [InlineData("suffix System.Char", "suffix System.Char")]
    [InlineData("System.String", "string")]
    [InlineData("System.Boolean", "bool")]
    [InlineData("System.Object", "object")]
    [InlineData("System.Decimal", "decimal")]
    [InlineData("System.Double", "double")]
    [InlineData("System.Single", "float")]
    [InlineData("System.Byte", "byte")]
    [InlineData("System.SByte", "sbyte")]
    [InlineData("System.Int16", "short")]
    [InlineData("System.UInt16", "ushort")]
    [InlineData("System.Int32", "int")]
    [InlineData("System.UInt32", "uint")]
    [InlineData("System.Int64", "long")]
    [InlineData("System.UInt64", "ulong")]
    [InlineData("System.String[]", "string[]")]
    [InlineData("IEnumerable<System.String>", "IEnumerable<string>")]
    [InlineData("KeyValuePair<System.String, System.String>", "KeyValuePair<string, string>")]
    [InlineData("KeyValuePair<System.String,System.String>", "KeyValuePair<string,string>")]
    [InlineData("TripleGeneric<System.String, System.String, System.String>", "TripleGeneric<string, string, string>")]
    [InlineData("TripleGeneric<System.String,System.String,System.String>", "TripleGeneric<string,string,string>")]
    public void GetCsharpFriendlyTypeName_Returns_Correct_Result(string input, string expectedResult)
    {
        // Act
        var actual = input.GetCsharpFriendlyTypeName();

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("System.Object", "")]
    [InlineData("ExpressionFramework.Domain.Contracts.ITypedExpression<System.Object>", "System.Object")]
    [InlineData("ExpressionFramework.Domain.Contracts.ITypedExpression<System.Collections.Generic.IEnumerable<System.Object>>", "System.Collections.Generic.IEnumerable<System.Object>")]
    public void GetGenericArguments_Returns_Correct_Result(string input, string expected)
    {
        // Act
        var result = input.GetGenericArguments();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData("someValidName", "someValidName")]
    [InlineData("operator", "@operator")]
    public void GetCsharpFriendlyName_Returns_Correct_Result(string? input, string? expected)
    {
        // Act
        var result = input!.GetCsharpFriendlyName();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("some $$$ invalid stuff", "some_____invalid_stuff")]
    public void Sanitize_Returns_Correct_Result(string? input, string expected)
    {
        // Act
        var result = input.Sanitize();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("System.String", true)]
    [InlineData("System.Boolean", false)]
    [InlineData("System.Object", false)]
    [InlineData("string", false)]
    public void IsStringTypeName_Returns_Correct_Result(string? input, bool expected)
    {
        // Act
        var result = input.IsStringTypeName();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("System.String", false)]
    [InlineData("System.Boolean", true)]
    [InlineData("System.Object", false)]
    [InlineData("boolean", false)]
    public void IsBooleanTypeName_Returns_Correct_Result(string? input, bool expected)
    {
        // Act
        var result = input.IsBooleanTypeName();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("System.Nullable<System.Boolean>", true)]
    [InlineData("System.Boolean", false)]
    [InlineData("Something else", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsNullableBooleanTypeName_Returns_Correct_Result(string? input, bool expected)
    {
        // Act
        var result = input.IsNullableBooleanTypeName();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("System.String", false)]
    [InlineData("System.Boolean", false)]
    [InlineData("System.Object", true)]
    [InlineData("object", false)]
    public void IsObjectTypeName_Returns_Correct_Result(string? input, bool expected)
    {
        // Act
        var result = input.IsObjectTypeName();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ConvertTypeNameToArray_Returns_Correct_Result()
    {
        // Arrange
        var input = typeof(List<string>).FullName!.FixTypeName(); // note that it's important to use the 'fixed type name', e.g. System.Collections.Generic.List<System.String> instead of List`...

        // Act
        var result = input.ConvertTypeNameToArray();

        // Assert
        result.Should().Be("System.String[]");
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("", "System.Collections.Generic.List", "")]
    [InlineData("SomeTypeThatIsNotAColl", "", "SomeTypeThatIsNotAColl")]
    [InlineData("SomeTypeThatIsNotAColl", "System.Collections.Generic.List", "SomeTypeThatIsNotAColl")]
    [InlineData("Custom.List<System.String>", "", "Custom.List<System.String>")]
    [InlineData("Custom.List<System.String>", "System.Collections.Generic.List", "System.Collections.Generic.List<System.String>")]
    [InlineData("System.String[]", "System.Collections.Generic.List", "System.Collections.Generic.List<System.String>")]
    public void FixCollectionTypeName_Returns_Correct_Result(string typeName, string newCollectionTypeName,string expectedResult)
    {
        // Act
        var result = typeName.FixCollectionTypeName(newCollectionTypeName);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("MyMethod", "MyMethod")]
    [InlineData("ISomeInterface.MyMethod", "MyMethod")]
    public void RemoveInterfacePrefix_Returns_Correct_Result(string input, string expectedResult)
    {
        // Act
        var result = input.RemoveInterfacePrefix();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("System.String", "System.String")]
    [InlineData("System.Collections.Generic.List`1[[System.String, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]", "System.Collections.Generic.List")]
    [InlineData("System.Collections.Generic.List<System.String>", "System.Collections.Generic.List<System.String>")] // note that you need WithoutProcessedGenerics to get the thing you want...
    public void WithoutGenerics_Returns_Correct_Result(string typeName, string expectedResult)
    {
        // Act
        var result = typeName.WithoutGenerics();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void AbbreviateNamespaces_Returns_Value_Unchanged_When_Namespace_Is_Not_Supplied()
    {
        // Arrange
        var sut = "MyNamespace.MyClass";
        var namespacesToAbbreviate = new[] { "System" };

        // Act
        var actual = sut.AbbreviateNamespaces(namespacesToAbbreviate);

        // Assert
        actual.Should().Be(sut);
    }

    [Fact]
    public void AbbreviateNamespaces_Returns_Abbreviated_Value_When_Namespace_Is_Supplied()
    {
        // Arrange
        var sut = "MyNamespace.MyClass";
        var namespacesToAbbreviate = new[] { "MyNamespace" };

        // Act
        var actual = sut.AbbreviateNamespaces(namespacesToAbbreviate);

        // Assert
        actual.Should().Be("MyClass");
    }

    [Theory,
        InlineData("System.String", true, false, false, "default(System.String)"),
        InlineData("System.String", false, false, false, "string.Empty"),
        InlineData("string", true, false, false, "default(string)"),
        InlineData("string?", true, false, false, "default(string?)"),
        InlineData("string", false, false, false, "string.Empty"),
        InlineData("System.Object", true, false, false, "default(System.Object)"),
        InlineData("System.Object", false, false, false, "new System.Object()"),
        InlineData("object", true, false, false, "default(object)"),
        InlineData("object?", true, false, false, "default(object?)"),
        InlineData("object", false, false, false, "new System.Object()"),
        InlineData("System.Int32", false, true, false, "default(System.Int32)"),
        InlineData("System.Int32", true, true, false, "default(System.Int32?)"),
        InlineData("System.Collections.IEnumerable", false, false, false, "System.Linq.Enumerable.Empty<System.Object>()"),
        InlineData("System.Collections.IEnumerable", true, false, false, "default(System.Collections.IEnumerable)"),
        InlineData("System.Collections.Generic.IEnumerable<int>", false, false, false, "System.Linq.Enumerable.Empty<int>()"),
        InlineData("System.Collections.Generic.IEnumerable<int>", true, false, false, "default(System.Collections.Generic.IEnumerable<int>)"),
        InlineData("SomeType", false, false, true, "default(SomeType)!"),
        InlineData("SomeType", false, true, true, "default(SomeType)")]
    public void GetDefaultValue_Returns_Correct_Result(string input, bool isNullable, bool isValueType, bool enableNullableReferenceTypes, string expected)
    {
        // Act
        var actual = input.GetDefaultValue(isNullable, isValueType, enableNullableReferenceTypes);

        // Assert
        actual.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("System.String", false, false, "System.String")]
    [InlineData("System.String", true, false, "System.String")]
    [InlineData("System.String", true, true, "System.String?")]
    [InlineData("System.String?", true, true, "System.String?")]
    [InlineData("System.Nullable<System.Int32>", true, true, "System.Nullable<System.Int32>")]
    public void AppendNullableAnnotation_Returns_Correct_Result(string typeName, bool isNullable, bool enableNullableReferenceTypes, string expectedResult)
    {
        // Act
        var result = typeName.AppendNullableAnnotation(isNullable, enableNullableReferenceTypes);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("System.Collections.Generic.IEnumerable<System.Int32>", "System.Linq.Enumerable.Empty<System.Int32>()")]
    [InlineData("System.Collections.Generic.List<System.Int32>", "new System.Collections.Generic.List<System.Int32>()")]
    public void GetCollectionInitializeStatement_Returns_Correct_Result(string typeName, string expectedResult)
    {
        // Act
        var result = typeName.GetCollectionInitializeStatement(string.Empty);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("System.Collections.Generic.IEnumerable<System.Int32>", "System.Int32")]
    [InlineData("System.Collections.Generic.List<System.Int32>", "System.Int32")]
    [InlineData("System.Int32[]", "System.Int32")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("SomeTypeNameThatIsNotAGenericCollectionOrArray", "")]
    public void GetCollectionItemType_Returns_Correct_Result(string? typeName, string expectedResult)
    {
        // Act
        var result = typeName.GetCollectionItemType();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, false, "")]
    [InlineData("", false, "")]
    [InlineData("no typename", false, "")]
    [InlineData("Func<blablabla", false, "")]
    [InlineData("Func<blablabla>", false, "blablabla")]
    [InlineData("Func<blablabla>", true, "<blablabla>")]
    public void GetProcessedGenericArguments_Returns_Correct_Result(string? typeName, bool addBrackets, string expectedResult)
    {
        // Act
        var result = typeName.GetProcessedGenericArguments(addBrackets);

        // Assert
        result.Should().Be(expectedResult);
    }
}
