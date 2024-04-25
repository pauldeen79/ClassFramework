namespace ClassFramework.TemplateFramework.Requests;

public class BuilderExtensionRequest : IRequest<Result>
{
    public BuilderExtensionRequest(IConcreteTypeBuilder model, BuilderExtensionContext context)
    {
        Guard.IsNotNull(model);
        Guard.IsNotNull(context);

        Model = model;
        Context = context;
    }

    public IConcreteTypeBuilder Model { get; }
    public BuilderExtensionContext Context { get; }
}
