using System.Reflection;

namespace DomainSpecificLanguage;

public class Program
{
    public static void Main()
    {
        var exampleResource = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(x => x.Contains("example.txt"));
        var exampleStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(exampleResource);
        var exampleReader = new StreamReader(exampleStream);
        var exampleText = exampleReader.ReadToEnd();

        using var lexer = new Lexer(exampleText);
        var tokens = lexer.Tokenize().ToList();
        foreach (var token in tokens)
        {
            Console.WriteLine(token.ToString());
        }
    }
}