# Oathsworn: Compact Wiki Draft

Last updated: April 8, 2026

## Overview

Oathsworn: Compact is a story-driven tactical RPG built around party combat, gacha-driven roster growth, and lore-native summoning through Prisms and Oath-Weavers. The current project snapshot supports a playable battle loop, JSON-driven units and enemies, banner configuration, item/material data, and an active Sigil Resonance and Oath-Weaving layer.

This draft is written as a player-facing wiki page, but it also notes where the current repository distinguishes between fully implemented systems and systems that are currently documented in design/player guides.

## Core Pillars

- Tactical turn-based battles with manual queue and execute phases
- Gacha-driven roster acquisition with pity and spark systems
- Unit building through rarity tiers, equipment, runes, and skills
- Sigil-based team building, passive resonance, and active Oath-Weaving
- Story chapters centered on Aric, Lyria, the Compact, and the Abyss Tyrant

## Summon Terminology

- **Prism**: Canon world term for infrastructure, vaults, nodes, and attunement architecture
- **Oath-Prism**: Standard player-facing summon vessel term
- **Covenant Prism**: Premium/event summon presentation term

## Combat Overview

### Battle Flow

Combat is built around a two-step player phase followed by an enemy phase.

1. Queue actions for each available unit.
2. Execute those actions in the order you choose.
3. Enemies act after your player phase ends.

Available player actions currently include:

- Attack
- Skill
- Magic
- Weave
- Defend
- Item use

### Party Size

- The battle system guide describes up to 6 active battle units.
- The combat loop is designed around sequencing, chain timing, support timing, and targeted burst windows.

## Chaining

The game uses a frame-based chain system rather than a broad action-window combo rule.

### Chain Timing Model

The current implementation emphasizes hit timing and frame cadence. Chain family acts primarily as a compatibility identity for authored hit profiles, not as a separate layered bonus system.

### How Chaining Works

- Hits that land close together extend a chain.
- Multi-hit units can rapidly build chain count.
- Same-family hit timing improves carry consistency.
- Higher chains increase damage through a chain multiplier.

### Why Chaining Matters

- Fast attackers can build chain momentum.
- Heavy finishers benefit from entering after the chain is already established.
- Manual execution order lets the player optimize timing instead of relying on auto-resolve.

## Oath-Weaving and Sigil Resonance

Sigil systems are one of the game's signature combat layers.

### Passive Resonance

When compatible sigils are present in the same party, units gain automatic flat stat bonuses at mission start.

Current passive bonuses affect:

- ATK
- DEF
- MAG
- SPR

### Active Weaves

Oath-Weavers can perform Weave actions in battle.

- Weaves combine two sigils into a named effect
- Effects can target a single enemy, all enemies, a single ally, or all allies
- Weaves can deal damage, heal, buff, debuff, or apply statuses
- Weaves consume the acting unit's action and draw from the mission's Oath/Weave resource model

### Current Oath-Weaver Status

The current sigil resolver explicitly marks these units as Oath-Weavers:

- Aric the Bold
- Lyria

### Example Weaves

Current weave data includes named combinations such as:

- Starfall
- Eclipse Strike
- Moonwater Veil
- Dawnweave
- Flare Oath
- Oath Flame
- Hollow Mark
- Bastion Prayer
- Precision Fracture

## Sigils

The project currently defines 10 sigil types.

| Sigil | Theme |
|---|---|
| Celestial | Starlight, compatibility, bridging |
| Void | Curse-resonance, compact alignment |
| Light | Faith, holy power, clarity |
| Tide | Water, restoration, grief, endurance |
| Shadow | Deception, darkness, precision |
| Storm | Lightning, motion, momentum |
| Ember | Flame, iron, endurance, oath-bound force |
| Rift | Spatial disruption, flanking, displacement |
| Earth | Weight, defense, bastion strength |
| Volt | Controlled energy, analysis, precision |

### Current Explicit Unit-to-Sigil Mapping

The current code explicitly maps these units to sigils:

| Unit | Sigil |
|---|---|
| Aric the Bold | Ember |
| Lyria | Celestial |
| Kain | Shadow |
| Sera | Light |
| Zephyr | Storm |
| Draven | Ember |
| Mirel | Tide |
| Vex | Void |
| Cassian | Ember |
| Selene | Light |
| Kael | Rift |
| Nadia | Tide |
| Rook | Earth |
| Iris | Volt |

Note: the Chapter 3 units exist in `units_game.json`, but their sigils are not yet explicitly mapped in the current `SigilProfileResolver` snapshot.

## Unit Stats

Units currently use these core battle stats:

| Stat | Purpose |
|---|---|
| HP | Health pool |
| MP | Resource for magic and some special actions |
| ATK | Physical offense |
| DEF | Physical defense |
| MAG | Magical offense |
| SPR | Magical defense and healing-related sustain |
| SPD | Speed and timing-oriented identity |
| CRIT | Critical chance |

### Other Unit Combat Fields

Units also define combat and progression properties such as:

- Normal attack hit count
- Per-hit damage tables
- Attack frame timing
- Innate element
- Element resistance
- Status resistance
- Physical resistance
- Magical resistance
- Rune slots
- Equipped skills
- Equipment loadout
- Level and EXP

### Rarity Structure

Units can carry rarity-specific entries in data, with separate stat lines and skill loadouts per tier.

The current rarity ladder used by banners is:

| Tier | Label |
|---|---|
| 1 | Oath-Called |
| 2 | Oath-Sworn |
| 3 | Oath-Tempered |
| 4 | Oath-Forged |
| 5 | Oath-Eternal |

## Playable Unit Roster

The current unit database contains 22 total unit records, with 21 marked summonable in data.

| Unit | Title | Job |
|---|---|---|
| Aric the Bold | Wandering Knight | Knight |
| Lyria | Starlight Mage | Mage |
| Kain | Shadow Assassin | Rogue |
| Sera | Holy Sentinel | Paladin |
| Zephyr | Storm Caller | Summoner |
| Draven | Iron Warlord | Warrior |
| Mirel | Tide Priestess | Cleric |
| Vex | Cursed Blade | Dark Knight |
| Rainier | Crimson Knight | Knight |
| Wassal | Azure Blade | Samurai |
| Cassian | Ember Warden | Guardian |
| Selene | Vale Seer | Oracle |
| Kael | Rift Strider | Rogue |
| Nadia | Abyss Chantress | Cleric |
| Rook | Bastion Reaver | Warrior |
| Iris | Volt Arbalist | Ranger |
| Aurel Voss | Sunforge Oath-Knight | Guardian |
| Rhea Flint | Runebreaker Sapper | Sapper |
| Talis Quen | Prism Wayfinder | Tactician |
| Nyra Halvek | Oath-Black Rival | Warden |
| Caelia Vorn | Frontline Celestial Weaver | Weaver |
| Kain Vale | Severance Rejoined | Vanguard |

### Current Banner Pool Notes

- The full unit database includes 22 units (`unit_001` through `unit_022`).
- Current banner JSON includes 11 enabled banners, including Chapter 4 featured rotations.
- Banner definitions reference IDs through `unit_022` in some pools.
- `unit_022` is currently marked non-summonable in unit data, so practical summon pools are effectively centered on summonable entries (notably through `unit_021`).

## Enemy Roster

The current enemy database contains the following enemies and bosses.

### Early and Core Enemies

- Goblin Scout
- Forest Wolf
- Bandit Raider
- Flame Imp
- Stone Golem
- Storm Hawk
- Swamp Shaman
- Mirror Wisp
- Dormant Idol
- Twinfang Chief
- Rune Sentry
- Cave Basilisk
- Night Stalker
- Priest of Mire
- Medic Drone
- Warlock Duo
- Rage Cyclops
- Blood Moon Avatar
- Hex Engineer
- Abyss Tyrant

### Chapter 3 and Later Enemy Additions

- Ember Gnasher
- Pylon Screamer
- Chain-Maw Brute
- Mirror Warden
- Null Scribe
- Astral Custodian
- Frostbound Oath-Wraith
- Rift Talon
- Whiteglass Revenant
- Compact Hunter
- Echo Leech
- Stairwell Warden Karkos
- Mire Stalker
- Lantern Leech
- Bog Tyrant Neth
- Tideglass Warden
- Undertow Siren
- Leviathan Fragment N-3
- Oath Revenant
- Crown Shade
- The Apocryphal Champion
- Loyalist Husk
- Throne Engine Sentinel
- Marshal-Engine Helion
- Crownbound Arbiter
- Abyss Lector
- Original Abyss Tyrant

### Enemy Behavior Notes

The enemy system now references behavior profiles instead of relying only on traditional enemy ability slot patterns.

Current enemy data supports:

- Enemy behavior profiles
- Forced HP-lock skill triggers
- Boss threshold mechanics
- Enemy-specific skill execution during battle

## Gacha Mechanics

### Banner Types

The project currently defines:

- Standard banners
- Featured banners
- Chapter launch banners
- Special all-units test/open banners

### Current Banner Rules

From the active banner JSON and player guide:

- Standard single pull cost: 100
- Featured single pull cost: 200
- Hard pity: 80
- Soft pity starts: 61
- Soft pity step: +0.5% Tier 5 rate per pull after soft pity begins
- 10-pull guarantee: at least Tier 4 or higher
- Featured spark threshold: 160

### Tier 5 Rate Structure

- Standard Tier 5 rate: 3%
- Featured banner Tier 5 featured rate: 1.5%
- Remaining Tier 5 pool rate: 1.5%

### Duplicate Conversion

Duplicate rules in banner data currently award shard currency by rarity:

| Rarity | Base Shards | Featured Bonus |
|---|---|---|
| 1 | 1 | 0 |
| 2 | 3 | 0 |
| 3 | 10 | 0 |
| 4 | 25 | 0 |
| 5 | 80 | 40 |

## Currencies and Economy

Because the project currently stores economy terms in multiple places, it is useful to separate fully exposed currency from documented progression currencies.

### API-Backed Currency

The inventory API currently exposes:

- `gachaTokens`

### Documented Player-Facing Gacha Currencies

The existing player guide and summary docs refer to:

- Compact Seals: summon currency spent on standard and featured banners (internal field: `gachaTokens`)
- Spark Points: banner-specific pity accumulation for featured claims
- High Compact Seals: permanent archive/shop currency created from leftover spark when banners rotate (internal field: `archiveTokens`)
- Prism Shards: duplicate compensation currency used for premium exchanges

### Recommended Wiki Framing

For now, the wiki should present this as:

- `gachaTokens` is the currently visible backend/API currency field
- Compact Seals, Spark Points, High Compact Seals, and Prism Shards are the intended player-facing gacha economy terms described by the game guides

## Items and Materials

### Combat Consumables

The item database currently includes combat-ready recovery and utility items such as:

- Potion
- Hi-Potion
- X-Potion
- Full Cure
- Ether
- Hi-Ether
- Turbo Ether
- Phoenix Down
- Mega Phoenix
- Elixir
- Megaelixir
- Antidote
- Gold Needle
- Echo Herbs
- Eye Drops
- Remedy

### Material Items

The current item database includes these material items:

| Material | Current Description |
|---|---|
| Iron Ore | Equipment crafting material |
| Silver Ore | Refined ore for higher-end equipment crafting |
| Shadow Crystal | Advanced crafting material tied to shadow energy |
| Life Crystal | Restorative crafting material |
| Burst Shard | Material used to power Limit Bursts |

### Documented Upgrade and Progression Materials

Other upgrade materials appear in guides and design docs, even if they are not all yet present in `items_game.json`.

- Spark
- Archive Tokens
- Prism Shards
- Universal Ascension Core
- Burst Shard
- Awakening materials
- Sigil-related progression concepts
- Prism-related progression concepts

## Equipment, Runes, and Build Systems

Unit records currently support:

- Equipment slot layouts
- Equipped equipment IDs
- Rune slot counts
- Equipped rune IDs
- Skill loadouts by rarity tier

This means unit building is not just about pulling a character. It also includes:

- Equipping the right weapon and armor profile
- Filling rune slots for specialization
- Matching skills to a role or boss fight
- Pairing units for Sigil Resonance bonuses

## Lore Framing

### Prisms

Prisms are not just a summoning animation device. In the setting, they are crystallized memory-lattices that preserve oath-patterns of defenders so they can answer the call again when the realm faces collapse.

### Oath-Weavers

Oath-Weavers are rare people whose sigil-bond is written into them rather than cast through ordinary technique. They are required to safely bridge prism summons and to perform higher-level weave effects in combat.

### The Compact

The story across Chapters 1 through 3 frames the Compact as an old, restrictive, dangerous system tied to the Binding, the Knight-Without-Title designation, Vex's curse-alignment, and the long rebirth cycle of the Abyss Tyrant.

## Current Story Scope

The repository currently includes story data and storyboard material covering:

- Chapter 1: Ashes of Oath
- Chapter 2: The Shattered March
- Chapter 3: The Compact Unbound

The playable roster and enemy set already reflect major story additions from Chapters 2 and 3, including the Compact characters and late-stage boss roster.

## Suggested Future Wiki Sections

This draft covers the essentials, but the full wiki can later be split into dedicated pages:

- Units
- Enemies and Bosses
- Gacha and Banner Rules
- Sigils and Weaves
- Status Effects
- Equipment and Runes
- Items and Materials
- Story Chapters
- Glossary of lore terms

## Quick Reference

### Current Counts

- Total unit records in unit database: 22
- Summonable unit records in unit database: 21
- Unique unit IDs referenced by banner pools: 22
- Sigil types defined: 10
- Units with explicit sigil mapping in resolver: 14
- Oath-Weavers explicitly enabled in code: 2
- Enemy records in enemy database: 62

### Most Important Systems for New Players

- Manual queue-and-execute combat
- Frame-based chaining
- Banner pity and spark
- Unit rarity progression
- Sigil Resonance team bonuses
- Oath-Weaving as a tactical power spike
