using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class LiveOpsRuleStorage
{
    private const string DataFolder = "Assets/LiveOpsRuleLabData";

    public static string SaveRule(LiveOpsRuleDefinition rule)
    {
        if (rule == null)
        {
            Debug.LogWarning("LiveOpsRuleLab: Cannot save a null rule.");
            return string.Empty;
        }

        EnsureDataFolder();

        string safeName = string.IsNullOrWhiteSpace(rule.displayName)
            ? "rule"
            : rule.displayName.Replace(" ", "_");

        string fileName = $"rule_{rule.ruleId}_{safeName}.json";
        string path = $"{DataFolder}/{fileName}";

        try
        {
            File.WriteAllText(path, JsonUtility.ToJson(rule, true));
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            return path;
        }
        catch (Exception ex)
        {
            Debug.LogError($"LiveOpsRuleLab: Failed to save rule. {ex.Message}");
            return string.Empty;
        }
    }

    public static LiveOpsRuleDefinition LoadRule(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        try
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<LiveOpsRuleDefinition>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"LiveOpsRuleLab: Failed to load rule from {filePath}. {ex.Message}");
            return null;
        }
    }

    public static List<string> ListRulePaths()
    {
        var paths = new List<string>();

        if (!Directory.Exists(DataFolder))
            return paths;

        string[] files = Directory.GetFiles(DataFolder, "rule_*.json");
        paths.AddRange(files);
        paths.Sort(StringComparer.OrdinalIgnoreCase);
        return paths;
    }

    public static void DeleteRule(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        File.Delete(filePath);

        string metaPath = filePath + ".meta";
        if (File.Exists(metaPath))
            File.Delete(metaPath);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private static void EnsureDataFolder()
    {
        if (Directory.Exists(DataFolder))
            return;

#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(DataFolder))
            AssetDatabase.CreateFolder("Assets", "LiveOpsRuleLabData");
#else
        Directory.CreateDirectory(DataFolder);
#endif
    }
}
