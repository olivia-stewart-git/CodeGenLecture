using System.Collections;
using System.Text;

namespace DomainSpecificLanguage;

public class ItemGenerator
{
	readonly ObjectDefinition definition;

	public ItemGenerator(ObjectDefinition definition)
	{
		this.definition = definition;
	}

	public string SourceCode => LinesOfCode(
		CodeForUsingDeclaration,
		string.Empty,
		CodeForNamespace,
		CodeForClassDeclaration,
		"{",
		"}"
		);

	string CodeForUsingDeclaration => "using EntityFramework.Abstractions";

	string CodeForNamespace => "namespace EntityFramework.Definitions;";

	string CodeForClassDeclaration => $"public class {definition.Name}Item : AutoItem";

	#region Loc
	public static string LinesOfCode(params string?[] lines)
	{
		return NewLineSeparatedText(lines) ?? string.Empty;
	}

	public static string? NewLineSeparatedText(IEnumerable text)
	{
		var result = new StringBuilder();
		var hasValue = false;
		foreach (string line in text)
		{
			if (line != null)
			{
				if (hasValue)
				{
					result.AppendLine();
				}
				result.Append(line);
				hasValue = true;
			}
		}

		return hasValue ? result.ToString() : null;
	}

	#endregion
}