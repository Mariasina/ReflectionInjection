using System.Reflection;
using InjectionLib;

var manager = new DependenciesManager<RestController>();

var obama = manager.Get<President>();

obama.SayHello();


[Configuration]
public class DependenciesConfiguration {

    [Bean]
    public string NomeDoObama() => "Barack Hussein Obama II";

    [Bean]
    public int IdadeDoObama() => 63;

    public float PesoDoObama() => 75;
}


[RestController]
public class President {

    [Injected]
    public string NomeDoObama;

    public string MulherDoObama;

    public void SayHello()
    {
        Console.WriteLine($"Hi! my name is {NomeDoObama}");
    }
}