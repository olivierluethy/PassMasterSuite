namespace PassMasterSuite.Core;

/// <summary>
/// A named attacker capability — how many password guesses per second it can make.
/// Selecting different presets shows how the same password's crack time changes; that
/// contrast is the educational payoff of the Checker and Cracker.
/// </summary>
public sealed record AttackerPreset(string Name, string Description, double GuessesPerSecond)
{
    public static readonly AttackerPreset OnlineThrottled =
        new("Online (throttled)", "≈10³ guesses/sec — a login form that limits attempts.", 1e3);

    public static readonly AttackerPreset OfflineSlowHash =
        new("Offline (slow hash)", "≈10⁴ guesses/sec — a stolen database using bcrypt/argon2.", 1e4);

    public static readonly AttackerPreset OfflineFastHash =
        new("Offline (fast hash)", "≈10¹⁰ guesses/sec — fast unsalted hashes on one PC.", 1e10);

    public static readonly AttackerPreset GpuArray =
        new("GPU array", "≈10¹² guesses/sec — a rig of high-end GPUs.", 1e12);

    /// <summary>All presets, slowest to fastest.</summary>
    public static readonly IReadOnlyList<AttackerPreset> All =
        new[] { OnlineThrottled, OfflineSlowHash, OfflineFastHash, GpuArray };

    /// <summary>Sensible default for headline estimates.</summary>
    public static AttackerPreset Default => OfflineFastHash;

    public override string ToString() => Name;
}
