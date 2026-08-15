# PassMasterSuite

A modern, offline, **educational** password toolkit for Windows. One consistent dark app with
three tools — a **Generator**, a **Checker**, and an educational **Cracker** that visualises how
a brute-force attack would try to guess a password and estimates how long it would take.

Everything runs locally. Nothing is sent over the network, and the Cracker never attacks any real
system — it only demonstrates, on a password you type in yourself, how guessing works.

## The three tools

- **Generator** — build a random password from the character classes you choose (lowercase,
  uppercase, digits, symbols) and a length slider. Reveal/hide, copy, regenerate, and a live
  strength meter. Uses a cryptographically secure random generator.
- **Checker** — measure a password's strength: entropy from its realized alphabet, which character
  classes it uses, common-password and simple-pattern detection, a visual strength meter, and an
  estimated time-to-crack shown across several attacker speeds.
- **Cracker** — type a password and watch an animated brute-force demonstration: the current guess,
  a live stream of recent attempts, a progress indicator, and a countdown of the estimated time
  remaining. Pick an **attacker speed** (online throttled → GPU array) and watch the estimate change.
  Weak passwords are cracked before your eyes and revealed; strong ones keep the guessing running
  while showing just how astronomically long it would really take.

The headline crack time is always **computed analytically** (`keyspace = alphabetSize ^ length`,
`time = keyspace ÷ 2 ÷ guessesPerSecond`) — the app never iterates the real keyspace, so it never
blocks or crashes.

## Tech stack

- **.NET 8** (`net8.0-windows`), **WPF**, C#, SDK-style project.
- One unified dark design system in `Themes/DesignSystem.xaml` (see `docs/STYLEGUIDE.md`).
- Vector icons via **Segoe MDL2 Assets** glyphs — no image or GIF assets anywhere.
- Animations via WPF Storyboards and a `DispatcherTimer` (no GIFs).

## Run it

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download) on Windows (WPF is Windows-only).

```bash
dotnet run
```

Or open `PassMasterSuite.sln` in Visual Studio 2022 and press F5.

## Project layout

```
App.xaml(.cs)            Application entry; merges the design system
Themes/DesignSystem.xaml Unified dark design system (colors, type, components)
Core/                    Pure logic (no UI): entropy, crack-time, generator, formatting
  Charset.cs             Character-class analysis and alphabet size
  PasswordStrength.cs    Entropy, rating, weaknesses, suggestions
  CrackTime.cs           Analytic keyspace & crack-time math (BigInteger)
  CrackModel.cs          Bundles the facts the Cracker visualises
  AttackerPreset.cs      Attacker-speed presets (guesses/second)
  PasswordGenerator.cs   Secure random password generation
  DurationFormat.cs      Adaptive duration formatting (seconds → centuries)
Controls/StrengthMeter   Reusable 5-segment strength meter
Views/                   Shell + Home, Generator, Checker, Cracker (in-place switching)
docs/                    CURRENT_STATE, CRITIQUE, STYLEGUIDE
```

## Docs

- `docs/CURRENT_STATE.md` — how the original app worked before the rebuild.
- `docs/CRITIQUE.md` — a hard critique of the original plus the improvement checklist.
- `docs/STYLEGUIDE.md` — the unified dark design system used throughout.

## License

MIT.
