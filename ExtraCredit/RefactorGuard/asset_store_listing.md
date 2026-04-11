# RefactorGuard Asset Store Listing Draft

## Product name
RefactorGuard - Asset Impact Analyzer

## Short description (max-style summary)
See exactly what breaks before you rename, move, or delete any asset in Unity.

## Full description
RefactorGuard is an Editor tool for Unity that answers the most expensive refactor question: what will break if this asset changes?

RefactorGuard scans project dependencies, builds a reverse-reference map, and gives you instant impact analysis for any asset. It highlights incoming references (what depends on the asset), outgoing references (what the asset depends on), and an immediate Safe or Caution status.

For larger audits, RefactorGuard can analyze current selection or entire folders, assign High, Medium, and Safe risk tiers, and export JSON or CSV reports for review workflows.

### Core capabilities
- Single-asset impact analysis with Safe or Caution status
- Incoming and outgoing dependency lists with Open and Ping actions
- Batch analysis for multi-selection and folder scope
- Tiered risk model: High, Medium, Safe
- Structured filters by free text, type, folder, and risk toggles
- One-click audit presets for common review modes
- Actionable dashboard cards for top risk asset, dangerous folder, and risky asset type
- Export options: JSON and CSV for both single and batch reports

### Who this is for
- Solo developers cleaning project debt before release
- Technical artists reorganizing textures, materials, and shader graphs
- Team leads reviewing high-risk content changes before merge
- Any Unity developer who has been burned by missing references after refactor

### Why teams buy it
- Reduce accidental breakage during refactors
- Prioritize risky assets quickly across large folders
- Attach concrete impact reports to pull requests and release checklists
- Make asset cleanup safer and faster

## Key technical details
- Unity 2021 LTS or newer
- Editor-only tool
- No runtime dependencies
- No external services required
- Writes report files only to Assets/RefactorGuardData

## Suggested pricing
Launch price: 19.99 USD
Standard price after launch window: 24.99 USD

## Search keywords
Unity refactor tool, Unity dependency analyzer, Unity asset impact, Unity editor utility, Unity missing reference prevention, Unity technical art tools, Unity project cleanup

## Support blurb
If you find edge cases in your project setup, include a sample path and expected behavior in support requests so we can reproduce quickly.
