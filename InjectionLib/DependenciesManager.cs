using System.Reflection;

namespace InjectionLib;

public class DependenciesManager<A> where A : Attribute
{
    private Dictionary<object, object> dependencies = new();
    private Dictionary<object, object> rootDependencies = new();

    public T Get<T>()
        => (T)rootDependencies[typeof(T)];

    public DependenciesManager()
    {
        var types = Assembly.GetEntryAssembly()!.GetTypes();

        var configs = FilterAttribute<ConfigurationAttribute>(types);
        ConfigureBeans(configs);

        var roots = FilterAttribute<A>(types);
        foreach (var root in roots)
        {
            var constructor = root.GetConstructor(Array.Empty<Type>()) 
                ?? throw new NoEmptyConstructorException();
            
            var instance = constructor.Invoke(Array.Empty<object>());
            
            var injectedFields = root.GetRuntimeFields()
                .Where(property => property.GetCustomAttributes()
                    .Any(attribute => attribute is InjectedAttribute)
                );
            
            foreach (var field in injectedFields)
                field.SetValue(instance, dependencies[field.FieldType]);
            
            if(!rootDependencies.TryAdd(root, instance))
                throw new DuplicatedDepencyTypeException();
        }
    }

    private static IEnumerable<Type> FilterAttribute<T>(Type[] types)
        where T : Attribute
    {
        return types.Where(type => type.GetCustomAttributes()
            .Any(attribute => attribute is T)
        );
    }

    private void ConfigureBeans(IEnumerable<Type> configs)
    {
        foreach (var config in configs)
        {
            var beans = config
                .GetRuntimeMethods()
                .Where(method => method.GetCustomAttributes()
                    .Any(attribute => attribute is BeanAttribute)
                );

            var configInstance = config
                .GetConstructor(Array.Empty<Type>())?
                .Invoke(Array.Empty<object>()) 
                ?? throw new NoEmptyConstructorException();

            foreach (var bean in beans)
            {
                var result = bean.Invoke(configInstance, Array.Empty<object>()) 
                ?? throw new NullReferenceException(); 

                if(!dependencies.TryAdd(bean.ReturnType, result))
                    throw new DuplicatedDepencyTypeException();
            }
        }
    }
}