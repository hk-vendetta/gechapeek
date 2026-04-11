using System;
using UnityEditor;
using UnityEngine;

public class BuildHealthWindow : EditorWindow
{
    private BuildHealthSnapshot currentSnapshot;
    private BuildHealthSnapshot baselineSnapshot;
    private Vector2 scroll;

    [MenuItem("Tools/Build Health/Build Health Dashboard")]
    public static void Open()
    {
        BuildHealthWindow window = GetWindow<BuildHealthWindow>("Build Health");
        window.minSize = new Vector2(520f, 420f);
        window.Show();
    }

    private void OnEnable()
    {
        baselineSnapshot = BuildHealthStorage.LoadBaseline();
        currentSnapshot = BuildHealthStorage.LoadLatest();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Build Health Intelligence (MVP)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Capture project metrics, save a baseline, and compare drift over time.");

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Capture Current Snapshot", GUILayout.Height(28f)))
            {
                currentSnapshot = BuildHealthAnalyzer.CaptureCurrent();
                BuildHealthStorage.SaveLatest(currentSnapshot);
            }

            GUI.enabled = currentSnapshot != null;
            if (GUILayout.Button("Save As Baseline", GUILayout.Height(28f)))
            {
                baselineSnapshot = currentSnapshot;
                BuildHealthStorage.SaveBaseline(baselineSnapshot);
            }

            GUI.enabled = true;
            if (GUILayout.Button("Reload Saved Data", GUILayout.Height(28f)))
            {
                baselineSnapshot = BuildHealthStorage.LoadBaseline();
                currentSnapshot = BuildHealthStorage.LoadLatest();
            }
        }

        EditorGUILayout.Space();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawSnapshotBlock("Current Snapshot", currentSnapshot);
        EditorGUILayout.Space(10f);
        DrawSnapshotBlock("Baseline", baselineSnapshot);

        EditorGUILayout.Space(12f);
        DrawDiffBlock();

        EditorGUILayout.EndScrollView();
    }

    private static void DrawSnapshotBlock(string label, BuildHealthSnapshot snapshot)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (snapshot == null)
        {
            EditorGUILayout.LabelField("No data captured.");
            EditorGUILayout.EndVertical();
            return;
        }

        DrawMetric("Timestamp (UTC)", snapshot.timestampUtc);
        DrawMetric("Unity Version", snapshot.unityVersion);
        DrawMetric("Active Build Target", snapshot.activeBuildTarget);
        DrawMetric("Total Assets", snapshot.totalAssets.ToString());
        DrawMetric("Total Scripts", snapshot.totalScripts.ToString());
        DrawMetric("Scenes In Build", snapshot.totalScenesInBuild.ToString());
        DrawMetric("Enabled Scenes", snapshot.enabledScenesInBuild.ToString());
        DrawMetric("Dependency Asset Count", snapshot.dependencyAssetCount.ToString());
        DrawMetric("Dependency Estimated Size", FormatBytes(snapshot.dependencyEstimatedBytes));
        DrawMetric("Texture Assets", snapshot.textureAssetCount.ToString());
        DrawMetric("Material Assets", snapshot.materialAssetCount.ToString());
        DrawMetric("Shader Assets", snapshot.shaderAssetCount.ToString());

        EditorGUILayout.EndVertical();
    }

    private void DrawDiffBlock()
    {
        EditorGUILayout.LabelField("Drift vs Baseline", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (baselineSnapshot == null || currentSnapshot == null)
        {
            EditorGUILayout.LabelField("Capture current snapshot and baseline to compare.");
            EditorGUILayout.EndVertical();
            return;
        }

        DrawDelta("Total Assets", currentSnapshot.totalAssets - baselineSnapshot.totalAssets, baselineSnapshot.totalAssets);
        DrawDelta("Total Scripts", currentSnapshot.totalScripts - baselineSnapshot.totalScripts, baselineSnapshot.totalScripts);
        DrawDelta("Enabled Scenes", currentSnapshot.enabledScenesInBuild - baselineSnapshot.enabledScenesInBuild, baselineSnapshot.enabledScenesInBuild);
        DrawDelta("Dependency Asset Count", currentSnapshot.dependencyAssetCount - baselineSnapshot.dependencyAssetCount, baselineSnapshot.dependencyAssetCount);
        DrawDeltaBytes("Dependency Estimated Size", currentSnapshot.dependencyEstimatedBytes - baselineSnapshot.dependencyEstimatedBytes, baselineSnapshot.dependencyEstimatedBytes);
        DrawDelta("Texture Assets", currentSnapshot.textureAssetCount - baselineSnapshot.textureAssetCount, baselineSnapshot.textureAssetCount);
        DrawDelta("Material Assets", currentSnapshot.materialAssetCount - baselineSnapshot.materialAssetCount, baselineSnapshot.materialAssetCount);
        DrawDelta("Shader Assets", currentSnapshot.shaderAssetCount - baselineSnapshot.shaderAssetCount, baselineSnapshot.shaderAssetCount);

        EditorGUILayout.EndVertical();
    }

    private static void DrawMetric(string name, string value)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(name, GUILayout.Width(220f));
            GUILayout.Label(value, EditorStyles.miniBoldLabel);
        }
    }

    private static void DrawDelta(string name, int delta, int baseline)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(name, GUILayout.Width(220f));

            Color originalColor = GUI.color;
            GUI.color = delta > 0 ? new Color(0.95f, 0.45f, 0.2f) : delta < 0 ? new Color(0.35f, 0.8f, 0.45f) : Color.white;
            GUILayout.Label($"{FormatSigned(delta)} ({FormatPercent(delta, baseline)})", EditorStyles.miniBoldLabel);
            GUI.color = originalColor;
        }
    }

    private static void DrawDeltaBytes(string name, long delta, long baseline)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(name, GUILayout.Width(220f));

            Color originalColor = GUI.color;
            GUI.color = delta > 0 ? new Color(0.95f, 0.45f, 0.2f) : delta < 0 ? new Color(0.35f, 0.8f, 0.45f) : Color.white;
            GUILayout.Label($"{FormatSignedBytes(delta)} ({FormatPercent(delta, baseline)})", EditorStyles.miniBoldLabel);
            GUI.color = originalColor;
        }
    }

    private static string FormatSigned(int value)
    {
        if (value > 0)
        {
            return "+" + value;
        }

        return value.ToString();
    }

    private static string FormatSignedBytes(long value)
    {
        if (value > 0)
        {
            return "+" + FormatBytes(value);
        }

        if (value < 0)
        {
            return "-" + FormatBytes(Math.Abs(value));
        }

        return FormatBytes(0);
    }

    private static string FormatPercent(long delta, long baseline)
    {
        if (baseline == 0)
        {
            return "n/a";
        }

        float pct = (delta / (float)baseline) * 100f;
        return pct.ToString("0.00") + "%";
    }

    private static string FormatBytes(long bytes)
    {
        const float kilo = 1024f;
        const float mega = kilo * 1024f;
        const float giga = mega * 1024f;

        if (bytes >= giga)
        {
            return (bytes / giga).ToString("0.00") + " GB";
        }

        if (bytes >= mega)
        {
            return (bytes / mega).ToString("0.00") + " MB";
        }

        if (bytes >= kilo)
        {
            return (bytes / kilo).ToString("0.00") + " KB";
        }

        return bytes + " B";
    }
}
