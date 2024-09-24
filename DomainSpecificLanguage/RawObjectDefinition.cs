namespace DomainSpecificLanguage;

public class RawObjectDefinition
{
	public required string Name { get; init; }
	public List<string> Lines { get; set; } = new();
}