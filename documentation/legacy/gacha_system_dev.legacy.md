# Oathsworn: Compact — Gacha System Developer Guide

## Currency Naming Convention

- Internal code/storage names are unchanged for compatibility:
  - `gachaTokens` = summon pull currency
  - `archiveTokens` = premium/archive progression currency
- Player-facing labels:
  - `gachaTokens` displays as **Compact Seals**
  - `archiveTokens` displays as **High Compact Seals**

This keeps save-data/API stability while preserving lore-accurate UI terminology.

## System Architecture Overview

The gacha system is a multi-layered architecture spanning game logic, persistence, UI, and data configuration.

### High-Level Flow

```
[Summon Pull (via UI)]
    ↓
[GachaSystem.PerformPull()]
    ├─ Deduct tokens
    ├─ Loop N times: GachaSystem.Pull()
    │  ├─ Check hard pity
    │  ├─ Check soft pity + apply rate bonuses
    │  ├─ Select rarity tier
    │  ├─ Check featured bias
    │  ├─ Pick unit from pool
    │  └─ Award spark + dupe shards
    ├─ Fire OnPullCompleted event
    └─ Return List<GachaItem>
        ↓
[UnitInventory.RegisterPulledUnits()]
    ├─ Add units to inventory
    └─ Save via SaveGameManager
        ↓
[SaveGameManager.BuildSaveData()]
    ├─ Capture GachaSystem state (pity, spark, tokens, shards)
    └─ Persist to SaveGameData JSON
```

---

## File Structure

### Core Systems

| File | Purpose | Key Classes |
|------|---------|-------------|
| [Assets/Scripts/GachaSystem.cs](Assets/Scripts/GachaSystem.cs) | Pull engine, pity/soft pity, featured bias, spark tracking | `GachaSystem`, `BannerRecord`, `BannerSparkEntry` |
| [Assets/StreamingAssets/gacha_banners.json](Assets/StreamingAssets/gacha_banners.json) | Banner pool + rates configuration | JSON array of `BannerRecord` |
| [Assets/Scripts/GachaRarity.cs](Assets/Scripts/GachaRarity.cs) | Rarity tier definitions (ScriptableObject) | `GachaRarity` |
| [Assets/Scripts/GachaItem.cs](Assets/Scripts/GachaItem.cs) | Unit template (ScriptableObject) | `GachaItem` |
| [Assets/Scripts/UnitRarityTierCatalog.cs](Assets/Scripts/UnitRarityTierCatalog.cs) | Rarity tier metadata (constants + helpers) | `UnitRarityTierCatalog` |
| [Assets/Scripts/UnitUpgradeSystem.cs](Assets/Scripts/UnitUpgradeSystem.cs) | Per-unit shard consumption and rarity tier upgrade logic | `UnitUpgradeSystem`, `UpgradeResult` |
| [Assets/Scripts/WeaveUnlockManager.cs](Assets/Scripts/WeaveUnlockManager.cs) | Secret weave unlock hub (events, gifts, exploration, shop) | `WeaveUnlockManager`, `WeaveShopEntry` |

### Persistence

| File | Purpose | Key Classes |
|------|---------|-------------|
| [Assets/Scripts/SaveGameManager.cs](Assets/Scripts/SaveGameManager.cs) | Profile load/save orchestration | `SaveGameManager`, `SaveGameData` |
| `SaveGameData` (in SaveGameManager.cs) | Serialized player profile | Fields: `featuredPityCounter`, `prismShards`, `archiveTokens`, `bannerSparkPoints` |
| | | Also: `unitShardCounts` (per-unit shard dict), `unlockedSecretWeaves` (granted weave IDs) |

### UI & Display

| File | Purpose | Key Methods |
|------|---------|-------------|
| [Assets/Scripts/SparkProgressDisplay.cs](Assets/Scripts/SparkProgressDisplay.cs) | Spark counter UI + claim button | `OnClaimButtonPressed()`, `UpdateDisplay()` |
| [Assets/Scripts/ArchiveTokenDisplay.cs](Assets/Scripts/ArchiveTokenDisplay.cs) | Archive token balance display | `OpenShop()`, `CloseShop()` |
| [Assets/Scripts/SummonInputHandler.cs](Assets/Scripts/SummonInputHandler.cs) | Summon button bridge | `PerformSummon()`, `PerformAllInSummon()` |
| [Assets/Scripts/SummonResultUI.cs](Assets/Scripts/SummonResultUI.cs) | Pull result grid display | `ShowSummonResults()` |
| [Assets/Scripts/SummonRevealSequencer.cs](Assets/Scripts/SummonRevealSequencer.cs) | Staged attunement reveal sequence (intro, featured prism, grid prism) | `HandlePullCompleted()`, `RunSequence()`, `CloseSequence()` |
| [Assets/Scripts/PrismWidget.cs](Assets/Scripts/PrismWidget.cs) | Per-prism reveal widget (set data, animate in, crack, reveal labels) | `SetData()`, `AnimateIn()`, `Crack()` |
| [Assets/Scripts/AttunementFlowController.cs](Assets/Scripts/AttunementFlowController.cs) | Attunement/reveal screen handoff controller | `HandlePullCompleted()`, `HideAttunementScreen()`, `ShowAttunementScreen()` |

### Lifecycle

| File | Purpose | Key Methods |
|------|---------|-------------|
| [Assets/Scripts/BannerLifecycleManager.cs](Assets/Scripts/BannerLifecycleManager.cs) | Banner rotation & spark conversion | `EndBannerAndRotateTo()` |

---

## JSON Banner Schema

Located at `Assets/StreamingAssets/gacha_banners.json`

### Full BannerRecord Structure

```json
{
  "id": "featured_knights",
  "name": "Featured Knights",
  "description": "Rate-up banner with frontline compact-aligned defenders.",
  "enabled": true,
  "banner_type": "featured",
  "token_cost": 200,
  
  "featured_rarity_tier": 5,
  "featured_unit_ids": ["unit_009", "unit_010"],
  "featured_rate": 0.015,
  
  "soft_pity_start": 61,
  "hard_pity": 80,
  "soft_pity_step_percent": 0.5,
  "carry_pity_across_featured": true,
  
  "spark_threshold": 160,
  "spark_to_archive_token_rate": 1,
  "guaranteed_min_rarity_on_ten_pull": 4,

  "weekly_dungeon_focus": [
    {
      "week": 1,
      "dungeon_id": "dungeon_001",
      "dungeon_name": "Verdant Clearing",
      "recommended_level": 1,
      "note": "Opening progression path"
    }
  ],
  
  "rarity_rates": [
    { "rarity_tier": 1, "rate": 0.15 },
    { "rarity_tier": 2, "rate": 0.30 },
    { "rarity_tier": 3, "rate": 0.37 },
    { "rarity_tier": 4, "rate": 0.15 },
    { "rarity_tier": 5, "rate": 0.03 }
  ],
  
  "dupe_shard_rules": [
    { "rarity_tier": 1, "shards": 1, "featured_bonus_shards": 0 },
    { "rarity_tier": 2, "shards": 3, "featured_bonus_shards": 0 },
    { "rarity_tier": 3, "shards": 10, "featured_bonus_shards": 0 },
    { "rarity_tier": 4, "shards": 25, "featured_bonus_shards": 0 },
    { "rarity_tier": 5, "shards": 80, "featured_bonus_shards": 40 }
  ],
  
  "unit_ids": ["unit_001", "unit_002", ...]
}
```

### Field Definitions

| Field | Type | Description | Example |
|-------|------|-------------|---------|
| `id` | string | Unique banner identifier | `"featured_knights"` |
| `name` | string | Display name | `"Featured Knights"` |
| `description` | string | Banner subtitle/marketing copy | `"Rate-up banner with frontline compact-aligned defenders."` |
| `enabled` | bool | Is banner active? | `true` |
| `banner_type` | string | `"standard"` or `"featured"` | `"featured"` |
| `token_cost` | int | Crystals per pull | `200` |
| `featured_rarity_tier` | int | Rarity tier for featured rate-up | `5` |
| `featured_unit_ids` | string[] | Unit IDs guaranteed on spark | `["unit_009"]` |
| `featured_rate` | float | Absolute Tier 5 rate for featured [0–1] | `0.015` |
| `soft_pity_start` | int | Pull count when soft pity kicks in | `61` |
| `hard_pity` | int | Pull count for guaranteed featured | `80` |
| `soft_pity_step_percent` | float | Rate increase per pull after soft_pity_start (%) | `0.5` |
| `carry_pity_across_featured` | bool | Does pity reset on new featured banner? | `true` |
| `spark_threshold` | int | Spark needed to claim featured | `160` |
| `spark_to_archive_token_rate` | int | Conversion ratio (spark → tokens) | `1` |
| `guaranteed_min_rarity_on_ten_pull` | int | Min tier for 10-pull guarantee | `4` |
| `weekly_dungeon_focus` | object[] | Optional weekly dungeon recommendation metadata for UI/event tie-ins | `[ { "week": 1, "dungeon_id": "dungeon_001", "dungeon_name": "Verdant Clearing", "recommended_level": 1, "note": "..." } ]` |
| `rarity_rates` | BannerRarityRate[] | Per-tier drop rates | `[...]` |
| `dupe_shard_rules` | DupeShardRule[] | Shard payouts by tier | `[...]` |
| `unit_ids` | string[] | Unit pool IDs for banner | `["unit_001", ...]` |

---

## Summon Reveal Runtime (Current)

`SummonRevealSequencer` now owns the modern reveal flow and subscribes directly to `GachaSystem.OnPullCompleted`.

### Sequence Phases

1. **Intro Overlay**
  - Optional intro animator
  - Overlay fade-in
2. **Featured Prism**
  - Highest rarity pulled item first
  - Explicit tap-to-unseal and tap-to-continue gates
3. **Prism Grid**
  - Remaining pulls instantiate from `prismWidgetPrefab`
  - Slot 0 is pre-revealed (featured unit)
  - Remaining slots auto-crack by interval
4. **Done / Close**
  - Close button displayed at end

### Input System Compatibility

The sequencer tap gate uses the Input System package (`UnityEngine.InputSystem`) instead of legacy `UnityEngine.Input` calls. This avoids runtime exceptions when the project is configured for Input System only.

### Legacy UI Compatibility

`legacyResultUI` is optional. If assigned, it is hidden while the reveal sequence is active and restored when closed.

### Attunement Handoff

`AttunementFlowController` listens to `OnPullCompleted` and hides the attunement root when reveal starts, then restores attunement when reveal close is clicked.

---

## Attunement Banner Text API

The attunement UI can now bind richer banner data directly from `GachaSystem`:

- `GetActiveBannerName()`
- `GetActiveBannerDescription()`
- `GetActiveBannerFeaturedUnitsSummary()`
- `GetActiveBannerRateSummary()`

`SummonInputHandler` supports these optional text fields:

- `bannerNameText`
- `bannerDescriptionText`
- `featuredUnitsText`
- `bannerRateText`

If a text field is unassigned, update flow continues without errors.

---

## Pull Algorithm (Detailed)

### PerformPull(pullCount: int)

```csharp
1. Deduct tokens (pullCount × token_cost) from gachaTokens
2. For each pull i in [0, pullCount):
   a. Capture owned unit IDs (for dupe detection)
   b. Call Pull(guaranteedMinRarityTier = ...)
   c. Add spark point to active banner
   d. Check if duplicate; if yes, award dupe shards
   e. Add to result list
3. Fire OnPullCompleted(resultList)
4. Return resultList
```

### Pull(guaranteedMinRarityTier = 0)

```csharp
1. Check hard pity:
   if (featuredPityCounter >= hard_pity)
      → Return featured Tier 5 unit
      → Reset featured pity to 0

2. Check standard pity guarantees:
   if (any rarity at its pity threshold)
      → Return that rarity
      → Increment all others

3. Calculate adjusted rates:
   for each rarity:
     rate = banner_overrides[tier] OR base_rate[tier]
     if (useDynamicRate AND pityCounter > 0)
       rate += cur_pity / pity_count * curve_value
     if (tier == featured_tier AND soft_pity_active)
       rate += soft_pity_bonus

4. Normalize all rates to sum = 1.0

5. Select rarity from weighted random

6. Pick unit from pool:
   if (selected_rarity == featured_tier)
     → 50% featured chance (featured_rate / total_tier5_rate)
     → 50% non-featured chance
   else
     → Random from pool

7. Build final GachaItem (clone template, set tier)

8. Apply featured pity outcome:
   if (selected_tier >= featured_tier)
     featured_pity = 0
   else
     featured_pity++

9. Return GachaItem
```

### Soft Pity Bonus Calculation

```csharp
if (featured_pity < soft_pity_start) return 0f
int pulls_into_soft = featured_pity - soft_pity_start + 1
bonus = soft_pity_step_percent * 0.01f * pulls_into_soft
```

Example (soft_pity_start=61, soft_pity_step_percent=0.5):
- Pull 60: +0% bonus
- Pull 61: +0.5% bonus
- Pull 70: +5% bonus
- Pull 79: +9.5% bonus

---

## Spark System Implementation

### Data Flow

1. **Pull Event** → `AddSparkPoint(banner_id)` increments `bannerSparkPoints[banner_id]`
2. **Check Claim** → `GetActiveBannerSparkPoints()` returns current counter
3. **Claim** → `ClaimFeaturedWithSpark()`:
   - Validates threshold met
   - Picks random featured unit from pool
   - Resets spark counter to 0
   - Registers unit to inventory
4. **Banner Rotation** → `ConvertBannerSparkToArchiveTokens(old_banner_id)`:
   - Reads `bannerSparkPoints[old_banner_id]`
   - Multiplies by `spark_to_archive_token_rate`
   - Adds to `archiveTokens`
   - Removes from `bannerSparkPoints`

### Persistence

**SaveGameData fields:**
```csharp
public List<BannerSparkEntry> bannerSparkPoints = new List<BannerSparkEntry>();
  // Serializable list of { bannerId, points }
```

**On Save:**
```csharp
data.bannerSparkPoints = GachaSystem.Instance.GetBannerSparkPointEntries();
```

**On Load:**
```csharp
GachaSystem.Instance.LoadSummonPersistentState(
  data.featuredPityCounter,
  data.prismShards,
  data.archiveTokens,
  data.bannerSparkPoints
);
```

---

## Featured Rate-Up Logic

When a Tier 5 is rolled on a featured banner:

```csharp
if (featuredUnitIds.Length > 0)
  featured_absolute_rate = configured_featured_rate (e.g., 0.015)
  top_tier_rate = total_tier5_rate_from_banner (e.g., 0.03)
  featured_chance = featured_absolute_rate / top_tier_rate = 0.5 (50%)
  
  if (Random.value <= 0.5)
    pick from featured_unit_ids
  else
    pick from non-featured Tier 5 pool
```

This ensures the published rates are honest: 1.5% featured, 1.5% non-featured in the top tier.

---

## Common Dev Tasks

### Add a New Banner

1. **Update `gacha_banners.json`:**
   ```json
   {
     "id": "new_banner_id",
     "name": "New Featured Banner",
     "enabled": true,
     "banner_type": "featured",
     "token_cost": 200,
     "featured_unit_ids": ["unit_NEW"],
     "featured_rate": 0.015,
     // ... (copy from featured_knights template)
   }
   ```

2. **Activate in-game:**
   ```csharp
   BannerLifecycleManager.Instance.EndBannerAndRotateTo("new_banner_id");
   ```

3. **Ensure unit exists in `units_game.json`** with `"is_summonable": true`

### Change Pity Thresholds

In `gacha_banners.json`:
```json
"soft_pity_start": 61,      // Change when soft pity starts
"hard_pity": 80,            // Change hard pity pull count
"soft_pity_step_percent": 0.5  // Change rate increase per pull
```

### Adjust Dupe Shard Payouts

In `gacha_banners.json`:
```json
"dupe_shard_rules": [
  { "rarity_tier": 5, "shards": 80, "featured_bonus_shards": 40 }
]
```

### Grant Archive Tokens (Event Reward)

```csharp
GachaSystem.Instance.AddArchiveTokens(100);  // Grant 100 tokens
SaveGameManager.Instance.SaveGame();
```

### Implement Archive Shop

Create a new `ArchiveShopUI.cs` that:
1. Reads `GachaSystem.Instance.ArchiveTokens`
2. Displays shop items + costs
3. On purchase, calls:
   ```csharp
   if (GachaSystem.Instance.SpendArchiveTokens(cost))
     GivePlayerItem(item);
   ```

---

## State Management

### GachaSystem Persistent State

Owned by: `GachaSystem` (via `LoadSummonPersistentState()`)

Fields:
- `featuredPityCounter` — Pulls since last featured Tier 5
- `prismShards` — Accumulated from duplicates
- `archiveTokens` — Earned from spark conversion + events
- `bannerSparkPoints` — Per-banner spark tracking

### Transient State

Reset on app restart (not persisted):
- `pityCounters[GachaRarity]` — Per-rarity pity counters (legacy system)
- Current banner pool

### Save/Load Cycle

```
Load Profile:
  SaveGameManager.LoadFromJson()
    → ApplySaveData()
      → GachaSystem.LoadSummonPersistentState(...)
        → Restore all counters

Save Profile:
  SaveGameManager.SaveGame()
    → BuildSaveData()
      → Capture GachaSystem.FeaturedPityCounter, ArchiveTokens, etc.
    → Persist JSON
```

---

## Testing Checklist

- [ ] Pull 10 units, verify Tier 4+ guarantee
- [ ] Pull to soft pity (pull 61+), verify rate increase
- [ ] Pull to hard pity (pull 80), verify guaranteed Tier 5
- [ ] Pull duplicate unit, verify shard payout
- [ ] Accumulate 160 spark, verify claim button enabled
- [ ] Claim featured unit via spark, verify registered to inventory
- [ ] Rotate banners, verify spark converts to archive tokens
- [ ] Hard pity on featured banner, verify 50/50 featured vs standard
- [ ] Toggle `carry_pity_across_featured` false, verify pity resets on new banner
- [ ] Save game, close app, reload, verify all counters persist

---

## Performance Notes

- **Pull simulation (1000 pulls):** <1ms (no allocation per pull)
- **JSON banner parsing:** <5ms on first load (cached thereafter)
- **Save data size:** ~2KB per counter + spark history

---

## Future Extensions

1. **Beginner Banner** — First 50 pulls rate-up on starter unit (reset per alternate account)
2. **Weapon Gacha** — Separate pity counter for equipment pulls
3. **Lost Prayer Counter** — 50/50 tracking (if you won 50/50 last pull, next hard pity is guaranteed featured)
4. **Seasonal Rate-Ups** — Multiple featured units on one banner with rotating schedules
5. **Batch Spark Purchase** — "Buy 10 spark at 50 crystals each" option
6. **Archive Token Milestone** — Earn archive tokens from limited-time boss challenges

---

*Last Updated: April 4, 2026*
*Maintained by: Development Team*
