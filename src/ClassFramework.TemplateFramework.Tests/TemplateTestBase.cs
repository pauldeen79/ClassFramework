namespace ClassFramework.TemplateFramework.Tests;

/// <summary>
/// Base class for Template/Generator unit tests. This base class uses plain acvitation, so without assigning fixtures to properties.
/// </summary>
/// <typeparam name="T">Template/Generator type</typeparam>
public class TemplateTestBase<T> : TestBase
    where T : new()
{
    protected static T CreateSut() => new T();

    protected static ITemplateContext CreateContext(ITemplateEngine engine, object template)
    {
        var context = Substitute.For<ITemplateContext>();
        context.Engine.Returns(engine);
        var parentContext = Substitute.For<ITemplateContext>();
        parentContext.ParentContext.Returns(default(ITemplateContext));
        parentContext.RootContext.Returns(parentContext);
        parentContext.Model.Returns(new ClassBuilder().WithName("MyClass").Build());
        parentContext.Template.Returns(template); // important to fill because otherwise you will get NullReferenceExceptions :)
        context.ParentContext.Returns(parentContext);
        context.RootContext.Returns(parentContext);
        return context;
    }
}
