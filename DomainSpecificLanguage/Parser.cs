using System.Data;

namespace DomainSpecificLanguage;

public class Parser
{
	public List<ObjectDefinition> GetAllItems(string code)
	{
		return new List<ObjectDefinition>();
	}
}

public class DbLookupAttribute : Attribute
{
    public DbLookupAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}

public class BurgerItem : AutoItem
{
	public override string Name => "Burger";
	public override double Price { get; }
	public override double Weight { get; }

	public override IEnumerable<string> Properties 
		=> Ingredients
		.Select(x => x.Ingredients.SelectMany(y => y.Properties))
		.SelectMany(x => x)
		.Distinct();

    public override AutoItem[] Ingredients =>
	[
		RanceSauce,
        Cheese,
		Lettuce
    ];

	[DbLookup(typeof(RanceSauce))]
	public AutoItem RanceSauce { get; set; } = null!;

    [DbLookup(typeof(Cheese))] 
	public AutoItem Cheese { get; set; } = null!;

    [DbLookup(typeof(Lettuce))]
    public AutoItem Lettuce { get; set; } = null!;

    public ItemDescription Description
		=> new ItemDescription(
            """
			A delicious burger with {0}, {1}, {2}, and {3} sauce.
			"""
            , RanceSauce
            , Cheese
            , Lettuce);
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
	readonly AutoItem[] references;

	public ItemDescription(string description, params AutoItem[] references)
	{
		this.references = references;
		Description = description;
	}
    public string Description { get; set; }
}