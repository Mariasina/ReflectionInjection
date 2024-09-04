using InjectionLib;

var manager = new DependenciesManager<RestController>();

var controller = manager.Get<AuthController>();
controller.SayHello();

public abstract class Service {}

public class Login : Service {

    [Injected]
    string NomeDoPrograma;

    [Injected]
    public JwtTokenManager tokenManager;
}

public class JwtTokenManager : Service {

    [Injected]
    string NomeDoPrograma;
}


[Configuration]
public class DependenciesConfiguration {

    [Bean]
    protected Login loginService() => new Login();

    [Bean]
    protected string NomeDoPrograma() => "RestAPI";

    [Bean]
    protected JwtTokenManager TokenManager() => new JwtTokenManager();

}


[RestController]
public class AuthController {

    [Injected]
    private string NomeDoPrograma;

    [Injected]
    Login loginService;

    public void SayHello()
    {
        Console.WriteLine($"This is a {NomeDoPrograma}");
        Console.WriteLine($"Meu serviço é o {loginService.GetType().Name}");
        Console.WriteLine($"e ele usa o {loginService.tokenManager.GetType().Name}");
    }
}