namespace ProofOfConcept;

public record MyEntity
{
    public int Id { get; }
    [Required(AllowEmptyStrings = false)] public string Name { get; }

    public MyEntity(int id, string name)
    {
        Id = id;
        Name = name;
        //TODO: trigger validation...
    }
}

public class MyEntityBuilder
{
    private int _id;
    private string _name;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public MyEntityBuilder()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        // this method is partial so you can override setting initials in code...
    }

    public int Id()
    {
        return _id;
    }

    public MyEntityBuilder Id(int id)
    {
        _id = id;
        return this;
    }

    public string Name()
    {
        return _name;
    }

    public MyEntityBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public MyEntity Build()
    {
        return new MyEntity(_id, _name);
    }
}