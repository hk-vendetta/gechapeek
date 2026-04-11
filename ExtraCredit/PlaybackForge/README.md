# PlaybackForge — Developer Guide

## Table of contents

1. [What it does](#1-what-it-does)
2. [Requirements](#2-requirements)
3. [Installation](#3-installation)
4. [Opening the Session Manager](#4-opening-the-session-manager)
5. [Adding the recorder to your scene](#5-adding-the-recorder-to-your-scene)
6. [Recording a session](#6-recording-a-session)
7. [Viewing saved sessions](#7-viewing-saved-sessions)
8. [Reading the frame preview](#8-reading-the-frame-preview)
9. [Extending the tracked key set](#9-extending-the-tracked-key-set)
10. [Exporting and sharing sessions](#10-exporting-and-sharing-sessions)
11. [Recommended QA workflow](#11-recommended-qa-workflow)
12. [Troubleshooting](#12-troubleshooting)
13. [File overview](#13-file-overview)

---

## 1. What it does

PlaybackForge captures raw input events — keyboard, mouse, and axes — every Update frame during Play Mode and persists them as a JSON file. The Session Manager Editor window lets you browse, inspect, and manage all saved sessions without leaving Unity.

The typical use case is bug reproduction: a tester triggers a bug, stops recording, and attaches the session file to the bug report. The developer opens the file and has an exact frame-by-frame account of every key and mouse event that led to the crash or wrong behavior.

---

## 2. Requirements

- Unity 2021 LTS or later
- Unity's legacy **Input Manager** enabled (the tool uses `UnityEngine.Input`)
- No additional packages required
- The `Horizontal` and `Vertical` axes must be defined in your Input Manager (they are present by default in all Unity projects)

> **New Input System:** If your project uses the New Input System package exclusively and has disabled the legacy Input Manager, the `Input.GetAxis`, `Input.GetKey`, and `Input.GetMouseButton` calls in `InputRecorder.cs` will throw exceptions. In that case, either enable the "Both" input handling mode in **Project Settings > Player > Active Input Handling**, or adapt `InputRecorder.cs` to use `UnityEngine.InputSystem` APIs.

---

## 3. Installation

PlaybackForge has both **runtime** and **Editor** components, so the files go into two separate folders:

### Runtime files (must be in a non-Editor folder)

```
Assets/
  Scripts/
    PlaybackForge/
      InputFrame.cs       ← data model (used at runtime and in Editor)
      InputRecorder.cs    ← MonoBehaviour (attached in scenes — must reach builds)
      PlaybackForgeStorage.cs ← file I/O (used at runtime and in Editor)
```

### Editor file (must be in an Editor folder)

```
Assets/
  Editor/
    PlaybackForge/
      PlaybackForgeWindow.cs  ← Editor window only
```

> **Why two folders?** `PlaybackForgeWindow.cs` uses `UnityEditor` APIs that do not exist at runtime. If it is placed outside an `Editor` folder, your build will fail. The other three files use `#if UNITY_EDITOR` guards for Editor-only calls (`AssetDatabase`), so they compile safely in both contexts.

After placing the files, Unity will compile automatically. Check the Console for errors before proceeding.

---

## 4. Opening the Session Manager

```
Tools > PlaybackForge > Session Manager
```

The window is resizable (minimum 560 × 520). It can be docked alongside the Inspector or floated. It opens to the session list, which is populated from `Assets/PlaybackForgeData/`.

---

## 5. Adding the recorder to your scene

There are two ways to attach `InputRecorder` to a scene:

**Option A — Via the Session Manager (recommended)**
1. Open `Tools > PlaybackForge > Session Manager`
2. Select any GameObject in the Hierarchy
3. Click **Add InputRecorder to Selected GameObject**
4. The component is added with full Undo support

**Option B — Manually**
1. Select any GameObject in the Hierarchy
2. Click **Add Component** in the Inspector
3. Find `PlaybackForge > Input Recorder`

Use a dedicated, persistent GameObject (for example, a `Managers` root object) so the recorder is not destroyed between scenes. `[DisallowMultipleComponent]` prevents accidental duplicate recorders on the same object.

---

## 6. Recording a session

### Manual recording

With `InputRecorder` in your scene and Play Mode active, call these methods from any script, UI button, or the Inspector debug toolbar:

```csharp
// Start capturing
GetComponent<InputRecorder>().StartRecording();

// Stop capturing and save
GetComponent<InputRecorder>().StopRecording();
```

`StopRecording()` automatically serializes the session and saves it to `Assets/PlaybackForgeData/`. The saved path is logged to the Console.

### Automatic recording

Check **Record On Start** in the Inspector (the `recordOnStart` public field). Recording begins the instant the component's `Start()` method fires — no code changes needed. Recording stops and saves automatically when the GameObject is destroyed (including when Play Mode ends).

### Session ID

Each session is assigned an 8-character hex ID on creation (e.g., `3f7a1c09`). The saved file name includes this ID: `session_3f7a1c09_20260410_143022.json`. The ID does not change if you re-export the session.

---

## 7. Viewing saved sessions

The **Saved Sessions** list shows all `session_*.json` files found in `Assets/PlaybackForgeData/`. Each row shows:

- Session ID
- Scene name
- Frame count and duration
- Recorded date (UTC)

Click a row to expand it in the **Session Detail** panel below the list. Clicking Refresh rescans the data folder (useful if you added session files externally or via version control sync).

---

## 8. Reading the frame preview

When you expand a session, the **Frame Preview** shows input state for each frame in pages of 60 rows:

```
F    47  t=   0.783s  Keys: W+LeftShift       Mouse: (640,360) L
F    48  t=   0.800s  Keys: W+LeftShift       Mouse: (638,359) L
F    49  t=   0.817s  Keys: W+Space            Mouse: (636,357)
```

| Column | Meaning |
|---|---|
| `F` + number | Frame index within the session (0-based) |
| `t=` | `Time.time` value when the frame was captured |
| `Keys:` | All keys held down during this frame, joined by `+` |
| `Mouse:` | Pixel position + held buttons (`L` left, `R` right, `M` middle) |

Keys pressed or released on a specific frame (transitions) are stored separately in `keysPressed` and `keysReleased` in the JSON, but the preview shows `keysHeld` for readability. Open the raw JSON file if you need the transition data.

Use the **◀ Prev** and **Next ▶** buttons to page through long sessions.

---

## 9. Extending the tracked key set

`InputRecorder.cs` contains a `static readonly KeyCode[]` named `TrackedKeys`:

```csharp
private static readonly KeyCode[] TrackedKeys =
{
    KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
    // ... (26 keys by default)
};
```

To track additional keys, append them to the array. There is no performance concern for up to ~100 keys. Tracking all 500+ Unity KeyCodes is not recommended — use a targeted set relevant to your game.

Example additions for a menu-driven game:

```csharp
KeyCode.I,      // inventory
KeyCode.M,      // map
KeyCode.Delete, // discard
KeyCode.Backspace,
```

---

## 10. Exporting and sharing sessions

### Re-export from the window

Click **Export / Re-save Session JSON** at the bottom of the Session Detail panel. Unity will write a new timestamped copy to `Assets/PlaybackForgeData/` and open the containing folder in Explorer.

### What the JSON contains

The JSON file is human-readable and structured as follows:

```json
{
  "sessionId": "3f7a1c09",
  "recordedAtUtc": "2026-04-10T14:30:22.0000000Z",
  "unityVersion": "6000.0.23f1",
  "sceneName": "Level_01",
  "totalFrames": 1847,
  "totalDurationSeconds": 30.78,
  "frames": [
    {
      "frameIndex": 0,
      "timeSeconds": 0.016,
      "mouseX": 640.0,
      "mouseY": 360.0,
      "mouse0": false,
      "mouse1": false,
      "mouse2": false,
      "axisHorizontal": 0.0,
      "axisVertical": 0.0,
      "keysPressed": [],
      "keysReleased": [],
      "keysHeld": []
    },
    ...
  ]
}
```

Sessions with 1,000 frames at minimal input typically produce files of ~200–400 KB. Sessions with heavy simultaneous input may be larger.

### Attaching to a bug report

Export the session and attach `session_{id}_{timestamp}.json` directly to your issue tracker. Include the Unity version and build target. Developers can load the file with `PlaybackForgeStorage.LoadSession(path)` from any script to inspect it programmatically.

### Excluding from version control

Add these lines to your `.gitignore` to keep session data out of your repository:

```
Assets/PlaybackForgeData/
Assets/PlaybackForgeData.meta
```

---

## 11. Recommended QA workflow

**Setting up a QA build:**
1. Create a persistent `Managers` GameObject in your first scene
2. Attach `InputRecorder` with **Record On Start** enabled
3. Distribute the build to testers

**When a tester finds a bug:**
1. Tester notes the session file in `Assets/PlaybackForgeData/` (or triggers a custom UI button that calls `StopRecording()`)
2. Tester attaches the session JSON to the bug report along with a description of what they were doing

**Developer reproduces:**
1. Open `Tools > PlaybackForge > Session Manager`
2. Refresh the session list
3. Click the session to expand the frame preview
4. Use the frame log to identify the exact moment the bug was triggered
5. Manually reproduce the input sequence — or step through the log to identify the suspicious frame

**Regression testing:**
If your project uses test harnesses or automation, you can load a session file programmatically:

```csharp
RecordedSession session = PlaybackForgeStorage.LoadSession("Assets/PlaybackForgeData/session_3f7a1c09.json");
foreach (InputFrame frame in session.frames)
{
    // Feed frame data into your custom input simulation layer
}
```

---

## 12. Troubleshooting

**Session is not saved when I stop Play Mode.**
Check that `InputRecorder` is in the scene from the start of Play Mode. If the component's GameObject is destroyed before Play Mode ends (e.g., scene transition), `OnDestroy` triggers the save — this is correct behavior. If the component is never initialized, nothing is saved.

**"Input.GetAxis: Horizontal axis not defined" error.**
The Horizontal and Vertical axes are not defined in your Input Manager. Go to **Edit > Project Settings > Input Manager** and ensure both axes exist. They are present in all default Unity projects.

**Session file appears in the Inspector but has 0 frames.**
Recording started but `Update()` never ran, which typically means the GameObject was disabled. Ensure the GameObject and all its parent objects are active in the Hierarchy during Play Mode.

**"Record On Start" did not begin recording.**
`Start()` is called after the first frame. If your game transitions to a new scene on the first frame, the recorder may not have started in time. Use `Awake()` instead: change `private void Start()` to `private void Awake()` in `InputRecorder.cs`.

**Session list in the window is empty after recording.**
The data folder (`Assets/PlaybackForgeData/`) may not exist or `AssetDatabase.Refresh()` may not have fired yet. Click **Refresh Session List** in the window. If the folder still does not appear, close and reopen the window.

**Axis values are always 0.**
The legacy Input Manager may be disabled. Go to **Edit > Project Settings > Player** and set **Active Input Handling** to `Input Manager (Old)` or `Both`.

---

## 13. File overview

| File | Role | Folder |
|---|---|---|
| `InputFrame.cs` | `InputFrame` and `RecordedSession` data models | `Assets/Scripts/PlaybackForge/` |
| `InputRecorder.cs` | MonoBehaviour — records input each Update; saves on stop | `Assets/Scripts/PlaybackForge/` |
| `PlaybackForgeStorage.cs` | Saves, loads, lists, and deletes session JSON files | `Assets/Scripts/PlaybackForge/` |
| `PlaybackForgeWindow.cs` | Editor window — session list, frame preview, export | `Assets/Editor/PlaybackForge/` |
