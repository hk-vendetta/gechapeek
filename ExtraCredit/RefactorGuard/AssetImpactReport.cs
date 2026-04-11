using System;
using System.Collections.Generic;

[Serializable]
public class AssetImpactReport
{
    public string generatedAtUtc;
    public string targetAssetPath;
    public string targetAssetGuid;
    public string targetAssetType;

    // True when nothing else references this asset — safe to remove or rename.
    public bool isSafeToRemove;

    // How many assets point at this one (incoming references).
    public int directReferenceCount;

    // How many assets this one points at (outgoing references).
    public int outgoingReferenceCount;

    // Paths of assets that would break if this asset were removed or renamed.
    public List<string> referencedByPaths;

    // Paths of assets this asset depends on.
    public List<string> referencesPaths;
}

[Serializable]
public class BatchImpactSummaryItem
{
    public string assetPath;
    public string assetType;
    public string riskTier;
    public bool isSafeToRemove;
    public int directReferenceCount;
    public int outgoingReferenceCount;
}

[Serializable]
public class BatchImpactReport
{
    public string generatedAtUtc;
    public string sourceLabel;
    public int totalAnalyzedAssetCount;
    public int safeAssetCount;
    public int mediumRiskAssetCount;
    public int highRiskAssetCount;
    public int cautionAssetCount;
    public int totalIncomingReferences;
    public List<BatchImpactSummaryItem> items;

    public BatchImpactReport()
    {
        items = new List<BatchImpactSummaryItem>();
    }
}
