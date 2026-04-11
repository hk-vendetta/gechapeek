# Battle System — Developer Guide

## Architecture Snapshot

The battle stack is turn-based and event-driven. `BattleSystem` remains the source of truth for combat state. UI classes subscribe to events and issue requests, but do not author game state directly.

Current behavior is built around **queue first, execute second** player turns, with enemy phase deferred until player phase completion. The stack now includes mission-scoped **Sigil Resonance** and **Oath-Weaver Weave actions**.

---

## High-Level Runtime Flow

```text
Mission launch context selected
    -> BattleSystem.SetupBattle()
    -> Build playerUnits from active PartySystem party (max 6)
    -> Build enemyUnits for current wave
    -> SigilResonanceManager.BeginMission(playerUnits)
    -> Apply passive flat resonance bonuses to compatible sigil pairs
    -> UI binds cards + battlefield slots

Player phase
    -> Queue actions per unit (attack/skill/magic/weave/defend/item paths)
    -> Execute queued actions manually in chosen order
    -> Per-hit chain registration and damage/heal/status resolution
    -> Units blocked by action-preventing statuses do not auto-consume turn

Enemy phase
    -> Starts after eligible player actions are complete
    -> Enemy actions resolve
    -> Turn cleanup (defend clear, chain reset where configured, status timing)
    -> Outcome check
```

---

## Core Files

| File | Role |
|------|------|
| [Assets/Scripts/BattleSystem.cs](Assets/Scripts/BattleSystem.cs) | Combat authority: turn sequencing, queue execution, chain, hit resolution, status/item integration, target selection events. |
| [Assets/Scripts/BattleUnit.cs](Assets/Scripts/BattleUnit.cs) | Runtime unit state: effective stats, HP/MP, status application/tick/removal, action gating, receive damage/heal. |
| [Assets/Scripts/BattleCardsUIController.cs](Assets/Scripts/BattleCardsUIController.cs) | Player-facing queue/execute controller, input lock logic, action dispatch, selected-target capture. |
| [Assets/Scripts/BattleUnitCardUI.cs](Assets/Scripts/BattleUnitCardUI.cs) | Unit card visuals: HP/MP, acted/locked/queued states, status badges, slot labels, skill menus, dedicated Weave panel/preview flow. |
| [Assets/Scripts/BattlefieldSlotVisualController.cs](Assets/Scripts/BattlefieldSlotVisualController.cs) | Battlefield slot placement, target indicator visuals, player move-to-target presentation. |
| [Assets/Scripts/BattlefieldTargetTapHandler.cs](Assets/Scripts/BattlefieldTargetTapHandler.cs) | Tap relay from battlefield sprites into battle target selection. |
| [Assets/Scripts/SigilResonanceManager.cs](Assets/Scripts/SigilResonanceManager.cs) | Sigil compatibility rules, mission weave budget, default weave catalog, codex progression persistence hooks. |
| [Assets/Scripts/SigilProfileResolver.cs](Assets/Scripts/SigilProfileResolver.cs) | Unit-to-sigil mapping and Oath-Weaver eligibility (including Aric latent path). |
| [Assets/Scripts/SigilType.cs](Assets/Scripts/SigilType.cs) | Shared sigil enum used by battle and resonance systems. |

### Data and Lookup

| File | Role |
|------|------|
| [Assets/Scripts/StatusEffectData.cs](Assets/Scripts/StatusEffectData.cs) | Status definition model. |
| [Assets/Scripts/ActiveStatusEffect.cs](Assets/Scripts/ActiveStatusEffect.cs) | Runtime active status payload per unit. |
| [Assets/Scripts/StatusEffectDatabase.cs](Assets/Scripts/StatusEffectDatabase.cs) | Loads status definitions and provides id lookup. |
| [Assets/StreamingAssets/status_effects_game.json](Assets/StreamingAssets/status_effects_game.json) | Status effect source data. |
| [Assets/Scripts/UnitSkillData.cs](Assets/Scripts/UnitSkillData.cs) | Skill DTO, including hit timing and movement flag. |
| [Assets/Scripts/SkillDatabase.cs](Assets/Scripts/SkillDatabase.cs) | Ability JSON loader and lookup. |
| [Assets/Scripts/MagicSkillDatabase.cs](Assets/Scripts/MagicSkillDatabase.cs) | Magic JSON loader and lookup. |
| [Assets/StreamingAssets/skills_ability_game.json](Assets/StreamingAssets/skills_ability_game.json) | Physical/ability skill source data. |
| [Assets/StreamingAssets/skills_magic_game.json](Assets/StreamingAssets/skills_magic_game.json) | Magic skill source data. |
| [Assets/Scripts/ItemRecord.cs](Assets/Scripts/ItemRecord.cs) | Combat item metadata including cure/revive/heal settings. |
| [Assets/StreamingAssets/items_game.json](Assets/StreamingAssets/items_game.json) | Item combat behavior definitions. |
| [Assets/Scripts/EquipmentInventory.cs](Assets/Scripts/EquipmentInventory.cs) | Equipment stat bonuses and weapon status proc metadata. |

---

## Turn Model (Queue Then Execute)

### Queue Stage

- Player selects actions for units without immediately ending player phase.
- Cards track queued/acted/locked states.
- Input lock prevents illegal re-entry while preserving deliberate execution control.

### Execute Stage

- Queued actions execute only on explicit second tap.
- Execution order is player-chosen, enabling chain routing and tactical cleanse-first sequencing.
- Weaves are queued/executed through the same controller path as other actions.

### Weave Stage (Integrated Action Type)

- Oath-Weavers expose a dedicated Weave panel from the skill menu.
- Panel renders runtime weave options plus preview metadata (targeting/type/power/status).
- Single-target weave UX:
    - Panel sets enemy-only selection mode.
    - Enemy tap queues the weave and closes the panel immediately.
- AoE/all-target weaves queue immediately from panel selection.
- Mission weave budget is enforced in `SigilResonanceManager`.

### Consumption Rules

- Turn consumption is tied to successful action execution paths.
- Units with action-blocking statuses are not force-consumed.
- If cleansed before player phase ends, those units can still act.

---

## Chain Model (Hit/Frame Driven)

Chain progression is based on landed hits and their timing cadence, not broad per-action windows.

- Each hit can register/update chain state.
- Multi-hit attacks contribute multiple chain steps.
- Attack frames and inter-hit timing drive chain reliability.
- Chain family acts as compatibility identity for timing sets; there is no extra family bonus layer.

Developer impact: when authoring skills, `attack_frames` and hit distribution are now first-order balance inputs.

---

## Status Effect System

Status support is now production-integrated:

- Data-driven definitions in status JSON.
- Runtime active status list per `BattleUnit`.
- Apply/remove/tick lifecycle hooks in battle flow.
- Action-gating statuses (for example sleep/stone style behavior) enforce unit restrictions.
- Wake/clear behavior is handled for supported conditions.

Infliction sources:

- Skill and magic effects.
- Weapon proc chance from equipped weapon metadata.

Cure sources:

- Skills/magic with cleanse metadata.
- Combat items with cure effect metadata.

UI:

- Card-level status badges.
- Hover-triggered tooltip for full status names via [Assets/Scripts/HoverTooltipTrigger.cs](Assets/Scripts/HoverTooltipTrigger.cs).

---

## Targeting and Battlefield Visual Layer

- Party and battlefield are aligned by explicit slot mapping.
- Party size increased to 6 and reflected in card slot labels and battlefield placement.
- Targeting is direct: tap sprite to select target.
- Selected target is shown with above-head indicator visuals.
- Selection state is cleared and rebuilt at relevant action boundaries to prevent stale target UX.
- `move_to_target_before_attack` is now per-skill data and drives player movement presentation.
- Enemies remain stationary in current visual rules.
- Single-target weave targeting uses the same battlefield tap path and target indicator system.

---

## Damage and Healing Notes

- Physical damage path: attacker ATK-driven versus target DEF.
- Magical damage path: attacker MAG-driven versus target SPR.
- Healing paths now include SPR scaling behavior.
- Defend mitigation remains part of receive-damage resolution.

---

## Data Authoring Notes

### Sigils and Weaves (Current)
### Sigils and Weaves (Current)

Weave catalog is **JSON-driven** via `Assets/StreamingAssets/sigil_weaves.json`.

- Sigil identity is resolved via `SigilProfileResolver`.
- Weave definitions are loaded in `SigilResonanceManager.TryLoadWeaveCatalogFromJson()`.
- Secret weaves are gated behind `SigilResonanceManager.unlockedSecretWeaves` (a `HashSet<string>`).

**Progression persistence:**
- `SaveGameData.discoveredWeaves` — standard codex discovery (used at least once)
- `SaveGameData.unlockedSecretWeaves` — explicitly granted secret weave IDs

**Story gate:**
- Codex unlock is tied to `mission_006` completion.

**Unlock sources for secret weaves:**
- `Mission` — `MissionClearRewardRecord.first_clear_weave_unlocks` → `BattleRewardSystem` on first clear
- `Event`, `Gift`, `Exploration`, `Shop` — `WeaveUnlockManager` (see `Assets/Scripts/WeaveUnlockManager.cs`)

### Add or Modify a Skill
### Add or Modify a Skill

1. Edit [Assets/StreamingAssets/skills_ability_game.json](Assets/StreamingAssets/skills_ability_game.json) or [Assets/StreamingAssets/skills_magic_game.json](Assets/StreamingAssets/skills_magic_game.json).
2. Define timing, chain, and movement fields where needed:
    - `attack_count` — number of hits (must equal array lengths)
    - `attack_frames` — integer array, frame timing per hit
    - `attack_damage` — integer array, damage weight per hit (first hit is base 100)
    - `chainFamily` — chain compatibility group (`OathBlade`, `CovenantWave`, `SigilBurst`, `EternalStrike`)
    - `move_to_target_before_attack` — bool
3. Ensure skill id is reachable from unit native skill ids or materia-granted ids.

**Chain family standards (shared with sigil_weaves.json):**

| Family | Hits | Frame signature |
|--------|------|-----------------|
| OathBlade | 9 | `[42, 6×8]` |
| CovenantWave | 8 | `[42, 20×7]` |
| SigilBurst | 30 | `[42, 4×29]` |
| EternalStrike | 10 | `[110, 10×9]` |

### Add or Modify a Weave

1. Edit [Assets/StreamingAssets/sigil_weaves.json](Assets/StreamingAssets/sigil_weaves.json).
2. Pair key `id` must be `"SigilA|SigilB"` with the **lower SigilType enum value first** (see `SigilType.cs`).
3. For enemy-targeted weaves, include attack frame fields matching the chain family standard above.
4. For secret weaves, set `is_secret: true` and provide `unlock_source`, `unlock_hint`, and `portrait_id`.
5. Non-secret weaves are available immediately once the catalog is loaded.

### Add or Modify Status Effects

1. Add status records to [Assets/StreamingAssets/status_effects_game.json](Assets/StreamingAssets/status_effects_game.json).
2. Reference those status ids from skill/magic/item/equipment effect metadata.
3. Validate application, cure, and duration behavior in battle scene.

### Add or Modify Battle Items

1. Update item records in [Assets/StreamingAssets/items_game.json](Assets/StreamingAssets/items_game.json).
2. Configure combat effect fields for heal/revive/cure.
3. Verify item route in battle action flow and UI state refresh.

---

## Validation Checklist

- [ ] Setup builds 6-slot player party correctly from active party data.
- [ ] Queue/execute flow requires explicit second tap for execution.
- [ ] Enemy phase waits until player phase completion.
- [ ] Action-blocked unit is not auto-consumed.
- [ ] Cleanse-then-act in same player phase succeeds.
- [ ] Hit/frame timing drives chain progression for normal and skill multi-hit paths.
- [ ] Weapon status proc chance applies on eligible hits.
- [ ] Skill/magic/item cleanse paths remove targeted statuses correctly.
- [ ] Status badges and hover tooltip reflect current active statuses.
- [ ] Battlefield tap targeting selects expected enemy and indicator updates correctly.
- [ ] Player skill movement toggles obey `move_to_target_before_attack`.
- [ ] Enemy visuals remain stationary during enemy action presentation.
- [ ] Compatible sigil pairs apply only small flat mission bonuses (ATK/DEF/MAG/SPR).
- [ ] Incompatible sigil pairs apply no passive bonus.
- [ ] Oath-Weaver units can open Weave panel; non-weavers cannot.
- [ ] Single-target weave requires enemy selection and auto-closes panel on enemy tap.
- [ ] AoE/all-target weave queues without requiring a target.
- [ ] Mission weave budget decrements on successful weave execution.
- [ ] Codex unlock and discovered weave persistence survive save/load cycles.

---

## Known Gaps / Next Iterations

| Area | Current | Candidate Next |
|------|---------|----------------|
| Target mode UX | Baseline direct target selection works | Enforce strict ally/enemy target mode prompts per action type. |
| Indicator visuals | Functional small markers | Add prefab-driven pulse/bob and visibility polish. |
| AOE targeting | Single-target flow emphasized | Expand explicit AOE target feedback and preview. |
| Crit/speed layers | Stats exist with partial use | Complete crit damage and initiative-driven turn mechanics. |
