using InjectionLib;

var manager = new DependenciesManager<RestController>();

var obama = manager.Get<President>();

obama.SayHello();

public abstract class Animal {}
public class Cachorro : Animal {}


[Configuration]
public class DependenciesConfiguration {

    [Bean]
    private string NomeDoObama() => "Barack Hussein Obama II";

    [Bean]
    public int IdadeDoObama() => 63;

    [Bean]
    protected Animal Animal() => new Cachorro();

    public float PesoDoObama() => 75;
}


public class President {

    [Injected]
    private string NomeDoObama;

    public string MulherDoObama;

    [Injected]
    Animal PresidentialPet;

    public void SayHello()
    {
        Console.WriteLine($"Hi! my name is {NomeDoObama}");
        Console.WriteLine($"Meu pet é um {PresidentialPet.GetType().Name}");
    }
}