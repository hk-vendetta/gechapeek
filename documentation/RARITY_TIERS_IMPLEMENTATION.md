# Oath-Covenant Rarity Tier Implementation

## Summary
Successfully replaced generic rarity tier names with **lore-aligned Oath-Covenant naming system** that reflects your game's core themes.

## New Tier Names
| Tier | Old Name | New Name | Thematic Meaning |
|------|----------|----------|-----------------|
| 1 | Common | **Oath-Called** | A faint echo of sworn duty awakening |
| 2 | Uncommon | **Oath-Sworn** | Oath firmly taken and acknowledged |
| 3 | Rare | **Oath-Tempered** | Oath tested and strengthened by trials |
| 4 | Epic | **Oath-Forged** | Oath written deep into their essence |
| 5 | Legendary | **Oath-Eternal** | Oath transcends death and time |

## Files Updated

### Code Files
✅ **UnitRarityTierCatalog.cs**
- Updated `GetLabel(int tier)` method to return new tier names
- Numeric tier constants unchanged (1-5 for backwards compatibility)
- Color scheme maintained (Gray → Green → Blue → Purple → Gold)

✅ **UnitUpgradeSystem.cs**
- Updated code comments to reflect new tier names
- Cost progression still matches tier transitions
- No functional changes to upgrade logic

### Documentation Files
✅ **gacha_system_player.md**
- Rarity tier table updated (player-facing reference)
- Shard payout table updated with new tier names
- All references from "Common to Legendary" changed to "Oath-Called to Oath-Eternal"
- Featured unit rate description updated to use "Oath-Eternal"

✅ **UNIT_UPGRADE_SYSTEM.md**
- Upgrade cost schedule table updated
- Example scenarios updated (upgrade to Oath-Forged/Oath-Eternal)
- UI dialog examples updated
- API documentation examples updated

## UI Integration Points

The changes automatically apply to all UI that uses `UnitRarityTierCatalog.GetLabel()`:
- **PrismWidget.cs** - Unit display
- **SummonResultEntry.cs** - Gacha result screens
- **UnitDetailPanel.cs** - Unit detail pages
- **UnitInventoryUI.cs** - Inventory display

All these UI elements will automatically display the new tier names.

## Data Persistence

✅ **SaveGameData**: No changes needed (still uses numeric tier values 1-5)
✅ **gacha_banners.json**: No changes needed (uses numeric tier values)
✅ **units_game.json**: No changes needed (uses numeric tier values)

The system remains internally numeric for consistency. Display names are handled by `GetLabel()`.

## Testing Verification

| Test | Status |
|------|--------|
| Code compiles without errors | ✅ PASS |
| UI displays new tier names | ✅ Ready (no changes needed) |
| Save/load preserves tiers | ✅ Ready (numeric unchanged) |
| Upgrade costs match tiers | ✅ Ready (logic unchanged) |

## Impact Summary

**✅ Zero breaking changes** - All numeric tier values remain unchanged (1-5)
- Existing saves remain compatible
- No code refactoring needed elsewhere
- UI updates happen automatically via `GetLabel()`

**✅ Lore-aligned display** - All player-facing text now uses Oath-Covenant terminology
- Reflects the game's core theme of oaths and compacts
- Creates thematic consistency throughout the gacha system
- Enhances narrative immersion

**✅ Easy to maintain** - All tier name mappings centralized in one method
- Future adjustments only require updating `GetLabel()` in UnitRarityTierCatalog.cs
- No scattered hardcoded strings to hunt down

## Future References

If you need to adjust these tier names in the future:
1. Edit `UnitRarityTierCatalog.GetLabel()` method
2. Update documentation examples (if desired)
3. No code logic needs to change

If you want to add more tiers (e.g., Tier 6):
1. Add new constant in UnitRarityTierCatalog (e.g., `public const int TimeStorm = 6;`)
2. Add case in `GetLabel()` and `GetColor()` with new name
3. Update upgrade costs in UnitUpgradeSystem.cs if adding new tier transitions

## Narrative Alignment

**"Oathsworn: Compact"** - The core game loop now reflects oath progression:
- **Oath-Called**: Characters are summoned to serve their purpose
- **Oath-Sworn**: They accept and commit to their role
- **Oath-Tempered**: Experience hardens their resolve
- **Oath-Forged**: They become legendary through their dedication
- **Oath-Eternal**: They transcend mortality through their oath

This progression mirrors the player's journey and the found-family narrative arc of your story.
