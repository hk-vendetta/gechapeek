using System;

[Serializable]
public class BuildHealthSnapshot
{
    public string timestampUtc;
    public string unityVersion;
    public string activeBuildTarget;

    public int totalAssets;
    public int totalScripts;
    public int totalScenesInBuild;
    public int enabledScenesInBuild;

    public int dependencyAssetCount;
    public long dependencyEstimatedBytes;

    public int textureAssetCount;
    public int materialAssetCount;
    public int shaderAssetCount;

    public static BuildHealthSnapshot CreateEmpty()
    {
        return new BuildHealthSnapshot
        {
            timestampUtc = DateTime.UtcNow.ToString("o")
        };
    }
}
