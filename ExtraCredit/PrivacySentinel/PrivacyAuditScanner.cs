using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PrivacyAuditScanner
{
    private struct ScanRule
    {
        public string Pattern;
        public string Severity;
        public string Category;
        public string Message;
    }

    private static readonly ScanRule[] Rules =
    {
        new ScanRule
        {
            Pattern = "PlayerPrefs",
            Severity = "Medium",
            Category = "Local Data Storage",
            Message = "Review whether user data written to PlayerPrefs needs retention, consent, or deletion handling."
        },
        new ScanRule
        {
            Pattern = "deviceUniqueIdentifier",
            Severity = "High",
            Category = "Device Identifier",
            Message = "Device identifiers may require disclosure, consent, or platform-specific handling."
        },
        new ScanRule
        {
            Pattern = "Input.location",
            Severity = "High",
            Category = "Location Data",
            Message = "Location access should be reviewed for permission flows and jurisdiction-specific compliance."
        },
        new ScanRule
        {
            Pattern = "Microphone",
            Severity = "High",
            Category = "Audio Capture",
            Message = "Microphone access should be justified and paired with explicit permission messaging."
        },
        new ScanRule
        {
            Pattern = "WebCamTexture",
            Severity = "High",
            Category = "Camera Access",
            Message = "Camera capture should be reviewed for permission prompts and data use disclosure."
        },
        new ScanRule
        {
            Pattern = "RequestUserAuthorization",
            Severity = "Medium",
            Category = "Permission Request",
            Message = "Verify that runtime permission requests are accompanied by user-facing rationale."
        },
        new ScanRule
        {
            Pattern = "UnityWebRequest",
            Severity = "Medium",
            Category = "Network Transmission",
            Message = "Outbound network calls should be reviewed for personal data handling and disclosure."
        },
        new ScanRule
        {
            Pattern = "Advertisement",
            Severity = "Medium",
            Category = "Ad Technology",
            Message = "Ad SDK usage may require regional consent handling and privacy-policy coverage."
        },
        new ScanRule
        {
            Pattern = "Analytics",
            Severity = "Medium",
            Category = "Analytics",
            Message = "Analytics instrumentation should be reviewed for opt-in, opt-out, and data minimization requirements."
        },
        new ScanRule
        {
            Pattern = "SystemInfo",
            Severity = "Low",
            Category = "Device Metadata",
            Message = "Device metadata collection should be reviewed to ensure only necessary fields are gathered."
        },
    };

    public static PrivacyAuditReport RunScan(string projectRoot)
    {
        var report = new PrivacyAuditReport
        {
            generatedAtUtc = DateTime.UtcNow.ToString("o"),
            unityVersion = Application.unityVersion,
        };

        if (string.IsNullOrWhiteSpace(projectRoot))
            return report;

        string assetsPath = Path.Combine(projectRoot, "Assets");
        if (!Directory.Exists(assetsPath))
            return report;

        string[] files = Directory.GetFiles(assetsPath, "*.cs", SearchOption.AllDirectories);
        report.scannedFileCount = files.Length;

        for (int i = 0; i < files.Length; i++)
            ScanFile(files[i], report.findings);

        report.findingCount = report.findings.Count;
        return report;
    }

    private static void ScanFile(string filePath, List<PrivacyAuditFinding> findings)
    {
        string[] lines;
        try
        {
            lines = File.ReadAllLines(filePath);
        }
        catch
        {
            return;
        }

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            for (int ruleIndex = 0; ruleIndex < Rules.Length; ruleIndex++)
            {
                ScanRule rule = Rules[ruleIndex];
                if (line.IndexOf(rule.Pattern, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                findings.Add(new PrivacyAuditFinding
                {
                    severity = rule.Severity,
                    category = rule.Category,
                    filePath = NormalizePath(filePath),
                    lineNumber = lineIndex + 1,
                    pattern = rule.Pattern,
                    message = rule.Message,
                    linePreview = line.Trim(),
                });
            }
        }
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }
}
