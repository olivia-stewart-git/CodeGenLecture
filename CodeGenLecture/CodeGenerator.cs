using System.Reflection;

namespace CodeGenLecture;

public abstract class CodeGenerator
{
	public abstract Assembly Generate(string code, Action<string> onOutput);
}