# Getcha Next Sprint Board

## How To Use This Board
- Status legend: `[TODO]`, `[IN PROGRESS]`, `[DONE]`
- Priority label: `[SHIP BLOCKER]` means must be complete before release candidate.
- Test label: each milestone includes `TEST NOW` cases for immediate validation in-editor.
- Suggested sprint goal: finish all Ship Blockers first, then take Non-Blockers in order.

---

## Milestone 1 - Mission Result and Reward Loop
**Label:** `[SHIP BLOCKER]`  
**Status:** `[TODO]`  
**Goal:** Complete mission-to-battle-to-reward flow so players get deterministic outcomes and progression updates.

### Scope
- Wire battle victory/defeat outcome into mission completion pipeline.
- Implement post-battle result screen (victory/defeat + rewards summary).
- Award mission rewards (gil/items/exp) from mission data.
- Mark mission completion/star state and persist it.
- Ensure `MissionLaunchContext` is consumed and cleared reliably.

### TEST NOW
- [ ] Win mission from world map and verify result screen appears with correct mission name.
- [ ] Verify gil increases by expected value from mission data.
- [ ] Verify item drops (if configured) are added to inventory and saved.
- [ ] Verify unit EXP/level updates after mission and persists after reload.
- [ ] Verify mission completion marker/check state appears in mission list.
- [ ] Lose mission and verify failure path does not grant victory-only rewards.
- [ ] Relaunch game and verify completion + rewards remain persisted.

### Exit Criteria
- End-to-end mission completion works without manual editor resets.
- Reward payouts are correct and persisted across save/load.

---

## Milestone 2 - Story Runtime Integration (Chapter 1)
**Label:** `[SHIP BLOCKER]`  
**Status:** `[TODO]`  
**Goal:** Turn authored story assets into playable narrative flow in-game.

### Scope
- Build/finish runtime loader for Chapter 1 dialogue pack JSON + playback order.
- Trigger chapter cutscenes, mission pre/post beats, and optional camp scenes at correct points.
- Hook state flags to progression events.
- Add skip/continue UX and safe fallback behavior if a segment file is missing.

### TEST NOW
- [ ] Start Chapter 1 and confirm opening cutscene loads from dialogue pack.
- [ ] Enter a mission and verify pre-mission dialogue appears in correct order.
- [ ] Complete mission and verify post-mission dialogue triggers.
- [ ] Trigger an optional camp scene and verify it can be skipped safely.
- [ ] Resume from save mid-chapter and verify playback order remains correct.
- [ ] Simulate missing dialogue file and verify fallback does not hard-lock progression.

### Exit Criteria
- Chapter 1 narrative loop is playable from start to finale with no dead ends.

---

## Milestone 3 - Battle UX and Rules Polish
**Label:** `[SHIP BLOCKER]`  
**Status:** `[TODO]`  
**Goal:** Close known battle-system usability and rules gaps that affect player trust.

### Scope
- Enforce strict target mode prompts for ally/enemy targeting where applicable.
- Improve AOE targeting feedback/preview.
- Refine target indicator readability and state transitions.
- Verify queue/execute + lock-state visuals are always accurate.

### TEST NOW
- [ ] Queue single-target ally item and verify only valid ally targets are selectable.
- [ ] Queue enemy-only skill and verify ally selection is blocked.
- [ ] Use AOE skill and verify affected targets are previewed clearly.
- [ ] Confirm target indicators clear correctly after action execution.
- [ ] Confirm acted/locked/queued card states match actual battle state each turn.

### Exit Criteria
- No target selection ambiguity in common battle flows.
- UI always reflects true action eligibility/state.

---

## Milestone 4 - Economy Surface Completion (Archive Shop + Reward Sinks)
**Label:** `[SHIP BLOCKER]`  
**Status:** `[TODO]`  
**Goal:** Complete player-facing economy loop beyond currency accumulation.

### Scope
- Implement functional Archive Shop using archive tokens.
- Add at least one validated sink for each major currency (crystals, gil, archive tokens, shards).
- Connect purchases to inventory/progression/save systems.

### TEST NOW
- [ ] Earn archive tokens via banner spark conversion and spend them in shop.
- [ ] Verify purchase inventory grants are immediate and persisted.
- [ ] Verify insufficient-currency handling and messaging are correct.
- [ ] Verify duplicate purchase constraints (if any) are enforced.

### Exit Criteria
- Archive tokens have a complete spend loop.
- Economy currencies have clear player-facing uses.

---

## Milestone 5 - Critical Combat Depth Follow-Through
**Label:** `[NON-BLOCKER]`  
**Status:** `[TODO]`  
**Goal:** Advance depth systems already scaffolded in docs/code.

### Scope
- Complete crit behavior end-to-end (chance + damage effects + UI feedback).
- Extend speed/initiative behavior beyond placeholder status.
- Add balancing passes for chain timing windows and damage spikes.

### TEST NOW
- [ ] Verify critical hits can occur and display expected feedback.
- [ ] Verify crit damage formula behaves as configured.
- [ ] Verify speed influences turn dynamics as designed.
- [ ] Run 10+ battle simulations and compare DPS variance before/after tuning.

### Exit Criteria
- Crit and speed systems are no longer partial placeholders.

---

## Milestone 6 - Content Readiness for Chapter 2
**Label:** `[NON-BLOCKER]`  
**Status:** `[TODO]`  
**Goal:** Move Chapter 2 from authored data to production-ready content package.

### Scope
- Fill `unit_asset_manifest.json` placeholders for unit_011 to unit_016.
- Verify all referenced assets load correctly in UI and battle.
- Validate new banner pool integration and unit summonability.

### TEST NOW
- [ ] Summon each new Chapter 2 unit via test banner seed/tools.
- [ ] Verify icon/portrait/idle/attack assets resolve correctly.
- [ ] Verify no missing sprite fallback appears in summon, inventory, or battle.
- [ ] Verify new units have correct stats/skills at each rarity entry.

### Exit Criteria
- Chapter 2 units are fully presentable and playable in-game.

---

## Milestone 7 - Server-Ready Save Hardening (Local Default)
**Label:** `[NON-BLOCKER]`  
**Status:** `[TODO]`  
**Goal:** Keep local-first workflow while de-risking future backend cutover.

### Scope
- Add integration tests around `ISaveStorage` local/server parity.
- Validate save schema migration coverage for newest fields.
- Add error handling and retry strategy for HTTP storage mode.

### TEST NOW
- [ ] Save/load with local backend across all core systems.
- [ ] Switch to server mode in test config and verify round-trip parity.
- [ ] Corrupt/partial response simulation: verify graceful recovery behavior.
- [ ] Validate backward compatibility loading older profile snapshots.

### Exit Criteria
- Save architecture is backend-agnostic for future migration.

---

## Recommended Sprint Order
1. Milestone 1 - Mission Result and Reward Loop `[SHIP BLOCKER]`
2. Milestone 2 - Story Runtime Integration `[SHIP BLOCKER]`
3. Milestone 3 - Battle UX and Rules Polish `[SHIP BLOCKER]`
4. Milestone 4 - Economy Surface Completion `[SHIP BLOCKER]`
5. Milestone 5 - Critical Combat Depth Follow-Through `[NON-BLOCKER]`
6. Milestone 6 - Content Readiness for Chapter 2 `[NON-BLOCKER]`
7. Milestone 7 - Server-Ready Save Hardening `[NON-BLOCKER]`

---

## Sprint Sign-Off Checklist
- [ ] All Ship Blockers marked `[DONE]`.
- [ ] Each Ship Blocker passed all TEST NOW checks.
- [ ] Known defects logged for remaining Non-Blockers.
- [ ] Build accepted for next playtest gate.