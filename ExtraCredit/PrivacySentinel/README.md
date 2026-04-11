# PrivacySentinel — Developer Guide

## Table of contents

1. [What it does](#1-what-it-does)
2. [Requirements](#2-requirements)
3. [Installation](#3-installation)
4. [Opening the audit window](#4-opening-the-audit-window)
5. [Running your first audit](#5-running-your-first-audit)
6. [Understanding findings](#6-understanding-findings)
7. [Severity model](#7-severity-model)
8. [Exporting reports](#8-exporting-reports)
9. [What gets scanned](#9-what-gets-scanned)
10. [Extending the rule set](#10-extending-the-rule-set)
11. [Recommended workflow](#11-recommended-workflow)
12. [Troubleshooting](#12-troubleshooting)
13. [File overview](#13-file-overview)

---

## 1. What it does

PrivacySentinel is an Editor-only source scanner for Unity projects. It looks for code patterns commonly associated with privacy-sensitive behavior, including identifiers, location, camera, microphone, analytics, advertising, and outbound network calls.

The goal is not to block development. The goal is to give engineering, production, and publishing teams an actionable first-pass review before a release, store submission, or publisher compliance check.

Each match becomes a structured finding with:

- severity
- category
- file path
- line number
- matched pattern
- human-readable explanation
- code preview

---

## 2. Requirements

- Unity 2021 LTS or later
- No additional packages required
- Editor-only usage

The tool reads C# source files under `Assets/` and produces a JSON report. It does not analyze binaries, native plugins, or compiled DLLs.

---

## 3. Installation

Place the four source files into any folder under `Assets/Editor/`. Example:

```text
Assets/
  Editor/
    PrivacySentinel/
      PrivacyAuditFinding.cs
      PrivacyAuditScanner.cs
      PrivacyAuditStorage.cs
      PrivacyAuditWindow.cs
```

Because the tool depends on `UnityEditor`, it should not be placed in runtime folders.

---

## 4. Opening the audit window

Open the tool from:

```text
Tools > Privacy Sentinel > Compliance Audit
```

The window contains:

1. Scan controls
2. Summary panel
3. Findings list

On enable, the tool defaults the project root to Unity's current working directory.

---

## 5. Running your first audit

Click **Run Privacy Audit**.

The scanner will:

1. Resolve the `Assets/` folder from the current project root
2. Enumerate all `.cs` files recursively
3. Read each file line by line
4. Compare each line against the built-in scan rules
5. Produce a `PrivacyAuditReport`

The summary panel will then show:

- generation timestamp
- Unity version
- number of scanned source files
- total findings

This gives you an immediate view of how much privacy-sensitive surface area exists in the project.

---

## 6. Understanding findings

Every finding includes these fields:

| Field | Meaning |
|---|---|
| `Severity` | Priority level for review |
| `Category` | Type of privacy-sensitive behavior |
| `File` | File path of the matched source line |
| `Line` | 1-based line number |
| `Pattern` | The rule keyword or API name that matched |
| `Message` | Why the pattern matters |
| `Code` | Trimmed preview of the matching line |

The findings list is designed for quick triage, not full legal interpretation. A finding means "review this area", not necessarily "this is a violation".

---

## 7. Severity model

PrivacySentinel uses three severity levels.

### High

Used for APIs with direct privacy or permission implications, such as:

- `deviceUniqueIdentifier`
- `Input.location`
- `Microphone`
- `WebCamTexture`

These usually warrant immediate review because they can trigger platform disclosure, permission prompts, or jurisdiction-specific compliance obligations.

### Medium

Used for patterns that often involve consent or disclosure depending on implementation, such as:

- `PlayerPrefs`
- `UnityWebRequest`
- analytics references
- ad-tech references
- `RequestUserAuthorization`

These are not inherently unsafe, but they often require process and policy review.

### Low

Used for supporting or contextual APIs such as general `SystemInfo` access. These are less likely to be problematic on their own, but can matter if combined with analytics or profiling logic.

---

## 8. Exporting reports

After a scan, click **Export Report to JSON**.

The tool writes a file to:

```text
Assets/PrivacyAuditData/privacy_audit_{timestamp}.json
```

This file can be:

- attached to a release checklist
- reviewed by a publisher or producer
- handed to legal for follow-up
- committed temporarily for code-review context

The report is serialized from `PrivacyAuditReport` and contains the complete findings list.

The folder can be ignored in version control if desired:

```text
Assets/PrivacyAuditData/
Assets/PrivacyAuditData.meta
```

---

## 9. What gets scanned

The scanner currently reviews:

- every `.cs` file under `Assets/`

It does **not** currently scan:

- `Packages/`
- `ProjectSettings/`
- native plugins
- precompiled assemblies
- YAML scene or prefab files
- backend config outside the Unity project

That limitation is intentional. The current version is focused on fast, low-friction source review inside the main Unity codebase.

---

## 10. Extending the rule set

The scan rules live in `PrivacyAuditScanner.cs` as a static `ScanRule[]` array.

Each rule has:

- `Pattern`
- `Severity`
- `Category`
- `Message`

Example:

```csharp
new ScanRule
{
    Pattern = "Microphone",
    Severity = "High",
    Category = "Audio Capture",
    Message = "Microphone access should be justified and paired with explicit permission messaging."
}
```

To add a new rule, append a new entry to the array.

Good candidates for project-specific rules:

- third-party SDK namespaces
- attribution providers
- fingerprinting helpers
- custom analytics wrappers
- age-gate bypass logic
- cloud-save or account-linking code

---

## 11. Recommended workflow

### Before a store submission

1. Run a full audit
2. Export the report
3. Review all high-severity items first
4. Confirm each item has permission handling and policy coverage

### Before a publisher milestone

1. Export the latest report
2. Share it with production and release management
3. Confirm that ad, analytics, and identifier usage is accounted for in documentation

### During integration of a new SDK

1. Add one or more scanner rules for the SDK's namespace
2. Run the audit after integration
3. Use the findings list to make sure all touchpoints are documented

### Before code freeze

1. Run the scanner as part of the release checklist
2. Archive the JSON alongside other release artifacts
3. Resolve or explicitly waive any open high-severity findings

---

## 12. Troubleshooting

**The audit returns zero findings, but I know the project uses analytics.**
The current rules are string-based. If your analytics wrapper hides the underlying SDK names, add a custom rule for your wrapper class or namespace.

**File paths look absolute instead of project-relative.**
The scanner currently normalizes slashes but preserves the underlying path string. If you want project-relative output, update `NormalizePath()` in `PrivacyAuditScanner.cs`.

**The scanner is too noisy.**
Remove or refine rules that are too broad for your codebase. For example, if `SystemInfo` appears everywhere for benign reasons, lower its severity or delete the rule.

**A finding is technically correct but not a real issue.**
That is expected. PrivacySentinel is an audit aid, not a final compliance authority. Treat findings as review prompts.

**I need to scan packages or YAML assets too.**
This version does not do that yet. Extend `RunScan()` to include more directories or file types if your project needs a broader review pass.

---

## 13. File overview

| File | Role |
|---|---|
| `PrivacyAuditFinding.cs` | Serializable models for individual findings and full reports |
| `PrivacyAuditScanner.cs` | Static scan engine and built-in rule set |
| `PrivacyAuditStorage.cs` | JSON export support for audit reports |
| `PrivacyAuditWindow.cs` | Editor window for running scans and reviewing results |
