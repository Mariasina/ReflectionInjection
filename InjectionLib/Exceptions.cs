public class NoEmptyConstructorException : Exception 
{
    public NoEmptyConstructorException() 
        : base("Your configuration classes must have empty constructors.") {}
}

public class DuplicatedDepencyTypeException : Exception 
{
    public DuplicatedDepencyTypeException()
        : base("You cannot have two beans with the same return type.") {}
}

public class BeanNotFoundException : Exception
{
    public BeanNotFoundException()
        : base("The name speaks for itself") {}
}

public class InjectedWithoutSetMethod : Exception
{
    public InjectedWithoutSetMethod()
        : base("Your injecteds cannot be readonly, you must provide a set method") {}
}
