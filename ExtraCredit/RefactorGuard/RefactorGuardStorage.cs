using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class RefactorGuardStorage
{
    private const string DataFolder = "Assets/RefactorGuardData";

    /// <summary>
    /// Exports an impact report to a timestamped JSON file inside Assets/RefactorGuardData/.
    /// Returns the path the file was written to, or an empty string on failure.
    /// </summary>
    public static string ExportReport(AssetImpactReport report)
    {
        if (report == null)
        {
            Debug.LogWarning("RefactorGuard: Cannot export a null report.");
            return string.Empty;
        }

        EnsureDataFolder();

        string assetName = Path.GetFileNameWithoutExtension(report.targetAssetPath ?? "unknown")
                               .Replace(" ", "_");

        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string fileName  = $"impact_{assetName}_{timestamp}.json";
        string fullPath  = $"{DataFolder}/{fileName}";

        string json = JsonUtility.ToJson(report, true);

        try
        {
            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();
            Debug.Log($"RefactorGuard: Impact report saved to {fullPath}");
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.LogError($"RefactorGuard: Failed to write report to {fullPath}. {ex.Message}");
            return string.Empty;
        }
    }

    public static string ExportReportCsv(AssetImpactReport report)
    {
        if (report == null)
        {
            Debug.LogWarning("RefactorGuard: Cannot export a null CSV report.");
            return string.Empty;
        }

        EnsureDataFolder();

        string assetName = Path.GetFileNameWithoutExtension(report.targetAssetPath ?? "unknown")
            .Replace(" ", "_");

        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string fileName = $"impact_{assetName}_{timestamp}.csv";
        string fullPath = $"{DataFolder}/{fileName}";

        var lines = new List<string>
        {
            "generatedAtUtc,targetAssetPath,targetAssetGuid,targetAssetType,isSafeToRemove,directReferenceCount,outgoingReferenceCount,referencedByPaths,referencesPaths",
            string.Join(",",
                EscapeCsv(report.generatedAtUtc),
                EscapeCsv(report.targetAssetPath),
                EscapeCsv(report.targetAssetGuid),
                EscapeCsv(report.targetAssetType),
                EscapeCsv(report.isSafeToRemove.ToString()),
                EscapeCsv(report.directReferenceCount.ToString()),
                EscapeCsv(report.outgoingReferenceCount.ToString()),
                EscapeCsv(JoinList(report.referencedByPaths)),
                EscapeCsv(JoinList(report.referencesPaths)))
        };

        return WriteTextFile(fullPath, string.Join(Environment.NewLine, lines), "single CSV impact report");
    }

    public static string ExportBatchReport(BatchImpactReport report)
    {
        if (report == null)
        {
            Debug.LogWarning("RefactorGuard: Cannot export a null batch report.");
            return string.Empty;
        }

        EnsureDataFolder();

        string safeLabel = string.IsNullOrWhiteSpace(report.sourceLabel)
            ? "selection"
            : report.sourceLabel.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");

        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string fileName = $"impact_batch_{safeLabel}_{timestamp}.json";
        string fullPath = $"{DataFolder}/{fileName}";

        string json = JsonUtility.ToJson(report, true);

        try
        {
            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();
            Debug.Log($"RefactorGuard: Batch impact report saved to {fullPath}");
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.LogError($"RefactorGuard: Failed to write batch report to {fullPath}. {ex.Message}");
            return string.Empty;
        }
    }

    public static string ExportBatchReportCsv(BatchImpactReport report)
    {
        if (report == null)
        {
            Debug.LogWarning("RefactorGuard: Cannot export a null batch CSV report.");
            return string.Empty;
        }

        EnsureDataFolder();

        string safeLabel = string.IsNullOrWhiteSpace(report.sourceLabel)
            ? "selection"
            : report.sourceLabel.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");

        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string fileName = $"impact_batch_{safeLabel}_{timestamp}.csv";
        string fullPath = $"{DataFolder}/{fileName}";

        var builder = new StringBuilder();
        builder.AppendLine("generatedAtUtc,sourceLabel,totalAnalyzedAssetCount,safeAssetCount,mediumRiskAssetCount,highRiskAssetCount,cautionAssetCount,totalIncomingReferences");
        builder.AppendLine(string.Join(",",
            EscapeCsv(report.generatedAtUtc),
            EscapeCsv(report.sourceLabel),
            EscapeCsv(report.totalAnalyzedAssetCount.ToString()),
            EscapeCsv(report.safeAssetCount.ToString()),
            EscapeCsv(report.mediumRiskAssetCount.ToString()),
            EscapeCsv(report.highRiskAssetCount.ToString()),
            EscapeCsv(report.cautionAssetCount.ToString()),
            EscapeCsv(report.totalIncomingReferences.ToString())));
        builder.AppendLine();
        builder.AppendLine("assetPath,assetType,riskTier,isSafeToRemove,directReferenceCount,outgoingReferenceCount");

        for (int i = 0; i < report.items.Count; i++)
        {
            BatchImpactSummaryItem item = report.items[i];
            builder.AppendLine(string.Join(",",
                EscapeCsv(item.assetPath),
                EscapeCsv(item.assetType),
                EscapeCsv(item.riskTier),
                EscapeCsv(item.isSafeToRemove.ToString()),
                EscapeCsv(item.directReferenceCount.ToString()),
                EscapeCsv(item.outgoingReferenceCount.ToString())));
        }

        return WriteTextFile(fullPath, builder.ToString(), "batch CSV impact report");
    }

    private static void EnsureDataFolder()
    {
        if (!AssetDatabase.IsValidFolder(DataFolder))
            AssetDatabase.CreateFolder("Assets", "RefactorGuardData");
    }

    private static string WriteTextFile(string fullPath, string contents, string label)
    {
        try
        {
            File.WriteAllText(fullPath, contents);
            AssetDatabase.Refresh();
            Debug.Log($"RefactorGuard: {label} saved to {fullPath}");
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.LogError($"RefactorGuard: Failed to write {label} to {fullPath}. {ex.Message}");
            return string.Empty;
        }
    }

    private static string JoinList(List<string> values)
    {
        return values == null || values.Count == 0
            ? string.Empty
            : string.Join(" | ", values);
    }

    private static string EscapeCsv(string value)
    {
        string safeValue = value ?? string.Empty;
        if (safeValue.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0)
            return $"\"{safeValue.Replace("\"", "\"\"")}\"";

        return safeValue;
    }
}
