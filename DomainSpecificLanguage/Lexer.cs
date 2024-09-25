using System.Text;

namespace DomainSpecificLanguage;

public class Lexer : IDisposable
{
	readonly TextReader _reader;
	readonly string _text;

	int lineNumber;
	int columnNumber;
	string currentLine = string.Empty;

	public Lexer(string text)
	{
		_text = text;
		_reader = new StringReader(text);
	}

	public IEnumerable<Token> Tokenize()
	{
		var tokens = new List<Token>();
		while (_reader.Peek() != -1)
		{
			char c;
			while (char.IsWhiteSpace(c = (char)_reader.Peek())
				|| c == '\t')
			{
				Read();
			}

			if (_reader.Peek() == -1)
				break;

			c = (char)_reader.Peek();
			switch (c)
			{
				case ':':
					tokens.Add(new Syntax.ReferenceToken(lineNumber, columnNumber));
					Read();
					break;
				case ',':
					tokens.Add(new Syntax.CommaToken(lineNumber, columnNumber));
					Read();
					break;
				case '{':
					tokens.Add(new Syntax.OpenBraceToken(lineNumber, columnNumber));
					Read();
					break;
				case '}':
					tokens.Add(new Syntax.CloseBraceToken(lineNumber, columnNumber));
					Read();
					break;
				case '[':
					tokens.Add(new Syntax.OpenBlockToken(lineNumber, columnNumber));
                    Read();
                    tokens.Add(ParseIdentifier());
					Read();
					break;
				case ']':
					tokens.Add(new Syntax.CloseBlockToken(lineNumber, columnNumber));
					Read();
					break;
				case '"':
					tokens.Add(new Syntax.QuoteToken(lineNumber, columnNumber));
					Read();
                    var stringTkn = ParseString();
					if (stringTkn != null)
					{
						tokens.Add(stringTkn);
					}
					Read();

                    break;
				case '=':
					tokens.Add(new Syntax.AssignmentToken(lineNumber, columnNumber));
					Read();
					break;
				case '|':
					tokens.Add(new Syntax.PropertyToken(lineNumber, columnNumber));
					Read();
					break;
				default:
					if (char.IsDigit(c))
					{
						tokens.Add(ParseNumber());
						Read();
					}
					else
					{
						var remainingText = _reader.ReadToEnd() ?? string.Empty;
						throw new InvalidSyntaxException(@$"Unknown grammar found at position {_text.Length - remainingText.Length}" + GetLinePositionMessage(), lineNumber, columnNumber);
					}
					break;
			}
		}
		return tokens;
	}

	Token ParseNumber()
	{
		var isDecimal = false;
		var sb = new StringBuilder();
		while (char.IsDigit((char)_reader.Peek()) || (char)_reader.Peek() == '.')
		{
			if ((char)_reader.Peek() == '.')
			{
				if (isDecimal)
				{
					throw new InvalidSyntaxException(@"Invalid number format" + GetLinePositionMessage(), lineNumber, columnNumber);
				}
				isDecimal = true;
			}
			sb.Append((char)Read());
		}

		if (isDecimal)
		{
			return new Syntax.DecimalToken(sb.ToString(), lineNumber, columnNumber);
		}

		return new Syntax.IntegerToken(sb.ToString(), lineNumber, columnNumber);
	}

	bool stringOpen;
	Token? ParseString()
	{
		stringOpen = !stringOpen;
		if (!stringOpen)
			return null;

		var sb = new StringBuilder();
		char next;
		while (char.IsLetterOrDigit(next = (char)_reader.Peek())
			|| allowedStringChars.Contains(next)
			|| next == ' '
			)
		{
			sb.Append((char)Read());
		}
		if (next != '"')
		{
			throw new InvalidSyntaxException($"Expected '\"' but found '{(char)next}'" + GetLinePositionMessage(), lineNumber, columnNumber);
		}
		return new Syntax.StringToken(sb.ToString(), lineNumber, columnNumber);
	}

	Token ParseIdentifier()
	{
		var sb = new StringBuilder();
		char next;
        while (char.IsLetterOrDigit(next = (char)_reader.Peek())
			|| next == '_'
            || next == '-'
            || next == '.'
        )
		{
			sb.Append((char)Read());
		} 
		if (next != ']')
		{
			throw new InvalidSyntaxException($"Expected ']' but found '{(char)next}'" + GetLinePositionMessage(), lineNumber, columnNumber);
		}
		return new Syntax.IdentifierToken(sb.ToString(), lineNumber, columnNumber);
	}

	int Read()
	{
		var c = _reader.Read();
		if (c == '\n')
		{
			currentLine = string.Empty;
			lineNumber++;
			columnNumber = 0;
		}
		else
		{
            currentLine += (char)c;
            columnNumber++;
		}
		return c;
	}

	string GetLinePositionMessage()
	{
		var completeLine = _reader.ReadLine();
		if (completeLine == null)
            return currentLine;
        var substring = completeLine[(currentLine.Length - 1)..];

        var totalLine = currentLine + " ^ " + substring;
		var topPointer = new string(' ', currentLine.Length) + "^";
        return Environment.NewLine + totalLine + Environment.NewLine + topPointer;
    }

	public void Dispose()
	{
		_reader.Dispose();
	}

	const string allowedStringChars = ".-_@?><!#$%^&*()_+, ";
}