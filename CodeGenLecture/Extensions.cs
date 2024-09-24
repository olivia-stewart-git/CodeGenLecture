using System.Reflection;

public static class Extensions
{
	public static void ExecuteMain(this Assembly assembly)
	{
		var program = assembly.GetType("Program");
		var main = program?.GetMethod("Main");
		ArgumentNullException.ThrowIfNull(main);
		main.Invoke(null, null);
	}
}