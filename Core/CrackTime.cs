using System.Numerics;

namespace PassMasterSuite.Core;

/// <summary>
/// Analytic brute-force crack-time model. Crack time is computed from the keyspace, never by
/// iterating it: keyspace = alphabetSize ^ length; on average an attacker tries half of it.
/// </summary>
public static class CrackTime
{
    /// <summary>Total number of candidates of the given length over the alphabet.</summary>
    public static BigInteger Keyspace(int alphabetSize, int length)
    {
        if (alphabetSize <= 0 || length <= 0) return BigInteger.Zero;
        return BigInteger.Pow(alphabetSize, length);
    }

    /// <summary>Average seconds to crack: (keyspace / 2) / guessesPerSecond. May be +Infinity.</summary>
    public static double AverageSeconds(int alphabetSize, int length, double guessesPerSecond)
    {
        if (guessesPerSecond <= 0) return double.PositiveInfinity;
        BigInteger keyspace = Keyspace(alphabetSize, length);
        if (keyspace.IsZero) return 0;

        // (double) on an out-of-range BigInteger yields +Infinity, which the formatter handles.
        double ks = (double)keyspace;
        return ks / 2.0 / guessesPerSecond;
    }

    /// <summary>The 0-based index of <paramref name="value"/> in canonical enumeration order over
    /// <paramref name="alphabet"/>, treating the string as a fixed-length base-N number
    /// (leftmost character is most significant). Returns null if any character is not in the alphabet.</summary>
    public static BigInteger? IndexOf(string value, string alphabet)
    {
        BigInteger index = BigInteger.Zero;
        int n = alphabet.Length;
        foreach (char c in value)
        {
            int d = alphabet.IndexOf(c);
            if (d < 0) return null;
            index = index * n + d;
        }
        return index;
    }

    /// <summary>Builds the fixed-length candidate string at <paramref name="index"/> in canonical order.</summary>
    public static string Candidate(BigInteger index, int length, string alphabet)
    {
        int n = alphabet.Length;
        var buffer = new char[length];
        for (int i = length - 1; i >= 0; i--)
        {
            index = BigInteger.DivRem(index, n, out BigInteger rem);
            buffer[i] = alphabet[(int)rem];
        }
        return new string(buffer);
    }
}
