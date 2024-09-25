namespace DomainSpecificLanguage;

public class InvalidSyntaxException : Exception
{
	public int Line { get; }
	public int Column { get; }

	public InvalidSyntaxException(string message, int line, int column) : 
		base(message + $" at line {line}, column {column}")
    {
		Line = line + 1;
		Column = column;
	}
}