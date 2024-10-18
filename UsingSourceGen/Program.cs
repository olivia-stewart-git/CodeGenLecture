public class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.WriteLine("Enter your name:");
            var name = Console.ReadLine();
            Console.WriteLine(name);
            Console.WriteLine("Enter your age:");
            var age = int.Parse(Console.ReadLine());
            Console.WriteLine(age);

            var person = new Person { Name = name, Age = age };
            person.ValidateAll();
        }
    }
}

public partial class Person
{
    public string Name { get; set; }

    [RangeValidation(18, 65)]
    public int Age { get; set; }
}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class RangeValidationAttribute : Attribute
{
	public int Min { get; }
	public int Max { get; }

	public RangeValidationAttribute(int min, int max)
	{
		Min = min;
		Max = max;
	}
}