using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Snapshot of all tracked input state for a single Update frame.
/// </summary>
[Serializable]
public class InputFrame
{
    public int   frameIndex;
    public float timeSeconds;

    // Mouse state
    public float mouseX;
    public float mouseY;
    public bool  mouse0;   // primary / left
    public bool  mouse1;   // secondary / right
    public bool  mouse2;   // middle

    // Legacy input axes
    public float axisHorizontal;
    public float axisVertical;

    // Key transitions and held state for this frame.
    public List<string> keysPressed;   // went down this frame
    public List<string> keysReleased;  // went up this frame
    public List<string> keysHeld;      // currently held

    public InputFrame()
    {
        keysPressed  = new List<string>();
        keysReleased = new List<string>();
        keysHeld     = new List<string>();
    }
}

/// <summary>
/// A complete record of one play session: metadata header + ordered list of frames.
/// </summary>
[Serializable]
public class RecordedSession
{
    public string sessionId;
    public string recordedAtUtc;
    public string unityVersion;
    public string sceneName;
    public int    totalFrames;
    public float  totalDurationSeconds;
    public List<InputFrame> frames;

    public static RecordedSession CreateNew(string scene)
    {
        return new RecordedSession
        {
            sessionId            = Guid.NewGuid().ToString("N").Substring(0, 8),
            recordedAtUtc        = DateTime.UtcNow.ToString("o"),
            unityVersion         = Application.unityVersion,
            sceneName            = scene,
            totalFrames          = 0,
            totalDurationSeconds = 0f,
            frames               = new List<InputFrame>(),
        };
    }
}
