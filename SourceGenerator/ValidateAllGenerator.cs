using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator;

[Generator]
public class ValidateAllGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			return;

		foreach (var classSymbol in receiver.CandidateClasses)
		{
			var semanticModel = context.Compilation.GetSemanticModel(classSymbol.SyntaxTree);
			if (semanticModel.GetDeclaredSymbol(classSymbol) is not INamedTypeSymbol classTypeSymbol)
				continue;

			var source = GeneratePartialClass(classTypeSymbol);
			
			context.AddSource($"{classTypeSymbol}_ValidateAll.g.cs", SourceText.From(source, Encoding.UTF8));
		}
	}

	string GeneratePartialClass(INamedTypeSymbol classSymbol)
	{
		var className = classSymbol.Name;
		var namespaceName = classSymbol.ContainingNamespace?.ToDisplayString();
		if (namespaceName is null)
            return "SourceGenerator";
		var methodBuilder = new StringBuilder();
        return $@"
using System;

namespace {namespaceName}
{{
    public partial class {className}
    {{
		public void ValidateAll()
		{{
{CodeForPropertyValidation(classSymbol, methodBuilder)}
		}}{methodBuilder}
    }}
}}";
	}

	string CodeForPropertyValidation(INamedTypeSymbol classSymbol, StringBuilder methodBuilder)
	{
        var properties = classSymbol.GetMembers()
            .Where(member => member.Kind == SymbolKind.Property)
            .Select(member => member as IPropertySymbol)
            .Where(property => property is not null);

        var code = new StringBuilder();
        foreach (var property in properties)
        {
            var attributes = property.GetAttributes();
            var maxStringLengthAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass?.Name == "MaxStringLengthAttribute");
            if (maxStringLengthAttribute is not null)
            {
                var maxLength = maxStringLengthAttribute.ConstructorArguments[0].Value;
				var codeForMethodCall = "Validate" + property.Name + "()";
				code.AppendLine("			" + codeForMethodCall + ";");

                methodBuilder.AppendLine($@"
		public void {codeForMethodCall}
		{{
			if ({property.Name}.Length > {maxLength})
			{{
				throw new Exception(""{property.Name} exceeds the maximum length of {maxLength}"");
			}}
		}}");
            }
        }

        return code.ToString();
    }

	class SyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> CandidateClasses { get; } = new ();

		public void OnVisitSyntaxNode(SyntaxNode context)
		{
			// Look for class declarations with the [RequiresValidation] attribute
			if (context is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classSyntax &&
				classSyntax.AttributeLists
					.SelectMany(list => list.Attributes)
					.Any(attr => attr.Name.ToString() == "RequiresValidation"))
			{
				CandidateClasses.Add(classSyntax);
			}
		}
	}
}