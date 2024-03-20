namespace ClassFramework.Pipelines.Extensions;

public static class AttributeExtensions
{
    public static Domain.Attribute ConvertToDomainAttribute(this System.Attribute instance, Func<System.Attribute, Domain.Attribute> initializeDelegate)
    {
        initializeDelegate = initializeDelegate.IsNotNull(nameof(initializeDelegate));

        var prefilled = initializeDelegate(instance);

        return new AttributeBuilder()
            .WithName(prefilled.Name)
            .AddParameters(prefilled.Parameters.Select(x => x.ToBuilder()))
            .Build();
    }
}
