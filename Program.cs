var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
Console.WriteLine("Hi");
Console.WriteLine("Hi2");
app.Run();

public static class Test
{
    public static int add(int n1, int n2)
    {
        return n1 + n2;
    }
}