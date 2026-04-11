# RefactorGuard Demo Capture Runbook

Demo project:
- Unity Open Project 1: Chop Chop
- Repository: https://github.com/UnityTechnologies/open-project-1

Goal:
- Capture all RefactorGuard listing screenshots using only public sample content.

## 1. Download and open the sample project

1. Clone or download the repository.
2. Open it in Unity Hub with the recommended project version.
3. Wait for package restore and import to complete fully.

## 2. Install RefactorGuard in the sample

1. Copy these files into the sample at `Assets/Editor/RefactorGuard/`:
   - AssetImpactReport.cs
   - DependencyScanner.cs
   - RefactorGuardStorage.cs
   - RefactorGuardWindow.cs
2. Open Unity and confirm no compile errors.
3. Open the tool via `Tools > RefactorGuard > Impact Analyzer`.
4. Click `Scan Project` and wait for completion.

## 3. Build a predictable capture baseline

1. Keep Unity in one editor theme for all screenshots.
2. Dock Project window and RefactorGuard side by side.
3. Set Inspector narrow enough that long asset paths wrap naturally.
4. Disable unrelated panels if they distract from the shot.
5. Keep the same zoom/UI scale across every screenshot.

## 4. Shot-by-shot capture script

## Shot 1 - Hero value
Target outcome:
- Caution state with multiple incoming references.

Steps:
1. In Project window, select a shared material, shader, or prefab used widely.
2. In RefactorGuard, use `Use Current Project Selection`.
3. Ensure CAUTION banner appears and incoming list has several entries.
4. Capture with top title area and caution banner visible.

## Shot 2 - Single asset report
Target outcome:
- Clear metadata plus Open/Ping utility actions.

Steps:
1. Keep the same selected asset or choose another with mixed dependencies.
2. Ensure metadata rows are visible:
   - Asset Path
   - GUID
   - Type
   - Incoming and Outgoing counts
3. Ensure Open and Ping buttons are in frame.
4. Capture tightly around report panel.

## Shot 3 - Batch analysis
Target outcome:
- High, Medium, Safe grouped sections with actionable rows.

Steps:
1. Select a populated folder in Project, for example an art or gameplay content folder.
2. In RefactorGuard batch section, click `Analyze Folder`.
3. Ensure grouped batch sections are visible:
   - High Risk Assets
   - Medium Risk Assets
   - Safe Assets
4. Ensure row actions (Analyze/Open/Ping) are visible.
5. Capture.

## Shot 4 - Filters and presets
Target outcome:
- Show scale control and rapid narrowing.

Steps:
1. Start from populated batch results.
2. Click `Materials & Shaders` preset.
3. Add a Folder Filter value matching an art subtree.
4. Optionally toggle off Safe to focus high/medium.
5. Capture with filters and narrowed results visible.

## Shot 5 - Dashboard actions
Target outcome:
- Summary cards and one-click drilldown actions.

Steps:
1. Keep a broad enough batch scope that dashboard cards are meaningful.
2. Ensure dashboard cards are visible:
   - Top Risk Asset
   - Most Dangerous Folder
   - Highest-Risk Asset Type
3. Show action buttons in frame:
   - Analyze / Open on Top Risk Asset
   - Use Folder
   - Use Type
4. Capture.

## Shot 6 - Export workflow
Target outcome:
- Team-ready artifacts in file system.

Steps:
1. In single report, click `Export JSON` and `Export CSV`.
2. In batch report, click `Export Batch JSON` and `Export Batch CSV`.
3. Open the generated folder (`Assets/RefactorGuardData`).
4. Capture Unity export buttons and file explorer view in a second supporting image.

## 5. Safety checks before publishing screenshots

- Confirm no private folder names appear in captured paths.
- Confirm no proprietary package names are visible.
- Confirm only public sample project assets are shown.
- Confirm export files shown are generated from sample project data only.

## 6. Optional short demo video flow (30-45 seconds)

1. Open RefactorGuard and run Scan.
2. Analyze one high-impact asset and show CAUTION.
3. Switch to Batch and run Analyze Folder.
4. Apply one preset and one filter.
5. Click dashboard `Use Folder`.
6. Export batch CSV.

This gives a complete narrative in under one minute without exposing any private project content.
