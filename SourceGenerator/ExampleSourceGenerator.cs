using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator
{
	[Generator]
	public class ExampleSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
		}

		public void Execute(GeneratorExecutionContext context)
		{
			var sourceBuilder = new StringBuilder("""
			                                      using System;
			                                      namespace BuildConstants
			                                      {
			                                          public static class BuildConstants
			                                          {
			                                              public const int MyValue = 10;
			                                          }
			                                      }
			                                      """);

			// inject the created ExampleSourceGenerator into the users compilation
			context.AddSource("ExampleSourceGenerator.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
