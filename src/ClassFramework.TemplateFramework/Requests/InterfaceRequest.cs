namespace ClassFramework.TemplateFramework.Requests;

public class InterfaceRequest : IRequest<Result>
{
    public InterfaceRequest(InterfaceBuilder model, InterfaceContext context)
    {
        Guard.IsNotNull(model);
        Guard.IsNotNull(context);

        Model = model;
        Context = context;
    }

    public InterfaceBuilder Model { get; }
    public InterfaceContext Context { get; }
}
