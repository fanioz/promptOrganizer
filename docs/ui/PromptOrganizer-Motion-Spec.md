# Prompt Organizer – Motion & Microinteraction Spec

## Motion Style & Principles

- Speed: fast and purposeful; most UI changes complete within 120–260ms
- Easing: productivity-optimized ease-out; no cinematic drags; minimal overshoot
- Subtlety: tint, opacity, small elevation changes; avoid large scale unless for modal/overlay
- Consistency: same curves for similar actions; predictable timing hierarchy
- Clarity: motion communicates state change, focus, and spatial relation; never distracts
- Elevation: lightweight shadows to indicate layering and affordance; avoid heavy blur

## Easing Curves

- Quick ease-out: `cubic-bezier(0.2, 0, 0, 1)` – primary for hover/tint/opacity
- Standard ease: `cubic-bezier(0.25, 0.1, 0.25, 1)` – general transitions
- Decel-out: `cubic-bezier(0.15, 0.85, 0.35, 1)` – modal settling, overlay expansion
- Spring-lite: `cubic-bezier(0.2, 0.7, 0.2, 1)` – slight lift on drag start (no overshoot)

## Durations (ms)

- Hover: 120–140ms (opacity, tint)
- Press/tap: 80–100ms (scale to 0.98 and shadow reduce)
- Card drag start: 120–140ms (elevation + translate), drag settle: 160–200ms
- Card drop: 160–200ms (snap to placeholder)
- Modal open: 240–260ms (scale 0.98→1.0, fade-in, elevation)
- Modal close: 180–220ms (fade-out, scale 1.0→0.98)
- Global search overlay: 200–220ms (fade-in + slight scale)
- Snackbar/toast: 160ms fade-in, visible ~1800ms, 160ms fade-out

## Elevation & Shadow Behavior

- Card default: 1dp shadow `rgba(0,0,0,0.08) 0 1px 3px`
- Card hover: 2dp `rgba(0,0,0,0.10) 0 3px 8px`
- Card drag: 3dp `rgba(0,0,0,0.12) 0 6px 12px` + outline accent
- Modal: 4dp `rgba(0,0,0,0.14) 0 8px 16px` + subtle background blur
- Overlay: 3dp `rgba(0,0,0,0.12) 0 6px 12px`

## Microinteractions

- Card hover (reveal actions)
  - Actions fade from `0 → 1` opacity in 120ms (quick ease-out)
  - Card background tints to `SurfaceAlt`; shadow lifts to 2dp in 140ms
  - Title and preview remain stationary; no layout shift
  - Implementation anchor: VisualStates in `PromptOrganizer.App/PromptOrganizer.App/Controls/PromptCardControl.xaml:68–97`

- Card drag & drop
  - On drag start: translateY `-2px`, lift to 3dp, accent outline `1px` in 140ms
  - Placeholder appears in original slot with subtle tint; `snap` on drop in 180ms
  - Drop target columns tint lightly; invalid drop triggers a brief shake (no motion on content)

- Card creation
  - New card inserts with fade-in 140ms and slight translateY `-2px → 0` 140ms
  - Optional highlight ring `FocusBrush` for 1s to guide attention (no blinking)

- Card deletion
  - Fade-out 120ms + scale `1.0 → 0.98` 100ms; collapse height afterwards (no layout jump)

- Modal editor open/close
  - Open: surface scales `0.98 → 1.0`, opacity `0 → 1` in 240–260ms (decel-out), elevation to 4dp
  - Close: opacity `1 → 0`, scale `1.0 → 0.98` in 180–220ms
  - Backdrop: fade	int `0 → 0.4` 220ms; reverse on close

- Global search overlay (Ctrl+K)
  - Entry: fade-in 200–220ms with scale `0.98 → 1.0`; focus ring on input in 120ms
  - Exit: fade-out 160–180ms
  - List item hover: background tint + 1dp raise 120ms; selection snap on Enter

- Button feedback + copy confirmation
  - Press: scale `1.0 → 0.98` 80ms then revert 80ms
  - Snackbar: fade-in 160ms, visible ~1800ms, fade-out 160ms; anchored near top bar

- Favorite/Pin interactions
  - Toggle: icon fills with quick `opacity` 120ms; subtle ring pulse `alpha 0.15 → 0` 180ms
  - Avoid overshoot or bounce; keep intentional and quiet

## Haptics (Mobile, Future)

- Light tap: 10–15ms `impact light` for button press
- Selection: gentle `selection change` on moving between results
- Drag pick/drop: tiny `impact medium` on pickup, soft tick on drop
- Copy succeed: brief `success` tick; avoid repetitive patterns

## Accessibility – Reduced Motion

- Respect OS reduced-motion preference: disable scale transformations; use instant state changes
- Keep tint and opacity transitions ≤ 80ms
- Avoid blur or large shadow changes; maintain clear focus rings
- Provide non-motion cues (color, outline, snackbar text)

## Mistakes To Avoid (Productivity)

- Long cinematic animations; anything >300ms for primary interactions
- Large scale changes on list items; causes perceived jitter and distracts
- Animating layout properties that reflow content during interaction
- Overuse of shadows or blur; muddy visuals and add cognitive load
- Inconsistent curves across similar actions; leads to unpredictability
- Excessive color shifts on hover; minimal tint only

## Implementation Guidance (UNO)

- Use `VisualStateManager` with `Storyboard` (DoubleAnimation for Opacity, Scale via Composition)
- Leverage `ImplicitAnimations` with Composition for hover/press where available
- Keep animations on transform/opacity; avoid layout-affecting properties
- Centralize durations in a `Motion` ResourceDictionary for reuse (future extension)
- Reference anchors:
  - Card actions: `PromptOrganizer.App/PromptOrganizer.App/Controls/PromptCardControl.xaml:42–46`
  - VisualStates: `PromptOrganizer.App/PromptOrganizer.App/Controls/PromptCardControl.xaml:68–97`

