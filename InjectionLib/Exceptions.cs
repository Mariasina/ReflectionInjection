public class NoEmptyConstructorException : Exception 
{
    public NoEmptyConstructorException() 
        : base("Your configuration classes must have empty constructors.") {}
}

public class DuplicatedDepencyTypeException : Exception 
{
    public DuplicatedDepencyTypeException()
        : base("You cannot have two beans with the same return type. How the fuck are we suposed to know which one to give you morons") {}
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

public class RootNotFoundException : Exception
{
    public RootNotFoundException()
        : base("That type hasn't been declared with the root attribute") {}
}
