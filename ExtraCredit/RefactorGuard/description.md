# RefactorGuard — Asset Impact Analyzer

**Stop breaking your project. Know what changes before you make them.**

RefactorGuard is an Editor window for Unity that answers the single most dangerous question in any mid-to-large project:

> *"If I rename, move, or delete this asset — what breaks?"*

Unity has no built-in answer to this question. Without RefactorGuard, developers are left guessing, searching, or learning the hard way with a cascade of missing references, broken prefabs, and pink materials at runtime.

---

## What it does

RefactorGuard scans every asset dependency in your project and builds a complete reverse-reference map. You pick any asset in your project. It instantly shows you:

- Every asset that **references** the selected asset (incoming references — the things that will break)
- Every asset the selected asset **depends on** (outgoing references — the things it needs)
- A clear **SAFE / CAUTION** indicator telling you whether removing or renaming the asset is safe right now

From there you can **Ping** any asset in the list to jump directly to it in the Project window. When you're done, you can **export the full impact report to JSON** for code review, refactor planning, or team documentation.

---

## Key features

| Feature | Description |
|---|---|
| Full project scan | Indexes all Assets using `AssetDatabase.GetDependencies` |
| Reverse reference map | Per-asset: who references it, and who it references |
| SAFE / CAUTION indicator | Instant color-coded answer to "is it safe to remove this?" |
| Scan progress feedback | Progress bar during large project scans so users know the tool is working |
| Project-selection shortcut | Pull the active Project window selection directly into the analyzer |
| Batch selection analysis | Inspect a whole Project selection and rank assets by incoming reference risk |
| Folder-wide analysis | Scan every asset inside a folder to spot high-impact refactor targets fast |
| High / Medium / Safe tiers | Group batch results into severity bands so teams can prioritize first |
| Structured batch filters | Narrow audits by risk tier, asset type, folder, or free-text path matching |
| Batch dashboard cards | Surface the top-risk asset, most dangerous folder, and highest-risk asset type instantly |
| One-click audit presets | Jump straight to common review modes like High Risk Only or Safe Cleanup |
| Dashboard drilldown actions | Jump from summary cards into folder, type, or asset-specific investigation |
| Incoming reference count | Exactly how many things will break if the asset disappears |
| Outgoing reference count | How many assets this one depends on |
| Filterable result lists | Narrow incoming and outgoing paths by filename, folder, or keyword |
| Open + Ping navigation | Open or highlight any referenced asset directly from the report |
| Batch JSON export | Save a multi-asset risk summary for PRs, cleanup passes, or refactor planning |
| CSV export | Push single-asset and batch results directly into spreadsheets and release checklists |
| JSON export | Save a full impact report for documentation or review |
| In-memory cache | Scan once, query instantly as many assets as you want |

---

## Who is it for

- **Solo developers** cleaning up and restructuring projects before shipping
- **Team leads** who need to communicate the impact of a rename before committing it
- **Technical artists** reorganizing texture atlases, materials, and shader graphs
- **Any Unity developer** who has ever seen the pink "missing reference" material after a refactor

---

## What it does NOT do

RefactorGuard reports references — it does not perform renames or deletions. It is a read-only analysis tool. All actual changes are still made by you, through normal Unity workflows.

---

## Assets created by this tool

RefactorGuard only writes files to `Assets/RefactorGuardData/` when you use the **Export** button. It creates no prefabs, no ScriptableObjects, and no runtime files. The `Assets/RefactorGuardData/` folder may be excluded from version control via `.gitignore` as it contains only local analysis artifacts.

---

## Suggested retail price: $24.99
