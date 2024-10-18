
using System.Text;

var objectDefinition = new ObjectDefinition("Person", new[]
{
    new ColumnDefinition("Id", typeof(int), 0),
    new ColumnDefinition("Name", typeof(string), null),
    new ColumnDefinition("Age", typeof(int), 0),
});

var generator = new ObjectCodeGenerator(objectDefinition);
var code = generator.GenerateCode();
Console.WriteLine(code);

public class ObjectCodeGenerator(ObjectDefinition definition)
{
    public string GenerateCode()
    {
        var code = new StringBuilder();
        code.AppendLine($"public class {definition.Name}Model");
        code.AppendLine("{");

        //Code for name
        code.AppendLine($"   public string Name => \"{definition.Name}\";");
        code.AppendLine();

        //create column dictionary
        code.AppendLine("   public Dictionary<string, object> Columns { get; } = new()");
        code.AppendLine("   {");
        foreach (var column in definition.Columns)
        {
            code.AppendLine($"      {{ \"{column.Name}\", new ColumnInstance(\"{column.Name}\", typeof({column.Type.Name}), {column.DefaultValue}) }},");
        }
        code.AppendLine("   };");
        code.AppendLine();

        //Code for column properties
        foreach (var column in definition.Columns)
        {
            code.AppendLine($"  public ColumnInstance {column.Name} => Columns[\"{column.Name}\"].Value;");
            code.AppendLine();
        }
        code.AppendLine("}");
        return code.ToString();
    }
}

public class ObjectDefinition
{
    public ObjectDefinition(string name, ColumnDefinition[] columns)
    {
        Name = name;
        Columns = columns;
    }
    public string Name { get; set; }
    public ColumnDefinition[] Columns { get; set; }
}

public class ColumnDefinition
{
    public ColumnDefinition(string name, Type type, object? defaultValue)
    {
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
    }
    public string Name { get; set; }
    public Type Type { get; set; }
    public object? DefaultValue { get; set; }
}