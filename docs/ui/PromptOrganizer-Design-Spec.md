# Prompt Organizer – High-Fidelity UI Design Spec

## Visual Foundations

- Background: Light `#F7F7F8` (neutral soft gray), Dark `#111316` (near-black, low-gloss)
- Accent palette:
  - Blue `#3B82F6`, Green `#22C55E`, Yellow `#FACC15`, Pink `#EC4899`, Red `#EF4444`, Violet `#8B5CF6`
- Neutral scale (light): `#FFFFFF`, `#F7F7F8`, `#EDEEF0`, `#D9DBE1`, `#B8BCC6`, `#8A90A3`, `#6B7280`, `#4B5563`, `#1F2937`
- Neutral scale (dark): `#0B0D10`, `#111316`, `#16181C`, `#1D2026`, `#242832`, `#2E3440`, `#3B4252`, `#6B7280`, `#E5E7EB`
- Elevation: 1dp shadow `rgba(0,0,0,0.08) 0 1px 3px`, 2dp `rgba(0,0,0,0.10) 0 3px 8px`

## Typography System

- UI font: `Segoe UI` (fallback: `Inter`, `System UI`)
- Monospace editor: `Cascadia Mono` (fallback: `Consolas`, `SF Mono`)
- Type scale (px): `12`, `14`, `16`, `18`, `20`, `24`, `32`
  - Title L `24` / `600`
  - Title M `20` / `600`
  - Subtitle `16` / `500`
  - Body `14–16` / `400`
  - Caption `12` / `400`
  - Code editor base `14` / monospace

## Spacing System

- Base unit `4px` → `4, 8, 12, 16, 24, 32, 40, 56`
- Column gap `24–32px`
- Card gap `16–20px`
- Page padding `24px` (desktop), `16px` (compact)
- Card padding `16px` (internal), `12px` (compact)
- Modal padding `24px` body, `16px` header/footer

## Corner Radii

- Card `8px`
- Chip `10px`
- Button `6px`
- Modal `12px`
- Column header `8px`

## Main Board View

- Top bar: 56px height, sticky; left `Prompt Organizer`, right `+ New Prompt`, `Search`, `Settings`
  - Title font `20px 600`
  - Search width `360px` default, icon-left, placeholder “Search prompts… (Ctrl+K)”
  - Actions spacing `12px` between controls
- Board area: horizontal scroll; columns in flex row with gap `24–32px`
- Column:
  - Width `320–360px`
  - Header: Title (bold `16px`), `+ Add` secondary button
  - Body: vertical stack of cards with gap `16–20px`
- Drag-and-drop affordances:
  - On drag: card elevates to 2dp, outline `#3B82F6` 1px
  - Drop zones: subtle highlight background `#EDEEF0` (light) or `#1D2026` (dark)

### Prompt Card Layout

- Size: fluid, min height `96px`
- Structure:
  - Left color stripe: `4px` width, full height; uses label color
  - Top row: emoji icon (20px), Title bold `16px`, Favorite star
  - Tags: wrap chips under title (gap `8px`)
  - Body preview: 2–3 lines, truncated; neutral-700 color
  - Quick actions: ⭐ Favorite, 📋 Copy, ✏️ Edit show on hover (fade-in 120ms)
- States:
  - Default: elevation 1dp, background `#FFFFFF` light / `#16181C` dark
  - Hover: elevation 2dp, background tint `#FAFAFB` / `#1D2026`
  - Focus: 2px outline `#8B5CF6`
  - Active: compress `-2px` translateY, shadow reduce
  - Disabled: `opacity 0.6`, interactions off
- Chip style:
  - Background `#EDEEF0` / `#242832`
  - Text `#4B5563` / `#E5E7EB`
  - Padding `6px 10px`, radius `10px`

### Card Quick Actions

- Placement: top-right overlay on hover
- Icons: 16px, hit target `32px`
- Spacing: `8px`
- Tooltips: brief labels “Favorite”, “Copy”, “Edit”
- Keyboard: `Enter` opens editor, `Ctrl+C` copies prompt, `F` toggles favorite

## Prompt Editor Modal

- Container: centered, max width `960px`, radius `12px`, elevation 2dp
- Header:
  - Title field (inline editable), Category dropdown, Icon picker, Label color picker, Tags input
  - Alignment grid: 2 columns for compact controls
- Body:
  - Markdown editor: monospaced `14px`; toolbar minimal (bold, italics, code, list)
  - Preview toggle (split view optional)
- Advanced settings:
  - Temperature (0–1 slider), Top-K (int), Max Tokens (int), Model Name (text)
  - Layout: 2-column grid; labels `12px` caption; controls `14px`
- Footer:
  - Primary Save button (blue), Secondary Copy, Tertiary Export
  - Button spacing `12px`
- Shortcuts:
  - `Ctrl+S` Save, `Ctrl+Enter` Copy, `Ctrl+E` Export, `Esc` Close

## Global Search Overlay

- Trigger: `Ctrl+K`
- Position: centered overlay, width `720px`
- Input: large field `48px` height, left search icon, placeholder “Search prompts…”, focus outline `#8B5CF6`
- Results list:
  - Row height `56px`
  - Contents: icon, title, category pill, tags
  - Hover: background `#FAFAFB` / `#1D2026`
  - Selection via arrows; `Enter` open editor; `Ctrl+Enter` copy

## Button Styles

- Primary: background `#3B82F6`, text `#FFFFFF`; hover darken `#2563EB`; focus outline `#8B5CF6`
- Secondary: background `#EDEEF0` / `#242832`; text `#1F2937` / `#E5E7EB`
- Tertiary (text button): text `#3B82F6`; hover underline
- Sizes: S `28px`, M `32px`, L `40px`; horizontal padding `12–16px`

## Iconography

- Emoji-style minimal for cards; system `Segoe Fluent Icons` for controls
- Icon size: `16–20px` in cards; `20–24px` in top bar

## Microinteractions

- Hover fade: `120ms` ease-out for actions
- Shadow transition: `140ms` cubic-bezier(0.2, 0, 0, 1)
- Drag lift: `transform: translateY(-2px)` and shadow to 2dp
- Chip hover: subtle raise `1dp`
- Search overlay open: scale `0.98 → 1.0` and opacity `0 → 1` in `160ms`

## Accessibility

- Contrast: all text/background ≥ `4.5:1`; small text ≥ `7:1`
- Focus ring: clearly visible `2px` violet `#8B5CF6`
- Keyboard: tab order from top bar → columns → cards → actions
- ARIA:
  - Cards: `role="article"` with `aria-label` title
  - Columns: `role="list"`, cards `role="listitem"`
  - Search results: `role="listbox"` with `option`s
  - Modal: `role="dialog"` with `aria-modal="true"` and title

## Light & Dark Modes

- Light base: background `#F7F7F8`, surfaces `#FFFFFF`, text `#1F2937`
- Dark base: background `#111316`, surfaces `#16181C`, text `#E5E7EB`
- Accents unchanged; adjust hover tints accordingly

## Component Specs Summary

- Card: padding `16px`; gap `8px` between title and tags; stripe `4px`; actions hover
- Column: width `340px`; header `56px`; gap `16–20px` between cards
- Top bar: `56px`; left title, right actions; search `360px`
- Modal: max `960px`; padding `24px`; footer aligned right
- Search overlay: `720px` width; results `56px` rows

## Implementation Notes (UNO)

- Use ResourceDictionary for tokens; Light/Dark dictionaries for brushes
- Use `ThemeShadow` where available; fallback to subtle borders for elevation
- Use `ItemsRepeater` or `ListView` for cards; enable `CanDragItems` for DnD
- Use `ContentDialog` or custom modal for editor; ensure keyboard shortcuts via `KeyboardAccelerators`

## Annotations Key

- Spacing values in px; type weights indicated by numeric (`400`, `500`, `600`)
- Colors provided in hex; use brushes named per token in implementation

