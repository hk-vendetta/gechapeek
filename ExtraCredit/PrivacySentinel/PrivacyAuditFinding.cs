using System;
using System.Collections.Generic;

[Serializable]
public class PrivacyAuditFinding
{
    public string severity;
    public string category;
    public string filePath;
    public int lineNumber;
    public string pattern;
    public string message;
    public string linePreview;
}

[Serializable]
public class PrivacyAuditReport
{
    public string generatedAtUtc;
    public string unityVersion;
    public int scannedFileCount;
    public int findingCount;
    public List<PrivacyAuditFinding> findings;

    public PrivacyAuditReport()
    {
        findings = new List<PrivacyAuditFinding>();
    }
}
