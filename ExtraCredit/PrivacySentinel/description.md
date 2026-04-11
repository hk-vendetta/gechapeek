# PrivacySentinel — Privacy & Compliance Audit for Unity

**Find risky data-collection patterns before store review, legal review, or launch.**

PrivacySentinel is an Editor tool for Unity teams that need a fast, practical way to scan their project for privacy-sensitive APIs and data-handling patterns. It audits your C# files, flags common compliance risk areas, and exports a review-ready JSON report.

For most Unity teams, privacy work is reactive. It begins only after a platform rejection, legal review, publisher checklist, or last-minute panic before launch. That is expensive and avoidable.

PrivacySentinel gives you a local first-pass audit before those problems reach production.

---

## What it scans for

PrivacySentinel searches your project for common patterns that often require disclosure, permission handling, or extra legal review:

- `PlayerPrefs`
- `SystemInfo.deviceUniqueIdentifier`
- `Input.location`
- `Microphone`
- `WebCamTexture`
- `Application.RequestUserAuthorization`
- `UnityWebRequest`
- ad-tech references
- analytics references
- general device metadata access via `SystemInfo`

Each finding is categorized and tagged with a severity level (`Low`, `Medium`, or `High`) so you can review the most critical issues first.

---

## What it does

- Scans all `.cs` files under `Assets/`
- Detects privacy-sensitive API usage line by line
- Assigns category and severity to each match
- Shows findings in an Editor window
- Exports the full audit as JSON to `Assets/PrivacyAuditData/`

---

## Who it is for

- **Mobile developers** preparing for App Store or Play Store submission
- **Free-to-play teams** integrating ads, analytics, attribution, or remote config
- **Studios working with publishers** who need a pre-submission audit checklist
- **Technical leads** who want a quick compliance pass before code freeze

---

## What it does NOT do

PrivacySentinel is not legal advice, and it is not a substitute for a privacy policy, DPA review, or region-specific legal analysis. It is a practical engineering audit tool that helps surface likely review points early.

---

## Assets created by this tool

The tool exports JSON reports to `Assets/PrivacyAuditData/`. It does not create prefabs, ScriptableObjects, scenes, or runtime objects.

---

## Suggested retail price: $29.99
