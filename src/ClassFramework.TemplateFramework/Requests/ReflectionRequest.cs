namespace ClassFramework.TemplateFramework.Requests;

public class ReflectionRequest : IRequest<Result>
{
    public ReflectionRequest(TypeBaseBuilder model, ReflectionContext context)
    {
        Guard.IsNotNull(model);
        Guard.IsNotNull(context);

        Model = model;
        Context = context;
    }

    public TypeBaseBuilder Model { get; }
    public ReflectionContext Context { get; }
}
