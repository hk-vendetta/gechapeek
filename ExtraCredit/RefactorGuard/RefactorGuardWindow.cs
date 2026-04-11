using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RefactorGuardWindow : EditorWindow
{
    private const string SafeRiskTier = "Safe";
    private const string MediumRiskTier = "Medium";
    private const string HighRiskTier = "High";

    // --- State ---
    private Dictionary<string, HashSet<string>> reverseMap;
    private string scanStatus = "Project not yet scanned. Click \"Scan Project\" to begin.";
    private int totalAssetsIndexed;

    private UnityEngine.Object selectedAsset;
    private DefaultAsset selectedFolder;
    private AssetImpactReport currentReport;
    private BatchImpactReport currentBatchReport;

    private Vector2 referencedByScroll;
    private Vector2 referencesScroll;
    private Vector2 batchScroll;
    private string referencedByFilter = string.Empty;
    private string referencesFilter = string.Empty;
    private string batchFilter = string.Empty;
    private string batchFolderFilter = string.Empty;
    private string batchAssetTypeFilter = string.Empty;
    private bool showSafeBatchItems = true;
    private bool showMediumBatchItems = true;
    private bool showHighBatchItems = true;

    // --- Menu ---
    [MenuItem("Tools/RefactorGuard/Impact Analyzer")]
    public static void Open()
    {
        RefactorGuardWindow window = GetWindow<RefactorGuardWindow>("RefactorGuard");
        window.minSize = new Vector2(540f, 520f);
        window.Show();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Main GUI
    // ──────────────────────────────────────────────────────────────────────────

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RefactorGuard — Asset Impact Analyzer", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("See exactly what breaks before you rename, move, or delete any asset.");

        EditorGUILayout.Space();
        DrawScanSection();

        EditorGUILayout.Space();
        DrawAssetPickerSection();

        EditorGUILayout.Space();
        DrawBatchSection();

        if (currentReport != null)
        {
            EditorGUILayout.Space();
            DrawReportSection();
        }

        if (currentBatchReport != null)
        {
            EditorGUILayout.Space();
            DrawBatchReportSection();
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Scan section
    // ──────────────────────────────────────────────────────────────────────────

    private void DrawScanSection()
    {
        EditorGUILayout.LabelField("Step 1 — Scan Your Project", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(scanStatus, EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.Space(4f);

        if (reverseMap != null)
            EditorGUILayout.LabelField($"Indexed assets: {totalAssetsIndexed:N0}", EditorStyles.miniLabel);

        EditorGUILayout.Space(2f);

        if (GUILayout.Button("Scan Project", GUILayout.Height(28f)))
            RunScan();

        EditorGUILayout.EndVertical();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Asset picker section
    // ──────────────────────────────────────────────────────────────────────────

    private void DrawAssetPickerSection()
    {
        EditorGUILayout.LabelField("Step 2 — Select an Asset to Analyze", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (reverseMap == null)
        {
            EditorGUILayout.LabelField("Scan the project first to enable asset analysis.", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
            return;
        }

        GUI.enabled = reverseMap != null;

        UnityEngine.Object picked = EditorGUILayout.ObjectField(
            "Asset", selectedAsset, typeof(UnityEngine.Object), false);

        if (picked != selectedAsset)
        {
            selectedAsset = picked;
            currentReport = null;

            if (selectedAsset != null)
                AnalyzeSelected();
        }

        EditorGUILayout.Space(4f);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Use Current Project Selection", GUILayout.Height(22f)))
            UseCurrentProjectSelection();

        GUI.enabled = selectedAsset != null;
        if (GUILayout.Button("Clear", GUILayout.Width(60f), GUILayout.Height(22f)))
        {
            selectedAsset = null;
            currentReport = null;
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4f);

        GUI.enabled = selectedAsset != null && reverseMap != null;

        if (GUILayout.Button("Analyze Selected Asset", GUILayout.Height(24f)))
            AnalyzeSelected();

        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }

    private void DrawBatchSection()
    {
        EditorGUILayout.LabelField("Step 3 — Batch Analysis", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (reverseMap == null)
        {
            EditorGUILayout.LabelField("Scan the project first to enable batch analysis.", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.LabelField(
            "Analyze the current Project selection or a whole folder to get a quick risk summary for multiple assets.",
            EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.Space(4f);
        selectedFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Folder", selectedFolder, typeof(DefaultAsset), false);

        EditorGUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Analyze Current Selection", GUILayout.Height(24f)))
            AnalyzeBatchSelection();

        GUI.enabled = selectedFolder != null;
        if (GUILayout.Button("Analyze Folder", GUILayout.Height(24f)))
            AnalyzeFolderSelection();
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Report section
    // ──────────────────────────────────────────────────────────────────────────

    private void DrawReportSection()
    {
        AssetImpactReport r = currentReport;

        EditorGUILayout.LabelField("Impact Report", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        // Safety indicator
        Color originalColor = GUI.color;
        GUI.color = r.isSafeToRemove
            ? new Color(0.35f, 0.8f, 0.45f)
            : new Color(0.95f, 0.45f, 0.2f);

        string safeLabel = r.isSafeToRemove
            ? "SAFE — Nothing else references this asset. Safe to rename, move, or delete."
            : $"CAUTION — {r.directReferenceCount} asset(s) reference this. Removing or renaming it will break them.";

        EditorGUILayout.LabelField(safeLabel, EditorStyles.boldLabel);
        GUI.color = originalColor;

        EditorGUILayout.Space(6f);

        DrawRow("Asset Path",              r.targetAssetPath);
        DrawRow("GUID",                    r.targetAssetGuid);
        DrawRow("Type",                    r.targetAssetType);
        DrawRow("Referenced By (incoming)", r.directReferenceCount.ToString());
        DrawRow("References (outgoing)",    r.outgoingReferenceCount.ToString());

        EditorGUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ping Selected Asset", GUILayout.Height(22f)))
            PingAssetPath(r.targetAssetPath);

        if (GUILayout.Button("Open Selected Asset", GUILayout.Height(22f)))
            OpenAssetPath(r.targetAssetPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        // --- Referenced By ---
        if (r.referencedByPaths != null && r.referencedByPaths.Count > 0)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                $"Assets that reference this ({r.referencedByPaths.Count}) — these would break:",
                EditorStyles.boldLabel);

            referencedByFilter = EditorGUILayout.TextField("Filter Incoming", referencedByFilter);

            int visibleIncoming = CountVisiblePaths(r.referencedByPaths, referencedByFilter);
            EditorGUILayout.LabelField($"Visible entries: {visibleIncoming}", EditorStyles.miniLabel);

            referencedByScroll = EditorGUILayout.BeginScrollView(
                referencedByScroll, GUILayout.MaxHeight(160f));

            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < r.referencedByPaths.Count; i++)
            {
                string path = r.referencedByPaths[i];
                if (!ShouldShowPath(path, referencedByFilter))
                    continue;

                DrawPathActionRow(path);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Assets that reference this (0)", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("No incoming references found. This asset is currently safe to rename or remove.", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
        }

        // --- References ---
        if (r.referencesPaths != null && r.referencesPaths.Count > 0)
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField(
                $"Assets this depends on ({r.referencesPaths.Count}):",
                EditorStyles.boldLabel);

            referencesFilter = EditorGUILayout.TextField("Filter Outgoing", referencesFilter);

            int visibleOutgoing = CountVisiblePaths(r.referencesPaths, referencesFilter);
            EditorGUILayout.LabelField($"Visible entries: {visibleOutgoing}", EditorStyles.miniLabel);

            referencesScroll = EditorGUILayout.BeginScrollView(
                referencesScroll, GUILayout.MaxHeight(120f));

            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < r.referencesPaths.Count; i++)
            {
                string path = r.referencesPaths[i];
                if (!ShouldShowPath(path, referencesFilter))
                    continue;

                DrawPathActionRow(path);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("Assets this depends on (0):", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("No outgoing dependencies found for this asset.", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(8f);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Export JSON", GUILayout.Height(24f)))
            RevealExportedFile(RefactorGuardStorage.ExportReport(currentReport));

        if (GUILayout.Button("Export CSV", GUILayout.Height(24f)))
            RevealExportedFile(RefactorGuardStorage.ExportReportCsv(currentReport));
        EditorGUILayout.EndHorizontal();
    }

    private void DrawBatchReportSection()
    {
        BatchImpactReport r = currentBatchReport;

        EditorGUILayout.LabelField("Batch Impact Summary", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        DrawRow("Source", r.sourceLabel);
        DrawRow("Analyzed Assets", r.totalAnalyzedAssetCount.ToString());
        DrawRow("Safe Assets", r.safeAssetCount.ToString());
        DrawRow("Medium Risk Assets", r.mediumRiskAssetCount.ToString());
        DrawRow("High Risk Assets", r.highRiskAssetCount.ToString());
        DrawRow("Caution Assets", r.cautionAssetCount.ToString());
        DrawRow("Total Incoming References", r.totalIncomingReferences.ToString());

        EditorGUILayout.Space(6f);
        batchFilter = EditorGUILayout.TextField("Filter Batch Rows", batchFilter);
        batchAssetTypeFilter = EditorGUILayout.TextField("Asset Type Filter", batchAssetTypeFilter);
        batchFolderFilter = EditorGUILayout.TextField("Folder Filter", batchFolderFilter);

        EditorGUILayout.BeginHorizontal();
        showHighBatchItems = EditorGUILayout.ToggleLeft("High", showHighBatchItems, GUILayout.Width(60f));
        showMediumBatchItems = EditorGUILayout.ToggleLeft("Medium", showMediumBatchItems, GUILayout.Width(80f));
        showSafeBatchItems = EditorGUILayout.ToggleLeft("Safe", showSafeBatchItems, GUILayout.Width(60f));
        if (GUILayout.Button("Reset Filters", GUILayout.Width(96f)))
            ResetBatchFilters();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("High Risk Only", GUILayout.Height(22f)))
            ApplyBatchPresetHighRiskOnly();

        if (GUILayout.Button("Safe Cleanup", GUILayout.Height(22f)))
            ApplyBatchPresetSafeCleanup();

        if (GUILayout.Button("Materials & Shaders", GUILayout.Height(22f)))
            ApplyBatchPresetMaterialsAndShaders();

        if (GUILayout.Button("Prefabs & Scenes", GUILayout.Height(22f)))
            ApplyBatchPresetPrefabsAndScenes();
        EditorGUILayout.EndHorizontal();

        int visibleRows = CountVisibleBatchItems(r.items, batchFilter);
        EditorGUILayout.LabelField($"Visible rows: {visibleRows}", EditorStyles.miniLabel);

        List<BatchImpactSummaryItem> visibleItems = GetVisibleBatchItems(r.items, batchFilter);
        DrawBatchDashboardCards(visibleItems);

        batchScroll = EditorGUILayout.BeginScrollView(batchScroll, GUILayout.MaxHeight(220f));
        EditorGUILayout.BeginVertical("box");
        DrawBatchGroup(r.items, HighRiskTier, "High Risk Assets", batchFilter);
        DrawBatchGroup(r.items, MediumRiskTier, "Medium Risk Assets", batchFilter);
        DrawBatchGroup(r.items, SafeRiskTier, "Safe Assets", batchFilter);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(8f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Export Batch JSON", GUILayout.Height(24f)))
            RevealExportedFile(RefactorGuardStorage.ExportBatchReport(r));

        if (GUILayout.Button("Export Batch CSV", GUILayout.Height(24f)))
            RevealExportedFile(RefactorGuardStorage.ExportBatchReportCsv(r));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Logic
    // ──────────────────────────────────────────────────────────────────────────

    private void RunScan()
    {
        scanStatus = "Scanning... this may take a few seconds on large projects.";
        Repaint();

        try
        {
            reverseMap = DependencyScanner.BuildReverseMap(UpdateScanProgress);
            totalAssetsIndexed = reverseMap.Count;
            scanStatus = $"Scan complete — {totalAssetsIndexed:N0} assets indexed. Last scanned: {DateTime.Now:HH:mm:ss}";

            if (selectedAsset != null)
                AnalyzeSelected();
        }
        catch (Exception ex)
        {
            reverseMap = null;
            scanStatus = $"Scan failed: {ex.Message}";
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Repaint();
    }

    private void AnalyzeSelected()
    {
        if (selectedAsset == null || reverseMap == null)
            return;

        string path = AssetDatabase.GetAssetPath(selectedAsset);
        if (string.IsNullOrEmpty(path))
            return;

        currentReport = DependencyScanner.BuildReport(path, reverseMap);
        Repaint();
    }

    private void AnalyzeAssetPath(string path)
    {
        if (string.IsNullOrEmpty(path) || reverseMap == null)
            return;

        selectedAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        currentReport = DependencyScanner.BuildReport(path, reverseMap);
        referencedByFilter = string.Empty;
        referencesFilter = string.Empty;
        Repaint();
    }

    private void UseCurrentProjectSelection()
    {
        selectedAsset = Selection.activeObject;
        currentReport = null;
        currentBatchReport = null;

        if (selectedAsset != null)
            AnalyzeSelected();
    }

    private void AnalyzeBatchSelection()
    {
        List<string> paths = CollectPathsFromObjects(Selection.objects);
        currentBatchReport = BuildBatchReport(paths, "current_selection");
        Repaint();
    }

    private void AnalyzeFolderSelection()
    {
        if (selectedFolder == null)
            return;

        string folderPath = AssetDatabase.GetAssetPath(selectedFolder);
        List<string> paths = CollectPathsFromFolder(folderPath);
        currentBatchReport = BuildBatchReport(paths, folderPath);
        Repaint();
    }

    private static void RevealExportedFile(string savedPath)
    {
        if (!string.IsNullOrEmpty(savedPath))
            EditorUtility.RevealInFinder(savedPath);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static void UpdateScanProgress(float progress)
    {
        EditorUtility.DisplayProgressBar(
            "RefactorGuard",
            "Scanning asset dependencies and building reverse-reference map...",
            Mathf.Clamp01(progress));
    }

    private static bool ShouldShowPath(string path, string filter)
    {
        return string.IsNullOrWhiteSpace(filter)
            || path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static int CountVisiblePaths(List<string> paths, string filter)
    {
        int visibleCount = 0;
        for (int i = 0; i < paths.Count; i++)
        {
            if (ShouldShowPath(paths[i], filter))
                visibleCount++;
        }

        return visibleCount;
    }

    private static bool ShouldShowBatchItem(BatchImpactSummaryItem item, string filter)
    {
        if (item == null)
            return false;

        if (string.IsNullOrWhiteSpace(filter))
            return true;

        return item.assetPath.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
            || (!string.IsNullOrWhiteSpace(item.riskTier)
                && item.riskTier.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
            || (!string.IsNullOrWhiteSpace(item.assetType)
                && item.assetType.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private int CountVisibleBatchItems(List<BatchImpactSummaryItem> items, string filter)
    {
        int visibleCount = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (ShouldShowBatchItem(items[i], filter) && PassesBatchStructuredFilters(items[i]))
                visibleCount++;
        }

        return visibleCount;
    }

    private List<BatchImpactSummaryItem> GetVisibleBatchItems(List<BatchImpactSummaryItem> items, string filter)
    {
        var visibleItems = new List<BatchImpactSummaryItem>();
        for (int i = 0; i < items.Count; i++)
        {
            if (ShouldShowBatchItem(items[i], filter) && PassesBatchStructuredFilters(items[i]))
                visibleItems.Add(items[i]);
        }

        return visibleItems;
    }

    private BatchImpactReport BuildBatchReport(List<string> paths, string sourceLabel)
    {
        var report = new BatchImpactReport
        {
            generatedAtUtc = DateTime.UtcNow.ToString("o"),
            sourceLabel = sourceLabel,
        };

        for (int i = 0; i < paths.Count; i++)
        {
            AssetImpactReport itemReport = DependencyScanner.BuildReport(paths[i], reverseMap);
            if (itemReport == null)
                continue;

            report.items.Add(new BatchImpactSummaryItem
            {
                assetPath = itemReport.targetAssetPath,
                assetType = itemReport.targetAssetType,
                riskTier = GetBatchRiskTier(itemReport),
                isSafeToRemove = itemReport.isSafeToRemove,
                directReferenceCount = itemReport.directReferenceCount,
                outgoingReferenceCount = itemReport.outgoingReferenceCount,
            });

            if (itemReport.isSafeToRemove)
                report.safeAssetCount++;
            else
                report.cautionAssetCount++;

            string riskTier = report.items[report.items.Count - 1].riskTier;
            if (riskTier == HighRiskTier)
                report.highRiskAssetCount++;
            else if (riskTier == MediumRiskTier)
                report.mediumRiskAssetCount++;

            report.totalIncomingReferences += itemReport.directReferenceCount;
        }

        report.items.Sort(CompareBatchItems);
        report.totalAnalyzedAssetCount = report.items.Count;
        ResetBatchFilters();
        return report;
    }

    private bool PassesBatchStructuredFilters(BatchImpactSummaryItem item)
    {
        if (item == null)
            return false;

        if (item.riskTier == HighRiskTier && !showHighBatchItems)
            return false;

        if (item.riskTier == MediumRiskTier && !showMediumBatchItems)
            return false;

        if (item.riskTier == SafeRiskTier && !showSafeBatchItems)
            return false;

        if (!string.IsNullOrWhiteSpace(batchAssetTypeFilter)
            && !MatchesAnyToken(item.assetType, batchAssetTypeFilter))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(batchFolderFilter)
            && !MatchesAnyToken(item.assetPath, batchFolderFilter))
        {
            return false;
        }

        return true;
    }

    private static bool MatchesAnyToken(string value, string tokenList)
    {
        if (string.IsNullOrWhiteSpace(tokenList))
            return true;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        string[] tokens = tokenList.Split(',');
        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i].Trim();
            if (!string.IsNullOrWhiteSpace(token)
                && value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetBatchFilters()
    {
        batchFilter = string.Empty;
        batchFolderFilter = string.Empty;
        batchAssetTypeFilter = string.Empty;
        showSafeBatchItems = true;
        showMediumBatchItems = true;
        showHighBatchItems = true;
    }

    private void ApplyBatchPresetHighRiskOnly()
    {
        ResetBatchFilters();
        showHighBatchItems = true;
        showMediumBatchItems = false;
        showSafeBatchItems = false;
    }

    private void ApplyBatchPresetSafeCleanup()
    {
        ResetBatchFilters();
        showHighBatchItems = false;
        showMediumBatchItems = false;
        showSafeBatchItems = true;
    }

    private void ApplyBatchPresetMaterialsAndShaders()
    {
        ResetBatchFilters();
        batchAssetTypeFilter = "Material, Shader";
    }

    private void ApplyBatchPresetPrefabsAndScenes()
    {
        ResetBatchFilters();
        batchAssetTypeFilter = "GameObject, SceneAsset";
    }

    private static List<string> CollectPathsFromObjects(UnityEngine.Object[] objects)
    {
        var uniquePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objects[i]);
            if (string.IsNullOrWhiteSpace(path))
                continue;

            if (AssetDatabase.IsValidFolder(path))
            {
                List<string> folderPaths = CollectPathsFromFolder(path);
                for (int folderIndex = 0; folderIndex < folderPaths.Count; folderIndex++)
                    uniquePaths.Add(folderPaths[folderIndex]);

                continue;
            }

            if (path.StartsWith("Assets/", StringComparison.Ordinal))
                uniquePaths.Add(path);
        }

        return new List<string>(uniquePaths);
    }

    private static List<string> CollectPathsFromFolder(string folderPath)
    {
        var uniquePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
            return new List<string>(uniquePaths);

        string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (!string.IsNullOrWhiteSpace(path) && !AssetDatabase.IsValidFolder(path))
                uniquePaths.Add(path);
        }

        return new List<string>(uniquePaths);
    }

    private static int CompareBatchItems(BatchImpactSummaryItem left, BatchImpactSummaryItem right)
    {
        int tierCompare = GetRiskTierWeight(right.riskTier).CompareTo(GetRiskTierWeight(left.riskTier));
        if (tierCompare != 0)
            return tierCompare;

        int incomingCompare = right.directReferenceCount.CompareTo(left.directReferenceCount);
        if (incomingCompare != 0)
            return incomingCompare;

        return string.Compare(left.assetPath, right.assetPath, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetBatchRiskTier(AssetImpactReport report)
    {
        if (report == null || report.isSafeToRemove || report.directReferenceCount == 0)
            return SafeRiskTier;

        bool isHighCount = report.directReferenceCount >= 10;
        bool isSensitiveType = string.Equals(report.targetAssetType, "GameObject", StringComparison.OrdinalIgnoreCase)
            || string.Equals(report.targetAssetType, "SceneAsset", StringComparison.OrdinalIgnoreCase)
            || string.Equals(report.targetAssetType, "Material", StringComparison.OrdinalIgnoreCase)
            || string.Equals(report.targetAssetType, "Shader", StringComparison.OrdinalIgnoreCase);

        if (isHighCount || (isSensitiveType && report.directReferenceCount >= 3))
            return HighRiskTier;

        return MediumRiskTier;
    }

    private static int GetRiskTierWeight(string riskTier)
    {
        if (riskTier == HighRiskTier)
            return 3;

        if (riskTier == MediumRiskTier)
            return 2;

        return 1;
    }

    private void DrawBatchGroup(List<BatchImpactSummaryItem> items, string riskTier, string title, string filter)
    {
        int visibleCount = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].riskTier == riskTier
                && ShouldShowBatchItem(items[i], filter)
                && PassesBatchStructuredFilters(items[i]))
            {
                visibleCount++;
            }
        }

        if (visibleCount == 0)
            return;

        EditorGUILayout.LabelField($"{title} ({visibleCount})", EditorStyles.boldLabel);
        for (int i = 0; i < items.Count; i++)
        {
            BatchImpactSummaryItem item = items[i];
            if (item.riskTier != riskTier || !ShouldShowBatchItem(item, filter) || !PassesBatchStructuredFilters(item))
                continue;

            EditorGUILayout.BeginHorizontal();
            DrawRiskBadge(item.riskTier);
            EditorGUILayout.LabelField(
                $"{item.assetPath}  |  in: {item.directReferenceCount}  out: {item.outgoingReferenceCount}",
                EditorStyles.miniLabel);

            if (GUILayout.Button("Analyze", GUILayout.Width(56f)))
                AnalyzeAssetPath(item.assetPath);

            if (GUILayout.Button("Open", GUILayout.Width(48f)))
                OpenAssetPath(item.assetPath);

            if (GUILayout.Button("Ping", GUILayout.Width(42f)))
                PingAssetPath(item.assetPath);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(4f);
    }

    private void DrawBatchDashboardCards(List<BatchImpactSummaryItem> visibleItems)
    {
        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Dashboard", EditorStyles.boldLabel);

        if (visibleItems.Count == 0)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("No visible items match the current batch filters.", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
            return;
        }

        BatchImpactSummaryItem topAsset = GetTopRiskAsset(visibleItems);
        (string folderPath, int incomingReferences, int assetCount) topFolder = GetTopFolderSummary(visibleItems);
        (string assetType, int incomingReferences, int assetCount) topType = GetTopAssetTypeSummary(visibleItems);

        EditorGUILayout.BeginHorizontal();
        DrawDashboardCardWithButtons(
            "Top Risk Asset",
            topAsset != null ? topAsset.assetPath : "None",
            topAsset != null
                ? $"{topAsset.riskTier} | in: {topAsset.directReferenceCount} | out: {topAsset.outgoingReferenceCount}"
                : "No asset data",
            "Analyze",
            topAsset != null ? () => AnalyzeAssetPath(topAsset.assetPath) : null,
            "Open",
            topAsset != null ? () => OpenAssetPath(topAsset.assetPath) : null);

        DrawDashboardCardWithButton(
            "Most Dangerous Folder",
            topFolder.folderPath,
            $"incoming: {topFolder.incomingReferences} | assets: {topFolder.assetCount}",
            "Use Folder",
            () => batchFolderFilter = topFolder.folderPath == "None" ? string.Empty : topFolder.folderPath);

        DrawDashboardCardWithButton(
            "Highest-Risk Asset Type",
            topType.assetType,
            $"incoming: {topType.incomingReferences} | assets: {topType.assetCount}",
            "Use Type",
            () => batchAssetTypeFilter = string.Equals(topType.assetType, "Unknown", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : topType.assetType);
        EditorGUILayout.EndHorizontal();
    }

    private static BatchImpactSummaryItem GetTopRiskAsset(List<BatchImpactSummaryItem> items)
    {
        BatchImpactSummaryItem topItem = null;
        for (int i = 0; i < items.Count; i++)
        {
            BatchImpactSummaryItem candidate = items[i];
            if (topItem == null || CompareBatchItems(candidate, topItem) < 0)
                topItem = candidate;
        }

        return topItem;
    }

    private static (string folderPath, int incomingReferences, int assetCount) GetTopFolderSummary(List<BatchImpactSummaryItem> items)
    {
        var folderTotals = new Dictionary<string, (int incomingReferences, int assetCount)>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < items.Count; i++)
        {
            BatchImpactSummaryItem item = items[i];
            string folderPath = GetFolderPath(item.assetPath);
            if (!folderTotals.TryGetValue(folderPath, out (int incomingReferences, int assetCount) current))
                current = (0, 0);

            current.incomingReferences += item.directReferenceCount;
            current.assetCount += 1;
            folderTotals[folderPath] = current;
        }

        string bestFolder = "None";
        int bestIncoming = -1;
        int bestCount = 0;

        foreach (KeyValuePair<string, (int incomingReferences, int assetCount)> pair in folderTotals)
        {
            if (pair.Value.incomingReferences > bestIncoming)
            {
                bestFolder = pair.Key;
                bestIncoming = pair.Value.incomingReferences;
                bestCount = pair.Value.assetCount;
            }
        }

        return (bestFolder, Mathf.Max(0, bestIncoming), bestCount);
    }

    private static (string assetType, int incomingReferences, int assetCount) GetTopAssetTypeSummary(List<BatchImpactSummaryItem> items)
    {
        var typeTotals = new Dictionary<string, (int incomingReferences, int assetCount)>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < items.Count; i++)
        {
            BatchImpactSummaryItem item = items[i];
            string assetType = string.IsNullOrWhiteSpace(item.assetType) ? "Unknown" : item.assetType;
            if (!typeTotals.TryGetValue(assetType, out (int incomingReferences, int assetCount) current))
                current = (0, 0);

            current.incomingReferences += item.directReferenceCount;
            current.assetCount += 1;
            typeTotals[assetType] = current;
        }

        string bestType = "Unknown";
        int bestIncoming = -1;
        int bestCount = 0;

        foreach (KeyValuePair<string, (int incomingReferences, int assetCount)> pair in typeTotals)
        {
            if (pair.Value.incomingReferences > bestIncoming)
            {
                bestType = pair.Key;
                bestIncoming = pair.Value.incomingReferences;
                bestCount = pair.Value.assetCount;
            }
        }

        return (bestType, Mathf.Max(0, bestIncoming), bestCount);
    }

    private static string GetFolderPath(string assetPath)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
            return "Assets";

        int slashIndex = assetPath.LastIndexOf('/');
        return slashIndex > 0 ? assetPath.Substring(0, slashIndex) : "Assets";
    }

    private static void DrawDashboardCard(string title, string headline, string detail)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(72f));
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(headline, EditorStyles.wordWrappedMiniLabel);
        EditorGUILayout.Space(2f);
        EditorGUILayout.LabelField(detail, EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
    }

    private static void DrawDashboardCardWithButton(string title, string headline, string detail, string buttonLabel, Action onButtonClick)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(72f));
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(headline, EditorStyles.wordWrappedMiniLabel);
        EditorGUILayout.Space(2f);
        EditorGUILayout.LabelField(detail, EditorStyles.miniLabel);
        if (GUILayout.Button(buttonLabel, GUILayout.Height(20f)))
            onButtonClick?.Invoke();
        EditorGUILayout.EndVertical();
    }

    private static void DrawDashboardCardWithButtons(
        string title,
        string headline,
        string detail,
        string primaryButtonLabel,
        Action onPrimaryClick,
        string secondaryButtonLabel,
        Action onSecondaryClick)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(72f));
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(headline, EditorStyles.wordWrappedMiniLabel);
        EditorGUILayout.Space(2f);
        EditorGUILayout.LabelField(detail, EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = onPrimaryClick != null;
        if (GUILayout.Button(primaryButtonLabel, GUILayout.Height(20f)))
            onPrimaryClick?.Invoke();
        GUI.enabled = onSecondaryClick != null;
        if (GUILayout.Button(secondaryButtonLabel, GUILayout.Height(20f)))
            onSecondaryClick?.Invoke();
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private static void DrawRiskBadge(string riskTier)
    {
        Color originalColor = GUI.color;

        if (riskTier == HighRiskTier)
            GUI.color = new Color(0.95f, 0.45f, 0.2f);
        else if (riskTier == MediumRiskTier)
            GUI.color = new Color(0.95f, 0.75f, 0.25f);
        else
            GUI.color = new Color(0.35f, 0.8f, 0.45f);

        GUILayout.Label(riskTier.ToUpperInvariant(), GUILayout.Width(52f));
        GUI.color = originalColor;
    }

    private static void DrawPathActionRow(string path)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(path, EditorStyles.miniLabel);

        if (GUILayout.Button("Open", GUILayout.Width(48f)))
            OpenAssetPath(path);

        if (GUILayout.Button("Ping", GUILayout.Width(42f)))
            PingAssetPath(path);

        EditorGUILayout.EndHorizontal();
    }

    private static void PingAssetPath(string path)
    {
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (obj != null)
            EditorGUIUtility.PingObject(obj);
    }

    private static void OpenAssetPath(string path)
    {
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (obj != null)
            AssetDatabase.OpenAsset(obj);
    }

    private static void DrawRow(string label, string value)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(label, GUILayout.Width(200f));
            GUILayout.Label(value, EditorStyles.wordWrappedMiniLabel);
        }
    }
}
