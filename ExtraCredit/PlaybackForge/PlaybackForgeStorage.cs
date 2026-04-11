using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles JSON persistence for recorded input sessions.
/// Works in both Editor and runtime (dev build) contexts.
/// </summary>
public static class PlaybackForgeStorage
{
    private const string DataFolder = "Assets/PlaybackForgeData";

    // -----------------------------------------------------------------------
    // Save
    // -----------------------------------------------------------------------

    /// <summary>
    /// Saves a session to Assets/PlaybackForgeData/ and returns the file path.
    /// </summary>
    public static string SaveSession(RecordedSession session)
    {
        if (session == null)
        {
            Debug.LogWarning("PlaybackForge: Cannot save a null session.");
            return string.Empty;
        }

        EnsureDataFolder();

        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string fileName  = $"session_{session.sessionId}_{timestamp}.json";
        string fullPath  = $"{DataFolder}/{fileName}";

        try
        {
            File.WriteAllText(fullPath, JsonUtility.ToJson(session, true));

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.LogError($"PlaybackForge: Failed to save session. {ex.Message}");
            return string.Empty;
        }
    }

    // -----------------------------------------------------------------------
    // Load
    // -----------------------------------------------------------------------

    /// <summary>
    /// Loads and deserializes a session from the given file path.
    /// Returns null if the file is missing or cannot be parsed.
    /// </summary>
    public static RecordedSession LoadSession(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"PlaybackForge: File not found: {filePath}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<RecordedSession>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"PlaybackForge: Failed to load session from {filePath}. {ex.Message}");
            return null;
        }
    }

    // -----------------------------------------------------------------------
    // List
    // -----------------------------------------------------------------------

    /// <summary>
    /// Returns a sorted list of all session file paths in the data folder.
    /// </summary>
    public static List<string> ListSessionPaths()
    {
        var paths = new List<string>();

        if (!Directory.Exists(DataFolder))
            return paths;

        string[] files = Directory.GetFiles(DataFolder, "session_*.json");
        paths.AddRange(files);
        paths.Sort();
        return paths;
    }

    // -----------------------------------------------------------------------
    // Delete
    // -----------------------------------------------------------------------

    /// <summary>
    /// Deletes a session file (and its .meta sidecar, if present).
    /// </summary>
    public static void DeleteSession(string filePath)
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

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static void EnsureDataFolder()
    {
        if (Directory.Exists(DataFolder))
            return;

#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(DataFolder))
            AssetDatabase.CreateFolder("Assets", "PlaybackForgeData");
#else
        Directory.CreateDirectory(DataFolder);
#endif
    }
}
