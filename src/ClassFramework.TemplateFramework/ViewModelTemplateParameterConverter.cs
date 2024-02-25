namespace ClassFramework.TemplateFramework;

public class ViewModelTemplateParameterConverter : ITemplateParameterConverter
{
    private readonly Func<IEnumerable<IViewModel>> _factory;

    public ViewModelTemplateParameterConverter(Func<IEnumerable<IViewModel>> factory)
    {
        Guard.IsNotNull(factory);

        _factory = factory;
    }

    public bool TryConvert(object? value, Type type, ITemplateEngineContext context, out object? convertedValue)
    {
        if (value is null)
        {
            convertedValue = null;
            return false;
        }

        var viewModelItem = _factory.Invoke()
            .Select(viewModel => new { ViewModel = viewModel, ModelProperty = viewModel.GetType().GetProperty(nameof(IModelContainer<object>.Model)) })
            .FirstOrDefault(x => x.ModelProperty is not null && x.ModelProperty.PropertyType.IsInstanceOfType(value));

        if (viewModelItem is null)
        {
            convertedValue = null;
            return false;
        }

        // Copy Model to ViewModel
        if (viewModelItem.ModelProperty!.GetValue(viewModelItem.ViewModel) is null)
        {
            viewModelItem.ModelProperty.SetValue(viewModelItem.ViewModel, value);
        }

        convertedValue = viewModelItem.ViewModel;
        return true;
    }
}
