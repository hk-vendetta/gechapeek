using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public static class BuildHealthAnalyzer
{
    public static BuildHealthSnapshot CaptureCurrent()
    {
        BuildHealthSnapshot snapshot = BuildHealthSnapshot.CreateEmpty();

        snapshot.unityVersion = Application.unityVersion;
        snapshot.activeBuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();

        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        int totalAssets = 0;
        int totalScripts = 0;

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string path = assetPaths[i];
            if (!path.StartsWith("Assets/", StringComparison.Ordinal))
            {
                continue;
            }

            totalAssets++;

            if (path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                totalScripts++;
            }
        }

        snapshot.totalAssets = totalAssets;
        snapshot.totalScripts = totalScripts;

        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        snapshot.totalScenesInBuild = scenes.Length;

        List<string> enabledScenePaths = new List<string>();
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].enabled)
            {
                enabledScenePaths.Add(scenes[i].path);
            }
        }

        snapshot.enabledScenesInBuild = enabledScenePaths.Count;

        HashSet<string> dependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < enabledScenePaths.Count; i++)
        {
            string[] sceneDependencies = AssetDatabase.GetDependencies(enabledScenePaths[i], true);
            for (int j = 0; j < sceneDependencies.Length; j++)
            {
                string depPath = sceneDependencies[j];
                if (!depPath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    continue;
                }

                dependencies.Add(depPath);
            }
        }

        snapshot.dependencyAssetCount = dependencies.Count;

        long estimatedBytes = 0;
        foreach (string depPath in dependencies)
        {
            try
            {
                FileInfo info = new FileInfo(depPath);
                if (info.Exists)
                {
                    estimatedBytes += info.Length;
                }
            }
            catch
            {
                // Ignore inaccessible file paths while preserving overall snapshot capture.
            }
        }

        snapshot.dependencyEstimatedBytes = estimatedBytes;

        snapshot.textureAssetCount = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" }).Length;
        snapshot.materialAssetCount = AssetDatabase.FindAssets("t:Material", new[] { "Assets" }).Length;
        snapshot.shaderAssetCount = AssetDatabase.FindAssets("t:Shader", new[] { "Assets" }).Length;

        return snapshot;
    }
}
