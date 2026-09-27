# ADR-0001: Avalonia UI for the PromptOrganizer App layer

- Date: 2026-09-27
- Status: Accepted
- Decided by: the user, in the wayfinder verdict session ([issue #5](https://github.com/fanioz/promptOrganizer/issues/5); map: [issue #1](https://github.com/fanioz/promptOrganizer/issues/1))

## Context

PromptOrganizer's UI was built on Uno Platform (.NET 10, `net10.0-desktop`, SkiaRenderer) with a UI-agnostic core: Domain, Data, and Services carry 213+ xUnit tests and a LiteDB store, while the App layer is small (10 XAML files, ~860 lines; 34 C# files). The developer felt DX/fidelity friction with Uno, and the original framework choice was anchored on an R2 roadmap of Android/iOS companion apps and a Web viewer.

A wayfinder effort ([map: issue #1](https://github.com/fanioz/promptOrganizer/issues/1)) compared Uno vs Avalonia with research ([#2](https://github.com/fanioz/promptOrganizer/issues/2), [#3](https://github.com/fanioz/promptOrganizer/issues/3)) and a board-screen spike ([#4](https://github.com/fanioz/promptOrganizer/issues/4), branch `spike/avalonia-board`):

- **Research:** Uno leads mobile maturity (16+ shipped apps) and WASM (Avalonia's own docs call WASM "early stages"); Avalonia leads community (31.6k vs 10.1k stars); Windows packaging and licensing tie. Avalonia's free tier lacks first-party XAML hot reload; Uno ships hot-reload reliability bugs of its own.
- **Spike:** user verdict "it is smooth"; motion authoring ~30 declarative Style/Transition lines vs ~160 VisualStateManager storyboard lines in Uno's `PromptCardControl`; dark-theme column tints fixed for free via theme dictionaries; three undocumented Avalonia 11.3.22 gotchas encountered.
- **Grilling settled the roadmap's weight:** R2 mobile/Web is soft — desktop is the product.

## Decision

Rebuild only `PromptOrganizer.App` on Avalonia UI. Domain, Data, and Services carry over unchanged. R1 remains Windows-desktop-first. If a Web viewer is ever built, it is a separate non-XAML web app over a sync API.

## Consequences

- The framework bet is confined to the App layer and stays revisitable: a future mobile effort could reuse the same core under Avalonia 12 (mobile rewrite maturing) or a different UI framework entirely.
- XAML hot reload on Avalonia's free tier is weaker than Uno's on paper; accepted after the spike reaction. Uno's hot-reload reliability bugs are escaped.
- Interaction-heavy controls drop the VSM storyboard tax (~160 lines → ~30 declarative lines in the card's case).
- The spike (branch `spike/avalonia-board`) seeds the real migration; migration planning/execution is a separate effort, out of scope of the wayfinder map.
- Known Avalonia 11.3.22 gotchas (property-element `Transitions` NRE, unit-less `BoxShadow`, directional easing classes) are documented in `spike/README.md`.
