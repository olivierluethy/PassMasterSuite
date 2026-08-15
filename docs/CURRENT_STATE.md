# Current State — PassMasterSuite (pre-rebuild snapshot)

This document captures the application **exactly as it exists before the rebuild**, so the
redesign can be traced back to what it replaced. It describes behaviour and structure only —
it is not a design target. (The new design system lives in `STYLEGUIDE.md`; the criticism and
improvement plan live in `CRITIQUE.md`.)

## 1. What the app actually is

Despite being described colloquially as a "web app", the current product is a **Windows
desktop application built with WPF (.NET Framework, C#)**. It uses XAML for layout and
code-behind (`*.xaml.cs`) for logic. It cannot run on Linux/macOS or in a browser and
requires Visual Studio + the .NET Framework to build.

It is an offline, educational password toolkit with three tools plus a home/menu screen.
Nothing is sent over a network.

## 2. File inventory

| File | Role |
|------|------|
| `Password Checker, Generator and Cracker.sln` | Visual Studio solution |
| `Password Checker, Generator and Cracker.csproj` | Project file (.NET Framework, WPF) |
| `packages.config` | NuGet deps: `WpfAnimatedGif`, `Extended.Wpf.Toolkit` |
| `App.xaml` / `App.xaml.cs` | WPF application entry, startup URI = `MainWindow.xaml` |
| `MainWindow.xaml` / `.cs` | Home / mode-selection screen |
| `Password_Generator.xaml` / `.cs` | Generator tool |
| `Password_Strength_Checker.xaml` / `.cs` | Checker tool |
| `Password_Cracker.xaml` / `.cs` | Cracker tool |
| `Images/` | All raster/GIF assets (see §7) |
| `Properties/` | Auto-generated WPF resources/settings/assembly info |
| `App.config` | .NET runtime config |
| `*.ico`, `*.png`, `*.jpg` (repo root duplicates) | Stray copies of icon/background assets |

## 3. Architecture & navigation

- Each screen is a separate top-level **`Window`**. Navigation is done by **destroying and
  recreating windows**: every handler calls `this.Hide()`, `new OtherWindow().ShowDialog()`,
  then `this.Close()`. There is no shared app shell, no shared state, and no shared styling —
  each window redefines its own resources and layout from scratch.
- Layout uses fixed pixel `Margin`, `Width`, and `Height` values with star-sized `Grid`
  rows/columns. Sizing is largely absolute, so the layout does not adapt gracefully.
- Logic lives entirely in code-behind event handlers; there is no view-model, no separation
  of concerns, and no unit-testable core.

## 4. Home / mode-selection (`MainWindow`)

- A `RadialGradientBrush` background (Black → DarkGray).
- A large Segoe MDL2 glyph (`&#xE80F;`, a lock) as a title.
- Three stacked black `Button`s: "Password Strength Checker", "Password Generator",
  "Password Cracker", each with a tooltip. Fixed 269×76 with hard-coded margins.
- Window is 800×450 and **not** resizable in a meaningful way (children use absolute margins).

## 5. Password Generator (`Password_Generator`)

- Four `Slider`s (Length, Digits, Capitals, Symbols), each 0–50, snap-to-tick every 5, bound
  to a read-only `TextBox` showing the value.
- Background is a JPG image (`password-generator-background.jpg`); a second decorative JPG
  (`password-generator-image.jpg`) fills the middle row.
- `RandomPasswordGenerator()` builds a password by concatenating N digits + N capitals +
  N symbols + remaining lowercase, then shuffles with `OrderBy(c => rand.Next())`.
- **Auto-copies** the generated password to the clipboard on every generation and shows a
  "Password automatically copyied!" label (note the typo).
- A "Refresh" button (uses `refresh-password.png`) regenerates.
- Slider `ValueChanged` handlers (`ColorSlider_ValueChanged`, `LengthSlider_ValueChanged`)
  regenerate the password on **every tick** and pop up **`MessageBox`** dialogs when the sum
  of digits+capitals+symbols exceeds length. They also detach/re-attach event handlers and
  mutate other sliders' values re-entrantly (`AdjustSlidersToMaxLength`).
- German strings appear here: `"Bitte geben Sie eine Länge an!"`,
  `"Die Länge darf nicht kleiner sein!"`, plus German code comments.

## 6. Password Strength Checker (`Password_Strength_Checker`)

- A `PasswordBox` (masked) plus a hidden `TextBox` for the "shown" state; show/hide toggled by
  two buttons using `show.png` / `hide.png`.
- `CheckStrength()` returns an integer 0–5 by adding points for length, digits, mixed case, and
  special characters, then maps it to a sentence ("The password is weak!" … "very strong!").
- A white panel with five non-interactive `CheckBox`es reflects which criteria are met
  (8+ chars, lowercase, uppercase, number, special char).
- **No entropy, no time-to-crack estimate, no common-password/pattern detection, no visual
  meter.** The length scoring has a dead branch: `< 4 → +1` else `>= 6 → +1`, so lengths 4–5
  score 0 from that block.
- Background is black; the criteria panel is white — inconsistent with the rest.

## 7. Password Cracker (`Password_Cracker`)

- Text box for the password, a "Start Cracking" button, an attempts label, a hidden `ListBox`
  ("See/Hide cracking process"), and a static "Important" info panel warning that 4+ character
  passwords may **crash the computer**.
- `CheckInput()` picks a character set by regex-classifying the typed password (digits only,
  lowercase only, … up to "everything"), then calls `Cracker()`.
- `Cracker()` performs a **real exhaustive brute force on the UI thread**: a `while` loop with
  eight hand-rolled nested counters (`first`…`eigth`) increments through the keyspace, adding
  **every attempted candidate string to the `ListBox`**, until it matches the typed password.
  This blocks the UI thread completely, has no cancellation, and is capped at 8 characters.
- A loading GIF (`loading-for-crack.gif`) is shown in `btnStart_Click` and only collapsed at
  the very end of `Cracker()`.
- German strings appear here too, e.g. the 4+ character warning and
  `"Bitte geben sie ein Passwort ein befor sie mit dem Crackingvorgang starten"`.

## 8. Images, GIFs, and icons

All icons and decoration are **raster assets loaded from `Images/`** (and duplicated at repo
root). None come from an icon library.

| Asset | Used for |
|-------|----------|
| `loading-for-crack.gif` | Cracker "loading" spinner |
| `refresh-password.png` | Generator refresh button icon |
| `show.png` / `hide.png` | Show/hide password toggle icons |
| `password-generator-background.jpg` | Generator window background |
| `password-generator-image.jpg` | Generator decorative image |
| `lock-image.png` | Large lock decoration |
| `closed-padlock-…​.ico` | Window icon |

The only non-raster icon is the Segoe MDL2 lock glyph on the home screen.

## 9. Slider behaviour (bug source)

`ValueChanged` runs heavy work synchronously on every tick: it regenerates the password and, on
overflow, shows a blocking `MessageBox` **and** programmatically changes other sliders (which
re-enter the same handlers). Dragging quickly fires many overlapping ticks, stacking modal
dialogs and re-entrant mutations — the documented "don't slide too fast or it crashes" problem.

## 10. Loading / finished state (bug source)

The GIF's `Visibility` is set to `Visible` before cracking and `Collapsed` after the loop, but
because the loop runs **synchronously on the UI thread**, the window freezes for the entire run
(seconds to minutes, or a crash for 4+ chars). The GIF cannot animate and, on long/failed runs,
appears to "never stop" — the reported persistent-loader bug.

## 11. Per-view styling summary

| View | Background | Notable |
|------|-----------|---------|
| Home | Radial gradient Black→DarkGray | MDL2 lock glyph, 3 black buttons |
| Generator | JPG photo background | Rounded button style, decorative JPG, dark inputs |
| Checker | Solid Black | White criteria panel (clashes), light-blue "home" button |
| Cracker | Solid Black | Gray info panel, green start button, GIF spinner |

No shared color tokens, typography scale, spacing scale, or component styles exist across the
four windows; each redefines button templates and colors independently.
