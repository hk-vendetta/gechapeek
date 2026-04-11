using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class PrivacyAuditStorage
{
    private const string DataFolder = "Assets/PrivacyAuditData";

    public static string ExportReport(PrivacyAuditReport report)
    {
        if (report == null)
        {
            Debug.LogWarning("PrivacySentinel: Cannot export a null report.");
            return string.Empty;
        }

        EnsureDataFolder();

        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string fileName = $"privacy_audit_{timestamp}.json";
        string path = $"{DataFolder}/{fileName}";

        try
        {
            File.WriteAllText(path, JsonUtility.ToJson(report, true));
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            return path;
        }
        catch (Exception ex)
        {
            Debug.LogError($"PrivacySentinel: Failed to export report. {ex.Message}");
            return string.Empty;
        }
    }

    private static void EnsureDataFolder()
    {
        if (Directory.Exists(DataFolder))
            return;

#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(DataFolder))
            AssetDatabase.CreateFolder("Assets", "PrivacyAuditData");
#else
        Directory.CreateDirectory(DataFolder);
#endif
    }
}
