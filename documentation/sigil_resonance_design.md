# Oathsworn: Compact — Sigil Resonance System Design Plan

---

## Design Pillars

1. **Readable at a glance** — sigil types are color-coded and icon-distinct; a new player sees the system working before they fully understand it.
2. **Automatic on entry** — the lowest tier activates passively with no input. New players benefit immediately.
3. **Deepens with engagement** — players who study party composition unlock the system's full power over time.
4. **Every combination is unique** — no two sigil pairings produce the same effect. Discovering combinations is part of the reward loop.
5. **Lore-native** — the mechanic is the same Oath-Weave system the story describes. The UI language matches the world's language.

---

## The Ten Sigil Types

Each unit in the roster belongs to exactly one Sigil Type, determined by their oath-pattern and combat identity.

| Sigil | Color | Associated Units | Essence |
|-------|-------|-----------------|---------|
| **Celestial** | Silver-Gold | Lyria | Starlight, compatibility, bridging |
| **Void** | Deep Violet | Vex | Curse-resonance, compact-alignment |
| **Light** | White-Gold | Sera | Faith, holy, clarity |
| **Tide** | Aqua-Blue | Mirel, Nadia | Water, grief, restoration |
| **Shadow** | Dark Grey | Kain | Darkness, deception, precision |
| **Storm** | Electric Blue | Zephyr | Wild lightning, wind, momentum |
| **Ember** | Deep Orange | Draven, Cassian | Flame, oath-bound iron, endurance |
| **Rift** | Teal-Green | Kael | Spatial disruption, flanking |
| **Earth** | Brown-Gold | Rook | Bastion, immovability, weight |
| **Volt** | Bright Yellow | Iris | Controlled energy, analysis, precision |

> **Design note:** Storm (Zephyr) is wild and impulsive energy. Volt (Iris) is measured, directed energy. Same element, opposing natures — they produce a specific combination effect when paired rather than overlapping.

---

## The Three-Tier System

### Tier 1 — Passive Resonance *(Always On)*

**When:** Two or more units with compatible sigil types are present in the same party of 6.

**How it works:** No player input required. The resonance bonus is displayed in the Party Setup screen as a glowing connection line between compatible units. In battle, a small resonance badge appears on those unit cards.

**Purpose:** New players benefit from this without knowing it exists. Veterans build parties specifically around resonance chains.

**Rule:** Compatible sigils are adjacent or harmonically linked on the Resonance Web (see below). Not all pairs have Tier 1 compatibility — incompatible pairs only activate at Tier 2.

---

### Tier 2 — Weave Chains *(Active, Requires Oath-Weaver)*

**When:** An Oath-Weaver is in the party.

**How it works:** During the Queue Phase, the Oath-Weaver's action menu gains a **Weave** option alongside Attack / Skill / Magic / Defend. Selecting Weave shows available sigil pairs in the current party and the effect each combination produces. The player targets an enemy or ally (depending on the effect type), queues the Weave, then executes it during the Execute Phase.

Executing a Weave spends the Oath-Weaver's action for that turn and an MP cost determined by the combination tier.

**Compatibility rules:**
- **Lyria (Celestial)** can Weave any two sigils present in the party. She is the universal catalyst — this is her mechanical identity.
- **Other Oath-Weavers** (e.g. Aric in future implementation) can only Weave with their compatible set, based on their own sigil type.
- A party without an Oath-Weaver cannot Weave. Passive Resonance still works.

**Cooldown:** Weave Chains have a per-battle cooldown tracked per combination, not per Oath-Weaver. Once used, that pair cannot be woven again until 3 turns have passed.

---

### Tier 3 — Compact Surge *(Full Constellation, Once Per Battle)*

**When:** A specific predefined set of 3 or more sigil types are all present in the party at the same time.

**How it works:** A **Compact Surge** badge pulses on the Oath-Weaver's card when a full constellation is active. Activating it costs the Oath-Weaver's full turn plus a significant MP cost. The effect is powerful, unique to that constellation, and usable once per battle.

**Example constellations:**
- **The Oath Triangle:** Celestial + Light + Ember → *"First Compact"* — full party ATK/MAG buff for 3 turns + cleanse all debuffs
- **The Void Web:** Celestial + Void + Shadow → *"Eclipse Compact"* — AoE stun on all enemies for 1 turn
- **The Tidecaller:** Tide + Light + Celestial → *"Moonwater Covenant"* — revive all KO'd units at 50% HP
- **The Storm Front:** Storm + Volt + Rift → *"Fractured Sky"* — AoE damage that ignores DEF, applied to all enemies

Constellations are discovered in play and logged in the **Resonance Codex** (see Progression section).

---

## The Full Weave Combination Table

## The Full Weave Combination Table

26 unique sigil pairs are currently implemented in `sigil_weaves.json`. Additional pairs from the original 45-pair design vision are planned for future content updates.

All pairs use pair key ordering determined by SigilType enum value (lower value first). Keys are resolved identically regardless of input order.

**Chain Family Standards** (shared with `skills_ability_game.json`):

| Family | Hits | Frame Pattern | Damage Pattern |
|--------|------|---------------|----------------|
| **OathBlade** | 9 | `[42, 6×8]` | `[100, 9×8]` |
| **CovenantWave** | 8 | `[42, 20×7]` | `[100, 10×7]` |
| **SigilBurst** | 30 | `[42, 4×29]` | `[100, 6×29]` |
| **EternalStrike** | 10 | `[110, 10×9]` | `[100, 6×9]` |

### Damage — Single Enemy

| Pair | Weave Name | Hits | Chain Family | Notes |
|------|-----------|------|-------------|-------|
| Celestial + Void | **Eclipse Strike** | 9 | OathBlade | Pure damage |
| Shadow + Rift | **Ghost Blade** | 9 | OathBlade | Pure damage |
| Storm + Rift | **Thunderstep** | 9 | OathBlade | Pure damage |

### Damage + Status — Single Enemy

| Pair | Weave Name | Hits | Chain Family | Status |
|------|-----------|------|-------------|--------|
| Celestial + Shadow | **Starless Cloak** | 1 | — | blind |
| Void + Shadow | **Hollow Mark** | 1 | — | atk_break |
| Void + Rift | **Void Step** | 1 | — | blind |
| Ember + Earth | **Magma Press** | 30 | SigilBurst | def_break |
| Rift + Volt | **Precision Fracture** | 30 | SigilBurst | paralyze |
| Shadow + Earth | **Buried Name** *(secret)* | 1 | — | atk_break |

### Damage — All Enemies

| Pair | Weave Name | Hits | Chain Family | Notes |
|------|-----------|------|-------------|-------|
| Celestial + Storm | **Starfall** | 8 | CovenantWave | Pure damage |
| Void + Ember | **Ashen Covenant** *(secret)* | 8 | CovenantWave | Pure damage |
| Storm + Ember | **The Warbond** *(secret)* | 10 | EternalStrike | Pure damage |
| Void + Light | **The First Compact** *(secret)* | 10 | EternalStrike | def_break |

### Damage + Status — All Enemies

| Pair | Weave Name | Hits | Chain Family | Status |
|------|-----------|------|-------------|--------|
| Celestial + Rift | **Open Gate** | 8 | CovenantWave | def_break |
| Celestial + Volt | **Precision Light** | 8 | CovenantWave | atk_break |
| Storm + Volt | **Overcharge** | 8 | CovenantWave | paralyze |
| Shadow + Storm | **Stormblind** *(secret)* | 8 | CovenantWave | blind |
| Void + Tide | **The Drowned Compact** *(secret)* | 8 | CovenantWave | def_break |

### Heal / Recovery — All Allies

| Pair | Weave Name | Effect |
|------|-----------|--------|
| Celestial + Tide | **Moonwater Veil** | Party HP restore |

### Buff — All Allies (Status)

| Pair | Weave Name | Status | Notes |
|------|-----------|--------|-------|
| Celestial + Light | **Dawnweave** | protect | — |
| Celestial + Ember | **Flare Oath** | shell | — |
| Celestial + Earth | **Starroot** | protect | — |
| Light + Ember | **Oath Flame** | shell | — |
| Light + Tide | **Blessed Current** | hp_regen | — |
| Tide + Earth | **Deep Root** | mp_regen | — |
| Light + Storm | **Sera's Vigil** *(secret)* | hp_regen | — |

---

## The Resonance Web

Visual diagram logic for Party Setup screen. Sigils are displayed as nodes. Active resonance connections between nodes in your current party glow. The web shows players at a glance which combinations are possible with their current roster.

```
        [Celestial]
       /     |      \
  [Light] [Storm] [Void]
    |   \  / \   /  |
 [Ember] [Volt] [Shadow]
    |      |      |
  [Earth] [Rift] [Tide]
        \   |   /
         [Celestial]
```

Celestial (Lyria) sits at the center of the web — visually represents her cross-compatibility role without requiring an explanatory tooltip.

---

## Oath-Weaver Roster Roles

| Unit | Oath-Weaver Status | Weave Compatibility |
|------|--------------------|---------------------|
| **Lyria** | Celestial Oath-Weaver | ALL pairs — universal catalyst |
| **Aric** | Knight-Without-Title (Ch3 unlock) | Compact-adjacent pairs: Celestial, Light, Ember, Earth |
| **Future Units** | Varies by lore role | Defined per character at release |

**Design intent:** Lyria is irreplaceable as the universal weaver. Aric's restricted set creates a different strategic profile — he can Weave, but his party must be built around his limits. Future characters expand the web in new directions without making Lyria redundant.

---

## New Player Experience Flow

**Mission 1 (Tutorial):**
- Lyria and Aric are in the default party.
- Passive Resonance between Celestial + Ember (Aric's oath-pattern) activates automatically — a golden line glows between their cards.
- Tutorial tooltip: *"Sigil Resonance is active. Units with compatible sigil types support each other passively."*
- No Weave introduced yet.

**Mission 3–5:**
- Kain joins. Shadow + Celestial passive resonance adds a second connection.
- First Weave tutorial: *"Lyria is an Oath-Weaver. In battle, she can Weave two sigils together for a powerful combined effect."*
- Player is prompted to try Celestial + Shadow → Starless Cloak (make one ally untargetable). Low-stakes, visible, satisfying.

**Mission 6+:**
- Weave system fully unlocked. Resonance Codex accessible from the main menu.
- Players begin discovering combinations on their own.

**Endgame:**
- Full 10-sigil roster available. Party building is now a meaningful strategic layer.
- Compact Surges discoverable through constellation completion.
- Codex tracks all 45 combinations + discovered constellations.

---

## The Resonance Codex
## The Resonance Codex

A collectible log accessible from the main menu and party setup screen, served by `SigilResonanceManager.GetCodexEntries()` which returns `WeaveCodexEntry` objects for UI consumption.

### Standard Weaves
- All 19 non-secret weaves are always visible in the codex.
- Undiscovered entries show `displayName = "???"`, `sigilA/B = None`, and a discovery hint: *"Weave [Sigil A] and [Sigil B] to reveal."*
- Once used in battle, the entry is marked discovered and shows full name, description, and targeting info.

### Secret Weaves
- 7 secret weaves are locked by default — they do not appear in the codex at all until explicitly unlocked.
- Each has an `unlock_source` (`Mission`, `Event`, `Gift`, `Exploration`, `Shop`) and a cryptic `unlock_hint` shown in the locked codex entry.
- Unlock sources:
  - **Mission** — granted via `first_clear_weave_unlocks` in `MissionClearRewardRecord` on first clear
  - **Event** — via `WeaveUnlockManager.UnlockFromEvent(weaveId, eventId)`
  - **Gift** — via `WeaveUnlockManager.UnlockFromGift(weaveId, giftingUnitId)`
  - **Exploration** — via `WeaveUnlockManager.UnlockFromExploration(weaveId, locationId)`
  - **Shop** — via `WeaveUnlockManager.TryUnlockFromShop(weaveId, archiveTokenCost)`
- Once unlocked, secret weaves appear with their locked codex entry flipping to show full name and effect.

### WeaveCodexEntry Fields
```csharp
public class WeaveCodexEntry {
    public string id;           // "Shadow|Rift"
    public string displayName;  // Full name or "???" if undiscovered
    public SigilType sigilA;    // None if undiscovered
    public SigilType sigilB;    // None if undiscovered
    public bool isDiscovered;   // Used in battle at least once
    public bool isUnlocked;     // Secret weave has been granted (always true for non-secret)
    public bool isSecret;       // Is this a secret weave?
    public bool isKnown;        // isDiscovered || isUnlocked
    public string unlockHint;   // Cryptic hint shown before unlock
    public string portraitId;   // Art asset id for codex portrait
    public string targeting;    // "SingleEnemy", "AllEnemies", etc.
    public WeaveUnlockSource unlockSource;
    public string statusId;     // Inflicted status, if any
}
```

Persistence: `SaveGameData.unlockedSecretWeaves` (List\<string\> of weave IDs) — loaded and applied via `SigilResonanceManager.LoadUnlockedSecretWeaves()`.

---

## Implementation Notes (Unity C#)
## Implementation Notes (Unity C#)

**Implemented Architecture:**

```
SigilType (enum) — 10 values (None=0, Celestial=1, Void=2, Light=3, Tide=4,
                               Shadow=5, Storm=6, Ember=7, Rift=8, Earth=9, Volt=10)

SigilProfileResolver (Singleton)
  - Unit-to-sigil mapping
  - IsOathWeaver(unit) eligibility check (including Aric latent path)

SigilResonanceManager (Singleton)
  - LoadWeaveCatalogFromJson()  ← loads sigil_weaves.json
  - BuildPairKey(a, b) → always lower-enum-value sigil first
  - GetAvailableWeavesForUnit(unit) → filters to currently unlocked recipes
  - GetCodexEntries() → List<WeaveCodexEntry> for UI
  - UnlockSecretWeave(weaveId) → adds to unlockedSecretWeaves HashSet
  - IsWeaveAvailable(weaveId) → bool
  - LoadUnlockedSecretWeaves(List<string>) ← called by SaveGameManager
  - GetUnlockedSecretWeavesSnapshot() → List<string> for save

WeaveDefinition (internal data class, populated from sigil_weaves.json)
  Fields: id, a, b, displayName, targeting, powerMultiplier, flatPower,
          magical, statusId, isSecret, unlockSource, unlockHint, portraitId,
          attackCount, attackFrames, attackDamage, chainFamily

WeaveUnlockSource (enum): Standard, Mission, Event, Gift, Exploration, Shop

WeaveUnlockManager (Singleton, DontDestroyOnLoad)
  - UnlockFromEvent(weaveId, eventId)
  - UnlockFromGift(weaveId, giftingUnitId)
  - UnlockFromExploration(weaveId, locationId)
  - TryUnlockFromShop(weaveId, archiveTokenCost)
  - SetShopCatalog(entries) / GetAvailableShopEntries()
  - static event OnWeaveUnlocked(weaveId, source)

SaveGameData fields:
  - discoveredWeaves: List<string>        — standard codex discovery tracking
  - unlockedSecretWeaves: List<string>    — explicitly granted secret weave IDs
```

All combination keys are sorted by SigilType enum value before lookup so `(Storm, Celestial)` and `(Celestial, Storm)` resolve to the same entry. Weave catalog is JSON-driven — new combinations ship as `sigil_weaves.json` data updates with no code changes required.

---

## Open Questions for Next Session
## Open Questions

1. ~~Should Weave cooldowns be tracked per-battle or per-mission (across waves)?~~ — **Resolved:** Mission weave budget tracked in `SigilResonanceManager` per-battle.
2. ~~Does passive resonance produce a flat stat bonus or a conditional trigger?~~ — **Resolved:** Flat stat bonus applied at mission start to compatible sigil pairs.
3. Should incompatible sigil pairs still produce a minor generic effect, or nothing at all? — *Open.*
4. ~~Is the Resonance Codex unlocked immediately or after a story beat in Ch1?~~ — **Resolved:** Codex unlock tied to `mission_006` completion.
5. ~~Should Aric's Oath-Weaver role be unlocked narratively in Ch3 or available as a latent mechanic earlier?~~ — **Resolved:** Ch3 narrative unlock, latent path already in `SigilProfileResolver`.
