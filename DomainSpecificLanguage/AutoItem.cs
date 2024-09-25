namespace DomainSpecificLanguage;

public abstract class AutoItem
{
	public abstract string Name { get; }
	public abstract double Price { get; }
	public abstract double Weight { get; }

    public abstract IEnumerable<string> Properties { get; }
	public abstract AutoItem[]? Ingredients { get; }
}