# LiveOpsRuleLab — Developer Guide

## Table of contents

1. [What it does](#1-what-it-does)
2. [Requirements](#2-requirements)
3. [Installation](#3-installation)
4. [Opening the simulator](#4-opening-the-simulator)
5. [Creating a rule](#5-creating-a-rule)
6. [Defining a preview player](#6-defining-a-preview-player)
7. [Running a simulation](#7-running-a-simulation)
8. [Understanding the result](#8-understanding-the-result)
9. [Saving and loading rules](#9-saving-and-loading-rules)
10. [Rule data format](#10-rule-data-format)
11. [Recommended workflow](#11-recommended-workflow)
12. [Troubleshooting](#12-troubleshooting)
13. [File overview](#13-file-overview)

---

## 1. What it does

LiveOpsRuleLab is an Editor-only Unity tool that lets you prototype and validate live-ops rules before they are mirrored into production config. It focuses on the kind of targeting logic most teams end up managing in spreadsheets, JSON blobs, or backend dashboards:

- time window checks
- level-based gating
- country / territory filters
- player segment include lists
- player segment exclude lists
- simple weekend enable / disable logic

The tool answers one operational question quickly:

**Would this player receive this content at this time?**

That makes it useful for event QA, design reviews, content planning, and pre-launch validation.

---

## 2. Requirements

- Unity 2021 LTS or later
- No additional packages required
- Editor-only usage

The tool stores rules as JSON and evaluates them locally. No runtime systems or backend services are required.

---

## 3. Installation

Place the four `.cs` files inside any folder under `Assets/Editor/`. Example:

```text
Assets/
  Editor/
    LiveOpsRuleLab/
      LiveOpsRuleDefinition.cs
      LiveOpsRuleValidator.cs
      LiveOpsRuleStorage.cs
      LiveOpsRuleLabWindow.cs
```

Because the window depends on `UnityEditor`, the package should not be installed into runtime folders.

---

## 4. Opening the simulator

Open the window from:

```text
Tools > LiveOps Rule Lab > Simulator
```

The window is split into four sections:

1. Rule configuration
2. Preview player profile
3. Simulation controls
4. Saved rules list

When you open it for the first time, the tool creates a default sample rule and a sample preview player.

---

## 5. Creating a rule

The **Configure the LiveOps Rule** section contains the editable fields for one rule.

### Core fields

| Field | Meaning |
|---|---|
| `Display Name` | Human-readable label for the event or offer |
| `Rule ID` | Stable identifier used in filenames and config sync workflows |
| `Enabled` | Whether the rule can ever pass simulation |
| `Start UTC` | Event start time as ISO 8601 text |
| `End UTC` | Event end time as ISO 8601 text |
| `Min Player Level` | Lowest eligible player level |
| `Max Player Level` | Highest eligible player level |
| `Allow All Segments` | If enabled, segment checks are skipped |
| `Allow Weekends` | If disabled, Saturday and Sunday fail schedule gating |
| `Notes` | Freeform design notes |

### List fields

The following fields accept comma-separated values:

- `Included Segments`
- `Excluded Segments`
- `Allowed Regions`

Example:

```text
Included Segments: returning, spender, high_value
Excluded Segments: churn_risk
Allowed Regions: US, CA, GB
```

If `Allow All Segments` is enabled, the included and excluded segment lists are ignored.

---

## 6. Defining a preview player

The **Define a Preview Player** section lets you create a synthetic player profile against which the rule is evaluated.

Fields:

| Field | Meaning |
|---|---|
| `Player ID` | Arbitrary identifier for the simulated player |
| `Player Level` | Current player level |
| `Region` | Country or territory code |
| `Player Segments` | Comma-separated segmentation labels |

This is intentionally lightweight. The goal is not to mirror your full production user model, only to test the rule logic quickly.

Example preview player:

```text
Player ID: player_001
Player Level: 20
Region: US
Player Segments: spender, returning
```

---

## 7. Running a simulation

The **Simulate Eligibility** section gives you two ways to set the evaluation time:

- `Use Current UTC` enabled: the tool evaluates the rule at the current time
- `Use Current UTC` disabled: you enter a manual timestamp in `Evaluation UTC`

Click **Run Simulation** to evaluate the current rule against the current preview player.

Under the hood, the tool performs these checks in order:

1. Structural validation of the rule
2. Enabled / disabled state
3. Time window membership
4. Weekend block check
5. Player level range check
6. Region eligibility check
7. Segment include / exclude evaluation

The result is stored in a `LiveOpsSimulationResult` object and rendered in the result panel.

---

## 8. Understanding the result

The result panel is color-coded:

- **Green PASS**: the preview player is eligible for the rule at the selected time
- **Orange BLOCKED**: one or more conditions failed

The panel also shows four status rows:

| Row | Meaning |
|---|---|
| `Evaluated At` | UTC timestamp used for the simulation |
| `Validation` | Whether the rule itself is structurally valid |
| `Schedule Check` | Whether the event window matched the evaluation time |
| `Audience Check` | Whether the player matched the audience filters |

Below that, the **Messages** list explains the decision in plain language.

Examples:

- `Rule is disabled.`
- `Evaluation time is outside the active time window.`
- `Player region is not in the allowed region list.`
- `Player is in excluded segment: churn_risk`
- `Simulation passed: this player would see the event or offer.`

---

## 9. Saving and loading rules

Click **Save Rule JSON** to persist the current rule to:

```text
Assets/LiveOpsRuleLabData/
```

File names follow this shape:

```text
rule_{ruleId}_{displayName}.json
```

The **Saved Rules** section lists every saved rule file. Each row includes:

- filename
- `Load` button
- `Delete` button

Use **Refresh Saved Rules** if you edited files outside the window or pulled new rules from version control.

---

## 10. Rule data format

Rules are stored as plain JSON serialized from `LiveOpsRuleDefinition`.

Example:

```json
{
  "ruleId": "c1a8e932",
  "displayName": "Double Drop Weekend",
  "enabled": true,
  "startUtc": "2026-04-10T00:00:00Z",
  "endUtc": "2026-04-13T23:59:59Z",
  "minPlayerLevel": 1,
  "maxPlayerLevel": 999,
  "allowAllSegments": false,
  "allowWeekends": true,
  "includedSegments": ["returning", "spender"],
  "excludedSegments": ["churn_risk"],
  "allowedRegions": ["US", "CA", "GB"],
  "notes": "Weekend progression boost event"
}
```

This makes the tool useful in code reviews: designers can attach the JSON to a ticket, and engineers can verify the rule locally before it is copied into a remote config system.

---

## 11. Recommended workflow

### Event planning

1. Create the rule in LiveOpsRuleLab
2. Enter the intended event window
3. Add included / excluded segments
4. Save the JSON
5. Share it with design, production, and backend owners for review

### QA verification

1. Load the saved rule
2. Enter multiple preview players representing edge cases
3. Simulate current UTC and one or more manual UTC timestamps
4. Verify that each test account passes or fails as expected

### Release review

1. Check that the rule is enabled
2. Confirm that the date range is correct in UTC
3. Confirm that the allowed regions are spelled consistently
4. Confirm that excluded segments are still valid
5. Export or commit the rule JSON as part of the launch checklist

### Postmortem debugging

If a live event reached the wrong audience, load the original rule JSON and simulate it against known player profiles to identify whether the logic or the production data was wrong.

---

## 12. Troubleshooting

**The simulation always fails validation.**
Check `Start UTC` and `End UTC`. They should be valid date strings, ideally in ISO 8601 UTC format such as `2026-04-10T18:00:00Z`.

**The rule blocks on weekends when it should not.**
Make sure `Allow Weekends` is enabled. If it is disabled, Saturday and Sunday fail schedule gating even if the time window is valid.

**A player in the right segment is still blocked.**
Check `Excluded Segments`. Exclusions override inclusions. If the player belongs to an excluded segment, the rule is blocked.

**The allowed region list seems ignored.**
An empty `Allowed Regions` list means all regions are allowed. Add one or more explicit values to activate region filtering.

**Saved rule files do not appear in the list.**
Click **Refresh Saved Rules**. If the folder still appears empty, confirm the files are in `Assets/LiveOpsRuleLabData/` and are named `rule_*.json`.

**Deleting a rule removes the JSON but not the `.meta` file.**
The tool attempts to delete both the file and the `.meta` sidecar. If Unity is currently importing assets, wait for import to finish and try again.

---

## 13. File overview

| File | Role |
|---|---|
| `LiveOpsRuleDefinition.cs` | Serializable rule, preview profile, and simulation result models |
| `LiveOpsRuleValidator.cs` | Validation and rule simulation logic |
| `LiveOpsRuleStorage.cs` | JSON persistence for save / load / list / delete operations |
| `LiveOpsRuleLabWindow.cs` | Editor window UI and interaction layer |
