# Oathsworn: Compact - Gacha Guide (Developer v2)

Last updated: 2026-04-08

## Scope
This guide documents the active gacha runtime, banner data schema, attunement UI integration, and reveal sequence pipeline.

## Key Runtime Files
- Assets/Scripts/GachaSystem.cs
- Assets/Scripts/SummonInputHandler.cs
- Assets/Scripts/SummonRevealSequencer.cs
- Assets/Scripts/PrismWidget.cs
- Assets/Scripts/AttunementFlowController.cs
- Assets/StreamingAssets/gacha_banners.json

## Event and Flow
1. SummonInputHandler.PerformSummon triggers pull.
2. GachaSystem.PerformPull resolves rates, pity, featured bias, spark, and dupe shards.
3. GachaSystem.OnPullCompleted publishes pulled items.
4. SummonRevealSequencer receives event and runs staged reveal.
5. AttunementFlowController hides attunement root on pull complete and restores it on close.

## Banner Text API (Attunement UI)
GachaSystem now exposes:
- GetActiveBannerName()
- GetActiveBannerDescription()
- GetActiveBannerFeaturedUnitNames()
- GetActiveBannerFeaturedUnitsSummary()
- GetActiveBannerRateSummary()

SummonInputHandler supports optional text bindings:
- bannerNameText
- bannerDescriptionText
- featuredUnitsText
- bannerRateText

## Reveal Sequence Runtime
SummonRevealSequencer phases:
1. Intro overlay fade and optional intro animation.
2. Featured prism reveal with tap gates.
3. Grid reveal for remaining units.
4. Close button and sequence teardown.

Input handling uses UnityEngine.InputSystem mouse/touch checks to avoid legacy-input runtime errors.

## Prism Widget Contract
PrismWidget responsibilities:
- SetData(item)
- AnimateIn()
- Crack()
- Apply reveal data to portrait/name/rarity labels

Inspector requirements per widget prefab:
- Reveal Root
- Unit Portrait Image
- Unit Name Text
- Rarity Text

Recent safety additions:
- display-name fallbacks
- rarity-label fallbacks
- warning logs for missing reveal/text bindings

## Banner Schema Notes
Banner fields used by runtime:
- id, name, description, enabled, banner_type, token_cost
- featured_rarity_tier, featured_unit_ids, featured_rate
- soft_pity_start, hard_pity, soft_pity_step_percent
- carry_pity_across_featured, spark_threshold, spark_to_archive_token_rate
- guaranteed_min_rarity_on_ten_pull
- rarity_rates, dupe_shard_rules, weekly_dungeon_focus, unit_ids

weekly_dungeon_focus expected structure:
- week
- dungeon_id
- dungeon_name
- recommended_level
- note

## Validation Checklist
- GachaSystem exists in scene and is initialized.
- SummonInputHandler button OnClick points to PerformSummon.
- SummonRevealSequencer is active and subscribed to OnPullCompleted.
- Grid and featured PrismWidget bindings are valid.
- AttunementFlowController references are assigned.
- Banner text fields update when SetActiveBanner is called.

## Common Failure Points
- No GachaSystem in scene.
- Button OnClick not wired.
- Reveal prefab missing Unit Name Text or Rarity Text.
- Legacy input APIs used while project runs Input System only.
