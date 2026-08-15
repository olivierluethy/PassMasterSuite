using System.Numerics;

namespace PassMasterSuite.Core;

/// <summary>
/// Bundles everything the Cracker view needs to visualise a brute-force attack on a typed
/// password at a chosen attacker speed. All headline numbers are analytic; the view only ever
/// samples candidates for display, it never iterates the keyspace.
/// </summary>
public sealed class CrackModel
{
    private readonly Random _rng = new();

    public string Target { get; private init; } = "";
    public string Alphabet { get; private init; } = "";
    public BigInteger Keyspace { get; private init; }
    public double AverageSeconds { get; private init; }

    /// <summary>True when the selected attacker would realistically crack this quickly, so the
    /// simulation animates all the way to the real password and reveals it.</summary>
    public bool Realizable { get; private init; }

    /// <summary>Canonical enumeration index of the real password (only meaningful when Realizable).</summary>
    public BigInteger TargetIndex { get; private init; }

    public int Length => Target.Length;

    public static CrackModel Build(string target, AttackerPreset preset, double revealThresholdSeconds = 60)
    {
        var cs = Charset.Analyze(target);
        double avg = CrackTime.AverageSeconds(cs.Size, target.Length, preset.GuessesPerSecond);
        BigInteger? index = CrackTime.IndexOf(target, cs.Alphabet);

        bool realizable = target.Length > 0
                          && index is not null
                          && !double.IsInfinity(avg)
                          && avg <= revealThresholdSeconds;

        return new CrackModel
        {
            Target = target,
            Alphabet = cs.Alphabet,
            Keyspace = CrackTime.Keyspace(cs.Size, target.Length),
            AverageSeconds = avg,
            Realizable = realizable,
            TargetIndex = index ?? BigInteger.Zero,
        };
    }

    /// <summary>The candidate at a given enumeration index (used to "count up" toward the password).</summary>
    public string CandidateAt(BigInteger index) => CrackTime.Candidate(index, Length, Alphabet);

    /// <summary>An illustrative random candidate of the right length (used when the password is too
    /// strong to actually reach — the stream keeps flowing while the estimate is displayed).</summary>
    public string RandomCandidate()
    {
        if (Length == 0 || Alphabet.Length == 0) return "";
        var buffer = new char[Length];
        for (int i = 0; i < Length; i++)
            buffer[i] = Alphabet[_rng.Next(Alphabet.Length)];
        return new string(buffer);
    }
}
