namespace DomainSpecificLanguage;

public class ObjectDefinition
{
	public string Name { get; set; }
	public string Definition { get; set; }
    public List<ObjectDefinition> DefinitionReferences { get; set; } = new();

	public List<ObjectDefinition> Ingredients { get; set; } = new();
}