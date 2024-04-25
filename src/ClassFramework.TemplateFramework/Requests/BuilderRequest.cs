namespace ClassFramework.TemplateFramework.Requests;

public class BuilderRequest : IRequest<Result>
{
    public BuilderRequest(ClassBuilder model, BuilderContext context)
    {
        Guard.IsNotNull(model);
        Guard.IsNotNull(context);

        Model = model;
        Context = context;
    }

    public ClassBuilder Model { get; }
    public BuilderContext Context { get; }
}
