namespace ClassFramework.Pipelines.Extensions;

public static class TypeBuilderExtensions
{
    public static T AddObservableMembers<T>(this T instance)
        where T : ITypeBuilder
    {
        return instance
            .AddFields(new FieldBuilder()
                .WithName(nameof(INotifyPropertyChanged.PropertyChanged))
                .WithType(typeof(PropertyChangedEventHandler))
                .WithEvent()
                .WithIsNullable()
                .WithVisibility(Visibility.Public)
            )
            .AddMethods(new MethodBuilder()
                .WithName("HandlePropertyChanged")
                .AddParameter("propertyName", typeof(string))
                .WithProtected()
                .AddStringCodeStatements("PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));")
            );
    }
}
