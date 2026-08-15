using System.Text;

namespace PassMasterSuite.Core;

/// <summary>
/// Describes which character classes a password draws on and how large the resulting
/// search space (alphabet) is. Shared by the Checker (entropy) and the Cracker (keyspace).
/// </summary>
public sealed class Charset
{
    public const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    public const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string Digits = "0123456789";
    // Common printable symbols reachable on a standard keyboard.
    public const string Symbols = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

    public bool HasLower { get; }
    public bool HasUpper { get; }
    public bool HasDigit { get; }
    public bool HasSymbol { get; }

    /// <summary>The ordered alphabet the attacker would enumerate (lower, upper, digits, symbols).</summary>
    public string Alphabet { get; }

    /// <summary>Number of distinct symbols in the alphabet.</summary>
    public int Size => Alphabet.Length;

    private Charset(bool lower, bool upper, bool digit, bool symbol, string alphabet)
    {
        HasLower = lower; HasUpper = upper; HasDigit = digit; HasSymbol = symbol;
        Alphabet = alphabet;
    }

    public static Charset Analyze(string password)
    {
        bool lower = false, upper = false, digit = false, symbol = false;
        foreach (char c in password)
        {
            if (c is >= 'a' and <= 'z') lower = true;
            else if (c is >= 'A' and <= 'Z') upper = true;
            else if (c is >= '0' and <= '9') digit = true;
            else symbol = true;
        }

        var sb = new StringBuilder();
        if (lower) sb.Append(Lowercase);
        if (upper) sb.Append(Uppercase);
        if (digit) sb.Append(Digits);
        if (symbol) sb.Append(Symbols);

        return new Charset(lower, upper, digit, symbol, sb.ToString());
    }
}
