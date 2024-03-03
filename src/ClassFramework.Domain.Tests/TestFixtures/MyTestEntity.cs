namespace ClassFramework.Domain.Tests.TestFixtures;

public record TestValidatable : System.ComponentModel.DataAnnotations.IValidatableObject
{
    public TestValidatable(int property)
    {
        Property = property;
        // Default built-in validation, using ValidationException:
        /// System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);

        // Convert validation exception to ArgumentException, for more DDD style validation:
        var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this), results, true);
        var error = results.Find(x => !string.IsNullOrEmpty(x.ErrorMessage) && x.MemberNames.Any());
        if (error is not null)
        {
            throw new ArgumentException(error.ErrorMessage, error.MemberNames.First());
        }
    }

    public int Property { get; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield return new ValidationResult("Error1", ["Member1"]);
        yield return new ValidationResult("Error2", ["Member2"]);
    }
}

public class TestValidatableBuilder : IValidatableObject
{
    public int Property { get; set; }

    // Example of shared validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var instance = new TestValidatable(Property);
        var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(instance, new System.ComponentModel.DataAnnotations.ValidationContext(instance), results, true);
        return results;
    }
}

public partial record MyCustomEntity
{
    public int Property1 { get; }

    public MyCustomEntity(int property1)
    {
        Property1 = property1;

        // When using validation (either DDD or IValidatableObject style):
        Validate();
    }

    // When using validation with IValidatableObject interface:
    protected void Validate()
    {
        System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
    }

    // When using domain driven style validation (just throw argument exceptions), use this:
    ///partial void Validate();

    public MyCustomEntityBuilder ToBuilder()
    {
        return new MyCustomEntityBuilder(this);
    }
}

public partial record MyTestEntity
{
    public MyTestEntity(int property1, MyCustomEntity property2, IEnumerable<MyCustomEntity> property3)
    {
        Property1 = property1;
        Property2 = property2.IsNotNull(nameof(property2));
        // When using validation:
        ///Property3 = new ReadOnlyValueCollection<MyCustomEntity>(property3 ?? Enumerable.Empty<MyCustomEntity>());
        // When not using null checks:
        ///Property3 = property3  is null ? null : new ReadOnlyValueCollection<MyCustomEntity>(property3);
        // When using null checks:
        ArgumentNullException.ThrowIfNull(property3);
        Property3 = new ReadOnlyValueCollection<MyCustomEntity>(property3);

        // When using validation (either DDD or IValidatableObject style):
        Validate();
    }

    // When using validation with IValidatableObject interface:
    protected void Validate()
    {
        System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
    }

    // When using domain driven style validation (just throw argument exceptions), use this:
    ///partial void Validate();

    public int Property1 { get; }
    public MyCustomEntity Property2 { get; }
    public IReadOnlyCollection<MyCustomEntity> Property3 { get; }

    public MyTestEntityBuilder ToBuilder()
    {
        return new MyTestEntityBuilder(this);
    }
}

public partial class MyTestEntityBuilder ///: System.ComponentModel.DataAnnotations.IValidatableObject
{
    private MyCustomEntityBuilder _property2;
    private Collection<MyCustomEntityBuilder> _property3;

    // When not nullable, use auto property and no backing field
    public int Property1 { get; set; }

    public MyCustomEntityBuilder Property2
    {
        get
        {
            return _property2;
        }
        set
        {
            System.ArgumentNullException.ThrowIfNull(value);
            _property2 = value;
        }
    }

    public Collection<MyCustomEntityBuilder> Property3
    {
        get
        {
            return _property3;
        }
        private set
        {
            ArgumentNullException.ThrowIfNull(value);
            _property3 = value;
        }
    }

    public MyTestEntityBuilder()
    {
        // Note that if you need to assign a default value to Property1, then you should either use _property1 or Property1, depending on the availability of a backing field.
        _property2 = new MyCustomEntityBuilder();
        _property3 = new Collection<MyCustomEntityBuilder>();
        SetDefaultValues();
    }

    public MyTestEntityBuilder(MyTestEntity source)
    {
        ArgumentNullException.ThrowIfNull(source);

        // Note that if you need to assign a default value to Property1, then you should either use _property1 or Property1, depending on the availability of a backing field.
        Property1 = source.Property1;
        _property2 = new MyCustomEntityBuilder(source.Property2); // MetadataNames.CustomBuilderConstructorInitializeExpression (with {SourcePropertyName} as placeholder?)
        _property3 = new Collection<MyCustomEntityBuilder>();
        //note that Collection<T> does not have an AddRange method...
        foreach (var x in source.Property3.Select(x => new MyCustomEntityBuilder(x)))
        {
            _property3.Add(x); // MetadataNames.CustomBuilderConstructorInitializeExpression (with {SourcePropertyName} as placeholder?)
        }
    }

    partial void SetDefaultValues();

    public MyTestEntity Build()
    {
        return new MyTestEntity(Property1, Property2.Build(), Property3.Select(x => x.Build())); // MetadataNames.CustomBuilderMethodParameterExpression (with {SourcePropertyName} as placeholder?)
    }
}

public partial class MyCustomEntityBuilder ///: System.ComponentModel.DataAnnotations.IValidatableObject
{
    public int Property1 { get; set; }

    public MyCustomEntityBuilder()
    {
    }

    public MyCustomEntityBuilder(MyCustomEntity source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Property1 = source.Property1;
    }

    public MyCustomEntity Build()
    {
        return new MyCustomEntity(Property1);
    }
}

public partial record MyObservableEntity : INotifyPropertyChanged
{
    private int _property1;

    public int Property1
    {
        get
        {
            return _property1;
        }
        set
        {
            _property1 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property1)));
        }
    }

    public ObservableValueCollection<int> Property2 { get; } // do not use backing fields on collections, gives CA2227 - Collection properties should be read only (and ensure event handlers on the observable collection are preserved, as you can't overwrite the collection instance)

    public event PropertyChangedEventHandler? PropertyChanged;

    public MyObservableEntity(int property1, IEnumerable<int> property2)
    {
        _property1 = property1;
        Property2 = new ObservableValueCollection<int>(property2);
    }
}
