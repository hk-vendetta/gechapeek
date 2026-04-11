# Per-Unit Shard & Rarity Upgrade System - Implementation Guide

## Overview
This system tracks unit-specific shards collected from summon duplicates and provides mechanics for players to upgrade unit rarity tiers using those shards.

## System Architecture

### 1. **Data Flow: Shard Acquisition**
```
Pull duplicate unit → GachaSystem.AwardDupeShardsIfDuplicate()
  ├─ ResolveDupeShardReward(rarity, isFeatured)  [determines shard amount]
  ├─ prismShards += shards                        [aggregate, for compatibility]
  └─ unitShardCounts[sourceUnitId] += shards     [NEW: per-unit tracking]
     
User pulls Unit "Aria" as duplicate (5-star):
  → 80 shards (base) + 40 shards (featured bonus) = 120 shards
  → unitShardCounts["aria_001"] = 120
```

### 2. **Data Flow: Rarity Upgrade**
```
Player clicks "Upgrade Aria to Epic"
  → UnitUpgradeSystem.TryUpgradeUnit(unit, targetTier=4)
     ├─ Validate: currentRarity (3) < targetRarity (4) ✓
     ├─ Check cost: GetUpgradeCost() → 60 shards required
     ├─ Verify shards: GetUnitShardCount("aria_001") → 120 ✓
     ├─ Consume: GachaSystem.TryConsumeUnitShards("aria_001", 60)
     │   └─ unitShardCounts["aria_001"] = 60 (120 - 60)
     ├─ Upgrade: unit.upgradedRarityTier = 4
     ├─ Resolve stats: UnitProgressionManager.ApplyLevelToUnit()
     │   └─ Fetches stats from rarity_entries[4] in unit definition
     └─ Save: SaveGameManager.SaveProfile()
        └─ Persists new shardCounts to save file
```

### 3. **Data Storage**

#### In GachaSystem
```csharp
private Dictionary<string, int> unitShardCounts;  // Key: sourceUnitId (e.g., "aria_001")
                                                  // Value: accumulated shards for that unit
```

#### In SaveGameData
```csharp
public Dictionary<string, int> unitShardCounts = new Dictionary<string, int>();
```

**Save File Format:**
```json
{
  "unitShardCounts": {
    "aria_001": 120,
    "thane_001": 80,
    "kess_001": 45
  }
}
```

### 4. **Upgrade Cost Schedule**
| Tier Transition | Cost | Cumulative |
|---|---|---|
| Oath-Called → Oath-Sworn (1→2) | 20 | 20 |
| Oath-Sworn → Oath-Tempered (2→3) | 40 | 60 |
| Oath-Tempered → Oath-Forged (3→4) | 60 | 120 |
| Oath-Forged → Oath-Eternal (4→5) | 80 | 200 |
| Oath-Eternal → Beyond (5→6) | 120 | 320 |

## Core Components

### UnitUpgradeSystem (NEW)
**File:** `Assets/Scripts/UnitUpgradeSystem.cs`

**Key Methods:**
```csharp
// Attempt to upgrade a unit to a specific tier
UpgradeResult TryUpgradeUnit(GachaItem unit, int targetRarityTier)

// Get cost to upgrade from current tier to target tier
int GetUpgradeCost(GachaItem unit, int targetRarityTier)

// Get maximum rarity tier for a unit (from unit definition)
int GetUnitMaxRarity(GachaItem unit)

// Get list of available tiers the unit can upgrade to
List<int> GetAvailableUpgradeTiers(GachaItem unit)

// Get tier name for UI display
string GetTierName(int tier)
```

**Usage Example:**
```csharp
GachaItem aria = UnitInventory.Instance.ownedUnits[0];
UnitUpgradeSystem.UpgradeResult result = UnitUpgradeSystem.Instance.TryUpgradeUnit(aria, 4);

if (result.success)
{
    Debug.Log($"Upgraded! Remaining shards: {result.remainingShards}");
    // UI updates automatically after screen refresh
}
else
{
    Debug.LogWarning(result.message);  // e.g., "Insufficient shards..."
}
```

### GachaSystem (MODIFIED)
**New Methods:**
```csharp
// Get shard count for a specific unit
int GetUnitShardCount(string unitId)

// Get copy of all unit shard counts
Dictionary<string, int> GetAllUnitShardCounts()

// Consume shards for a unit (returns success/failure)
bool TryConsumeUnitShards(string unitId, int amount)

// Load unit shards from save file (called by SaveGameManager)
void LoadUnitShardCounts(Dictionary<string, int> loadedUnitShards)
```

### SaveGameManager (MODIFIED)
**Changes:**
- `BuildSaveData()` now captures `GachaSystem.GetAllUnitShardCounts()`
- `ApplySaveData()` now calls `GachaSystem.LoadUnitShardCounts()`

**Persistence Flow:**
```
Game Running
  ↓
Pull duplicate → GachaSystem.unitShardCounts updated
  ↓
Auto-save triggers
  ↓
BuildSaveData() → captures unitShardCounts
  ↓
Save file written to disk
  ↓
Game Closes / Reloads
  ↓
ApplySaveData() → LoadUnitShardCounts() → restored to memory
```

## Integration with Existing Systems

### Unit Definition (units_game.json)
Each unit has multiple rarity tier entries with different stats and skills:
```json
{
  "id": "aria_001",
  "rarity_min": 3,
  "rarity_max": 5,
  "rarity_entries": [
    {
      "rarity": 3,
      "stats": { "hp": 1000, "atk": 120, ... },
      "skill_ids": ["aria_skill_1_rare"]
    },
    {
      "rarity": 4,
      "stats": { "hp": 1200, "atk": 150, ... },
      "skill_ids": ["aria_skill_1_epic", "aria_skill_2_epic"]
    },
    {
      "rarity": 5,
      "stats": { "hp": 1500, "atk": 200, ... },
      "skill_ids": ["aria_skill_1_legendary", "aria_skill_2_legendary", "aria_skill_3_legendary"]
    }
  ]
}
```

When a unit is upgraded to tier 4:
- `unit.upgradedRarityTier = 4`
- `UnitProgressionManager.ApplyLevelToUnit()` fetches stats from `rarity_entries[4]`
- Unit's effective rarity becomes 4 for all subsequent lookups
- Skills resolve to tier 4's skill_ids

### Gacha Banners (gacha_banners.json)
When a player pulls a unit as a duplicate, shards are awarded based on:
```json
{
  "dupe_shard_rules": [
    { "rarity_tier": 1, "shards": 5, "featured_bonus_shards": 0 },
    { "rarity_tier": 2, "shards": 10, "featured_bonus_shards": 5 },
    { "rarity_tier": 3, "shards": 25, "featured_bonus_shards": 15 },
    { "rarity_tier": 4, "shards": 60, "featured_bonus_shards": 30 },
    { "rarity_tier": 5, "shards": 80, "featured_bonus_shards": 40 }
  ]
    "dupe_shard_rules": [
      { "rarity_tier": 1, "shards": 1, "featured_bonus_shards": 0 },
      { "rarity_tier": 2, "shards": 3, "featured_bonus_shards": 0 },
      { "rarity_tier": 3, "shards": 10, "featured_bonus_shards": 0 },
      { "rarity_tier": 4, "shards": 25, "featured_bonus_shards": 0 },
      { "rarity_tier": 5, "shards": 80, "featured_bonus_shards": 40 }
    ]
}
```

## Setup Instructions

### 1. Add UnitUpgradeSystem to Scene
- Create a new empty GameObject: `UnitUpgradeSystem`
- Attach the `UnitUpgradeSystem.cs` script to it
- Ensure it persists across scenes (marked as DontDestroyOnLoad)

### 2. Verify Dependencies
Ensure these systems are already in your scene (they should be):
- `GachaSystem` (updates unitShardCounts on pulls)
- `SaveGameManager` (persists/loads unitShardCounts)
- `UnitInventory` (stores unit instances)
- `UnitProgressionManager` (applies level/stats)
- `GameUnitDatabase` (loads unit definitions)

### 3. Hook into UI (Next Phase)
You'll need to create UI screens for:
- **Unit Enhancement Screen**: Display unit → show available upgrades → button to upgrade
- **Shard Display**: Show "123 Aria Shards" / "60 required for next tier"
- **Confirmation Dialog**: "Spend 60 shards to upgrade Aria to Oath-Forged?"
- **Result Toast**: "Successfully upgraded to Oath-Forged!"

## Data Integrity & Edge Cases

### Scenario: Player pulls 3 duplicates of "Aria"
```
Pull 1: Aria (5-star duplicate)
  → unitShardCounts["aria_001"] = 80

Pull 2: Aria (5-star duplicate)
  → unitShardCounts["aria_001"] = 160

Pull 3: Aria (5-star duplicate)
  → unitShardCounts["aria_001"] = 240

Upgrade Aria to Oath-Forged (tier 4):
  → Cost: 60 shards
  → unitShardCounts["aria_001"] = 180 (240 - 60)

Upgrade Aria to Oath-Eternal (tier 5):
  → Cost: 80 shards
  → unitShardCounts["aria_001"] = 100 (180 - 80)
```

### Scenario: Save/Load after partial upgrade
1. Player upgrades Aria (shards consumed, file saved)
2. Game crashes/reload
3. Load save file → `unitShardCounts["aria_001"] = 40` restored
4. Unit instance also has `upgradedRarityTier = 6` → consistency maintained

### Scenario: Upgrade max-tier unit
```
Aria is already at tier 5 (max rarity for this unit)
UnitUpgradeSystem.GetAvailableUpgradeTiers(aria) → []  (empty list)
TryUpgradeUnit(aria, 5) → UpgradeResult.success = false
```

## API Summary for UI Development

```csharp
// Get player's shard count for a unit
int shards = GachaSystem.Instance.GetUnitShardCount("aria_001");

// Get cost to upgrade to next tier
int currentTier = unit.upgradedRarityTier > 0 ? unit.upgradedRarityTier : unit.sourceRarityTier;
int costToNext = UnitUpgradeSystem.Instance.GetUpgradeCost(unit, currentTier + 1);

// Get list of available tiers
List<int> availableTiers = UnitUpgradeSystem.Instance.GetAvailableUpgradeTiers(unit);

// Attempt upgrade
UnitUpgradeSystem.UpgradeResult result = UnitUpgradeSystem.Instance.TryUpgradeUnit(unit, targetTier);
if (result.success)
{
    // Show success message, refresh UI
    Debug.Log($"Success! {result.remainingShards} shards left for {unit.sourceUnitId}");
}
else
{
    // Show error message
    Debug.LogWarning(result.message);
}

// Get tier name for UI
string tierName = UnitUpgradeSystem.Instance.GetTierName(4);  // "Oath-Forged"
```

## Testing Checklist

- [ ] Pull duplicate unit → verify shards awarded and stored
- [ ] Check save file contains unitShardCounts dictionary
- [ ] Load game → verify unitShardCounts restored correctly
- [ ] Attempt upgrade with insufficient shards → error message
- [ ] Upgrade unit with sufficient shards → verify tier changed + stats updated
- [ ] Pull more duplicates after upgrade → verify new shards added to remaining count
- [ ] Upgrade unit to max tier → verify no further upgrades available
- [ ] Verify unit definition loads correct stats/skills per tier

## Troubleshooting

**Q: UnitUpgradeSystem instance not found**
- Ensure UnitUpgradeSystem prefab/GameObject exists in scene
- Check it's marked as DontDestroyOnLoad

**Q: Shards not persisting after reload**
- Check SaveGameManager.ApplySaveData() calls LoadUnitShardCounts()
- Verify unitShardCounts appears in save file JSON
- Confirm GachaSystem is loaded before attempting to restore shards

**Q: Upgrade fails with "Unit has no sourceUnitId"**
- Verify the pulled unit was properly registered via GachaSystem.PerformPull()
- Check UnitInventory.RegisterPulledUnits() completed successfully

**Q: Stats didn't update after upgrade**
- Verify UnitProgressionManager.Instance exists
- Check unit definition has rarity_entries for target tier
- Confirm unit's level is > 0 (stats are scaled from level)

## Future Enhancements

1. **Shard Shop**: Allow players to purchase unit shards with archive tokens
2. **Bulk Upgrade**: UI to select multiple upgrade tiers in one transaction
3. **Shard Trading**: Exchange rare shards for common shards (with rate conversion)
4. **Upgrade Notifications**: Show which units can be upgraded in unit list
5. **Efficiency Tracker**: Track shards earned per 10-pull (for player planning)
