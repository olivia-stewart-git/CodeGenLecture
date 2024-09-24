namespace  CodeGenLecture;
public static class CodeSource
{
	public static string PrintCode(string printing) => $$"""
	                                                     using System;
	                                                     using System.IO;

	                                                     public static class Program
	                                                     {
	                                                     	public static void Main()
	                                                     	{
	                                                     		Console.WriteLine("{{printing}}");
	                                                     	}
	                                                     }
	                                                     """;
}