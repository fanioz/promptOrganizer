# AVALONIA BOARD SPIKE — PROTOTYPE (throwaway)

Wayfinder ticket [#4 — Prototype: Avalonia spike of the board screen](https://github.com/fanioz/promptOrganizer/issues/4).

**The question:** can Avalonia UI faithfully render the PromptOrganizer board (per
`docs/ui/PromptOrganizer-Design-Spec.md` + `PromptOrganizer-Motion-Spec.md`), and what is the
authoring cost compared to the current Uno build? This is a framework-fidelity test, not a
design exploration — the design is already fixed, so there is deliberately ONE variant, not a
switcher gallery.

## Run it

```bash
dotnet run --project spike/AvaloniaBoardSpike
```

Headless measurement (prints startup time + memory to the console, then closes):

```bash
dotnet run --project spike/AvaloniaBoardSpike -- --spike-autoclose
```

Not referenced by `PromptOrganizer.sln`. No persistence, no LiteDB, mock data only.

## What's implemented (vs the Uno app)

- Top bar + three tinted columns (`Ide Awal` / `Dikembangkan` / `Siap Digunakan`) with prompt cards
- Card: 4px accent stripe, emoji icon, title, version chip, tag chips, 3-line truncated preview
- Card hover: background tint + 1dp→2dp shadow fade (140ms) + quick actions fade-in (120ms); press: scale 0.98 (80ms)
- Prompt editor modal: 920px (spec max 960), radius 12, backdrop fade 0→0.4 (220ms), scale 0.98→1.0 + fade (240ms, decel-out)
- Light/Dark toggle (🌗 in the top bar) — live theme switching
- `Ctrl+N` new prompt, `Ctrl+S` mock save, `Esc` close; card ⭐/📋/✏️ actions
- F12 opens Avalonia DevTools (Debug build) — compare against Uno's tooling

**Deliberately omitted:** search overlay (Ctrl+K stub only), drag & drop, trash/undo, clipboard
snackbar, accessibility pass, reduced-motion handling.

## Known deviations / notes

- Hover + press uses one code-behind class toggle on the card (mirroring the Uno card's own
  event wiring) **plus** a pure `:pointerover` pseudo-class style for the background tint. The
  transition/setter work itself is declarative XAML — vs ~160 lines of VSM storyboards in
  `PromptCardControl.xaml`.
- Dark column tints were **derived** (#1C2B20 / #2A251A / #17262C): the current Uno build has no
  dark variants for its pastel column backgrounds — a fidelity gap Avalonia's theme dictionaries
  close for free.
- Uno's `x:Bind` maps to Avalonia `CompiledBinding` (`x:DataType`, on by default here).
- Startup/memory: printed by `--spike-autoclose`. Observed on this machine: **2.8–12.4 s cold
  start to first frame (very noisy), ~86–93 MB working set, ~32–40 MB private memory**. No
  apples-to-apples Uno number exists (published Uno benchmarks target WinAppSDK, not its Skia
  desktop target).

## Avalonia 11.3.22 gotchas hit while building (DX evidence)

1. **Direct `<Border.Transitions>` property elements NRE at XAML populate** —
   `NullReferenceException` with no useful message. Moving the identical transitions into a
   `<Style>` Setter (`<Setter Property="Transitions">`) works. Cost: ~2 debugging rounds with
   unhelpful stacks.
2. **`BoxShadow` takes unit-less numbers** — `"0 8px 16px 0 #24000000"` throws
   `FormatException: '8px' was not in a correct format`; must be `"0 8 16 0 #24000000"`.
3. Easing key-spline strings (`Easing="0.2,0,0,1"`) work fine in Setter transitions;
   WPF-style `<CubicEase EasingMode="EaseOut">` elements do not exist — Avalonia 11 uses
   directional classes (`CubicEaseOut`) in `Avalonia.Animation.Easings`.
