using System.Globalization;

namespace PassMasterSuite.Core;

/// <summary>
/// Formats a number of seconds into human-readable text that adapts across units —
/// seconds, minutes, hours, days, years, and named large scales up to scientific notation.
/// Used for every duration the app shows (crack-time estimates and the live countdown).
/// </summary>
public static class DurationFormat
{
    private const double Minute = 60;
    private const double Hour = 3600;
    private const double Day = 86400;
    private const double Year = 31557600;         // 365.25 days
    private const double Century = Year * 100;
    private const double Millennium = Year * 1000;

    /// <summary>Short headline form, e.g. "instantly", "3 hours", "42 years", "≈ 2.1×10¹² years".</summary>
    public static string Headline(double seconds)
    {
        if (double.IsNaN(seconds) || seconds < 0) return "—";
        if (double.IsInfinity(seconds) || seconds > 1e30 * Year) return "longer than the universe has existed";
        if (seconds < 1e-6) return "instantly";
        if (seconds < 1) return "less than a second";

        if (seconds < Minute) return Round(seconds, "second");
        if (seconds < Hour) return Round(seconds / Minute, "minute");
        if (seconds < Day) return Round(seconds / Hour, "hour");
        if (seconds < Year) return Round(seconds / Day, "day");

        double years = seconds / Year;
        return $"{LargeNumber(years)} years";
    }

    /// <summary>Compact countdown form used while a run is active, e.g. "1d 04h 22m 09s" or a scaled year form.</summary>
    public static string Countdown(double seconds)
    {
        if (double.IsNaN(seconds) || seconds < 0) return "—";
        if (double.IsInfinity(seconds) || seconds >= Century) return Headline(seconds);

        long total = (long)Math.Ceiling(seconds);
        long days = total / 86400; total -= days * 86400;
        long hours = total / 3600; total -= hours * 3600;
        long mins = total / 60; long secs = total - mins * 60;

        if (days > 0) return $"{days}d {hours:00}h {mins:00}m {secs:00}s";
        if (hours > 0) return $"{hours}h {mins:00}m {secs:00}s";
        if (mins > 0) return $"{mins}m {secs:00}s";
        return $"{secs}s";
    }

    private static string Round(double value, string unit)
    {
        long v = (long)Math.Round(value);
        if (v < 1) v = 1;
        return v == 1 ? $"1 {unit}" : $"{v} {unit}s";
    }

    private static string LargeNumber(double years)
    {
        if (years < 1) return "less than 1";
        if (years < 1000) return ((long)Math.Round(years)).ToString("N0", CultureInfo.InvariantCulture);
        if (years < 1e6) return $"{years / 1e3:0.#} thousand";
        if (years < 1e9) return $"{years / 1e6:0.#} million";
        if (years < 1e12) return $"{years / 1e9:0.#} billion";
        if (years < 1e15) return $"{years / 1e12:0.#} trillion";
        if (years < 1e18) return $"{years / 1e15:0.#} quadrillion";
        return Scientific(years);
    }

    private static string Scientific(double value)
    {
        int exp = (int)Math.Floor(Math.Log10(value));
        double mantissa = value / Math.Pow(10, exp);
        return $"≈ {mantissa:0.0}×10{Superscript(exp)}";
    }

    private static string Superscript(int number)
    {
        const string digits = "⁰¹²³⁴⁵⁶⁷⁸⁹";
        string s = number.ToString(CultureInfo.InvariantCulture);
        var sb = new System.Text.StringBuilder();
        foreach (char c in s)
            sb.Append(c is >= '0' and <= '9' ? digits[c - '0'] : c);
        return sb.ToString();
    }
}
