# Oathsworn: Compact - Summon and Attunement Setup Guide (v2)

Last updated: 2026-04-08

## Goal
Build the full scene flow:
Attunement screen -> Summon -> Reveal sequence -> Close -> Return to attunement screen.

## Scene Objects
Required components in scene:
- GachaSystem
- SummonInputHandler
- SummonRevealSequencer
- AttunementFlowController

## Canvas Layout
1. AttunementCanvas (Sort Order 0)
   - AttunementRoot
2. RevealCanvas (Sort Order 100)
   - SummonRevealRoot

## SummonRevealRoot Wiring
SummonRevealSequencer fields:
- overlayCanvasGroup
- featuredPanel
- featuredPrism
- featuredTapHintText
- gridPanel
- prismGrid
- prismWidgetPrefab
- closeButton
- optional: legacyResultUI

## PrismWidget Requirements
For both featured widget and grid prefab:
- Reveal Root
- Unit Portrait Image
- Unit Name Text
- Rarity Text

## AttunementFlowController Wiring
- attunementRoot -> AttunementRoot
- revealSequencer -> SummonRevealRoot component
- gachaSystem -> optional (auto-find supported)

## Summon Button Wiring
Button OnClick should call:
- SummonInputHandler.PerformSummon()
or
- SummonInputHandler.PerformSingleSummon()

Avoid direct show/hide UI calls from button OnClick.

## Banner Text Wiring (Optional)
SummonInputHandler text fields:
- summonCostText
- summonButtonText
- bannerNameText
- bannerDescriptionText
- featuredUnitsText
- bannerRateText

## Common Debugs
- If summon does nothing: verify GachaSystem exists and button OnClick is assigned.
- If reveal runs but labels are empty: check PrismWidget text/reveal bindings.
- If input exception appears: verify Input System package mode and no legacy Input calls.
