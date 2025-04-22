using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Concurrent; // For thread-safe queue
using UnityEngine.UI; // If using UI for virtual cursor

// Define a class to hold the received data
[System.Serializable]
public class HandData {
    public float x;
    public float y;
    public bool click;
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
    public RectTransform virtualCursor; // Assign a UI Image's RectTransform in the Inspector
    public float smoothingFactor = 0.2f; // Adjust for smoother cursor movement (0=no smooth, 1=no move)

    public static Vector2 NormalizedHandPosition { get; private set; } // Static property other scripts can read
    public static bool IsClickDetectedThisFrame { get; private set; } // Static property for click

    private WebSocketServer wsServer;
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>(); // Thread-safe queue
    private Vector2 targetCursorPos; // Target position for smoothing

    void Start() {
        // Subscribe to the event from the WebSocketBehavior
        HandTrackingBehavior.OnDataReceived += HandleWebSocketMessage;

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

        // Initialize target position
        if (virtualCursor != null) {
            targetCursorPos = virtualCursor.anchoredPosition;
        }
        NormalizedHandPosition = Vector2.zero; // Initialize position
    }

    void OnDestroy() {
        // Unsubscribe
        HandTrackingBehavior.OnDataReceived -= HandleWebSocketMessage;

        // Stop the server when the object is destroyed or game stops
        if (wsServer != null && wsServer.IsListening) {
            Debug.Log("Stopping WebSocket Server...");
            wsServer.Stop();
            wsServer = null; // Allow garbage collection
        }
        Debug.Log("HandTrackingReceiver destroyed.");
    }

    // This method is called from the WebSocket thread via the event
    private void HandleWebSocketMessage(string jsonData) {
        // Add the message to the queue to be processed on the main thread
        messageQueue.Enqueue(jsonData);
    }

    void Update() {
        // Process messages from the queue on the main thread
        while (messageQueue.TryDequeue(out string jsonData)) {
            try {
                HandData receivedData = JsonUtility.FromJson<HandData>(jsonData);

                // --- Update Position ---
                // Flip Y coordinate (Web Y=0 is top, Unity Screen Y=0 is bottom)
                float targetX = receivedData.x * Screen.width;
                float targetY = (1.0f - receivedData.y) * Screen.height;
                targetCursorPos = new Vector2(targetX, targetY);

                // Update static normalized position for other scripts
                NormalizedHandPosition = new Vector2(receivedData.x, 1.0f - receivedData.y); // Store with flipped Y

                // --- Update Click Status ---
                IsClickDetectedThisFrame = receivedData.click;
                if (IsClickDetectedThisFrame) {
                    Debug.Log("CLICK Action Received from Web!");
                    // --- !!! TRIGGER YOUR GAME'S CLICK ACTION HERE !!! ---
                    // Example: FindObjectOfType<MyGameManager>()?.HandleHandClick();
                    // Or: Trigger an event, set a flag for player script, etc.
                }

            } catch (System.Exception ex) {
                Debug.LogError($"Error processing WebSocket message: {ex.Message}\nData: {jsonData}");
                IsClickDetectedThisFrame = false; // Ensure click is false on error
            }
        }

        // --- Update Virtual Cursor Position (Smoothly) ---
        if (virtualCursor != null) {
            // Use ScreenPointToLocalPointInRectangle if cursor is on a Screen Space Canvas
            // For simplicity, assuming direct mapping works or adjust as needed for your Canvas setup
            // Simple Lerp for smoothing
            virtualCursor.anchoredPosition = Vector2.Lerp(virtualCursor.anchoredPosition, targetCursorPos, Time.deltaTime / smoothingFactor);

            // Or set directly if no smoothing needed:
            // virtualCursor.anchoredPosition = targetCursorPos;
        }

        // --- IMPORTANT: Reset Click Flag ---
        // If click is only meant to be true for a single frame, reset it here.
        // If you need the state to persist until the next message, don't reset here.
        // Resetting here makes it behave like Input.GetMouseButtonDown()
        // if (!messageQueue.IsEmpty) // Only reset if we didn't just process a click
        // {
        //      IsClickDetectedThisFrame = false;
        // }
        // A potentially safer way is to ensure the JS only sends click=true once per detection event.
        // Let's assume the JS handles sending click=true only once, so we don't need to reset here,
        // but rely on the next message having click=false. If JS keeps sending click=true,
        // you WILL need to reset the flag here after processing.

        // For safety, let's reset it after processing all queued messages for the frame:
        if (messageQueue.IsEmpty) {
            IsClickDetectedThisFrame = false;
        }

    }
}