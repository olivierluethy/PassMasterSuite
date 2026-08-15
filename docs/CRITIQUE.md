# Critique — PassMasterSuite (as-is)

A hard, honest assessment of the current WPF application, followed by a grouped, actionable
improvement checklist. The tone is deliberately blunt: this is an educational school project
and the goal is to learn what "professional" actually demands.

## 1. Windows-only by nature — stay WPF, but modernise it properly

The product is a **WPF Windows desktop app** and will remain one (an explicit decision). That is
a legitimate platform — but the current project barely uses it well. It targets the legacy
**.NET Framework** with a `packages.config`/non-SDK `csproj`, needs Visual Studio to build, and
pins layout with absolute pixel margins so nothing scales. "Responsive" is possible in WPF
(star-sized `Grid`, `Viewbox`, `MinWidth/MinHeight`) but is not used. The fix is not a new
platform — it is to modernise to **SDK-style .NET 8 (`net8.0-windows`)**, adopt a real
resizable layout, and impose one design system.

## 2. The Cracker actually brute-forces on the UI thread — and says so

`Password_Cracker.Cracker()` runs a **real exhaustive brute-force `while` loop synchronously on
the UI thread**, appending *every* candidate to a `ListBox`. Consequences:

- The window **freezes** for the entire run. No repaint, no cancel, no progress.
- It is **capped at 8 characters** by eight hand-written nested counters (`first`…`eigth`) —
  brittle and arbitrary.
- The app's own info panel warns that **"the computer may crash"** for 4+ characters, and a
  `MessageBox` asks the user to confirm they accept the risk. Shipping a feature that you warn
  might crash the user's machine is a red flag, not a disclaimer.
- Educationally it's also misleading: real attackers never iterate the keyspace in a UI; they
  compute time-to-crack analytically. The demo teaches the wrong mental model *and* performs
  terribly.

**Fix direction:** compute crack time analytically (`keyspace = charset^length`,
`time = keyspace / guessesPerSecond`); render an *illustrative* animated sampling of guesses on
a `DispatcherTimer` (never a real exhaustive loop); make it cancellable and non-blocking.

## 3. The persistent loading-GIF bug

`loading-for-crack.gif` is shown in `btnStart_Click` and only collapsed at the end of
`Cracker()`. Because the crack loop blocks the UI thread, the GIF can't animate and — on long
or failed runs — appears to hang forever. The loading state is coupled to a synchronous loop
instead of to an explicit run state, so it has no reliable "off" condition.

**Fix direction:** a real state machine (`idle → running → cracked | exhausted | stopped`)
where the progress UI is rendered *only* while `running` and unmounts the instant the run ends.

## 4. The slider crashes on fast input

Every slider `ValueChanged` tick regenerates the whole password and, on overflow, opens a
blocking `MessageBox` **while also** mutating sibling sliders (re-entering the same handlers via
detach/re-attach hacks). Dragging quickly stacks modal dialogs and re-entrant mutations. The
"solution" was a UI warning telling users not to drag fast — which is not a fix, it's an
apology.

**Fix direction:** cheap, pure, debounced/rAF-batched updates; no modal dialogs on input;
constraints enforced declaratively (clamp values), not by re-entrant event juggling.

## 5. Image-based icons instead of a library

Every icon is a **raster asset** loaded from `Images/` (`refresh-password.png`, `show.png`,
`hide.png`) plus a GIF spinner and two JPG backgrounds. This is heavier, blurry when scaled,
impossible to recolor/theme, inconsistent in size, and adds ~1.6 MB of binary assets to the
repo. There is no icon system at all.

**Fix direction:** a vector icon set (Segoe MDL2 Assets glyphs — built into Windows, scalable,
recolorable) + WPF Storyboard animation; zero image assets.

## 6. Four inconsistent visual systems

There is no design system. Each window invents its own look:

- **Home**: radial gradient, MDL2 glyph, black buttons.
- **Generator**: JPG photo background, rounded light buttons, dark inputs.
- **Checker**: black background with a **white** criteria panel that clashes badly.
- **Cracker**: black background, gray info card, bright green button, GIF.

Button templates and colors are redefined per window. The result looks like four different
apps. "The window should look essentially identical across all tools" is currently false.

## 7. German / English language mix

The UI is nominally English, but `MessageBox` strings and code comments are German:
`"Bitte geben Sie eine Länge an!"`, `"Die Länge darf nicht kleiner sein!"`,
`"Sie haben vier oder mehr Zeichen eingegeben…"`,
`"Bitte geben sie ein Passwort ein befor sie mit dem Crackingvorgang starten"`. Plus an
English typo: "Password automatically copyied!". This reads as unfinished.

## 8. The Checker is too shallow to be credible

`CheckStrength()` returns an integer 0–5 from a handful of `if`s. Problems:

- **Dead branch**: `length < 4 → +1` else `length >= 6 → +1` means lengths **4–5 score 0** from
  the length rule.
- **No entropy**, no time-to-crack, no common-password list, no pattern/sequence/repeat
  detection, no visual meter, no per-criterion guidance beyond five checkboxes.
- Score maps to a single sentence — no rating scale, no color, no actionable feedback.

**Fix direction:** entropy from realized charset, adaptive time-to-crack across attacker
presets, common-password + simple-pattern detection, a visual meter and rating.

## 9. Surprising, privacy-unfriendly clipboard behaviour

The Generator **auto-copies to the clipboard on every keystroke/tick** and announces it. Users
don't expect silent clipboard writes; doing it on every slider tick is worse. Copy should be an
explicit, user-initiated action.

## 10. Accessibility & UX gaps

- Blocking `MessageBox` dialogs for routine input validation.
- Non-interactive checkboxes used purely as indicators (confusing semantics).
- Fixed pixel layout: content clips or floats when the window is resized.
- No focus styling system, no keyboard-first affordances beyond WPF defaults.
- Tooltips exist on some buttons but not consistently, and several contain typos ("Klick").

## 11. Architecture & state management weaknesses

- All logic is in code-behind event handlers; no view-model, no separation, no reusable core.
- Navigation destroys and recreates windows (`Hide` → `ShowDialog` → `Close`), losing all
  state and making a "consistent shell" impossible.
- Duplicated resource dictionaries and button templates across windows.
- Cracker's charset selection is a long `if/else if` regex ladder with duplicated arrays.

## 12. Performance & repo hygiene

- ~1.6 MB of binary image/GIF assets, several duplicated at repo root **and** in `Images/`.
- The synchronous brute-force is O(charset^length) on the UI thread — pathological by design.
- No build for non-Windows contributors; no `npm`-style one-command run.

---

# Improvement checklist (actionable)

## Functionality
- [ ] Modernise to **SDK-style .NET 8 (`net8.0-windows`)** WPF, runnable with `dotnet run`;
      drop the legacy `packages.config` and unused NuGet packages.
- [ ] Cracker: compute headline crack time **analytically** (`charset^length / guessesPerSecond`);
      never iterate the real keyspace.
- [ ] Cracker: add selectable **attacker-speed presets** (online throttled, offline slow hash,
      offline fast hash, GPU array) so the same password shows different crack times.
- [ ] Cracker: animate an *illustrative* live sampling of candidate guesses via a
      `DispatcherTimer`; reveal the password only when the simulation genuinely reaches it
      (short/weak inputs), otherwise keep visualising the estimate.
- [ ] Cracker: real state machine `idle → running → cracked | exhausted | stopped` with a Stop
      button and a live remaining-time countdown in adaptive units.
- [ ] Checker: compute entropy from the realized charset; estimate time-to-crack using the same
      attacker model; detect common passwords and simple patterns (sequences, repeats, years,
      keyboard walks); show a visual meter and a security rating.
- [ ] Generator: length + per-class options (lowercase/uppercase/digits/symbols), regenerate,
      show/hide, explicit copy, live strength readout.
- [ ] Adaptive duration formatting everywhere (seconds → minutes → hours → days → years →
      centuries) for all time estimates and countdowns.

## UX / UI
- [ ] One unified **dark-only** design system in `docs/STYLEGUIDE.md`, implemented as a shared
      `Themes/DesignSystem.xaml` ResourceDictionary; identical chrome across all views.
- [ ] Single app shell (one `Window`) with **in-place** view switching between `UserControl`s —
      no window destruction, no `ShowDialog` navigation.
- [ ] Replace **all** raster/GIF icons with a vector icon set (Segoe MDL2 Assets glyphs).
- [ ] Every interactive control has an icon **and** a tooltip explaining what it does.
- [ ] Remove auto-copy; make copy explicit with clear feedback.
- [ ] Remove all blocking dialogs for routine validation; use inline, non-modal feedback.
- [ ] All UI text in English; fix typos ("copyied", "Klick").
- [ ] "Grandfather test": Cracker visualisation must be understandable to a non-technical user.

## Performance
- [ ] Zero image/GIF assets; all animation via WPF Storyboards.
- [ ] Never block the UI thread; looping visualisation driven by `DispatcherTimer`/`async`.
- [ ] Delete duplicated/unused binary assets and legacy files; keep the repo lean.

## Robustness
- [ ] Slider fully robust under fast dragging: pure, clamped, rAF/debounced updates, **no**
      modal dialogs and **no** re-entrant sibling mutations; remove the "don't slide fast" warning.
- [ ] Loading/progress UI appears only while a run is active and disappears the instant it ends.
- [ ] No user action can freeze or crash the app; all long-running visualisation is cancellable.
- [ ] Fully fluid responsiveness: usable when the window shrinks, scales up when it grows.
- [ ] Clear separation of a pure logic core (entropy, crack-time, charset) from UI, so behaviour
      is predictable and reusable across Checker and Cracker.
