using System;
using UnityEngine;
using System.Collections;

public class GameDebugConsole : MonoBehaviour
{
    static GameDebugConsole mInstance = null;
    private Rect consoleRect;
    private Vector2 _consoleScrollPos;
    private bool _hide;
    private Vector2 _logScrollPos;
    private System.Text.StringBuilder _logBuilder;

    public static void Setup()
    {
        CreateInstance();
    }

    public static void Dispose()
    {
        if (mInstance != null)
        {
            GameObject.Destroy(mInstance.gameObject);
            mInstance = null;
        }
    }

    private static void CreateInstance()
    {
        if (mInstance == null)
        {
            GameObject go = new GameObject("_GameDebugConsole");
            mInstance = go.AddComponent<GameDebugConsole>();
            DontDestroyOnLoad(go);
        }
    }

    public static void Log(string condition, string stackTrace, LogType type)
    {
        if (mInstance != null)
        {
            mInstance.AddLog(condition, stackTrace, type);
        }
    }

    public void AddLog(string condition, string stackTrace, LogType type)
    {
        if (_logBuilder != null)
        {
            _logBuilder.AppendFormat("{0} <{1}>\n{2}\n{3}\n", DateTime.Now, type, condition, stackTrace);
        }
    }

    void Start()
    {
        consoleRect = new Rect(0f, 0f, Screen.width * 0.8f, Screen.height * 0.8f);
        _logBuilder = new System.Text.StringBuilder(1024);

        //FPS Init
        StartCoroutine(RefreshFPS());
        //FPS Init

        //Memory Init
        StartCoroutine(RefreshMemory());
        //Memory Init

        Application.logMessageReceived += AddLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= AddLog;
    }

    void Update()
    {
        //FPS Update
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        //FPS Update

        //Socket Update
        _sendInfo = string.Format("SendCount: {0} SendBytes: {1}", HaConnector.TotalSendCount, AppStringHelper.FormatBytes(HaConnector.TotalSendBytes));
        _receiveInfo = string.Format("ReceiveCount: {0} ReceiveBytes: {1}", HaConnector.TotalReceiveCount, AppStringHelper.FormatBytes(HaConnector.TotalReceiveBytes));
        socketColor = SocketManager.IsOnLink ? Color.green : Color.red;
        //Socket Update
    }

    void OnGUI()
    {
        GUILayout.BeginArea(consoleRect);
        if (GUILayout.Button("Hide", GUILayout.Height(50f)))
        {
            _hide = !_hide;
        }

        if (!_hide)
        {
            _consoleScrollPos = GUILayout.BeginScrollView(_consoleScrollPos, "TextArea");

            DrawFPSInfo();
            DrawMemoryInfo();
            DrawSocketInfo();
            //DrawAssetInfo ();
            DrawStopwatchInfo();
            DrawLogPanel();
            GUILayout.EndScrollView();
        }
        GUILayout.EndArea();
    }

    void DrawLogPanel()
    {
        if (GUILayout.Button("Clear"))
        {
            _logBuilder.Length = 0;
        }

        _logScrollPos = GUILayout.BeginScrollView(_logScrollPos, "TextArea", GUILayout.Height(400f));
        GUILayout.Label(_logBuilder.ToString());
        GUILayout.EndScrollView();
    }

    #region FPS Display

    // Attach this to any object to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // corstartRect overall FPS even if the interval renders something like
    // 5.5 frames.

    public float fpsFrequency = 0.5F;
    // The update frequency of the fps
    public int nbDecimal = 1;
    // How many decimal do you want to display

    private float accum = 0f;
    // FPS accumulated over the interval
    private int frames = 0;
    // Frames drawn over the interval
    private Color fpsColor = Color.white;
    // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
    private string sFPS = "";
    // The fps formatted into a string.

    IEnumerator RefreshFPS()
    {
        // Infinite loop executed every "frenquency" secondes.
        while (true)
        {
            // Update the FPS
            float fps = accum / frames;
            sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));

            //Update the color
            fpsColor = (fps >= 30f) ? Color.green : ((fps > 10f) ? Color.yellow : Color.red);

            accum = 0.0F;
            frames = 0;

            yield return new WaitForSeconds(fpsFrequency);
        }
    }

    void DrawFPSInfo()
    {
        GUI.color = fpsColor;
        GUILayout.Label(sFPS + " FPS");
        GUI.color = Color.white;
    }

    #endregion

    #region Memory Display

    public float memoryFrequency = 5f;

    private long _freeMemory = 0;
    private long _totalMemory = 0;
    private long _useHeapSize = 0;
    private long _monoUsedSize = 0;
    private long _monoHeapSize = 0;

    private IEnumerator RefreshMemory()
    {
        while (true)
        {
            _useHeapSize = UnityEngine.Profiling.Profiler.usedHeapSize;
            _monoUsedSize = UnityEngine.Profiling.Profiler.GetMonoUsedSize();
            _monoHeapSize = UnityEngine.Profiling.Profiler.GetMonoHeapSize();

            _freeMemory = BaoyugameSdk.getFreeMemory();
            _totalMemory = BaoyugameSdk.getTotalMemory();
            yield return new WaitForSeconds(memoryFrequency);
        }
    }

    void DrawMemoryInfo()
    {
        GUILayout.Label(string.Format("UseHeapSize : {0}", AppStringHelper.FormatBytes(_useHeapSize)));
        GUILayout.Label(string.Format("MonoUsedSize : {0}", AppStringHelper.FormatBytes(_monoUsedSize)));
        GUILayout.Label(string.Format("MonoHeapSize : {0}", AppStringHelper.FormatBytes(_monoHeapSize)));
        GUILayout.Label(string.Format("空闲内存 : {0}", AppStringHelper.FormatBytes(_freeMemory)));
        GUILayout.Label(string.Format("总内存 : {0}", AppStringHelper.FormatBytes(_totalMemory)));
    }

    #endregion

    #region SocketStream Display

    private Color socketColor = Color.white;
    private string _sendInfo = "";
    private string _receiveInfo = "";

    void DrawSocketInfo()
    {
        GUI.color = socketColor;
        GUILayout.Label(_sendInfo);
        GUILayout.Label(_receiveInfo);
        GUI.color = Color.white;
    }

    #endregion

    #region Asset Info Display

    //void DrawAssetInfo ()
    //{
    //	if (GUILayout.Button ("Dump [DONT_DESTROY] Info")) {
    //		_logBuilder.AppendLine (ResourcePoolManager.Instance.DumpSpawnPoolInfo (ResourcePoolManager.PoolType.DONT_DESTROY));
    //	}

    //	if (GUILayout.Button ("Dump [DESTROY_NO_REFERENCE] Info")) {
    //		_logBuilder.AppendLine (ResourcePoolManager.Instance.DumpSpawnPoolInfo (ResourcePoolManager.PoolType.DESTROY_NO_REFERENCE));
    //	}

    //	if (GUILayout.Button ("Dump [DESTROY_CHANGE_SCENE] Info")) {
    //		_logBuilder.AppendLine (ResourcePoolManager.Instance.DumpSpawnPoolInfo (ResourcePoolManager.PoolType.DESTROY_CHANGE_SCENE));
    //	}
    //}

    #endregion

    #region GameStopwatch

    void DrawStopwatchInfo()
    {
        if (GUILayout.Button("Dump Stopwatch Info"))
        {
            _logBuilder.AppendLine(GameStopwatch.DumpAllInfo());
        }
    }

    #endregion
}
