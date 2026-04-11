using System;
using System.IO;
using UnityEditor;

public static class BuildHealthStorage
{
    private const string DataFolder = "Assets/BuildHealthData";
    private const string BaselineFileName = "build_health_baseline.json";
    private const string LatestSnapshotFileName = "build_health_latest.json";

    public static string BaselinePath => Path.Combine(DataFolder, BaselineFileName).Replace("\\", "/");
    public static string LatestSnapshotPath => Path.Combine(DataFolder, LatestSnapshotFileName).Replace("\\", "/");

    public static void SaveBaseline(BuildHealthSnapshot snapshot)
    {
        SaveSnapshot(BaselinePath, snapshot);
    }

    public static BuildHealthSnapshot LoadBaseline()
    {
        return LoadSnapshot(BaselinePath);
    }

    public static void SaveLatest(BuildHealthSnapshot snapshot)
    {
        SaveSnapshot(LatestSnapshotPath, snapshot);
    }

    public static BuildHealthSnapshot LoadLatest()
    {
        return LoadSnapshot(LatestSnapshotPath);
    }

    private static void SaveSnapshot(string assetPath, BuildHealthSnapshot snapshot)
    {
        EnsureDataFolder();

        string json = JsonUtility.ToJson(snapshot, true);
        File.WriteAllText(assetPath, json);
        AssetDatabase.Refresh();
    }

    private static BuildHealthSnapshot LoadSnapshot(string assetPath)
    {
        if (!File.Exists(assetPath))
        {
            return null;
        }

        string json = File.ReadAllText(assetPath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonUtility.FromJson<BuildHealthSnapshot>(json);
    }

    private static void EnsureDataFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/BuildHealthData"))
        {
            AssetDatabase.CreateFolder("Assets", "BuildHealthData");
        }
    }
}
