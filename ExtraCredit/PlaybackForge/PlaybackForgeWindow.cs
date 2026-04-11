using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window — Tools > PlaybackForge > Session Manager.
/// Lists all saved input sessions, shows a frame-by-frame preview,
/// and lets the user delete or re-export sessions.
/// </summary>
public class PlaybackForgeWindow : EditorWindow
{
    // -----------------------------------------------------------------------
    // State
    // -----------------------------------------------------------------------
    private List<string>          sessionPaths   = new List<string>();
    private List<RecordedSession> sessionHeaders = new List<RecordedSession>();

    private int             selectedIndex    = -1;
    private RecordedSession expandedSession;

    private Vector2 sessionListScroll;
    private Vector2 framePreviewScroll;
    private int     framePageOffset;

    private const int FramesPerPage = 60;

    // -----------------------------------------------------------------------
    // Menu entry
    // -----------------------------------------------------------------------

    [MenuItem("Tools/PlaybackForge/Session Manager")]
    public static void Open()
    {
        PlaybackForgeWindow window = GetWindow<PlaybackForgeWindow>("PlaybackForge");
        window.minSize = new Vector2(560f, 520f);
        window.Show();
    }

    // -----------------------------------------------------------------------
    // Lifecycle
    // -----------------------------------------------------------------------

    private void OnEnable()
    {
        RefreshSessionList();
    }

    // -----------------------------------------------------------------------
    // Main GUI
    // -----------------------------------------------------------------------

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PlaybackForge — Session Manager", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Record, review, and reproduce player input sessions for bug reporting and QA.");

        EditorGUILayout.Space();
        DrawSetupSection();

        EditorGUILayout.Space();
        DrawSessionListSection();

        if (expandedSession != null)
        {
            EditorGUILayout.Space();
            DrawSessionDetailSection();
        }
    }

    // -----------------------------------------------------------------------
    // Setup section
    // -----------------------------------------------------------------------

    private void DrawSetupSection()
    {
        EditorGUILayout.LabelField("Step 1 — Add a Recorder to Your Scene", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(
            "Attach an InputRecorder component to any GameObject and enter Play Mode to start recording. " +
            "Enable \"Record On Start\" on the component to record automatically.",
            EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.Space(4f);

        if (GUILayout.Button("Add InputRecorder to Selected GameObject", GUILayout.Height(26f)))
            AddRecorderToSelection();

        EditorGUILayout.EndVertical();
    }

    // -----------------------------------------------------------------------
    // Session list section
    // -----------------------------------------------------------------------

    private void DrawSessionListSection()
    {
        EditorGUILayout.LabelField($"Step 2 — Saved Sessions ({sessionPaths.Count})", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (sessionPaths.Count == 0)
        {
            EditorGUILayout.LabelField(
                "No sessions found. Play with an InputRecorder in the scene to generate one.",
                EditorStyles.wordWrappedMiniLabel);
        }
        else
        {
            sessionListScroll = EditorGUILayout.BeginScrollView(sessionListScroll, GUILayout.MaxHeight(180f));

            for (int i = 0; i < sessionPaths.Count; i++)
            {
                bool isSelected = (i == selectedIndex);
                RecordedSession s = i < sessionHeaders.Count ? sessionHeaders[i] : null;

                string label = s != null
                    ? $"[{s.sessionId}]  Scene: {s.sceneName}  |  {s.totalFrames:N0} frames  ({s.totalDurationSeconds:F1}s)  —  {FormatDate(s.recordedAtUtc)}"
                    : Path.GetFileName(sessionPaths[i]);

                EditorGUILayout.BeginHorizontal();

                bool clicked = GUILayout.Toggle(isSelected, label, "Button");
                if (clicked && !isSelected)
                {
                    selectedIndex   = i;
                    framePageOffset = 0;
                    LoadExpandedSession(i);
                }

                EditorGUILayout.Space(4f);

                if (GUILayout.Button("Delete", GUILayout.Width(56f)))
                    ConfirmDelete(i);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space(4f);

        if (GUILayout.Button("Refresh Session List", GUILayout.Height(22f)))
            RefreshSessionList();

        EditorGUILayout.EndVertical();
    }

    // -----------------------------------------------------------------------
    // Session detail section
    // -----------------------------------------------------------------------

    private void DrawSessionDetailSection()
    {
        RecordedSession s = expandedSession;

        EditorGUILayout.LabelField("Session Detail", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        DrawRow("Session ID",        s.sessionId);
        DrawRow("Recorded At (UTC)", s.recordedAtUtc);
        DrawRow("Unity Version",     s.unityVersion);
        DrawRow("Scene",             s.sceneName);
        DrawRow("Total Frames",      s.totalFrames.ToString("N0"));
        DrawRow("Duration",          $"{s.totalDurationSeconds:F2} seconds");

        EditorGUILayout.Space(6f);

        // --- Frame preview pager ---
        if (s.frames != null && s.frames.Count > 0)
        {
            int totalPages  = Mathf.CeilToInt((float)s.frames.Count / FramesPerPage);
            int currentPage = framePageOffset / FramesPerPage;

            EditorGUILayout.LabelField($"Frame Preview — Page {currentPage + 1} / {totalPages}", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = framePageOffset > 0;
            if (GUILayout.Button("◀ Prev", GUILayout.Width(64f)))
                framePageOffset = Mathf.Max(0, framePageOffset - FramesPerPage);
            GUI.enabled = framePageOffset + FramesPerPage < s.frames.Count;
            if (GUILayout.Button("Next ▶", GUILayout.Width(64f)))
                framePageOffset = Mathf.Min(s.frames.Count - 1, framePageOffset + FramesPerPage);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            framePreviewScroll = EditorGUILayout.BeginScrollView(framePreviewScroll, GUILayout.MaxHeight(180f));
            EditorGUILayout.BeginVertical("box");

            int end = Mathf.Min(framePageOffset + FramesPerPage, s.frames.Count);
            for (int fi = framePageOffset; fi < end; fi++)
            {
                InputFrame f = s.frames[fi];

                string keys = f.keysHeld != null && f.keysHeld.Count > 0
                    ? string.Join("+", f.keysHeld)
                    : "—";

                string mouse = $"({f.mouseX:F0},{f.mouseY:F0})"
                    + (f.mouse0 ? " L" : "")
                    + (f.mouse1 ? " R" : "")
                    + (f.mouse2 ? " M" : "");

                EditorGUILayout.LabelField(
                    $"F{f.frameIndex,6}  t={f.timeSeconds,8:F3}s  Keys: {keys,-22}  Mouse: {mouse}",
                    EditorStyles.miniLabel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("Session contains no frames.", EditorStyles.miniLabel);
        }

        EditorGUILayout.Space(6f);

        if (GUILayout.Button("Export / Re-save Session JSON", GUILayout.Height(24f)))
        {
            string path = PlaybackForgeStorage.SaveSession(expandedSession);
            if (!string.IsNullOrEmpty(path))
                EditorUtility.RevealInFinder(path);
        }

        EditorGUILayout.EndVertical();
    }

    // -----------------------------------------------------------------------
    // Logic helpers
    // -----------------------------------------------------------------------

    private void RefreshSessionList()
    {
        sessionPaths   = PlaybackForgeStorage.ListSessionPaths();
        sessionHeaders = new List<RecordedSession>();

        for (int i = 0; i < sessionPaths.Count; i++)
            sessionHeaders.Add(PlaybackForgeStorage.LoadSession(sessionPaths[i]));

        expandedSession = null;
        selectedIndex   = -1;
        Repaint();
    }

    private void LoadExpandedSession(int index)
    {
        if (index < 0 || index >= sessionPaths.Count)
            return;

        expandedSession = PlaybackForgeStorage.LoadSession(sessionPaths[index]);
        Repaint();
    }

    private void ConfirmDelete(int index)
    {
        string name = Path.GetFileName(sessionPaths[index]);
        if (!EditorUtility.DisplayDialog("Delete Session",
                $"Permanently delete {name}?", "Delete", "Cancel"))
            return;

        PlaybackForgeStorage.DeleteSession(sessionPaths[index]);
        RefreshSessionList();

        if (selectedIndex >= sessionPaths.Count)
        {
            selectedIndex   = -1;
            expandedSession = null;
        }
    }

    private void AddRecorderToSelection()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null)
        {
            EditorUtility.DisplayDialog("PlaybackForge",
                "Select a GameObject in the Hierarchy first, then click this button.", "OK");
            return;
        }

        if (go.GetComponent<InputRecorder>() != null)
        {
            EditorUtility.DisplayDialog("PlaybackForge",
                $"{go.name} already has an InputRecorder component.", "OK");
            return;
        }

        Undo.AddComponent<InputRecorder>(go);
        Debug.Log($"PlaybackForge: InputRecorder added to {go.name}.");
    }

    // -----------------------------------------------------------------------
    // UI helpers
    // -----------------------------------------------------------------------

    private static void DrawRow(string label, string value)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(label, GUILayout.Width(170f));
            GUILayout.Label(value, EditorStyles.wordWrappedMiniLabel);
        }
    }

    private static string FormatDate(string iso8601)
    {
        if (string.IsNullOrEmpty(iso8601) || iso8601.Length < 10)
            return iso8601;

        return iso8601.Substring(0, 10);
    }
}
