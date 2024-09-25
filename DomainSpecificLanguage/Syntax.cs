namespace DomainSpecificLanguage;

public abstract class Token
{
	protected Token(int line, int column, Kind tokenKind)
	{
		Line = line;
		Column = column;
		TokenKind = tokenKind;
	}

	public int Column { get; }
	public Kind TokenKind { get; }

	public int Line { get; }

	public enum Kind
	{
		Identifier,
		Decimal,
		Integer,
		String,
		OpenBrace,
		CloseBrace,
		OpenBlock,
		CloseBlock,
		Reference,
		Property,
		Assignment,
		Comma,
		Quote
	}

	public override string ToString()
	{
        return $"{TokenKind} [{Line}, {Column}]";
    }
}

public abstract class SymbolToken : Token
{
    protected SymbolToken(char symbol, int line, int column, Kind tokenKind) : base(line, column, tokenKind)
    {
	    Symbol = symbol;
    }

    public char Symbol { get; }
}

public abstract class ValueToken : Token
{
    protected ValueToken(string value, int line, int column, Kind tokenKind) : base(line, column, tokenKind)
    {
        Value = value;
    }

    public string Value { get; }
}

public abstract class NumericToken<T> : ValueToken
{
    protected NumericToken(string value, int line, int column, Kind tokenKind) : base(value, line, column, tokenKind)
    {
    }

    public abstract T NumericValue { get; }
}

public class Syntax
{
	public class IdentifierToken : ValueToken
	{
		public IdentifierToken(string value, int line, int column) : base(value, line, column, Kind.Identifier)
		{
		}
	}

	public class DecimalToken : NumericToken<decimal>
	{
		public DecimalToken(string value, int line, int column) : base(value, line, column, Kind.Decimal)
        {
		}

        public override decimal NumericValue => decimal.Parse(Value);
    }

	public class IntegerToken : NumericToken<int>
    {
		public IntegerToken(string value, int line, int column) : base(value, line, column, Kind.Integer)
        {
		}

        public override int NumericValue => int.Parse(Value);
    }

	public class StringToken : ValueToken
	{
		public StringToken(string value, int line, int column) : base(value, line, column, Kind.String)
        {
		}
	}

	public class OpenBraceToken : SymbolToken
	{
		public OpenBraceToken(int line, int column) : base('{', line, column, Kind.OpenBrace)
		{
		}
	}

	public class CloseBraceToken : SymbolToken
	{
		public CloseBraceToken(int line, int column) : base('}', line, column, Kind.CloseBrace)
		{
		}
	}

	public class OpenBlockToken : SymbolToken
	{
		public OpenBlockToken(int line, int column) : base('[', line, column, Kind.OpenBlock)
		{
		}
	}

	public class CloseBlockToken : SymbolToken
	{
		public CloseBlockToken(int line, int column) : base(']', line, column, Kind.CloseBlock)
		{
		}
	}

	public class ReferenceToken : SymbolToken
	{
		public ReferenceToken(int line, int column) : base(':', line, column, Kind.Reference)
		{
		}
	}

	public class PropertyToken : SymbolToken
	{
		public PropertyToken(int line, int column) : base('|', line, column, Kind.Property)
		{
		}
	}

	public class AssignmentToken : SymbolToken
	{
		public AssignmentToken(int line, int column) : base('=', line, column, Kind.Assignment)
		{
		}
	}

	public class CommaToken : SymbolToken
	{
		public CommaToken(int line, int column) : base(',', line, column, Kind.Comma)
		{
		}
	}

	public class QuoteToken : SymbolToken
	{
		public QuoteToken(int line, int column) : base('"', line, column, Kind.Quote)
		{
		}
	}
}