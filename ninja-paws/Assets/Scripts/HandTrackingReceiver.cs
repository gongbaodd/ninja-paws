using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Concurrent;
using System.Runtime.InteropServices; // For thread-safe queue
// using UnityEngine.UI; // No longer needed for the cursor itself

[System.Serializable]
public struct MessageData {
    public string type;
    public string mask;
    public float x;
    public float y;
}

// The behavior class that handles individual WebSocket connections
public class HandTrackingBehavior : WebSocketBehavior {
    public static event System.Action<string> OnDataReceived; // Event to notify main thread

    protected override void OnMessage(MessageEventArgs e) {
        // Debug.Log("WebSocket message received: " + e.Data);
        // Pass the raw JSON data to the main thread via the event
        OnDataReceived?.Invoke(e.Data);
    }

    protected override void OnOpen() {
        Debug.Log("WebSocket client connected!");
    }

    protected override void OnClose(CloseEventArgs e) {
        Debug.Log($"WebSocket client disconnected: {e.Reason} (Code: {e.Code})");
    }

    protected override void OnError(ErrorEventArgs e) {
        Debug.LogError($"WebSocket error: {e.Message}");
    }
}

// The main MonoBehaviour that sets up the server and processes data
public class HandTrackingReceiver : MonoBehaviour {
    public int port = 8080; // Must match the JS client
    public GameObject virtualCursor; // Assign a GameObject (e.g., a Sprite or 3D model) in the Inspector
    public float cursorDistanceFromCamera = 10f; // How far in front of the camera the cursor should appear

    public static Vector2 NormalizedHandPosition { get; private set; } // Static property other scripts can read (0,0 bottom-left to 1,1 top-right)
    public static bool IsClickDetectedThisFrame { get; private set; } // Static property for click

    private WebSocketServer wsServer;
    private readonly ConcurrentQueue<string> messageQueue = new (); // Thread-safe queue
    private Vector2 targetScreenPos; // Target position in Screen Coordinates
    private Camera mainCamera; // Cache the main camera

    [DllImport("__Internal")]
    private static extern void SetUpDataListener(string gameObjectName);
    void Start() {
        #region Initial Websocket Server
        #if UNITY_EDITOR
        // Cache the main camera
        mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogError("Could not find Main Camera in the scene. Please ensure a camera is tagged 'MainCamera'.", this);
        }

        // Subscribe to the event from the WebSocketBehavior
        HandTrackingBehavior.OnDataReceived += HandleMessage;

        // Initialize and start the WebSocket server
        wsServer = new WebSocketServer($"ws://localhost:{port}");
        wsServer.AddWebSocketService<HandTrackingBehavior>("/"); // Listen on the root path

        try {
            wsServer.Start();
            if (wsServer.IsListening) {
                Debug.Log($"WebSocket Server started on port {port}. Listening...");
            } else {
                Debug.LogError("WebSocket Server failed to start.");
            }
        } catch (System.Exception ex) {
            Debug.LogError($"Failed to start WebSocket server: {ex.Message}");
        }

        // Initialize target screen position (e.g., center)
        targetScreenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);

        NormalizedHandPosition = Vector2.zero; // Initialize position
        #endif
        #endregion

        #region Initial iframe env
        #if UNITY_WEBGL && !UNITY_EDITOR
            SetUpDataListener(gameObject.name);
        #endif
        #endregion
    }
    void OnDestroy() {
        // Unsubscribe
        HandTrackingBehavior.OnDataReceived -= HandleMessage;

        // Stop the server when the object is destroyed or game stops
        if (wsServer != null && wsServer.IsListening) {
            Debug.Log("Stopping WebSocket Server...");
            wsServer.Stop();
            wsServer = null; // Allow garbage collection
        }
        Debug.Log("HandTrackingReceiver destroyed.");
    }

    // This method is called from the WebSocket thread via the event
    private void HandleMessage(string jsonData) {
        // Add the message to the queue to be processed on the main thread
        messageQueue.Enqueue(jsonData);

        print(jsonData);
    }

    void Update()
    {
        while (messageQueue.TryDequeue(out string posStr)) {
            try {
                MessageData receivedData = JsonUtility.FromJson<MessageData>(posStr);

                if (receivedData.type == "cursorPos") {
                    float targetX = receivedData.x * Screen.width;
                    float targetY = (1.0f - receivedData.y) * Screen.height;
                    targetScreenPos = new Vector2(targetX, targetY);
                }

            } catch (System.Exception ex) {
                Debug.LogError($"Error processing WebSocket message: {ex.Message}\nData: {posStr}");
            }
        }


        if (virtualCursor != null && mainCamera != null) {
            Vector3 screenPoint = new(targetScreenPos.x, targetScreenPos.y, cursorDistanceFromCamera);
            Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(screenPoint);
            virtualCursor.transform.position = targetWorldPos;
        }
    }
}