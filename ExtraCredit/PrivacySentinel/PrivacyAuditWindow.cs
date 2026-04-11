using UnityEditor;
using UnityEngine;

public class PrivacyAuditWindow : EditorWindow
{
    private PrivacyAuditReport currentReport;
    private Vector2 findingsScroll;
    private string projectRoot = string.Empty;

    [MenuItem("Tools/Privacy Sentinel/Compliance Audit")]
    public static void Open()
    {
        PrivacyAuditWindow window = GetWindow<PrivacyAuditWindow>("Privacy Sentinel");
        window.minSize = new Vector2(700f, 560f);
        window.Show();
    }

    private void OnEnable()
    {
        if (string.IsNullOrWhiteSpace(projectRoot))
            projectRoot = System.IO.Directory.GetCurrentDirectory();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PrivacySentinel — Compliance Audit", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Scan your project for common privacy-sensitive APIs before release or review.");

        EditorGUILayout.Space();
        DrawControls();

        if (currentReport != null)
        {
            EditorGUILayout.Space();
            DrawSummary();
            EditorGUILayout.Space();
            DrawFindings();
        }
    }

    private void DrawControls()
    {
        EditorGUILayout.LabelField("Scan Controls", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Project Root", projectRoot, EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.Space(4f);
        if (GUILayout.Button("Run Privacy Audit", GUILayout.Height(28f)))
            currentReport = PrivacyAuditScanner.RunScan(projectRoot);

        GUI.enabled = currentReport != null;
        if (GUILayout.Button("Export Report to JSON", GUILayout.Height(24f)))
        {
            string path = PrivacyAuditStorage.ExportReport(currentReport);
            if (!string.IsNullOrEmpty(path))
                EditorUtility.RevealInFinder(path);
        }
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }

    private void DrawSummary()
    {
        EditorGUILayout.LabelField("Audit Summary", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        DrawRow("Generated At", currentReport.generatedAtUtc);
        DrawRow("Unity Version", currentReport.unityVersion);
        DrawRow("Scanned Files", currentReport.scannedFileCount.ToString("N0"));
        DrawRow("Findings", currentReport.findingCount.ToString("N0"));
        EditorGUILayout.EndVertical();
    }

    private void DrawFindings()
    {
        EditorGUILayout.LabelField($"Findings ({currentReport.findings.Count})", EditorStyles.boldLabel);
        findingsScroll = EditorGUILayout.BeginScrollView(findingsScroll);

        for (int i = 0; i < currentReport.findings.Count; i++)
        {
            PrivacyAuditFinding finding = currentReport.findings[i];

            EditorGUILayout.BeginVertical("box");
            DrawSeverityBanner(finding.severity);
            DrawRow("Category", finding.category);
            DrawRow("File", finding.filePath);
            DrawRow("Line", finding.lineNumber.ToString());
            DrawRow("Pattern", finding.pattern);
            EditorGUILayout.LabelField("Message", finding.message, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.LabelField("Code", finding.linePreview, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(4f);
        }

        EditorGUILayout.EndScrollView();
    }

    private static void DrawSeverityBanner(string severity)
    {
        Color originalColor = GUI.color;

        if (severity == "High")
            GUI.color = new Color(0.9f, 0.35f, 0.25f);
        else if (severity == "Medium")
            GUI.color = new Color(0.95f, 0.65f, 0.2f);
        else
            GUI.color = new Color(0.5f, 0.75f, 0.95f);

        EditorGUILayout.LabelField($"{severity.ToUpperInvariant()} PRIORITY", EditorStyles.boldLabel);
        GUI.color = originalColor;
    }

    private static void DrawRow(string label, string value)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(label, GUILayout.Width(120f));
            GUILayout.Label(value, EditorStyles.wordWrappedMiniLabel);
        }
    }
}
