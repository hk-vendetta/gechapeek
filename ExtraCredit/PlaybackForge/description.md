# PlaybackForge — Input Session Recorder

**Capture exactly what the player did. Reproduce any bug, instantly.**

PlaybackForge is a lightweight Unity tool that records every input event during a play session — keyboard, mouse, axes — and saves it as a readable JSON file. Open the Session Manager window to browse saved sessions, inspect them frame by frame, and re-export them for your team.

The hardest bugs to fix are the ones you cannot reproduce. PlaybackForge puts an end to the "I can't repro this" cycle.

---

## What it does

PlaybackForge has two parts that work together:

**InputRecorder** (MonoBehaviour)  
Attach it to any GameObject in your scene. Press Play. It captures input every frame — key transitions, held keys, mouse position and buttons, and both input axes — and saves a complete session file automatically when recording stops.

**Session Manager** (Editor window)  
Brings every saved session into a single view. For each session you can see: which scene, how many frames, how long it ran, and a scrollable frame-by-frame playback log. Delete old sessions, re-export a session to share with a teammate, or use the frame preview to manually walk through input history to identify the moment a bug was triggered.

---

## Key features

| Feature | Description |
|---|---|
| One-component setup | Attach InputRecorder to any GameObject — no code changes to your game |
| Automatic save on stop | Session saves automatically when Play Mode exits or StopRecording() is called |
| Record On Start toggle | Check a box to begin recording the instant Play Mode starts |
| 26 tracked keys | WASD, arrows, Space, Enter, Escape, modifier keys, Q/E/R/F, digit row 1–5 |
| Mouse tracking | Position (X, Y) + left / right / middle button state |
| Axis tracking | Horizontal and Vertical axes (Input.GetAxis) |
| Frame-by-frame preview | Paginated view: 60 frames per page, forward and back |
| Session metadata | Scene name, Unity version, recorded-at timestamp, total frames, total duration |
| Session Manager window | List, inspect, delete, and re-export sessions from one place |
| JSON export | Human-readable, version-control-friendly output |

---

## Who is it for

- **QA testers** who want to attach a playback log to a bug report instead of describing input step-by-step
- **Solo developers** who need to reproduce flaky edge-case bugs without relying on memory
- **Team leads** who want permanent records of specific input scenarios for regression testing
- **Any developer** who has ever heard "it just stopped working and I don't know what I did"

---

## Important: what PlaybackForge does and does not do

PlaybackForge **records** and **displays** input. It uses Unity's legacy `Input` class and does not require the New Input System.

PlaybackForge does **not** automatically replay input back into the game engine. Unity does not expose a public API for injecting synthetic input at runtime in a portable way. The recorded session JSON file is designed to be used as:

- A manual reference while you walk through the reproduction steps yourself
- An attachment on a bug report
- A seed for a custom test harness if your project uses a replay-friendly architecture

---

## Assets created by this tool

All session files are saved to `Assets/PlaybackForgeData/session_{id}_{timestamp}.json`. The data folder contains only local analysis artifacts and can be excluded from version control:

```
Assets/PlaybackForgeData/
Assets/PlaybackForgeData.meta
```

No prefabs, ScriptableObjects, materials, or runtime modifications are created.

---

## Suggested retail price: $19.99
