using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to any GameObject.  Call StartRecording() / StopRecording() at
/// runtime (or set Record On Start to begin automatically on Awake).
/// When recording stops, the session is automatically saved via
/// PlaybackForgeStorage.  The saved path is logged to the Console.
/// </summary>
[AddComponentMenu("PlaybackForge/Input Recorder")]
[DisallowMultipleComponent]
public class InputRecorder : MonoBehaviour
{
    // -----------------------------------------------------------------------
    // Tracked key set — practical coverage for most genres.
    // Extend this array to track additional keys.
    // -----------------------------------------------------------------------
    private static readonly KeyCode[] TrackedKeys =
    {
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.Space, KeyCode.Return, KeyCode.Escape,
        KeyCode.LeftShift, KeyCode.RightShift,
        KeyCode.LeftControl, KeyCode.RightControl,
        KeyCode.LeftAlt, KeyCode.RightAlt,
        KeyCode.Tab, KeyCode.F,
        KeyCode.Q, KeyCode.E, KeyCode.R,
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5,
    };

    // -----------------------------------------------------------------------
    // Inspector
    // -----------------------------------------------------------------------
    [Tooltip("Begin recording automatically when the component starts.")]
    public bool recordOnStart = false;

    // -----------------------------------------------------------------------
    // State
    // -----------------------------------------------------------------------
    private bool            isRecording;
    private RecordedSession session;

    // -----------------------------------------------------------------------
    // Lifecycle
    // -----------------------------------------------------------------------

    private void Start()
    {
        if (recordOnStart)
            StartRecording();
    }

    private void OnDestroy()
    {
        if (isRecording)
            StopRecording();
    }

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    public bool IsRecording => isRecording;

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("PlaybackForge: Already recording. Call StopRecording first.");
            return;
        }

        session = RecordedSession.CreateNew(SceneManager.GetActiveScene().name);
        isRecording = true;
        Debug.Log("PlaybackForge: Recording started.");
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("PlaybackForge: Not currently recording.");
            return;
        }

        isRecording = false;

        if (session != null)
        {
            session.totalFrames          = session.frames.Count;
            session.totalDurationSeconds = session.frames.Count > 0
                ? session.frames[session.frames.Count - 1].timeSeconds
                : 0f;

            string savedPath = PlaybackForgeStorage.SaveSession(session);
            Debug.Log($"PlaybackForge: Stopped. {session.totalFrames} frames saved to {savedPath}");
            session = null;
        }
    }

    // -----------------------------------------------------------------------
    // Frame capture
    // -----------------------------------------------------------------------

    private void Update()
    {
        if (!isRecording || session == null)
            return;

        InputFrame frame = new InputFrame
        {
            frameIndex       = session.frames.Count,
            timeSeconds      = Time.time,
            mouseX           = Input.mousePosition.x,
            mouseY           = Input.mousePosition.y,
            mouse0           = Input.GetMouseButton(0),
            mouse1           = Input.GetMouseButton(1),
            mouse2           = Input.GetMouseButton(2),
            axisHorizontal   = Input.GetAxis("Horizontal"),
            axisVertical     = Input.GetAxis("Vertical"),
        };

        for (int i = 0; i < TrackedKeys.Length; i++)
        {
            KeyCode key     = TrackedKeys[i];
            string  keyName = key.ToString();

            if (Input.GetKeyDown(key)) frame.keysPressed.Add(keyName);
            if (Input.GetKeyUp(key))   frame.keysReleased.Add(keyName);
            if (Input.GetKey(key))     frame.keysHeld.Add(keyName);
        }

        session.frames.Add(frame);
    }
}
