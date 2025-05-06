using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Concurrent;
using System.Runtime.InteropServices; // For thread-safe queue
// using UnityEngine.UI; // No longer needed for the cursor itself

[System.Serializable]
public struct MessageData
{
    public string type;
    public Data data;
}

[System.Serializable]

public struct Data
{
    public float x;
    public float y;
    public string base64;
}

// The behavior class that handles individual WebSocket connections
public class HandTrackingBehavior : WebSocketBehavior
{
    public static event System.Action<string> OnDataReceived; // Event to notify main thread

    protected override void OnMessage(MessageEventArgs e)
    {
        // Debug.Log("WebSocket message received: " + e.Data);
        // Pass the raw JSON data to the main thread via the event
        OnDataReceived?.Invoke(e.Data);
    }

    protected override void OnOpen()
    {
        Debug.Log("WebSocket client connected!");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log($"WebSocket client disconnected: {e.Reason} (Code: {e.Code})");
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Debug.LogError($"WebSocket error: {e.Message}");
    }
}

// The main MonoBehaviour that sets up the server and processes data
public class HandTrackingReceiver : MonoBehaviour
{
    public int port = 8080; 
    GameObject virtualCursor; 
    public Renderer mask;
    public float smoothingFactor = 0.01f;

    WebSocketServer wsServer;
    readonly ConcurrentQueue<string> messageQueue = new(); 
    Vector2 targetScreenPos; 
    readonly float desiredZ = 0;

    [DllImport("__Internal")]
    private static extern void SetUpDataListener(string gameObjectName);
    [DllImport("__Internal")]
    private static extern void StartMotionTracking();
    [DllImport("__Internal")]
    private static extern void StopMotionTracking();

    GameManagerController manager;
    GameSettings config;

    struct MotionEvent {
        public string type;
        public bool data;
    }

    void InitializeWebsocketServer()
    {

#if UNITY_EDITOR
        print("Initialize websocket");

        HandTrackingBehavior.OnDataReceived += HandleMessage;

        // Initialize and start the WebSocket server
        wsServer = new WebSocketServer($"ws://localhost:{port}");
        wsServer.AddWebSocketService<HandTrackingBehavior>("/"); // Listen on the root path

        try
        {
            wsServer.Start();
            if (wsServer.IsListening)
            {
                Debug.Log($"WebSocket Server started on port {port}. Listening...");
            }
            else
            {
                Debug.LogError("WebSocket Server failed to start.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to start WebSocket server: {ex.Message}");
        }
#endif
    }

    void InitializeIframeListener()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetUpDataListener(gameObject.name);
        StartMotionTracking();
#endif
    }

    // This method is called from the WebSocket thread via the event
    private void HandleMessage(string jsonData)
    {
        // Add the message to the queue to be processed on the main thread
        messageQueue.Enqueue(jsonData);
    }

    private Texture2D previousTexture = null;
    public static Vector3 DesiredCurPosition { get; private set; }
    public void ToggleService()
    {
        if (manager.useMotion)
        {
#if UNITY_EDITOR
            DesposeServer();
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            StopMotionTracking();
#endif
            DesiredCurPosition = Vector3.zero;
        }
        else
        {
            InitializeWebsocketServer();
            InitializeIframeListener();
            targetScreenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        }

    }

    public static bool isReceivingData = false;

    void DesposeServer()
    {
        // Unsubscribe
        HandTrackingBehavior.OnDataReceived -= HandleMessage;

        // Stop the server when the object is destroyed or game stops
        if (wsServer != null && wsServer.IsListening)
        {
            Debug.Log("Stopping WebSocket Server...");
            wsServer.Stop();
            wsServer = null; // Allow garbage collection
            isReceivingData = false;
        }
        Debug.Log("HandTrackingReceiver destroyed.");
    }

    void Start()
    {
        virtualCursor = gameObject;
        manager = GameManagerController.Instance;
        config = manager.config;
    }


    void Update()
    {
        if (!manager.useMotion) return;

        while (messageQueue.TryDequeue(out string posStr))
        {
            try
            {
                MessageData receivedData = JsonUtility.FromJson<MessageData>(posStr);

                isReceivingData = true;

                if (receivedData.type == "cursorPos")
                {
                    float targetX = receivedData.data.x * Screen.width;
                    float targetY = (1.0f - receivedData.data.y) * Screen.height;
                    targetScreenPos = new Vector2(targetX, targetY);
                }

                if (receivedData.type == "mask")
                {
                    string base64 = receivedData.data.base64[(receivedData.data.base64.IndexOf(",") + 1)..];

                    byte[] imagebytes = System.Convert.FromBase64String(base64);

                    Texture2D texture = new(1, 1, TextureFormat.RGBA32, false);

                    if (texture.LoadImage(imagebytes))
                    {
                        texture.Apply();

                        if (previousTexture != null)
                        {
                            Destroy(previousTexture);
                        }

                        mask.material.shader = Shader.Find("UI/Unlit/Transparent");
                        previousTexture = texture;
                        mask.material.mainTexture = texture;
                    }
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing WebSocket message: {ex.Message}\nData: {posStr}");
            }
        }


        if (virtualCursor != null && MotionButtonController.isMotion)
        {
            Vector3 screenPoint = new(targetScreenPos.x, targetScreenPos.y, Camera.main.nearClipPlane);
            Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(screenPoint);
            targetWorldPos.z = desiredZ;
            float lerpT = Time.deltaTime / smoothingFactor;
            DesiredCurPosition = Vector3.Lerp(DesiredCurPosition, targetWorldPos, lerpT);
            // DesiredCurPosition = targetWorldPos;
        }
    }

    void OnDestroy()
    {   
        DesposeServer();
    }
}