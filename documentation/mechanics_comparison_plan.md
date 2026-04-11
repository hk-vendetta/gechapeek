# Getcha Mechanics Comparison and Expansion Plan

## Purpose

This document compares major combat and progression mechanics described on the FFBE mechanics reference page against Getcha's current implementation. The goal is not to copy FFBE names or content. The goal is to identify which mechanic patterns translate cleanly into Getcha, which ones do not fit the current architecture, and which original systems should be prioritized so Getcha develops its own identity.

---

## Current Getcha Baseline

Getcha already has a strong mechanical base:

- Queue-then-execute player turns with deliberate action ordering.
- Hit-timed chaining with chain families, spark logic, element chain support, synergy bonus, and chain cap.
- Physical and magical damage paths.
- Multi-hit frame-authored skills.
- Status ailments with action lock, silence, berserk, poison ticks, wake-on-hit sleep handling, and cleanse support.
- Defend as an explicit player action.
- Equipment-driven stat growth.
- Mission energy system with offline recovery.
- Gacha pity, spark, archive conversion, and duplicate conversion systems.
- Enemy and mission reward drops.

This means Getcha is already mechanically denser than a basic mobile RPG. The strongest next steps are systems that deepen battle decisions without forcing a full combat rewrite.

---

## Similarities Between FFBE and Getcha

### Combat Similarities

- Both games use team-based battles where roster construction matters as much as in-battle decisions.
- Both games separate physical and magical offense/defense concepts.
- Both games support elemental attributes on skills.
- Both games use multi-hit attacks as an important damage expression tool.
- Both games reward sequencing attacks to raise chain damage.
- Both games use status ailments as a major tactical layer.
- Both games rely on equipment as a stat and utility layer.

### Progression and Economy Similarities

- Both games use stamina/energy-gated mission play.
- Both games use gacha with pity-like safety nets.
- Both games support long-tail roster growth beyond pure pull luck.

---

## Differences Between FFBE and Getcha

### Structural Differences

- FFBE is built around live tap timing, multi-cast overlap, and animation-level execution windows.
- Getcha is built around queue-first, execute-second tactical sequencing.
- FFBE often assumes duplicate chainers or frame-identical units.
- Getcha is better suited to planned team sequencing across distinct roles.

### Current Mechanical Differences

- FFBE has a much broader combat ruleset: cover, provoke, evasion, accuracy, dual wield, reflect, jump, break gauge, limit burst gauge, espers, imperils, killers, and layered mitigation.
- Getcha currently uses a cleaner and simpler battle ruleset with fewer exception systems.
- FFBE damage formulas are heavily modifier-driven and highly layered.
- Getcha damage formulas are intentionally more readable and easier to balance.

### Design Identity Differences

- FFBE's identity comes from execution precision, chain families, burst setup turns, and deep formula interactions.
- Getcha's identity is better positioned around authored turn order, role synergy, mission progression, and readable tactical decision-making.

That last point matters. Getcha should borrow only the mechanics that strengthen tactical sequencing. It should avoid becoming a formula-heavy clone.

---

## Missing Mechanics That Getcha Can Implement Easily

These are the best candidates because the current data model already contains most of the needed inputs.

### 1. Element Resistance and Weakness Resolution

### Why it is easy

- Skills already have an `element` field.
- Units already carry `physicalResist` and `magicalResist` fields.
- Equipment already stores `elementResist` values.
- Battle damage currently does not appear to consume these values in resolution.

### What to add

- Compute target element resistance before final damage is applied.
- Support positive resistance, negative resistance, and immunity thresholds.
- Keep the naming original in presentation. Use terms like `Attunement Guard`, `Attunement Weakness`, or `Element Ward` instead of FFBE phrasing.

### Value to Getcha

- Makes team building matter more.
- Makes enemy identity more readable.
- Makes element chain teams more strategically meaningful.
- Reuses existing data instead of demanding large schema changes.

### 2. Status Resistance Enforcement

### Why it is easy

- Equipment already stores `statusResist` values.
- Status definitions already exist and battle status application is centralized.
- Status infliction already happens through skills and weapon procs.

### What to add

- Run a resist check before applying status.
- Aggregate resist from equipment and base unit definitions.
- Allow enemies to have explicit status resistance values over time.
- Present this with original language such as `Ward`, `Purity`, or `Resolve` rather than copying FFBE UI terminology.

### Value to Getcha

- Makes gear matter defensively.
- Creates more meaningful encounter prep.
- Reduces frustration spikes from unavoidable status lock teams.

### 3. Damage Mitigation Layers

### Why it is easy

- Getcha already distinguishes physical and magical damage.
- Defend already exists and halves damage.
- Units already store physical and magical resistance stats.

### What to add

- Apply physical resistance on physical hits and magical resistance on magical hits.
- Add a simple `general mitigation` buff category later if desired.
- Keep stacking rules clean and readable: one source per mitigation family, strongest wins.

### Value to Getcha

- Tanks and supports gain a clearer battlefield role.
- Defensive team planning becomes more interesting.
- Existing unit stats finally affect combat outcomes directly.

### 4. Structured Break and Buff Overwrite Rules

### Why it is easy

- Status data already supports ATK, DEF, and SPR modifiers.
- BattleUnit already queries modified attack, defense, and spirit.
- The project already uses a status database and duration tracking.

### What to add

- Formalize offensive debuff categories under original naming such as `Fracture` effects.
- Rule: same-stat positive and negative modifiers do not stack infinitely; strongest active source wins in each direction.
- Allow a buff and debuff on the same stat to coexist and net out.

### Value to Getcha

- Cleaner balance.
- Better readability.
- Strong support unit identity without formula bloat.

### 5. Consecutive Skill Ramping

### Why it is fairly easy

- Getcha already stores per-skill data in JSON.
- The queue-based turn model makes repeated-skill planning very readable.
- This can be implemented with small per-unit runtime state instead of global combat rewrites.

### What to add

- Introduce original `Cadence` skills that gain power each time the same skill is used consecutively.
- Reset conditions can be clean: guarding, using a different Cadence skill, or being KO'd.

### Value to Getcha

- Encourages deliberate turn planning.
- Fits the queue-execute model better than FFBE's live tap timing.
- Creates identity for specialists and boss-focused sustained attackers.

---

## Missing Mechanics That Are Medium Difficulty

These are possible, but they need broader rule additions or more UI support.

### 1. Enemy Target Control

Original naming candidates:

- `Challenge` instead of provoke.
- `Shroud` instead of camouflage.

Why medium:

- Requires AI targeting rewrites.
- Needs targeting weights and status/buff persistence.
- Strong upside because it helps tank identity.

### 2. Cover and Intercept Mechanics

Original naming candidates:

- `Intercept` for single-target cover.
- `Bulwark Line` for party cover.

Why medium:

- Requires redirection logic during damage resolution.
- Needs clear priority rules.
- Excellent for tank/support roles, but harder than mitigation alone.

### 3. Accuracy and Evasion

Original naming candidates:

- `Precision` and `Dodge`.

Why medium:

- Requires hit-rate checks across attack types.
- Needs UI clarity when an attack misses.
- Risks making combat feel swingy if added too early.

### 4. Counterattacks

Original naming candidates:

- `Riposte`, `Hex Recoil`, `Aegis Return`.

Why medium:

- Requires reaction windows and recursion guards.
- Best added after mitigation and targeting rules are stable.

---

## Missing Mechanics That Are High Difficulty or Low Fit Right Now

These should not be early priorities.

### 1. Dual Wield as FFBE-Style Action Duplication

Why not early:

- It deeply changes damage flow, equipment logic, hit generation, and balance.
- It would force a larger audit of skills, attack animations, and itemization.

### 2. Limit Burst Gauge Equivalent

Why not early:

- Requires a full new resource layer, skill tiering, UI, pacing rules, and encounter rebalance.
- Best saved for a later combat expansion.

### 3. Esper / Summon Gauge Combat Layer

Why not early:

- Requires summon content, assets, resource generation rules, and identity work.
- Bigger content burden than code burden.

### 4. Break Gauge / Stagger Bar

Why not early:

- Requires enemy UI, weapon typing logic, boss design updates, and reward loops.
- Good mechanic, but not a quick adoption from the current codebase.

### 5. Reflect and Jump Systems

Why not early:

- Both are exception-heavy and harder to explain cleanly in the current flow.
- They add edge cases faster than they add tactical depth at this stage.

---

## Best Easy-Win Adoption Set

If the goal is to gain the most tactical depth with the least engineering risk, the best immediate package is:

1. Element resistance and weakness resolution.
2. Status resistance enforcement.
3. Physical and magical resistance application.
4. Formalized break and buff overwrite rules.
5. A small first batch of Cadence skills.

This package stays original, uses systems Getcha already owns, and expands decision density without turning battle resolution into rules soup.

---

## Original Standout Mechanic Proposal

## Oath Weave

This should be the mechanic that makes Getcha feel unmistakably like Getcha.

### Core idea

Each queued action leaves behind an `Oath Sigil` based on its role:

- Assault
- Arcane
- Guard
- Aid
- Ruin

When actions are executed in a meaningful sequence, adjacent sigils form an `Oath Weave` that grants bonus effects to later actions in the same player phase.

### Example behaviors

- Assault -> Assault: grants extra chain stability for the second action.
- Aid -> Assault: grants a finishing damage surge to the next damaging skill.
- Guard -> Arcane: grants spell shelter or reduced interruption risk.
- Ruin -> Assault: extends debuff duration or increases damage against fractured targets.
- Assault -> Aid -> Assault: creates a triad weave that boosts both healing throughput and the follow-up finisher.

### Why it fits Getcha specifically

- It rewards queue planning, which is already Getcha's strongest battle identity.
- It does not depend on duplicate units or live tap timing.
- It makes action order matter in a way FFBE does not.
- It creates room for uniquely authored units whose skills generate rare sigils or alter weave outcomes.

### Why it is original

- It is not a renamed copy of chaining, limit bursts, espers, or tag attacks.
- It is rooted in Getcha's queue-execute flow and story tone.
- It can become the foundation for future unit identities, boss gimmicks, and support roles.

### Recommended first version

- Only support 5 to 7 weave recipes.
- Limit effects to same-turn bonuses only.
- Surface results clearly in UI with a small banner such as `Weave Formed: Wardfire`.
- Keep recipes authored in data so new units can extend the system later.

---

## Proposed Rollout Plan

## Phase 1: Defensive Rules Pass

Deliverables:

- Element resistance and weakness in damage resolution.
- Physical and magical resistance application.
- Status resistance checks on infliction.

Why first:

- Uses existing data.
- Improves both player and enemy identity.
- Low UI burden.

## Phase 2: Combat State Clarity Pass

Deliverables:

- Formal break/buff overwrite rules.
- Clear status tooltip text for resist, weakness, fracture, and ward states.
- Battle result/debug overlay updates for resist and weakness messaging.

Why second:

- Makes the new rules visible and understandable.
- Prevents invisible-stat frustration.

## Phase 3: Offensive Depth Pass

Deliverables:

- First Cadence skill family.
- Enemy archetypes that reward element and fracture play.
- A few support skills that enhance chain or resist play indirectly.

Why third:

- Gives players new tools after the defense model is in place.
- Expands unit role differentiation.

## Phase 4: Getcha Signature Pass

Deliverables:

- Oath Weave prototype.
- 5 to 7 recipe combinations.
- UI callout for generated weave effects.
- 3 to 5 units authored to interact with weaves in different ways.

Why fourth:

- By this point, the combat foundation is rich enough that an original identity system will have room to matter.

---

## Concrete Engineering Roadmap

This section turns the above recommendations into an implementation sequence with explicit file ownership, task boundaries, and suggested delivery order.

## Guiding Rules For Implementation

- Preserve Getcha's readable combat flow. Do not import FFBE's full formula complexity.
- Prefer extending current DTOs and runtime classes over adding parallel systems.
- Keep all new mechanic labels original in player-facing text.
- Ship in narrow vertical slices that are testable in battle immediately.
- Add debug output for every new combat rule before adding heavy UI polish.

---

## Phase 1 Roadmap: Element and Resistance Foundation

### Scope

- Element resistance and weakness.
- Physical and magical resistance application.
- Debug visibility for resist and weakness outcomes.

### Files to modify

#### Assets/Scripts/BattleUnit.cs

Tasks:

- Add helper methods to aggregate combat defenses from all available sources.
- Add `GetPhysicalMitigationMultiplier()` and `GetMagicalMitigationMultiplier()` or equivalent helpers.
- Add `GetElementResistancePercent(string element)` using:
	- base unit values if present later,
	- currently equipped items from `EquipmentInventory`,
	- safe fallback to 0.
- Extend `CalculateDamage(UnitSkillData ability, BattleUnit target, float chainMultiplier)` so final damage passes through:
	1. stat-based damage,
	2. chain multiplier,
	3. type mitigation,
	4. element resistance or weakness.
- Keep immunity behavior simple: cap minimum final damage at 0 before the usual floor-to-1 rule is applied. If design wants true immunity, skip the `Mathf.Max(1, ...)` floor after resist resolution.

Implementation note:

- Move the final damage floor to the last stage of the pipeline so resist can actually reduce damage to 0 when needed.

#### Assets/Scripts/EquipmentInventory.cs

Tasks:

- Add aggregation helpers:
	- `GetElementResistanceForUnit(GachaItem unit, string element)`
	- `GetStatusResistanceForUnit(GachaItem unit, string statusId)`
- Reuse `GetEquippedItemsForUnit()` and existing equipment fallback logic.
- Keep parsing logic in one place; do not duplicate `ParseResistValue` loops elsewhere.

Implementation note:

- This file is the cleanest place to centralize equipment-derived resistance queries so `BattleUnit` and `BattleSystem` do not each reconstruct equipment parsing.

#### Assets/Scripts/EquipmentItem.cs

Tasks:

- No structural rewrite needed.
- Confirm `GetElementResist()` and `GetStatusResist()` remain the primitive parsers used by aggregation helpers.

#### Assets/Scripts/BattleSystem.cs

Tasks:

- Extend `HitEventData` with optional fields for combat result explanation, for example:
	- `string element`
	- `int elementResistancePercent`
	- `bool hitWeakness`
	- `bool hitResistance`
	- `bool hitImmunity`
- Populate those fields when raising hit events.
- Add debug logs at hit resolution so QA can confirm that resist math is behaving correctly.

Implementation note:

- Keep battle flow authority in `BattleSystem`, but avoid duplicating damage math there if `BattleUnit` can return a richer damage result object.

#### Assets/Scripts/BattleChainUI.cs

Tasks:

- Optionally extend popup or chain label messaging so hits can display `Weak`, `Resist`, or `Immune`.
- If that is too much UI work for Phase 1, defer visuals and log via debug only.

#### Assets/Scripts/UI/BattleRewardDebugOverlay.cs

Tasks:

- No direct reward logic change needed.
- Consider whether a separate battle debug overlay is more appropriate for hit-resolution debugging than overloading reward UI.

#### Assets/Scripts/GachaItem.cs

Tasks:

- Review whether player units should carry innate element resistance fields in the future.
- If not needed immediately, leave unchanged for Phase 1.

#### Assets/Scripts/GameUnitDatabase.cs

Tasks:

- If innate element resistance is added later to player unit JSON, map it here.
- Not required for the first slice if equipment-only resistance is acceptable.

#### Assets/Scripts/EnemyUnitDatabase.cs

Tasks:

- Check whether enemy JSON already exposes enough data for elemental identity.
- If not, add a future-ready mapping field for element resist arrays in a later subpass.

#### Assets/documentation/battle_system_dev.md

Tasks:

- Update after code lands to reflect the real damage order and resist logic.

### Acceptance criteria

- Element-tagged skills meaningfully deal more or less damage based on target resist state.
- Equipment-based element resist changes battle outcomes.
- Physical and magical resistance stats are applied to incoming damage.
- QA can see resist or weakness outcomes through logs or popup text.

---

## Phase 2 Roadmap: Status Resistance and Fracture Rules

### Scope

- Status resistance checks.
- Strongest-wins modifier resolution.
- Original `Fracture` model for offensive stat breaks.

### Files to modify

#### Assets/Scripts/BattleUnit.cs

Tasks:

- Rewrite `TryInflictStatus(string statusId)` so it can fail due to resistance instead of only duplicate/precondition failure.
- Add helper methods:
	- `GetStatusResistancePercent(string statusId)`
	- `GetStrongestPositiveAttackModifier()`
	- `GetStrongestNegativeAttackModifier()`
	- same pattern for DEF and SPR.
- Replace additive iteration in:
	- `GetModifiedAttack()`
	- `GetModifiedDefense()`
	- `GetModifiedSpirit()`
	so the model becomes:
	- strongest positive source,
	- strongest negative source,
	- net result = base × (1 + positive + negative).
- Preserve existing persistent duration and tick behavior.

Implementation note:

- This is the main file for buff/debuff correctness. Do not spread overwrite rules across multiple callers.

#### Assets/Scripts/StatusEffectData.cs

Tasks:

- Add optional metadata fields for cleaner classification, for example:
	- `string modifierFamily` or `string ruleGroup`
	- `bool isFracture`
	- `bool isWard`
- Keep them optional so legacy statuses continue to work.

#### Assets/Scripts/StatusEffectDatabase.cs

Tasks:

- No major logic change expected beyond loading any new DTO fields.

#### Assets/Scripts/BattleSystem.cs

Tasks:

- Update status application call sites so failed infliction due to resistance can be surfaced.
- Add a combat event or debug string for `Resisted` outcomes.
- Update weapon status proc application to use the new resistance-aware infliction path.

#### Assets/Scripts/EquipmentInventory.cs

Tasks:

- Finalize `GetStatusResistanceForUnit()` and reuse it from `BattleUnit.TryInflictStatus()`.

#### Assets/StreamingAssets/status_effects_game.json

Tasks:

- Add or normalize the statuses that will act as the first `Fracture` set.
- Keep IDs and naming original.
- Decide whether existing `break` status becomes a generalized fracture effect or a specific legacy status.

#### Assets/Scripts/BattleUnitCardUI.cs

Tasks:

- Update hover tooltip or badge naming if new fracture or ward statuses are added.
- Ensure abbreviations remain readable if more statuses are introduced.

#### Assets/documentation/battle_system_player.md

Tasks:

- Update player-facing explanation of buffs, debuffs, resistance, and fracture behavior after the implementation stabilizes.

### Acceptance criteria

- Gear with status resistance reduces or fully blocks infliction.
- Same-stat buffs and debuffs no longer stack infinitely.
- Fracture states are visible and behave predictably.
- Weapon status procs respect target resistance.

---

## Phase 3 Roadmap: Cadence Skills and Offensive Identity

### Scope

- Consecutive skill ramping.
- First authored offensive system that rewards repeated sequencing.

### Files to modify

#### Assets/Scripts/UnitSkillData.cs

Tasks:

- Add optional Cadence fields, for example:
	- `bool usesCadence`
	- `int cadenceMaxStacks`
	- `float cadencePerStackBonus`
	- `string cadenceGroup`
- Keep defaults neutral so existing skills are unaffected.

#### Assets/Scripts/SkillDatabase.cs

Tasks:

- Map any new cadence fields from ability JSON.

#### Assets/Scripts/MagicSkillDatabase.cs

Tasks:

- Decide whether magic can use Cadence in v1.
- If yes, map the same fields here.
- If no, leave out deliberately and document the limitation.

#### Assets/Scripts/BattleUnit.cs

Tasks:

- Add small runtime state for cadence tracking per unit.
- Suggested helpers:
	- `GetCadenceStacks(string cadenceGroup)`
	- `RegisterCadenceUse(UnitSkillData skill)`
	- `ResetCadence()` or `BreakCadence()`.
- Apply cadence bonus during damage calculation only when the skill opts in.

Implementation note:

- Store cadence on the runtime battle unit, not the global inventory unit, unless persistence across battles is explicitly desired.

#### Assets/Scripts/BattleSystem.cs

Tasks:

- On successful skill use, register cadence progression.
- Reset cadence when intended, such as:
	- defending,
	- using a different cadence group,
	- KO.
- Add battle logs for cadence stack changes.

#### Assets/StreamingAssets/skills_ability_game.json

Tasks:

- Add the first small batch of cadence-authored skills.
- Keep names original and tied to current roster tone.
- Start with 2 to 4 units only.

#### Assets/Scripts/BattleUnitCardUI.cs

Tasks:

- Optional: show current cadence stack on the acting unit card.
- If not in v1 UI, use debug logs first.

### Acceptance criteria

- Repeating a cadence-enabled skill increases its effect as authored.
- Breaking cadence resets the bonus predictably.
- Only opted-in skills participate.

---

## Phase 4 Roadmap: Oath Weave Prototype

### Scope

- First implementation of Getcha's signature sequencing mechanic.

### Files to create or modify

#### Assets/Scripts/UnitSkillData.cs

Tasks:

- Add a new authored field such as `oathSigil`.
- Recommended first enum/string values:
	- `Assault`
	- `Arcane`
	- `Guard`
	- `Aid`
	- `Ruin`

#### Assets/Scripts/BattleSystem.cs

Tasks:

- Track the executed action sigil sequence for the current player phase.
- Detect recipe matches when actions are executed, not merely queued.
- Apply one-shot same-turn bonuses to the next relevant action.
- Reset weave state at the end of the player phase or enemy phase.
- Add event hooks such as:
	- `OnOathWeaveFormed`
	- `OnOathWeaveConsumed`

Implementation note:

- Use executed actions rather than queued actions so the mechanic stays grounded in actual play and cannot be gamed by cancelled queues.

#### Assets/Scripts/BattleCardsUIController.cs

Tasks:

- Surface queued sigils lightly if desired.
- Not required for v1, but helpful once recipes expand.

#### Assets/Scripts/BattleUnitCardUI.cs

Tasks:

- Optional small sigil badge per selected action.

#### Assets/Scripts/BattleChainUI.cs or new Assets/Scripts/BattleWeaveUI.cs

Tasks:

- Prefer a separate `BattleWeaveUI.cs` if messaging starts to exceed chain UI scope.
- Show messages like `Weave Formed: Ember Oath` or whatever original recipe names are chosen.

#### Assets/StreamingAssets/skills_ability_game.json

Tasks:

- Assign oath sigils to the first wave of skills.

#### New data file option: Assets/StreamingAssets/oath_weaves.json

Tasks:

- Author recipe definitions in data if you want future expansion without code edits.
- Suggested schema:
	- recipe id
	- sigil sequence
	- effect type
	- effect magnitude
	- consume behavior

### Acceptance criteria

- At least 5 recipes work in battle.
- The player can understand when a weave forms.
- The system changes same-turn decision-making in a way chaining alone does not.

---

## Recommended Coding Order

This is the recommended implementation order for actual engineering work.

### Sprint A

1. Add equipment aggregation helpers in `EquipmentInventory.cs`.
2. Add element resistance and mitigation helpers in `BattleUnit.cs`.
3. Thread resist result data through `BattleSystem.cs` hit events.
4. Validate in battle with debug logs before touching UI.

### Sprint B

1. Rewrite status resistance checks in `BattleUnit.TryInflictStatus()`.
2. Replace additive stat modifier stacking with strongest-wins logic.
3. Normalize `status_effects_game.json` for fracture-ready statuses.
4. Add player-facing tooltip updates once values are stable.

### Sprint C

1. Extend skill DTOs and loaders for Cadence.
2. Add cadence runtime state to `BattleUnit`.
3. Register cadence progression in `BattleSystem`.
4. Author 2 to 4 test skills and validate battle flow.

### Sprint D

1. Add Oath Sigil data to skills.
2. Create recipe detection in `BattleSystem`.
3. Add a lightweight weave UI banner.
4. Author a small test batch of recipes and tune them.

---

## Testing Checklist By Phase

### Phase 1 tests

- A fire skill hits a neutral, weak, resistant, and immune target correctly.
- Physical resist and magical resist change incoming damage in the expected direction.
- Element gear changes observed damage.

### Phase 2 tests

- A unit with 100% status ward ignores the status.
- Positive and negative modifiers net cleanly instead of stacking wildly.
- Fracture duration and cleanse behavior are correct.

### Phase 3 tests

- Cadence stacks only for opted-in skills.
- Guarding or switching groups breaks cadence.
- KO resets cadence.

### Phase 4 tests

- Weave recipes trigger from executed actions only.
- Weave bonus applies to the correct downstream action.
- End-of-phase reset prevents stale recipes from carrying over incorrectly.

---

## Recommended First Deliverable

If only one implementation slice is started next, it should be:

1. `EquipmentInventory.cs` aggregation helpers.
2. `BattleUnit.cs` element and resistance-aware damage resolution.
3. `BattleSystem.cs` hit-result diagnostics.

That slice has the best ratio of gameplay value to engineering risk and unlocks several later mechanics cleanly.

---

## Recommended Naming Direction

To stay inspired by genre leaders without sounding derivative, prefer original terms:

- `Element Chain` instead of elemental chain.
- `Synergy Bonus` instead of family bonus.
- `Fracture` for offensive stat breaks.
- `Ward` for status or element protection.
- `Challenge` for forced targeting.
- `Intercept` for cover.
- `Cadence` for consecutive-ramping skills.
- `Oath Weave` for the signature sequencing system.

---

## Recommendation

The highest-value path is not to chase every FFBE mechanic. The right move is to adopt only the systems that reinforce Getcha's existing strengths:

- readable tactical sequencing,
- authored chain play,
- status-driven battlefield control,
- and party planning before and during a mission.

The recommended next package is:

1. Element resistance and weakness.
2. Status resistance.
3. Physical and magical mitigation.
4. Fracture and buff overwrite rules.
5. Cadence skills.
6. Oath Weave as the long-term signature mechanic.

That keeps the design original, achievable, and meaningfully deeper than the current state.