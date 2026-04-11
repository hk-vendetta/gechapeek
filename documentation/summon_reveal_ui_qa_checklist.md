# Summon Reveal UI QA Checklist

Last updated: April 8, 2026
Owner: QA / UI / Gameplay

## Purpose

Use this checklist after any scene, prefab, or script change touching summon flow, reveal sequencing, grid layout, scroll behavior, or button wiring.

## Test Setup

- Scene: SummonAttunement
- Required token balance:
  - Enough for at least one 1-pull
  - Enough for at least one 10-pull
- Ensure Console is open and clear before each run.
- Use Play Mode with default resolution first, then test one narrow and one wide resolution.

## Core Smoke Tests

### 1. Attune 1 Button

- Click Attune 1 once.
- Verify one pull is consumed.
- Verify reveal sequence starts.
- Verify returned unit is added to inventory.

Pass if:
- Only one pull occurs.
- Inventory count increases by exactly 1.

### 2. Attune 10 Button

- Click Attune 10 once.
- Verify ten pulls are consumed.
- Verify reveal sequence starts.
- Verify all pulled units are added to inventory.

Pass if:
- Exactly 10 pulls occur.
- Inventory count increases by exactly 10.

## Reveal Sequence Validation

### 3. Phase Order

Validate order:

1. Overlay fade / intro
2. Featured prism phase
3. Grid phase
4. Done phase and close availability

Pass if:
- Phases appear in order with no skips or dead-ends.

### 4. Featured Prism Interaction

- Confirm tap-to-reveal behavior works.
- Confirm crack animation completes.
- Confirm transition to grid phase occurs.

Pass if:
- No stuck waiting state.

### 5. Grid Visibility

- Confirm grid appears during phase 2.
- Confirm prism widgets are visible, not hidden by overlays.
- Confirm reveal animations play for all expected units.

Pass if:
- No black/blank full-screen layer hides widgets.

## Grid Layout and Scrolling

### 6. Multi-Column Layout

- Run a 10-pull.
- Confirm widgets are laid out in multiple columns.
- Confirm content is top-aligned at phase start.

Pass if:
- No single long center column.

### 7. Vertical Scrollbar

- In grid phase, test all of:
  - Mouse wheel scroll
  - Dragging within viewport
  - Dragging scrollbar handle

Pass if:
- Scrolling moves content consistently.
- Scrollbar track/handle stay visible and interactable.

### 8. Overlay and Clipping Safety

- Confirm viewport does not hide entire content.
- Confirm close button remains visible at done phase.
- Confirm scrollbar does not cover full viewport with opaque layer.

Pass if:
- Grid content and close button are both visible when expected.

## Return Flow and State Reset

### 9. Close and Return

- Complete reveal, press Close.
- Confirm return to attunement screen.
- Run another summon without reloading scene.

Pass if:
- Second run behaves identically to first.
- No stale UI state, missing widgets, or stuck overlays.

### 10. Repeatability

- Run 3 back-to-back summons:
  - 1-pull
  - 10-pull
  - 10-pull

Pass if:
- No regression after repeated runs.
- No accumulating layout drift.

## Resolution Coverage

### 11. Narrow Resolution

- Test at a narrow ratio/resolution.
- Confirm columns reduce gracefully and scroll remains usable.

Pass if:
- Content remains readable and reachable.

### 12. Wide Resolution

- Test at a wide ratio/resolution.
- Confirm columns expand appropriately without clipping.

Pass if:
- Grid uses space effectively and remains centered/aligned.

## Console and Error Check

### 13. Console Cleanliness

- During and after each run, verify Console.

Pass if:
- No new errors.
- No repeated warning spam related to reveal/grid wiring.

## Regression Fields

Record for each run:

- Build/commit: 
- Tester: 
- Date/time: 
- Scene: SummonAttunement
- Resolution: 
- Pull type: 1 or 10
- Result: Pass / Fail
- Notes:

## Fast Failure Triage

If failure occurs, capture:

- Which phase failed
- Pull type (1 or 10)
- Whether inventory increment matched expected count
- Whether grid rendered
- Whether scrollbar input worked
- First relevant Console message

Then check:

- Summon button onClick target wiring
- Reveal canvas/root active state and scale
- ScrollRect content/viewport references
- PrismGrid parent/anchors/constraint settings
- Sequencer warnings from grid setup validation
