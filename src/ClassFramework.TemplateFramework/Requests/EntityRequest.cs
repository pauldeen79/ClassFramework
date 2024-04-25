namespace ClassFramework.TemplateFramework.Requests;

public class EntityRequest : IRequest<Result>
{
    public EntityRequest(ClassBuilder model, EntityContext context)
    {
        Guard.IsNotNull(model);
        Guard.IsNotNull(context);

        Model = model;
        Context = context;
    }

    public ClassBuilder Model { get; }
    public EntityContext Context { get; }
}
