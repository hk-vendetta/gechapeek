using System;
using System.Collections.Generic;
using UnityEditor;

public static class DependencyScanner
{
    /// <summary>
    /// Scans every asset under Assets/ and returns a reverse-reference map:
    ///   key   = asset path
    ///   value = set of asset paths that reference the key asset
    ///
    /// This lets us answer "who would break if I removed/renamed this asset?"
    /// for any asset in O(1) after the initial scan.
    /// </summary>
    public static Dictionary<string, HashSet<string>> BuildReverseMap(Action<float> progressCallback = null)
    {
        string[] allPaths = AssetDatabase.GetAllAssetPaths();

        // Pre-populate map for every asset inside Assets/.
        var reverseMap = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < allPaths.Length; i++)
        {
            if (allPaths[i].StartsWith("Assets/", StringComparison.Ordinal))
                reverseMap[allPaths[i]] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        // For each asset, resolve its direct (non-recursive) dependencies.
        // For every dependency found, record that the source asset references it.
        for (int i = 0; i < allPaths.Length; i++)
        {
            string sourcePath = allPaths[i];
            if (!sourcePath.StartsWith("Assets/", StringComparison.Ordinal))
                continue;

            progressCallback?.Invoke((float)i / allPaths.Length);

            string[] deps;
            try
            {
                deps = AssetDatabase.GetDependencies(sourcePath, false);
            }
            catch
            {
                // Skip any asset that cannot be inspected (e.g. corrupted files).
                continue;
            }

            for (int j = 0; j < deps.Length; j++)
            {
                string dep = deps[j];

                if (string.Equals(dep, sourcePath, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!dep.StartsWith("Assets/", StringComparison.Ordinal))
                    continue;

                if (!reverseMap.ContainsKey(dep))
                    reverseMap[dep] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                reverseMap[dep].Add(sourcePath);
            }
        }

        return reverseMap;
    }

    /// <summary>
    /// Builds a complete impact report for a single asset path using an
    /// already-computed reverse map. Returns null if the path is invalid.
    /// </summary>
    public static AssetImpactReport BuildReport(string assetPath, Dictionary<string, HashSet<string>> reverseMap)
    {
        if (string.IsNullOrEmpty(assetPath) || reverseMap == null)
            return null;

        AssetImpactReport report = new AssetImpactReport
        {
            generatedAtUtc    = DateTime.UtcNow.ToString("o"),
            targetAssetPath   = assetPath,
            targetAssetGuid   = AssetDatabase.AssetPathToGUID(assetPath),
            targetAssetType   = AssetDatabase.GetMainAssetTypeAtPath(assetPath)?.Name ?? "Unknown",
            referencedByPaths = new List<string>(),
            referencesPaths   = new List<string>(),
        };

        // --- Incoming references: who would break? ---
        if (reverseMap.TryGetValue(assetPath, out HashSet<string> referencedBy))
        {
            report.referencedByPaths.AddRange(referencedBy);
            report.referencedByPaths.Sort(StringComparer.OrdinalIgnoreCase);
        }

        // --- Outgoing references: what does this asset depend on? ---
        try
        {
            string[] deps = AssetDatabase.GetDependencies(assetPath, false);
            for (int i = 0; i < deps.Length; i++)
            {
                string dep = deps[i];
                if (!string.Equals(dep, assetPath, StringComparison.OrdinalIgnoreCase)
                    && dep.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    report.referencesPaths.Add(dep);
                }
            }

            report.referencesPaths.Sort(StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            // Leave referencesPaths empty if the asset cannot be read.
        }

        report.directReferenceCount  = report.referencedByPaths.Count;
        report.outgoingReferenceCount = report.referencesPaths.Count;
        report.isSafeToRemove        = report.referencedByPaths.Count == 0;

        return report;
    }
}
