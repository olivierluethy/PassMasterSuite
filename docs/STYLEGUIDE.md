# Styleguide — PassMasterSuite (unified dark design system)

This is the **single source of truth** for the app's look and feel. It is a *new* unified system
(an approved redesign), not an extraction of the old inconsistent windows. Every view — Home,
Generator, Checker, Cracker — shares this exact chrome. Implemented as a WPF `ResourceDictionary`
at `Themes/DesignSystem.xaml`, merged once in `App.xaml`.

## Design thesis

The app lives at the seam between **human** and **machine**: a person choosing a memorable
password, and a machine enumerating a keyspace. The design makes that tension visible.

- **Humanist chrome** (Segoe UI) for everything a person reads and controls.
- **Monospace data** (Consolas) for everything the machine produces — the password itself,
  entropy bits, keyspace size, and the brute-force attempt stream.
- **Signature element:** the *cipher ticker* — a glowing monospace stream of candidate guesses in
  the Cracker, echoed by the monospace password readouts elsewhere. It is the one memorable thing;
  everything around it stays quiet.

Dark-only. A deep **midnight-ink** base with a single **brass/amber** accent (the color of a lock
and key), plus a disciplined semantic ramp for password strength.

## Color tokens

Defined as `SolidColorBrush` / `Color` resources in `DesignSystem.xaml`.

| Token | Hex | Use |
|-------|-----|-----|
| `Brush.Bg` | `#0B0E14` | App background (midnight ink) |
| `Brush.Surface` | `#12161F` | Panels, nav rail, cards |
| `Brush.SurfaceRaised` | `#1A2130` | Inputs, elevated cards, table rows |
| `Brush.Border` | `#262E3F` | Hairline borders, dividers |
| `Brush.BorderStrong` | `#37425A` | Hover borders, focused outlines |
| `Brush.TextPrimary` | `#EAEEF7` | Headings, primary text |
| `Brush.TextSecondary` | `#9AA6BD` | Labels, secondary text |
| `Brush.TextMuted` | `#5E6980` | Hints, disabled, captions |
| `Brush.Accent` | `#F2B84B` | Primary action, focus, brand (brass) |
| `Brush.AccentHover` | `#F7C766` | Accent hover |
| `Brush.AccentPress` | `#D89F33` | Accent pressed |
| `Brush.AccentText` | `#0B0E14` | Text on accent fills |
| `Brush.Info` | `#4CC3E0` | Informational highlights |

### Strength ramp (5 steps) & semantic

| Token | Hex | Meaning |
|-------|-----|---------|
| `Brush.Strength0` | `#E5484D` | Very weak (crimson) |
| `Brush.Strength1` | `#F2803D` | Weak (orange) |
| `Brush.Strength2` | `#F2B84B` | Fair (amber) |
| `Brush.Strength3` | `#7BC96F` | Strong (lime) |
| `Brush.Strength4` | `#3FB950` | Very strong (emerald) |
| `Brush.Success` | `#3FB950` | Success / cracked / met criterion |
| `Brush.Danger` | `#E5484D` | Error / destructive |
| `Brush.Warning` | `#F2B84B` | Caution |

## Typography

Only fonts guaranteed on Windows — no font files shipped.

| Role | Family | Size / Weight | Notes |
|------|--------|---------------|-------|
| Display | Segoe UI Light | 32 | View titles, home hero |
| H1 | Segoe UI SemiBold | 22 | Panel titles |
| H2 | Segoe UI SemiBold | 16 | Section headers |
| Body | Segoe UI | 14 Regular | Default text |
| Small | Segoe UI | 12 | Secondary labels, captions |
| Data / mono | Consolas | 14–20 | Passwords, entropy, keyspace, attempt stream |

Named text styles: `Text.Display`, `Text.H1`, `Text.H2`, `Text.Body`, `Text.Small`,
`Text.Muted`, `Text.Mono`, `Text.MonoLarge`.

## Spacing, radius, elevation

- **Spacing scale (px):** 4, 8, 12, 16, 24, 32, 48. Applied via `Thickness` / `Margin` / `Padding`.
- **Radius (`CornerRadius`):** `sm` 6, `md` 10, `lg` 14, `pill` 999. Cards use `lg`, inputs/buttons
  `md`, chips/badges `pill`.
- **Elevation:** soft `DropShadowEffect`, `ShadowDepth=0`, `BlurRadius=24`, low opacity black for
  raised cards; an **accent glow** (amber, `BlurRadius=16`, opacity ~0.5) for focused inputs and
  the running/cracked states. No hard borders as the only separator — pair hairline + subtle shadow.

## Components

Each is a named `Style` in `DesignSystem.xaml`.

- **Button.Primary** — accent fill, `AccentText` foreground, radius `md`, hover → `AccentHover`,
  press → `AccentPress`, disabled → muted. Optional leading glyph.
- **Button.Ghost** — transparent fill, `Border` outline, `TextPrimary`; hover raises `SurfaceRaised`
  and `BorderStrong`. Used for secondary/back actions.
- **Button.Danger** — `Danger` outline/text; hover fills danger. Used for Stop.
- **IconButton** — square (36×36), radius `md`, glyph only, ghost styling, always paired with a
  tooltip. Used for refresh, show/hide, copy.
- **Input (TextBox / PasswordBox)** — `SurfaceRaised` fill, `Border` outline, `TextPrimary`,
  radius `md`, 12/14 padding; focus → `BorderStrong` + accent glow. Monospace variant for password
  fields uses Consolas.
- **Slider** — thin track (4px) on `Border`, filled portion in `Accent`, circular thumb
  (`SurfaceRaised` fill, accent ring) that scales on hover/drag. Fully robust to fast dragging.
- **Card / Panel** — `Surface` fill, hairline `Border`, radius `lg`, soft shadow, 24 padding.
- **Nav rail** — fixed left rail on the shell; icon + label items; active item shows an accent bar
  and raised background. This is the identical chrome across all views.
- **StrengthMeter** — 5 segmented bars; filled segments use the strength ramp color for the current
  level; unfilled use `Border`. Accompanied by a mono rating label.
- **DurationBadge** — pill, `SurfaceRaised` fill, mono text, small clock glyph; shows an adaptively
  formatted duration.
- **AttemptTable / cipher ticker** — mono rows on alternating `Surface`/`SurfaceRaised`; newest row
  highlighted with a brief accent fade; the current candidate shown large in `Text.MonoLarge` with a
  soft accent glow. **The signature element.**
- **ProgressBar** — 6px track, accent fill, indeterminate shimmer while running.
- **Tooltip** — `SurfaceRaised` fill, `BorderStrong` hairline, `TextPrimary`, radius `sm`, 8/6
  padding, small drop shadow. Applied to **every** interactive control.
- **Chip / Toggle** — option toggles (character classes) render as pill chips: active = accent
  outline + `TextPrimary`, inactive = `Border` outline + `TextMuted`.

## Icons

Vector **Segoe MDL2 Assets** glyphs — built into Windows, scalable, recolorable, **zero image
files**. Rendered via a shared `Icon` style (a `TextBlock` with `FontFamily="Segoe MDL2 Assets"`).
Glyph codes are centralized as string resources (`Glyph.*`) so any single icon is a one-line change:

| Resource | Glyph | Meaning |
|----------|-------|---------|
| `Glyph.Home` | `` (E80F) | Home |
| `Glyph.Lock` | `` (E72E) | Security / brand |
| `Glyph.Refresh` | `` (E72C) | Regenerate |
| `Glyph.Show` | `` (E7B3) | Reveal password |
| `Glyph.Hide` | `` (ED1A) | Mask password |
| `Glyph.Copy` | `` (E8C8) | Copy to clipboard |
| `Glyph.Check` | `` (E73E) | Criterion met / cracked |
| `Glyph.Cancel` | `` (E711) | Clear / close |
| `Glyph.Play` | `` (E768) | Start cracking |
| `Glyph.Stop` | `` (E71A) | Stop cracking |
| `Glyph.Info` | `` (E946) | Info / help |
| `Glyph.Warning` | `` (E7BA) | Caution |
| `Glyph.Speed` | `` (EC4A) | Attacker speed |
| `Glyph.Clock` | `` (E823) | Time / duration |
| `Glyph.Generate` | `` (E945) | Generator |
| `Glyph.Gauge` | `` (E9D9) | Checker / strength |

## Interactive & feedback states

- **Hover:** 150ms color/elevation transitions; borders strengthen, backgrounds lift one level.
- **Focus:** visible accent glow + `BorderStrong` outline on all inputs and buttons (keyboard-first).
- **Loading / running:** progress UI and indeterminate shimmer render **only** while a run is
  active; they unmount the instant it ends (no persistent spinner — fixes the legacy GIF bug).
- **Success / cracked:** accent→success glow pulse on the revealed value.
- **Error / invalid:** inline, non-modal — a `Danger`-colored helper line under the control. **No
  blocking dialogs** for routine validation.
- **Disabled:** `TextMuted` foreground, reduced opacity, no shadow.

## Motion

- View transitions: 200ms opacity + small upward translate on the incoming view.
- Micro-interactions: 120–180ms ease on hover/press; thumb scale on slider drag.
- Cipher ticker: rows fade/slide in at ~30–60ms cadence driven by a `DispatcherTimer`; the running
  state pulses softly. Motion is purposeful and sparse — the ticker is the one lively element.

## Layout & responsiveness

- One app shell: a fixed **nav rail** (icon rail; collapses to icons-only at narrow widths) + a
  fluid content area. The chrome is identical across every view.
- Content uses star-sized `Grid` columns and `MinWidth/MinHeight`; wide readouts sit in
  `Viewbox`/scrollable containers so they scale down when the window shrinks and up when it grows.
- No absolute pixel margins for layout structure; spacing comes from the scale above.
- Target: usable from ~720×520 up to maximized, with no clipping or horizontal overflow.
