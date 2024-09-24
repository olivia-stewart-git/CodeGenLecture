using CodeGenLecture;

Console .WriteLine("enter what you want to dynamically...");
var response = Console.ReadLine() ?? string.Empty;

new RoslynGenerator()
	.Generate(CodeSource.PrintCode(response), Console.WriteLine)
	.ExecuteMain();