using CodeGenLecture;

Console .WriteLine("starting...");

new CmdLineProcessGenerator()
	.Generate(CodeSource.HelloWorld, Console.WriteLine)
	.ExecuteMain();