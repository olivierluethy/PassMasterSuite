namespace PassMasterSuite.Core;

public sealed class StrengthResult
{
    /// <summary>0 = very weak … 4 = very strong.</summary>
    public int Level { get; init; }
    public string Rating { get; init; } = "";
    public double EntropyBits { get; init; }
    public int AlphabetSize { get; init; }
    public int Length { get; init; }
    public bool IsEmpty { get; init; }

    // Criteria the meter reflects.
    public bool HasLength { get; init; }   // 12+ characters
    public bool HasLower { get; init; }
    public bool HasUpper { get; init; }
    public bool HasDigit { get; init; }
    public bool HasSymbol { get; init; }

    public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Suggestions { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Evaluates password strength from realized-alphabet entropy, then applies penalties for common
/// passwords and simple, guessable patterns (repeats, sequences, keyboard walks, bare years).
/// </summary>
public static class PasswordStrength
{
    // A compact list of the most abused passwords — enough to flag the obvious ones.
    private static readonly HashSet<string> Common = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "password1", "passw0rd", "123456", "12345678", "123456789", "1234567890",
        "qwerty", "qwertz", "azerty", "abc123", "111111", "000000", "iloveyou", "admin",
        "welcome", "monkey", "dragon", "letmein", "login", "princess", "sunshine", "football",
        "master", "hello", "freedom", "whatever", "trustno1", "superman", "batman", "starwars",
        "qwerty123", "1q2w3e4r", "zaq12wsx", "asdfgh", "qazwsx", "michael", "shadow", "test",
    };

    private static readonly string[] KeyboardWalks =
        { "qwerty", "qwertz", "azerty", "asdf", "asdfgh", "zxcvbn", "qazwsx", "1q2w3e", "1234", "qweasd" };

    public static StrengthResult Evaluate(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return new StrengthResult
            {
                IsEmpty = true, Level = 0, Rating = "No password yet",
                Suggestions = new[] { "Type a password to see how strong it is." },
            };
        }

        var cs = Charset.Analyze(password);
        int len = password.Length;
        double entropy = cs.Size > 1 ? len * Math.Log2(cs.Size) : 0;

        var warnings = new List<string>();
        var suggestions = new List<string>();

        bool isCommon = Common.Contains(password);
        if (isCommon) warnings.Add("This is one of the most common passwords — it would be tried first.");

        if (IsSingleCharRepeat(password)) warnings.Add("It repeats a single character.");
        if (IsSequential(password)) warnings.Add("It's a simple sequence like \"1234\" or \"abcd\".");
        if (LooksLikeKeyboardWalk(password)) warnings.Add("It follows a keyboard pattern.");
        if (IsBareYear(password)) warnings.Add("A lone year is easy to guess.");

        // Base level from entropy.
        int level = entropy switch
        {
            < 28 => 0,
            < 36 => 1,
            < 60 => 2,
            < 128 => 3,
            _ => 4,
        };

        // Penalties for guessable structure.
        if (isCommon) level = 0;
        else if (warnings.Count > 0) level = Math.Min(level, 1);

        // Constructive suggestions.
        if (len < 12) suggestions.Add("Use at least 12 characters — length matters most.");
        if (!cs.HasUpper) suggestions.Add("Add uppercase letters.");
        if (!cs.HasLower) suggestions.Add("Add lowercase letters.");
        if (!cs.HasDigit) suggestions.Add("Add digits.");
        if (!cs.HasSymbol) suggestions.Add("Add symbols like !?#$.");
        if (suggestions.Count == 0 && level >= 3)
            suggestions.Add("Strong. Use a unique password for every account.");

        return new StrengthResult
        {
            Level = level,
            Rating = RatingName(level),
            EntropyBits = entropy,
            AlphabetSize = cs.Size,
            Length = len,
            HasLength = len >= 12,
            HasLower = cs.HasLower,
            HasUpper = cs.HasUpper,
            HasDigit = cs.HasDigit,
            HasSymbol = cs.HasSymbol,
            Warnings = warnings,
            Suggestions = suggestions,
        };
    }

    public static string RatingName(int level) => level switch
    {
        0 => "Very weak",
        1 => "Weak",
        2 => "Fair",
        3 => "Strong",
        _ => "Very strong",
    };

    private static bool IsSingleCharRepeat(string s) =>
        s.Length >= 3 && s.All(c => c == s[0]);

    private static bool IsSequential(string s)
    {
        if (s.Length < 4) return false;
        int up = 0, down = 0;
        for (int i = 1; i < s.Length; i++)
        {
            int diff = s[i] - s[i - 1];
            if (diff == 1) up++;
            else if (diff == -1) down++;
        }
        return up == s.Length - 1 || down == s.Length - 1;
    }

    private static bool LooksLikeKeyboardWalk(string s)
    {
        string lower = s.ToLowerInvariant();
        return KeyboardWalks.Any(w => lower.Contains(w));
    }

    private static bool IsBareYear(string s) =>
        s.Length == 4 && s.All(char.IsDigit) &&
        int.TryParse(s, out int y) && y is >= 1900 and <= 2099;
}
