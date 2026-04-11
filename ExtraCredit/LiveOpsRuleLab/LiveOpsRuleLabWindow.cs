using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LiveOpsRuleLabWindow : EditorWindow
{
    private LiveOpsRuleDefinition currentRule;
    private LiveOpsAudienceProfile previewProfile;
    private LiveOpsSimulationResult lastResult;

    private List<string> savedRulePaths = new List<string>();
    private Vector2 savedRulesScroll;
    private Vector2 mainScroll;

    private bool useCurrentUtc = true;
    private string evaluationUtcText;

    [MenuItem("Tools/LiveOps Rule Lab/Simulator")]
    public static void Open()
    {
        LiveOpsRuleLabWindow window = GetWindow<LiveOpsRuleLabWindow>("LiveOps Rule Lab");
        window.minSize = new Vector2(620f, 620f);
        window.Show();
    }

    private void OnEnable()
    {
        currentRule = LiveOpsRuleDefinition.CreateDefault();
        previewProfile = LiveOpsAudienceProfile.CreateExample();
        evaluationUtcText = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        RefreshSavedRules();
    }

    private void OnGUI()
    {
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("LiveOpsRuleLab — Event & Offer Simulator", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Validate your event windows and audience targeting before they reach production.");

        EditorGUILayout.Space();
        DrawRuleSection();

        EditorGUILayout.Space();
        DrawAudienceSection();

        EditorGUILayout.Space();
        DrawSimulationSection();

        EditorGUILayout.Space();
        DrawSavedRulesSection();

        if (lastResult != null)
        {
            EditorGUILayout.Space();
            DrawResultSection();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawRuleSection()
    {
        EditorGUILayout.LabelField("Step 1 — Configure the LiveOps Rule", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        currentRule.displayName = EditorGUILayout.TextField("Display Name", currentRule.displayName);
        currentRule.ruleId = EditorGUILayout.TextField("Rule ID", currentRule.ruleId);
        currentRule.enabled = EditorGUILayout.Toggle("Enabled", currentRule.enabled);
        currentRule.startUtc = EditorGUILayout.TextField("Start UTC", currentRule.startUtc);
        currentRule.endUtc = EditorGUILayout.TextField("End UTC", currentRule.endUtc);
        currentRule.minPlayerLevel = EditorGUILayout.IntField("Min Player Level", currentRule.minPlayerLevel);
        currentRule.maxPlayerLevel = EditorGUILayout.IntField("Max Player Level", currentRule.maxPlayerLevel);
        currentRule.allowAllSegments = EditorGUILayout.Toggle("Allow All Segments", currentRule.allowAllSegments);
        currentRule.allowWeekends = EditorGUILayout.Toggle("Allow Weekends", currentRule.allowWeekends);
        currentRule.notes = EditorGUILayout.TextField("Notes", currentRule.notes);

        EditorGUILayout.Space(4f);
        DrawStringListField("Included Segments", currentRule.includedSegments);
        DrawStringListField("Excluded Segments", currentRule.excludedSegments);
        DrawStringListField("Allowed Regions", currentRule.allowedRegions);

        EditorGUILayout.Space(6f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Rule", GUILayout.Height(24f)))
        {
            currentRule = LiveOpsRuleDefinition.CreateDefault();
            lastResult = null;
        }

        if (GUILayout.Button("Save Rule JSON", GUILayout.Height(24f)))
        {
            string path = LiveOpsRuleStorage.SaveRule(currentRule);
            if (!string.IsNullOrEmpty(path))
                RefreshSavedRules();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawAudienceSection()
    {
        EditorGUILayout.LabelField("Step 2 — Define a Preview Player", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        previewProfile.playerId = EditorGUILayout.TextField("Player ID", previewProfile.playerId);
        previewProfile.playerLevel = EditorGUILayout.IntField("Player Level", previewProfile.playerLevel);
        previewProfile.region = EditorGUILayout.TextField("Region", previewProfile.region);
        DrawStringListField("Player Segments", previewProfile.segments);

        if (GUILayout.Button("Reset Preview Player", GUILayout.Height(22f)))
        {
            previewProfile = LiveOpsAudienceProfile.CreateExample();
            lastResult = null;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSimulationSection()
    {
        EditorGUILayout.LabelField("Step 3 — Simulate Eligibility", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        useCurrentUtc = EditorGUILayout.Toggle("Use Current UTC", useCurrentUtc);
        GUI.enabled = !useCurrentUtc;
        evaluationUtcText = EditorGUILayout.TextField("Evaluation UTC", evaluationUtcText);
        GUI.enabled = true;

        if (GUILayout.Button("Run Simulation", GUILayout.Height(28f)))
        {
            DateTime evaluationUtc = DateTime.UtcNow;
            if (!useCurrentUtc && DateTime.TryParse(evaluationUtcText, out DateTime parsed))
                evaluationUtc = parsed.ToUniversalTime();

            lastResult = LiveOpsRuleValidator.Simulate(currentRule, previewProfile, evaluationUtc);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSavedRulesSection()
    {
        EditorGUILayout.LabelField($"Saved Rules ({savedRulePaths.Count})", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (savedRulePaths.Count == 0)
        {
            EditorGUILayout.LabelField("No saved rules found yet.", EditorStyles.miniLabel);
        }
        else
        {
            savedRulesScroll = EditorGUILayout.BeginScrollView(savedRulesScroll, GUILayout.MaxHeight(180f));
            for (int i = 0; i < savedRulePaths.Count; i++)
            {
                string path = savedRulePaths[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Path.GetFileName(path), EditorStyles.miniLabel);

                if (GUILayout.Button("Load", GUILayout.Width(50f)))
                {
                    LiveOpsRuleDefinition loaded = LiveOpsRuleStorage.LoadRule(path);
                    if (loaded != null)
                    {
                        currentRule = loaded;
                        lastResult = null;
                    }
                }

                if (GUILayout.Button("Delete", GUILayout.Width(56f)))
                {
                    LiveOpsRuleStorage.DeleteRule(path);
                    RefreshSavedRules();
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Refresh Saved Rules", GUILayout.Height(22f)))
            RefreshSavedRules();

        EditorGUILayout.EndVertical();
    }

    private void DrawResultSection()
    {
        EditorGUILayout.LabelField("Simulation Result", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        Color originalColor = GUI.color;
        GUI.color = lastResult.isEligible
            ? new Color(0.35f, 0.8f, 0.45f)
            : new Color(0.95f, 0.45f, 0.2f);

        EditorGUILayout.LabelField(
            lastResult.isEligible ? "PASS — This player would receive the content." : "BLOCKED — This player would not receive the content.",
            EditorStyles.boldLabel);

        GUI.color = originalColor;

        DrawRow("Evaluated At", lastResult.evaluatedAtUtc);
        DrawRow("Validation", lastResult.passedValidation ? "Valid" : "Invalid");
        DrawRow("Schedule Check", lastResult.passesSchedule ? "Pass" : "Fail");
        DrawRow("Audience Check", lastResult.passesAudience ? "Pass" : "Fail");

        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Messages", EditorStyles.boldLabel);
        for (int i = 0; i < lastResult.messages.Count; i++)
            EditorGUILayout.LabelField($"- {lastResult.messages[i]}", EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.EndVertical();
    }

    private void RefreshSavedRules()
    {
        savedRulePaths = LiveOpsRuleStorage.ListRulePaths();
        Repaint();
    }

    private static void DrawStringListField(string label, List<string> values)
    {
        string joined = values != null && values.Count > 0
            ? string.Join(", ", values)
            : string.Empty;

        string updated = EditorGUILayout.TextField(label, joined);
        values.Clear();

        if (string.IsNullOrWhiteSpace(updated))
            return;

        string[] split = updated.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            string trimmed = split[i].Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
                values.Add(trimmed);
        }
    }

    private static void DrawRow(string label, string value)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(label, GUILayout.Width(160f));
            GUILayout.Label(value, EditorStyles.wordWrappedMiniLabel);
        }
    }
}
