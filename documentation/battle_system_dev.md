# Oathsworn: Compact - Battle Guide (Developer v2)

Last updated: 2026-04-08

## Core Runtime Files
- Assets/Scripts/BattleSystem.cs
- Assets/Scripts/BattleUnit.cs
- Assets/Scripts/BattleCardsUIController.cs
- Assets/Scripts/BattleUnitCardUI.cs
- Assets/Scripts/BattlefieldSlotVisualController.cs
- Assets/Scripts/BattlefieldTargetTapHandler.cs
- Assets/Scripts/SigilResonanceManager.cs
- Assets/Scripts/SigilProfileResolver.cs

## Runtime Model
- BattleSystem is authoritative for turn flow and resolution.
- UI queues/dispatches actions but does not own state truth.
- Player phase is queue-then-execute.
- Enemy phase starts after eligible player actions resolve.

## Key Systems
### Action Queue
- Cards enter queued state.
- Explicit second input executes selected queued action.
- Locked/acted states prevent illegal actions.

### Targeting
- Direct tap targeting from battlefield sprites.
- Indicator state is rebuilt across action boundaries.

### Chain
- Chain progression is hit/frame-driven.
- attack_frames and hit cadence are balancing-critical.

### Status
- Data-driven status definitions loaded from JSON.
- Runtime apply/remove/tick in BattleUnit and BattleSystem.
- Supported infliction from skills, magic, and weapon procs.

### Weaves
- Loaded from sigil weave data.
- Oath-Weaver-only active actions.
- Mission-limited charges with progression hooks.

## Data Sources
- Assets/StreamingAssets/skills_ability_game.json
- Assets/StreamingAssets/skills_magic_game.json
- Assets/StreamingAssets/status_effects_game.json
- Assets/StreamingAssets/items_game.json
- Assets/StreamingAssets/sigil_weaves.json

## Test Checklist
- 6-unit setup loads correctly.
- Queue and manual execute order works.
- Target selection maps to expected enemy indices.
- Action-locked unit can act if cleansed before phase end.
- Weave charge budget decrements correctly.
- Status badges and tooltip labels stay in sync.
