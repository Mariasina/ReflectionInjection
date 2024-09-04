using System.Collections;
using System.Reflection;

namespace InjectionLib;

public class DependenciesManager<R> where R : Attribute
{
    private Dictionary<Type, MethodInfo> dependencies = new();
    private Dictionary<MethodInfo, object> configurations = new();
    private Dictionary<object, object> rootDependencies = new();

    public T Get<T>()
    {
        try
        {
            return (T)rootDependencies[typeof(T)];
        }
        catch (Exception)
        {
            throw new RootNotFoundException();
        }   
    }

    public DependenciesManager()
    {
        var types = Assembly.GetEntryAssembly()!.GetTypes();

        var configs = FilterAttribute<ConfigurationAttribute>(types);
        ConfigureBeans(configs);

        var roots = FilterAttribute<R>(types);
        SetUpRoots(roots);
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
                if(!dependencies.TryAdd(bean.ReturnType, bean))
                    throw new DuplicatedDepencyTypeException();
                configurations.Add(bean, configInstance);
            }
        }
    }

    private void SetUpRoots(IEnumerable<Type> roots)
    {
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
                field.SetValue(instance, BuildBean(field.FieldType));

            if(!rootDependencies.TryAdd(root, instance))
                throw new DuplicatedDepencyTypeException();
        }
    }

    private object BuildBean(Type type)
    {
        var dp = dependencies[type];
        var bean = dp.Invoke(configurations[dp], Array.Empty<object>())!;

        var injectedFields = bean.GetType().GetRuntimeFields()
            .Where(property => property.GetCustomAttributes()
                .Any(attribute => attribute is InjectedAttribute)
            );

        foreach (var field in injectedFields)
        {
            field.SetValue(bean, BuildBean(field.FieldType));
        }

        return bean;
    }
}