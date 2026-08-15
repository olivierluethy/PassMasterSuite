using System.Security.Cryptography;
using System.Text;

namespace PassMasterSuite.Core;

public sealed class GeneratorOptions
{
    public int Length { get; set; } = 16;
    public bool UseLower { get; set; } = true;
    public bool UseUpper { get; set; } = true;
    public bool UseDigits { get; set; } = true;
    public bool UseSymbols { get; set; } = true;

    public bool AnyClassSelected => UseLower || UseUpper || UseDigits || UseSymbols;
}

/// <summary>
/// Generates a random password from the selected character classes using a cryptographically
/// secure RNG. Guarantees at least one character from each selected class, then shuffles.
/// </summary>
public static class PasswordGenerator
{
    public static string Generate(GeneratorOptions options)
    {
        if (!options.AnyClassSelected || options.Length <= 0)
            return string.Empty;

        var pools = new List<string>();
        if (options.UseLower) pools.Add(Charset.Lowercase);
        if (options.UseUpper) pools.Add(Charset.Uppercase);
        if (options.UseDigits) pools.Add(Charset.Digits);
        if (options.UseSymbols) pools.Add(Charset.Symbols);

        string all = string.Concat(pools);
        var chars = new List<char>(options.Length);

        // Guarantee one of each selected class (only if there's room).
        foreach (string pool in pools)
        {
            if (chars.Count >= options.Length) break;
            chars.Add(Pick(pool));
        }

        // Fill the remainder from the combined pool.
        while (chars.Count < options.Length)
            chars.Add(Pick(all));

        Shuffle(chars);
        return new string(chars.ToArray());
    }

    private static char Pick(string pool) => pool[RandomNumberGenerator.GetInt32(pool.Length)];

    private static void Shuffle(IList<char> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
