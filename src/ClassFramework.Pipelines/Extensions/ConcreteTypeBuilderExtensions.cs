namespace ClassFramework.Pipelines.Extensions;

public static class ConcreteTypeBuilderExtensions
{
    public static void AddObservableMembers(this IConcreteTypeBuilder instance)
    {
        instance
            .AddFields(new FieldBuilder()
            .WithName(nameof(INotifyPropertyChanged.PropertyChanged))
            .WithType(typeof(PropertyChangedEventHandler))
            .WithEvent()
            .WithIsNullable()
            .WithVisibility(Visibility.Public)
        );

        instance
            .AddMethods(new MethodBuilder()
                .WithName("HandlePropertyChanged")
                .AddParameter("propertyName", typeof(string))
                .WithProtected()
                .AddStringCodeStatements("PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));")
            );
    }
}
