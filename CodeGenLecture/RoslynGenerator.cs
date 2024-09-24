using System.Reflection;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGenLecture;

public class RoslynGenerator : CodeGenerator
{
	public override Assembly Generate(string code, Action<string> onOutput)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(code);
		var assemblyName = Path.GetRandomFileName();
		var compilation = CSharpCompilation.Create(assemblyName)
			.WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
			.AddReferences
				(Net80.References.All)
			.AddSyntaxTrees(syntaxTree);

		using var ms = new MemoryStream();
		var result = compilation.Emit(ms);
		if (!result.Success)
		{
			foreach (var diagnostic in result.Diagnostics)
			{
				onOutput(diagnostic.ToString());
			}
			return null;
		}

		ms.Seek(0, SeekOrigin.Begin);
		return Assembly.Load(ms.ToArray());
	}
}