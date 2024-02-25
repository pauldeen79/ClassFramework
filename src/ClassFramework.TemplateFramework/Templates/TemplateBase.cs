namespace ClassFramework.TemplateFramework.Templates;

public abstract class TemplateBase : ITemplateContextContainer
{
    private ITemplateContext _context = default!;
    public ITemplateContext Context
    {
        get
        {
            return _context;
        }
        set
        {
            _context = value;
            OnSetContext(value!);
        }
    }

    protected virtual void OnSetContext(ITemplateContext value)
    {
    }
}
