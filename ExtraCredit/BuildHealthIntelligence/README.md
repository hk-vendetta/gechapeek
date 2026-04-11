# Build Health Intelligence — User Guide

This guide walks you through installing the tool, using it for the first time, and understanding everything it shows you. No prior experience with Unity Editor tools is needed.

---

## Table of Contents

1. [What this tool does](#1-what-this-tool-does)
2. [Requirements](#2-requirements)
3. [Installation](#3-installation)
4. [Opening the dashboard](#4-opening-the-dashboard)
5. [Your first snapshot](#5-your-first-snapshot)
6. [Saving a baseline](#6-saving-a-baseline)
7. [Reading the drift report](#7-reading-the-drift-report)
8. [Understanding every metric](#8-understanding-every-metric)
9. [Where your data is saved](#9-where-your-data-is-saved)
10. [Recommended workflow](#10-recommended-workflow)
11. [Troubleshooting](#11-troubleshooting)
12. [File overview](#12-file-overview)

---

## 1. What this tool does

Build Health Intelligence is a Unity Editor-only tool (it does not exist in your game at runtime). It scans your project and records twelve key metrics about its size and structure. You can then:

- **Capture** a snapshot of the current state at any time
- **Save** one snapshot as a "baseline" (a reference point — think of it as your "before" state)
- **Compare** any new snapshot against that baseline to see exactly what changed and by how much

This is useful for catching things like texture counts quietly doubling, dependency sizes growing unexpectedly between milestones, or shader files multiplying from third-party imports.

---

## 2. Requirements

- Unity 2021.3 or later (any render pipeline: Built-In, URP, HDRP)
- The four script files placed inside a folder that Unity compiles — specifically anywhere under `Assets/`

> **Important:** The scripts must be inside your `Assets/` folder for Unity to recognize and compile them. If they are stored outside `Assets/` (for example in an `ExtraCredit/` folder at the project root), Unity will not see them and the tool will not appear. See [Installation](#3-installation) below.

---

## 3. Installation

### Step 1 — Copy the scripts into Assets

Create a folder for the tool inside your project's Assets folder. A recommended location:

```
Assets/
  Editor/
    BuildHealthIntelligence/
      BuildHealthAnalyzer.cs
      BuildHealthSnapshot.cs
      BuildHealthStorage.cs
      BuildHealthWindow.cs
```

> The folder **must** be named `Editor` or be inside a folder named `Editor`. Unity only compiles Editor-only tools from folders with this name. If you skip this, you will get compile errors.

### Step 2 — Return to the Unity Editor

Unity will detect the new scripts and recompile automatically. Wait for the progress bar in the bottom-right corner to finish. This usually takes 5–30 seconds.

### Step 3 — Confirm no errors

Look at the **Console** window (Window > General > Console). If there are no red errors related to `BuildHealth`, the tool is installed correctly.

---

## 4. Opening the dashboard

In the Unity menu bar at the top of the screen:

```
Tools > Build Health > Build Health Dashboard
```

A window will open titled **Build Health**. You can dock it anywhere in your Unity layout, or leave it floating.

If you do not see the `Tools > Build Health` menu, Unity has not compiled the scripts yet. Check the Console for errors, confirm the scripts are inside an `Editor` folder under `Assets/`, and try restarting Unity.

---

## 5. Your first snapshot

When the dashboard opens, both panels will say **"No data captured."** — this is normal. Nothing has been measured yet.

**Click "Capture Current Snapshot."**

The tool will scan your project and fill in the **Current Snapshot** panel with your project's current numbers. This takes 1–10 seconds depending on project size.

The snapshot is also automatically saved to disk so it persists if you close Unity. You do not need to do anything extra to save it.

---

## 6. Saving a baseline

A baseline is your **reference point**. It represents the state of your project at a known, stable moment — for example, at the start of a sprint, after a release, or right now before you start a large import.

Once you have a Current Snapshot:

**Click "Save As Baseline."**

The **Baseline** panel will fill in with the same numbers. From this point on, every future snapshot you capture will be compared against this baseline in the **Drift vs Baseline** section at the bottom.

> **Tip:** Set your baseline at meaningful moments — before merging a large feature branch, before importing a new asset pack, or at the start of each week. This makes drift meaningful rather than noise.

---

## 7. Reading the drift report

The **Drift vs Baseline** section at the bottom of the dashboard appears once both a baseline and a current snapshot exist.

Each row shows:
- The metric name
- The signed change (e.g. `+14` or `-3`)
- The percentage change relative to the baseline (e.g. `+5.23%`)

**Color coding:**
- **Orange** — the metric increased. This flags potential growth to review.
- **Green** — the metric decreased. Generally a good sign (less bloat).
- **White/neutral** — no change from baseline.

> Growth is not always bad. Adding new scenes and textures is expected when building a game. The colors are attention flags, not pass/fail grades. Use your judgment about whether a change was intentional.

---

## 8. Understanding every metric

### Total Assets
The total number of files inside your `Assets/` folder recognized by Unity (textures, scripts, scenes, prefabs, audio, etc.). Useful for tracking overall project growth.

### Total Scripts
The number of `.cs` C# script files in the project. A sudden jump here might mean an import brought in a large number of scripts you were not expecting.

### Scenes In Build
How many scenes are registered in **File > Build Settings**. This includes both enabled and disabled scenes.

### Enabled Scenes
How many of those scenes are switched on (checked) in Build Settings. Only enabled scenes will be included in a real build.

### Dependency Asset Count
This is one of the most important metrics. It counts every unique asset that your **enabled scenes** depend on — anything a scene references directly or indirectly (a prefab, which references a material, which references a texture, and so on). This reflects what would actually ship in a build.

### Dependency Estimated Size
The total uncompressed file size of all the assets counted above. This is measured from the raw source files on disk, **not** the final compressed build size (which Unity calculates separately at build time). Use it as a relative indicator of growth, not an absolute build size.

### Texture Assets
The total number of texture files in the project (`t:Texture`). Textures are one of the most common sources of project bloat.

### Material Assets
The total number of material files in the project (`t:Material`).

### Shader Assets
The total number of shader files in the project (`t:Shader`). Shader counts can grow quickly when importing rendering-related packages. A high shader count can also increase build times significantly.

---

## 9. Where your data is saved

When you capture a snapshot or save a baseline, the tool writes JSON files to:

```
Assets/
  BuildHealthData/
    build_health_latest.json    ← most recent captured snapshot
    build_health_baseline.json  ← your saved baseline
```

These files are plain text and human-readable. You can open them in any text editor to see the raw numbers. You can also commit them to version control (Git) to keep a history of project health across your team.

> **Note:** These files are created inside `Assets/` so Unity can track them. You can safely ignore them in your game — they are never compiled or included in a build.

---

## 10. Recommended workflow

Here is a simple routine to get the most out of the tool:

**At the start of a new milestone or sprint:**
1. Open the dashboard
2. Click **Capture Current Snapshot**
3. Click **Save As Baseline**
4. Commit `build_health_baseline.json` to Git

**After completing a significant feature or importing assets:**
1. Open the dashboard
2. Click **Capture Current Snapshot**
3. Review the **Drift vs Baseline** section
4. Investigate any numbers that grew unexpectedly

**Common things to watch for:**
- Shader count jumping after importing a new package (shader variant bloat)
- Dependency asset count rising even when you did not add scenes (means assets are being referenced that you may not need)
- Texture count growing faster than expected (check for duplicate or test imports)

---

## 11. Troubleshooting

**"The menu Tools > Build Health does not appear"**  
The scripts are not inside an `Editor` folder under `Assets/`. Move all four `.cs` files to a path like `Assets/Editor/BuildHealthIntelligence/` and wait for Unity to recompile.

**"There are red compile errors in the Console"**  
Check that all four files are present. If any are missing, the remaining files will error because they reference each other. Re-copy any missing file.

**"Capture Current Snapshot takes a very long time"**  
This is normal on large projects. The tool scans every asset path and resolves dependencies for each enabled scene. For very large projects, consider disabling some scenes in Build Settings before scanning.

**"Dependency Estimated Size shows 0 B"**  
This means no scenes are enabled in Build Settings (File > Build Settings). The dependency scan only runs on enabled scenes. Enable at least one scene and re-capture.

**"The Drift vs Baseline section says 'Capture current snapshot and baseline to compare'"**  
You need both a current snapshot AND a baseline before the diff appears. Capture a snapshot first, then click **Save As Baseline**, then capture again (or click **Reload Saved Data** if you already have saved data).

**"I want to reset my baseline"**  
Simply capture a new snapshot reflecting the state you want to use as the new reference, then click **Save As Baseline** again. The old baseline is overwritten.

---

## 12. File overview

| File | Purpose |
|---|---|
| `BuildHealthSnapshot.cs` | Data class. Holds the twelve metric fields for one point-in-time snapshot. |
| `BuildHealthAnalyzer.cs` | Logic class. Performs the actual scan of the project using Unity's AssetDatabase API. |
| `BuildHealthStorage.cs` | Storage class. Handles reading and writing snapshot JSON files to `Assets/BuildHealthData/`. |
| `BuildHealthWindow.cs` | UI class. The Editor window with buttons and the three display panels (Current, Baseline, Drift). |
| `description.md` | Short user-facing product description. |
| `README.md` | This file. |
