using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGeneratorExample
{
	[Generator]
	public class RangeValidationSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			// No initialization required for now
		}

		public void Execute(GeneratorExecutionContext context)
		{
			var syntaxTrees = context.Compilation.SyntaxTrees;

			foreach (var syntaxTree in syntaxTrees)
			{
				var root = syntaxTree.GetRoot();
				var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

				foreach (var classDeclaration in classDeclarations)
				{
					// Find properties in this class with the RangeValidation attribute
					var propertiesWithRange = classDeclaration.Members
						.OfType<PropertyDeclarationSyntax>()
						.Where(prop => prop.AttributeLists
							.SelectMany(attrList => attrList.Attributes)
							.Any(attr => attr.Name.ToString() == "RangeValidation"));

					if (!propertiesWithRange.Any())
						continue;

					var className = classDeclaration.Identifier.Text;
					var namespaceDeclaration = classDeclaration.Ancestors()
						.OfType<NamespaceDeclarationSyntax>()
						.FirstOrDefault()?.Name.ToString() ?? "GlobalNamespace";

					var sourceBuilder = new StringBuilder()
						.AppendLine($"namespace {namespaceDeclaration}")
						.AppendLine("{")
						.AppendLine($"    public partial class {className}")
						.AppendLine("    {");

					sourceBuilder.AppendLine("        public void ValidateAll()")
						.AppendLine("        {");

					//foreach (var property in propertiesWithRange)
					//{
						//var propertyName = property.Identifier.Text;
						//sourceBuilder.AppendLine($"            Validate{propertyName}();");
					//}

					sourceBuilder.AppendLine("        }");

					//CreateValidationMethod(propertiesWithRange, sourceBuilder);

					sourceBuilder.AppendLine("    }");
					sourceBuilder.AppendLine("}");

					context.AddSource($"{className}Validation.g.cs", sourceBuilder.ToString());
				}
			}
		}

		static void CreateValidationMethod(IEnumerable<PropertyDeclarationSyntax> propertiesWithRange, StringBuilder sourceBuilder)
		{
			foreach (var property in propertiesWithRange)
			{
				var propertyName = property.Identifier.Text;

				sourceBuilder.AppendLine($"       public void Validate{propertyName}()")
							 .AppendLine("        {");

				var rangeAttribute = property.AttributeLists
					.SelectMany(attrList => attrList.Attributes)
					.FirstOrDefault(attr => attr.Name.ToString() == "RangeValidation");

				if (rangeAttribute != null)
				{
					var minArgument = rangeAttribute.ArgumentList.Arguments[0].ToString();
					var maxArgument = rangeAttribute.ArgumentList.Arguments[1].ToString();

					sourceBuilder.AppendLine($"            if(this.{propertyName} < {minArgument} && this.{propertyName} > {maxArgument};)")
						.AppendLine($"                throw new ArgumentOutOfRangeException(nameof({propertyName}), \"Value must be between {minArgument} and {maxArgument}\");");
				}

				sourceBuilder.AppendLine("        }");
			}
		}
	}
}
