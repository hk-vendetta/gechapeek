# RefactorGuard — Developer Guide

## Table of contents

1. [What it does](#1-what-it-does)
2. [Requirements](#2-requirements)
3. [Installation](#3-installation)
4. [Opening the window](#4-opening-the-window)
5. [Running your first scan](#5-running-your-first-scan)
6. [Selecting an asset to analyze](#6-selecting-an-asset-to-analyze)
7. [Reading the impact report](#7-reading-the-impact-report)
8. [Understanding SAFE vs CAUTION](#8-understanding-safe-vs-caution)
9. [Using Ping to navigate](#9-using-ping-to-navigate)
10. [Exporting a report to JSON](#10-exporting-a-report-to-json)
11. [Recommended workflow](#11-recommended-workflow)
12. [Troubleshooting](#12-troubleshooting)
13. [File overview](#13-file-overview)

---

## 1. What it does

RefactorGuard builds a complete reverse-reference map of your Unity project. For any asset you select, it tells you:

- **Who references it** — the assets that will develop missing references if you remove or rename the selected asset
- **What it depends on** — the assets the selected asset requires to function
- **Whether it is safe to remove** — a single color-coded indicator based on the incoming reference count

The scan is performed once per session. After the scan, every query is instant — no re-scanning is needed when you switch between assets.

---

## 2. Requirements

- Unity 2021 LTS or later
- No additional packages or dependencies required
- Editor-only tool — does not affect builds or runtime

---

## 3. Installation

1. Copy all four `.cs` files into any folder **inside** `Assets/Editor/` in your project. For example:

```
Assets/
  Editor/
    RefactorGuard/
      AssetImpactReport.cs
      DependencyScanner.cs
      RefactorGuardStorage.cs
      RefactorGuardWindow.cs
```

2. Unity will compile the scripts automatically. Check the Console for any errors before proceeding.

> **Important:** The scripts must be placed inside a folder named `Editor` (or any subfolder of it). Scripts placed outside an `Editor` folder will be included in your game build and will fail to compile because `UnityEditor` APIs are not available at runtime.

---

## 4. Opening the window

From the Unity menu bar:

```
Tools > RefactorGuard > Impact Analyzer
```

The window is resizable. Minimum size is 540 × 520 pixels. It can be docked alongside the Inspector or floated as a standalone window.

---

## 5. Running your first scan

When you first open the window, the status reads:

```
Project not yet scanned. Click "Scan Project" to begin.
```

Click **Scan Project**. RefactorGuard will:

1. Retrieve all asset paths using `AssetDatabase.GetAllAssetPaths()`
2. For each asset inside `Assets/`, call `AssetDatabase.GetDependencies(path, false)` to get its direct dependencies
3. Build a reverse map: for each dependency found, record which assets reference it
4. Cache the complete map in memory for the duration of the Editor session

During the scan, RefactorGuard shows a progress bar so large projects do not feel stalled while the reverse map is being built.

When the scan finishes, the status updates to:

```
Scan complete — 1,432 assets indexed. Last scanned: 14:23:07
```

**Scan time** scales with project size. A project with 500 assets typically scans in under 3 seconds. A project with 5,000+ assets may take 10–30 seconds.

The scan result is held in memory. Closing and reopening the window will discard the cached result. If you add or delete assets mid-session, click **Scan Project** again to rebuild the map.

---

## 6. Selecting an asset to analyze

After scanning, Step 2 becomes active.

Use the **Asset** object picker to select any asset in your project: a prefab, texture, material, audio clip, scene, script, ScriptableObject, or any other tracked asset.

You can also drag an asset from the Project window and drop it directly onto the object field.

If the asset is already highlighted in the Project window, click **Use Current Project Selection** to pull it into the analyzer instantly. Use **Clear** to reset the current selection without rescanning.

When you change the selected asset, the impact report regenerates instantly using the cached map. You can switch between assets freely without re-scanning.

---

## 7. Reading the impact report

The report has four parts:

### Safety indicator

The first line is color-coded:

- **Green — SAFE:** No other asset references this one. You can rename, move, or delete it without breaking anything.
- **Orange — CAUTION:** One or more assets reference this one. Those assets will develop missing references if this asset is removed or renamed.

### Metadata rows

| Field | Description |
|---|---|
| Asset Path | Project-relative path to the selected asset |
| GUID | Unity's internal identifier for this asset |
| Type | Unity asset type (Texture2D, GameObject, AudioClip, etc.) |
| Referenced By (incoming) | Count of assets that reference this one |
| References (outgoing) | Count of assets this one depends on |

### Referenced By list

The scrollable list under **"Assets that reference this"** shows every asset that holds a reference to the selected asset. These are the assets that will break if you remove or rename the selected asset.

Use **Filter Incoming** to narrow the list by folder, asset name, or any substring in the path.

Each entry has:

- **Open** — opens the asset directly (useful for prefabs, scenes, materials, and scripts)
- **Ping** — highlights the asset in the Project window

### References list

The scrollable list under **"Assets this depends on"** shows every asset the selected asset directly references. This is the selected asset's outgoing dependency set.

Use **Filter Outgoing** to narrow the list the same way.

Each entry also has **Open** and **Ping** actions.

---

## 8. Understanding SAFE vs CAUTION

**SAFE** does not mean the asset is unused in the scene. It means no other asset file in the project holds a serialized reference to it via Unity's dependency tracking.

A texture that is placed in a scene by direct drag-and-drop (not via a material or prefab file) may appear SAFE even though it is visible in the scene. This is expected — scene-level references are reflected in the scene file's own dependency list. To check a scene file, select the `.unity` file in the Project window and analyze it.

**CAUTION** count reflects direct references only (non-recursive). An asset that appears in the CAUTION list with 15 references has 15 assets directly referencing it. Those 15 assets may in turn be referenced by many more assets, but that cascading chain is not shown in the report.

---

## 9. Using Open and Ping to navigate

Every entry in the Referenced By and References lists has both an **Open** button and a **Ping** button.

Clicking **Open** calls `AssetDatabase.OpenAsset`, which opens the asset directly when Unity supports it.

Clicking **Ping** calls `EditorGUIUtility.PingObject` on the asset at that path, which highlights and scrolls to it in the Project window.

This is particularly useful when:

- You want to open a prefab that references your asset to inspect the reference
- You need to identify which scene file is holding a reference before renaming a shader
- You are walking a dependency chain manually and want to follow it step by step

The report header also includes **Ping Selected Asset** and **Open Selected Asset** buttons for the asset currently being analyzed.

---

## 10. Exporting reports

RefactorGuard supports both **JSON** and **CSV** export for single-asset and batch analysis.

### Single-asset export

Use the buttons at the bottom of the single-asset report:

- **Export JSON**
- **Export CSV**

RefactorGuard will:
1. Create `Assets/RefactorGuardData/` if it does not exist
2. Write either `impact_{assetName}_{timestamp}.json` or `impact_{assetName}_{timestamp}.csv`
3. Refresh the AssetDatabase so the file appears in the Project window
4. Open Finder / File Explorer to the saved file location

The JSON file contains all fields from the `AssetImpactReport` class:
- `generatedAtUtc` — timestamp in ISO 8601 format
- `targetAssetPath` — path of the analyzed asset
- `targetAssetGuid` — GUID of the analyzed asset
- `targetAssetType` — Unity type name
- `isSafeToRemove` — boolean
- `directReferenceCount` — number of incoming references
- `outgoingReferenceCount` — number of outgoing references
- `referencedByPaths` — full list of inbound paths
- `referencesPaths` — full list of outbound paths

The CSV file exports the same report in a spreadsheet-friendly shape, with inbound and outbound path lists collapsed into pipe-separated text columns.

These JSON files are useful for attaching to pull requests, sprint reviews, or team documentation to communicate the impact of a planned change.

The `Assets/RefactorGuardData/` folder is safe to exclude from version control. Add it to your `.gitignore`:

```
Assets/RefactorGuardData/
Assets/RefactorGuardData.meta
```

### Batch analysis and export

RefactorGuard also supports **batch analysis** for multi-selection and folder review.

In the **Batch Analysis** section you can:

- Click **Analyze Current Selection** to analyze every asset currently selected in the Project window
- Choose a folder and click **Analyze Folder** to analyze every asset inside that folder

The batch summary shows:

- total analyzed asset count
- how many assets are currently `SAFE`
- how many assets are `MEDIUM RISK`
- how many assets are `HIGH RISK`
- how many are `CAUTION`
- total incoming references across the whole set

The batch view also includes a small **Dashboard** section that summarizes the current visible result set:

- `Top Risk Asset` — the single highest-priority asset after filters and tier sorting
- `Most Dangerous Folder` — the folder contributing the most incoming references
- `Highest-Risk Asset Type` — the asset type with the largest incoming-reference footprint

The dashboard cards are actionable:

- `Top Risk Asset` includes `Analyze` and `Open` buttons
- `Most Dangerous Folder` includes `Use Folder` to push that folder into the active folder filter
- `Highest-Risk Asset Type` includes `Use Type` to push that asset type into the active type filter

Rows are grouped by severity tier and sorted by incoming reference count, so the riskiest assets rise to the top first.

You can further narrow the batch view with structured filters:

- `Filter Batch Rows` for free-text matching across path, risk tier, and asset type
- `Asset Type Filter` to narrow the list to types like `Material`, `Shader`, or `GameObject`, including comma-separated filters
- `Folder Filter` to focus on a specific subtree such as `Assets/Art/Characters`, including comma-separated filters
- risk-tier toggles for `High`, `Medium`, and `Safe`

Use `Reset Filters` to clear every batch filter at once.

The batch toolbar also includes one-click audit presets:

- `High Risk Only`
- `Safe Cleanup`
- `Materials & Shaders`
- `Prefabs & Scenes`

These presets reset the current batch filters and jump directly to common review workflows.

### Batch severity tiers

RefactorGuard assigns each batch row one of three tiers:

- `SAFE` — zero incoming references
- `MEDIUM` — at least one incoming reference, but below the high-risk threshold
- `HIGH` — 10 or more incoming references, or 3+ incoming references on high-impact asset types such as scenes, prefabs, materials, and shaders

This gives teams a fast triage layer before drilling into individual assets.

Each row includes:

- **Analyze** — opens that asset as the active single-asset report
- **Open** — opens the asset directly
- **Ping** — highlights it in the Project window

Use **Export Batch JSON** or **Export Batch CSV** to save the batch summary.

Batch CSV exports include:

- one summary row with aggregate totals
- one row per analyzed asset with `assetPath`, `assetType`, `riskTier`, `isSafeToRemove`, `directReferenceCount`, and `outgoingReferenceCount`

That makes the output easy to sort, filter, and annotate in Excel, Google Sheets, or release-review docs.

---

## 11. Recommended workflow

**Before renaming a texture atlas:**
1. Scan the project
2. Select the texture in the asset picker
3. Check the CAUTION count and review the Referenced By list
4. Update each referencing material or prefab if needed, or perform the rename using Unity's built-in rename (which updates GUIDs automatically in most cases)

**Before deleting an old script:**
1. Scan the project
2. Select the `.cs` file in the asset picker
3. If CAUTION shows 0, the script has no serialized references — safe to delete
4. If CAUTION is non-zero, Ping each referencing file to remove the component reference first

**Before submitting a PR with asset changes:**
1. Scan the project
2. Select the folder or set of assets touched by the PR
3. Run batch analysis to identify the highest-risk assets first
4. Export a batch report for team review
5. Open individual high-risk assets with **Analyze** for deeper inspection
6. Attach the JSON files to the PR for team review

**Periodic project audit:**
1. Scan the project
2. Step through unused-looking assets one by one
3. Any asset with SAFE status and an obvious naming artifact (e.g., `_OLD`, `_BACKUP`, `_v2`) is a candidate for deletion

---

## 12. Troubleshooting

**"Scan Project" button produces no status update.**
This can happen if Unity's compilation is in progress. Wait for the spinning progress indicator in the bottom-right of the Editor to finish, then click Scan again.

**Asset picker does not activate.**
The asset picker requires a completed scan. Click Scan Project first.

**An asset shows SAFE but I can see it used in a scene.**
The asset may be referenced only by the scene file. Select the `.unity` scene file in the Project window and analyze it to see its full dependency list.

**Scan takes more than 30 seconds.**
Large projects (5,000+ assets) benefit from periodic asset cleanup before scanning. You can also reduce scan time by removing unused third-party packages that add thousands of asset entries.

**Ping does not work for a specific entry.**
The asset at that path may have been deleted or moved after the last scan. Click Scan Project again to rebuild the map.

**Open does nothing for a specific entry.**
Some Unity asset types do not have a default editor action, or the asset may have moved since the last scan. Re-scan the project and try again.

**Export creates the JSON file but it does not appear in the Project window.**
Click the Refresh button in the Project window, or press Ctrl+R. Unity sometimes delays AssetDatabase.Refresh calls during active GUI repaints.

---

## 13. File overview

| File | Role |
|---|---|
| `AssetImpactReport.cs` | Serializable data class for one asset's impact analysis |
| `DependencyScanner.cs` | Builds the reverse-reference map; generates impact reports |
| `RefactorGuardStorage.cs` | Exports reports as JSON files into `Assets/RefactorGuardData/` |
| `RefactorGuardWindow.cs` | Editor window UI: scan controls, asset picker, report display |
