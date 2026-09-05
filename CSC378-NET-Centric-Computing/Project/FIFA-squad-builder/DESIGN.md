---
name: FIFA Squad Builder
description: A black-and-gold local squad-builder interface for EA FC-style teams.
colors:
  bg-deep: "oklch(8% 0.01 84)"
  bg-mid: "oklch(13% 0.014 84)"
  panel: "oklch(11% 0.012 84 / 0.92)"
  panel-soft: "oklch(18% 0.018 84 / 0.78)"
  border: "oklch(78% 0.11 86 / 0.28)"
  gold: "oklch(74% 0.14 84)"
  gold-light: "oklch(89% 0.12 90)"
  pitch-line: "oklch(80% 0.13 86)"
  text: "oklch(94% 0.012 88)"
  text-dim: "oklch(74% 0.035 88)"
  danger: "#ff8a8a"
typography:
  display:
    fontFamily: "Segoe UI, system-ui, -apple-system, BlinkMacSystemFont, sans-serif"
    fontSize: "3rem"
    fontWeight: 800
    lineHeight: 1.05
    letterSpacing: "0"
  title:
    fontFamily: "Segoe UI, system-ui, -apple-system, BlinkMacSystemFont, sans-serif"
    fontSize: "1.3rem"
    fontWeight: 700
    lineHeight: 1.2
    letterSpacing: "0"
  body:
    fontFamily: "Segoe UI, system-ui, -apple-system, BlinkMacSystemFont, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.5
    letterSpacing: "0"
  label:
    fontFamily: "Segoe UI, system-ui, -apple-system, BlinkMacSystemFont, sans-serif"
    fontSize: "0.85rem"
    fontWeight: 700
    lineHeight: 1
    letterSpacing: "0.08em"
rounded:
  control: "7px"
  pitch: "4px"
  panel: "10px"
  pill: "999px"
spacing:
  xs: "0.4rem"
  sm: "0.6rem"
  md: "1rem"
  lg: "1.5rem"
  xl: "3rem"
components:
  button-primary:
    backgroundColor: "{colors.gold-light}"
    textColor: "{colors.bg-deep}"
    rounded: "{rounded.control}"
    padding: "0.5rem 0.9rem"
  panel:
    backgroundColor: "{colors.panel}"
    textColor: "{colors.text}"
    rounded: "{rounded.panel}"
    padding: "1rem"
  chip:
    backgroundColor: "{colors.panel-soft}"
    textColor: "{colors.text-dim}"
    rounded: "{rounded.pill}"
    padding: "0.38rem 0.62rem"
---

# Design System: FIFA Squad Builder

## 1. Overview

**Creative North Star: "The Custom Squad Desk"**

FIFA Squad Builder is a product UI, not a marketing site. The interface should feel like a black-and-gold football workbench: the pitch is dominant, controls are close to the action, and the right-side analysis panel gives practical feedback without visual noise.

The system rejects copied third-party builder branding and generic AI-looking decoration. It can borrow the category expectation of real card artwork on a formation pitch, but the app chrome must stay independent, compact, and personal.

**Key Characteristics:**
- Pitch-led composition with restrained surrounding chrome.
- Real card artwork preferred over CSS imitation whenever available.
- Dense stats presented in calm grouped panels.
- Gold accents used for action, position state, and important football hierarchy.

## 2. Colors

The palette is warm near-black, worn gold, and ivory text. Pitch grass remains as content, not as the app chrome.

### Primary
- **Match Gold:** Primary action color for save, rename, and important squad actions.
- **Warm Highlight:** Headings, key totals, and selected emphasis.

### Neutral
- **Warm Near-Black:** Main app background and navigation chrome.
- **Dugout Black:** Analysis panels, bench strip, and modal surfaces.
- **Muted Chalk:** Body text and supporting labels.
- **Pitch Border:** Subtle dividers and field enclosure.

### Named Rules

**The Clean Pitch Rule.** The pitch must never carry decorative brand text, watermarks, or copied builder labels.

**The Gold Budget Rule.** Gold is for action, hierarchy, and football state; it is not background decoration.

## 3. Typography

**Display Font:** Segoe UI with system-ui fallback.
**Body Font:** Segoe UI with system-ui fallback.
**Label/Mono Font:** System sans only.

**Character:** The type should feel native to Windows and Visual Studio-era tooling: familiar, readable, and compact. No decorative display font belongs in data, labels, buttons, or squad slots.

### Hierarchy
- **Display** (800, 3rem, 1.05): Home headline only.
- **Headline** (700, 1.3rem, 1.2): Squad titles and panel titles.
- **Title** (700, 1rem, 1.2): Player names, stat groups, modal titles.
- **Body** (400, 1rem, 1.5): Forms, table copy, and explanatory text.
- **Label** (700, 0.85rem, 0.08em): Panel headers, compact metadata, and position labels.

### Named Rules

**The Working Type Rule.** If text is part of a control, table, card, stat, or modal, it uses the system sans scale and zero negative letter spacing.

## 4. Elevation

Depth is a hybrid of tonal layering, inset pitch shading, and short ambient shadows. Shadows should make cards feel clickable and panels readable, not glossy or futuristic.

### Shadow Vocabulary
- **Navigation shadow** (`0 2px 22px rgba(0,0,0,0.34)`): App bar separation from the page.
- **Pitch shadow** (`0 18px 40px rgba(0,0,0,0.48)`): The pitch as the main work surface.
- **Card lift** (`0 10px 18px rgba(0,0,0,0.48)`): Player cards and empty slots.

### Named Rules

**The Resting Surface Rule.** Panels stay mostly flat; lift belongs to cards, active controls, and the pitch.

## 5. Components

### Buttons
- **Shape:** Slightly rounded, practical controls (7px).
- **Primary:** Gold gradient background with deep green text and heavy weight.
- **Hover / Focus:** Brighter gold on hover; visible two-ring focus using dark base and green highlight.
- **Secondary / Ghost:** Text links stay quiet and should not compete with primary actions.

### Chips
- **Style:** Soft translucent panel background, pill radius, muted text.
- **State:** Used for workflow tags and compact metadata, not as decorative badges.

### Cards / Containers
- **Corner Style:** Panels use restrained rounding (10px); the pitch uses a sharper edge (4px).
- **Background:** Translucent dark green panels over a darker page.
- **Shadow Strategy:** Panels use tonal depth; player cards and pitch carry stronger lift.
- **Border:** Thin translucent borders only.
- **Internal Padding:** Default panel padding is 1rem.

### Inputs / Fields
- **Style:** Dark translucent fill, thin warm border, 7px radius.
- **Focus:** Slightly brighter fill with green line and visible outer focus ring.
- **Error / Disabled:** Error text uses warm red; disabled states should keep shape but reduce opacity.

### Navigation

The top navigation is compact, dark, and utilitarian. The brand reads as "FIFA Squad Builder"; links are short nouns. Admin links remain visible only to admin users and should not introduce extra marketing language.

### Squad Pitch

The pitch is the signature component. It uses a clean local pitch asset, formation-aware absolute slots, complete card images when available, and restrained black-and-gold empty-slot shells. Slot labels sit below cards in gold-on-black pills.

## 6. Do's and Don'ts

### Do:
- **Do** keep the pitch free of decorative text and third-party labels.
- **Do** use actual locally cached card artwork when a player has `CardImageUrl`.
- **Do** keep squad controls near the squad title and formation selector.
- **Do** use dense stat panels when the numbers help decisions.
- **Do** preserve visible focus states on all clickable controls.

### Don't:
- **Don't** make it look like the default Bootstrap starter template.
- **Don't** copy FUTWIZ, FUTBIN, EA, or any other third-party builder branding into the pitch or app chrome.
- **Don't** use generic AI-looking decoration: oversized marketing cards, purple-blue gradients, floating blobs, vague hero copy, or feature descriptions that explain obvious UI.
- **Don't** let the pitch become cluttered with banners, slogans, watermarks, or decorative text.
- **Don't** replace standard controls with strange custom affordances just for flavor.
