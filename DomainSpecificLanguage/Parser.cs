using System.Collections;
using System.Data;
using System.Text;

namespace DomainSpecificLanguage;

public class Parser
{
	public List<ObjectDefinition> GetAllItems(string code)
	{
		// Parse the code and get all items
		var reader = new StringReader(code);
		RawObjectDefinition? currentDefinition = null;
		List<RawObjectDefinition> foundDefinitions = new();
        while (reader.Peek() != -1)
        {
			var line = reader.ReadLine() ?? string.Empty;
			if (line.StartsWith(SyntaxToken.ObjectDelimiter))
			{
				if (currentDefinition != null)
                {
                    foundDefinitions.Add(currentDefinition);
                }

                currentDefinition = new RawObjectDefinition
                {
                    Name = line.Substring(SyntaxToken.ObjectDelimiter.Length).Trim()
                }; 
            }
			else if (currentDefinition != null && !string.IsNullOrWhiteSpace(line))
            {
                currentDefinition.Lines.Add(line);
			}
        }

		return GetDeepDefinitionValues(foundDefinitions.DistinctBy(x => x.Name));
    }

	List<ObjectDefinition> GetDeepDefinitionValues(IEnumerable<RawObjectDefinition> rawDefinitions)
	{
		var lookup = rawDefinitions.ToDictionary(x => x.Name, x => x);
		var result = new List<ObjectDefinition>();
		foreach (var rawDefinition in lookup.Values)
		{

		}
        return result;
    }
}

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

public class DbLookupAttribute : Attribute
{
    public DbLookupAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}

public class Item : AutoItem
{
	public override string Name => "Burger";

	public override IEnumerable<string> Properties 
		=> Ingredients
		.Select(x => x.Ingredients.SelectMany(y => y.Properties))
		.SelectMany(x => x)
		.Distinct();

    public override Item[] Ingredients =>
	[
		RanceSauce,
        Cheese,
		Lettuce
    ];

	[DbLookup(typeof(RanceSauce))]
	public Item RanceSauce { get; set; } = null!;

    [DbLookup(typeof(Cheese))] 
	public Item Cheese { get; set; } = null!;

    [DbLookup(typeof(Lettuce))]
    public Item Lettuce { get; set; } = null!;

    public ItemDescription Description
		=> new ItemDescription(
            """
			A delicious burger with {0}, {1}, tomato, and rance sauce.
			""");
}

public class RanceSauce
{
}
public class Cheese
{
}
public class Lettuce
{
}

public class ItemDescription
{
	readonly Item[] references;

	public ItemDescription(string description, params Item[] references)
	{
		this.references = references;
		Description = description;
	}
    public string Description { get; set; }
}