# Gacha & Inventory Systems Overview

## Quick Summary

**Pulls** are performed via `GachaSystem.PerformPull()`, which deducts tokens, applies pity logic, generates results, and triggers `OnPullCompleted`. Results are instant—no async calls. Pulled units land in `UnitInventory`. Player state persists in JSON via `SaveGameManager`, and gacha progression (pity, spark) is snapshot in `SaveGameData`.

---

## 1. Pull System End-to-End

### Entry Point
**`GachaSystem.PerformPull(int pullCount)`**
- Verifies token balance (`gachaTokens >= cost`)
- Deducts tokens
- Loops `pullCount` times, calling `Pull()` for each result
- Builds a `List<GachaItem>` of results
- Fires `OnPullCompleted` event
- Returns results immediately

### Per-Pull Mechanics: `GachaSystem.Pull(int guaranteedMinRarityTier)`
1. **Check Featured Hard Pity** — If `featuredPityCounter >= hard_pity` (default 80), guarantee featured rarity tier (5)
2. **Check General Pity** — All rarities have independent pity counters; if any hits its threshold, guarantee that rarity
3. **Calculate Adjusted Rates** — Start with base rarity rates, apply:
   - Banner-specific rarity rate overrides
   - Dynamic bonus from pity curves (if enabled)
   - Soft pity bonus (if enabled after pull 61)
   - For 10-pull guarantees: enforce minimum rarity on 10th pull
4. **Select Rarity** — Random weighted selection from adjusted rates
5. **Select Unit From Pool** — Random pick from eligible units at that rarity
   - **Featured Bias**: If pulling at featured tier (5), apply featured unit chance (~1.5%)
   - Featured units roll within that rarity; non-featured fill the remainder
6. **Update Pity** — Reset counter for selected rarity, increment all others
7. **Award Duplicates** — If unit source ID already owned, convert to **Prism Shards** (base: 1/3/10/25/80 by tier, +40 if featured)
8. **Add Spark Point** — Increment `bannerSparkPoints[activeBannerId]`

### Key Values (Configurable Per Banner)
```json
{
  "featured_rarity_tier": 5,           // Which rarity tier is featured
  "featured_unit_ids": [ "..." ],      // Featured unit pool
  "featured_rate": 0.015,              // 1.5% chance to roll featured within tier
  "soft_pity_start": 61,               // Soft pity activates at pull #61
  "soft_pity_step_percent": 0.5,       // +0.5% per pull after soft pity start
  "hard_pity": 80,                     // Guaranteed at pull #80
  "spark_threshold": 160,              // Pull 160 sparks to claim featured
  "guaranteed_min_rarity_on_ten_pull": 4  // 10th pull is ≥ rarity tier 4
}
```

---

## 2. Player Resources & Currencies

All tracked in `GachaSystem`:
- **`gachaTokens`** — Primary pull currency (deducted per pull, cost configurable per banner)
- **`featuredPityCounter`** — Incremented each pull, reset when featured rarity obtained
- **`prismShards`** — From duplicate units (1/3/10/25/80 base, +40 for featured dupes)
- **`archiveTokens`** — From banner conversions when swapping banners
- **`bannerSparkPoints[bannerId]`** — Per-banner spark accumulated (1 per pull)

Path: `SaveGameData` → restored via `GachaSystem.LoadSummonPersistentState()`

### Pity Counters
- **General Pity**: Each `GachaRarity` has independent counter (tracked in `pityCounters` dict)
  - Config: `pityPullCount` (default often 10 for lower rarities, 80 for featured)
- **Featured Pity**: `featuredPityCounter` (V1 summon progression)
  - Hard pity at 80 pulls
  - Soft pity starts at 61, ramping bonus each pull

---

## 3. Gacha System Data Structures

### GachaItem (Unit/Character)
```csharp
public class GachaItem : ScriptableObject
{
    public string sourceUnitId;          // Internal unit ID (e.g., "unit_001", "aric")
    public string inventoryInstanceId;   // Unique instance ID for owned copy
    public int inventorySlotIndex;       // UI slot position
    public int sourceRarityTier;         // Base rarity (1-5)
    public int upgradedRarityTier;       // Override rarity if > 0
    public string itemName;              // Display name
    
    // Combat stats
    public int maxHp, maxMp, attack, defense, magicAttack, spirit, speed, criticalChance;
    
    // Equipment & Runes
    public List<string> equippedEquipmentIds;
    public List<string> equippedRuneIds;
    
    // Progression
    public int level;                    // Current level
    public int currentExp;               // EXP toward next level
    
    // Abilities
    public List<string> skillIds;        // JSON-loaded skill references
    public List<UnitAbility> abilities;  // Inspector-authored abilities
}
```

### BannerRecord (from JSON)
```csharp
{
  "id": "featured_banner_name",
  "name": "Featured Summon: Unit Name",
  "description": "...",
  "banner_type": "featured",            // "featured" or "standard"
  "unit_ids": [ "unit_ids_available" ],
  "featured_unit_ids": [ "featured_ids" ],
  "featured_rate": 0.015,
  "soft_pity_start": 61,
  "hard_pity": 80,
  "soft_pity_step_percent": 0.5,
  "carry_pity_across_featured": true,   // Reset featured pity on banner change?
  "spark_threshold": 160,
  "guaranteed_min_rarity_on_ten_pull": 4,
  "rarity_rates": [
    { "rarity_tier": 5, "rate": 0.03 }
  ],
  "dupe_shard_rules": [
    { "rarity_tier": 5, "shards": 80, "featured_bonus_shards": 40 }
  ]
}
```

### SaveGameData (Persistence)
```csharp
{
  "version": 4,
  "playerId": "guid_string",
  "playerName": "Player",
  "gachaTokens": 200,
  
  // Units
  "ownedUnits": [ SavedUnitRecord[] ],
  "activePartyIndex": 0,
  "partySlots": [ SavedPartySlot[] ],
  
  // Gacha progression
  "featuredPityCounter": 15,
  "prismShards": 250,
  "archiveTokens": 50,
  "bannerSparkPoints": [
    { "bannerId": "standard", "points": 45 }
  ],
  
  // Inventory
  "ownedEquipment": [ EquipmentInventoryEntry[] ],
  "ownedRunes": [ RuneInventoryEntry[] ],
  "ownedItems": [ ItemInventoryEntry[] ],
  "gil": 5000,
  
  // Progression
  "completedMissions": [ "mission_ids" ],
  "currentNrg": 160,
  "maxNrg": 160,
  "lastNrgRecoveryUtc": 0
}
```
    // Per-unit shard tracking (key = sourceUnitId)
    "unitShardCounts": {
      "unit_lyria_001": 120,
      "unit_aric_001": 40
    },
  
    // Secret weave codex unlock state
    "unlockedSecretWeaves": [ "Shadow|Storm", "Void|Ember" ]
  }
  ```
- **`unitShardCounts[unitId]`** — Per-unit shard count from duplicate pulls (separate from aggregate `prismShards`; used by `UnitUpgradeSystem` for rarity tier upgrades)
- **`unlockedSecretWeaves`** — List of secret weave IDs explicitly granted to the player
  - Tracked **both** in aggregate (`prismShards`) and per-unit (`unitShardCounts[sourceUnitId]`)
  - Per-unit shards are the currency consumed by `UnitUpgradeSystem.TryUpgradeUnit()` for rarity tier upgrades; they cannot be spent on other units

---

## 4. Inventory Systems

All are **Singleton MonoBehaviours** with local dictionaries/lists:

| System | Storage | Tracks | Load/Save |
|--------|---------|--------|-----------|
| **UnitInventory** | `List<GachaItem>` | Owned units | `SaveGameData.ownedUnits` |
| **ItemInventory** | `Dict<itemId, qty>` | Consumables, mats | `SaveGameData.ownedItems` |
| **EquipmentInventory** | `List<EquipmentInventoryEntry>` | Equipped gear | `SaveGameData.ownedEquipment` |
| **RuneInventory** | `List<RuneInventoryEntry>` | Materia/runes | `SaveGameData.ownedRunes` |
| **PartySystem** | `List<PartySlot>` (3 slots, 6 units each) | Team setup | `SaveGameData.partySlots` |
| **GachaSystem** | `pityCounters`, `bannerSparkPoints`, etc. | Gacha state | Via `LoadSummonPersistentState()` |

**Load Flow**: `SaveGameManager.LoadProfile()` → deserialize JSON → call `.ReplaceOwnedEntries()` on each inventory → inventories rebuild runtime references

---

## 5. Banner System & Rotation

### Active Banner
- `GachaSystem.activeBannerId` — Currently active banner ID
- `GachaSystem.activeBannerRecord` — Deserialized banner config
- Loaded from `StreamingAssets/gacha_banners.json`

### Banner Rotation
**`BannerLifecycleManager.EndBannerAndRotateTo(string newBannerId)`**
1. Convert unspent spark from old banner to archive tokens: `ConvertBannerSparkToArchiveTokens(oldId)`
2. Switch pools: `GachaSystem.SetActiveBanner(newBannerId)`
3. Optionally reset featured pity (if `carry_pity_across_featured = false`)
4. Auto-save if enabled

### Pity Carry-Over
- If `carry_pity_across_featured = true`: featured pity meter persists across banner rotations
- If `false`: featured pity resets when rotating to a new featured banner

---

## 6. Where Player State Persists

### Local Storage (Default)
**`LocalFileSaveStorage`** → `Application.persistentDataPath`
- Saves `SaveGameData` as pretty-printed JSON
- File per player ID

### Server Storage (Optional)
**`ServerSaveStorage`** — Wraps HTTP client
- POSTs JSON to `http://serverUrl/v1/save/write`
- Falls back to local if server unavailable

### Auto-Save Hooks
`SaveGameManager.HookAutoSaveEvents()` wires up listeners:
- Unit inventory changes → `SaveGame()`
- GachaSystem `OnPullCompleted` → `SaveGame()`
- Equipment changes → `SaveGame()`
- Mission completion → `SaveGame()`
- Banner rotation → `SaveGame()` (optional)

---

## 7. Quick Reference: Key Classes & Files

| File | Purpose |
|------|---------|
| [GachaSystem.cs](GachaSystem.cs) | Pull logic, pity, spark, dupe shards |
| [UnitInventory.cs](UnitInventory.cs) | Owned unit tracking |
| [ItemInventory.cs](ItemInventory.cs) | Consumable/material tracking |
| [EquipmentInventory.cs](EquipmentInventory.cs) | Equipment & stat bonuses |
| [MateriaInventory.cs](MateriaInventory.cs) | Rune/materia system |
| [PartySystem.cs](PartySystem.cs) | Team management (3 slots, 6 units each) |
| [SaveGameManager.cs](SaveGameManager.cs) | Profile load/save orchestration |
| [SaveGameData](SaveGameManager.cs) | Serialized player state struct |
| [BannerLifecycleManager.cs](BannerLifecycleManager.cs) | Banner rotation & spark conversion |
| [gacha_banners.json](StreamingAssets/gacha_banners.json) | Banner definitions |
| [GachaItem.cs](GachaItem.cs) | Unit data (stats, skills, equipment) |
| [GachaRarity.cs](GachaRarity.cs) | Rarity tier config |

---

## 8. Example: A Complete Pull Flow

```
User requests 10 pulls
  ↓
GachaSystem.PerformPull(10)
  ├─ Check tokens: 10 × 1 = 10 gachaTokens needed ✓
  ├─ Deduct: gachaTokens -= 10
  ├─ Loop 10 times:
  │   └─ Pull()
  │       ├─ Check featured hard pity (feature pityCounter == 80?) → No
  │       ├─ Check general pity → No
  │       ├─ Adjust rates (soft pity +0.5% per pull after 61) 
  │       ├─ Pick rarity (weighted by adjusted rates)
  │       ├─ Pick unit from that rarity's pool
  │       │   ├─ If featured rarity: 1.5% featured bias
  │       │   └─ Else: random from pool
  │       ├─ Check if dupe → Award prism shards
  │       ├─ AddSparkPoint(activeBannerId) → spark++
  │       └─ Return GachaItem
  ├─ OnPullCompleted?.Invoke(results)
  ├─ UnitInventory.RegisterPulledUnits(results)
  ├─ SaveGameManager.SaveGame() (auto-save)
  └─ Return List<GachaItem>
```

---

## 9. Notes for Extension

### Adding a New Currency
1. Add field to `SaveGameData`
2. Wire up `SaveGameManager.BuildSaveData()` to snapshot it
3. Wire up `SaveGameManager.LoadFromJson()` to restore it
4. Create getter/setter on relevant manager (e.g., `GachaSystem`, `SaveGameManager`)
5. Update auto-save hooks if needed

### Changing Pity Thresholds
- Modify `gacha_banners.json` → `soft_pity_start`, `hard_pity`, `spark_threshold`
- No code changes needed; all values resolved at runtime

### Adding Featured Units to a Banner
- Edit `gacha_banners.json` → add unit IDs to `featured_unit_ids` and `unit_ids`
- Ensure units exist in `GameUnitDatabase` with `is_summonable: true`

---

## Key Insights

1. **Pity is Per-Rarity**: Each `GachaRarity` has its own counter; pulling a lower rarity resets only that rarity's counter and increments all others.
2. **Featured Pity is Separate**: `featuredPityCounter` tracks pulls toward hard pity at tier 5 featured units specifically.
3. **Dupe → Shards → Archive**: Duplicates become prism shards; shards convert to archive tokens; archive tokens buy selector rewards (not yet visible in current code).
4. **Spark is Per-Banner**: Spark points are tracked separately per banner. Unspent spark converts to archive tokens on banner rotation.
5. **All State Snapshots at Save**: `SaveGameManager` reads from all inventories and gacha systems, builds a `SaveGameData` struct, and serializes to JSON.
6. **No Async RNG**: Pulls are instant—no network calls, no delays. Perfect for testing/replay.
