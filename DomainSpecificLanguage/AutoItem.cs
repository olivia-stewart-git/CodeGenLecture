namespace DomainSpecificLanguage;

public abstract class AutoItem
{
	public abstract string Name { get; }
	public abstract IEnumerable<string> Properties { get; }
	public abstract Item[]? Ingredients { get; }
}